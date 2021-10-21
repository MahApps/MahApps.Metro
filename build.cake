///////////////////////////////////////////////////////////////////////////////
// TOOLS / ADDINS
///////////////////////////////////////////////////////////////////////////////

#tool dotnet:?package=NuGetKeyVaultSignTool&version=1.2.28
#tool dotnet:?package=AzureSignTool&version=3.0.0
#tool dotnet:?package=GitReleaseManager.Tool&version=0.12.1
#tool dotnet:?package=GitVersion.Tool&version=5.6.3
#tool dotnet:?package=XamlStyler.Console&version=3.2008.4

#tool xunit.runner.console&version=2.4.1
#tool vswhere&version=2.8.4
#addin nuget:?package=Cake.Figlet&version=2.0.1

///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");

///////////////////////////////////////////////////////////////////////////////
// PREPARATION
///////////////////////////////////////////////////////////////////////////////

var repoName = "MahApps.Metro";
var baseDir = MakeAbsolute(Directory(".")).ToString();
var srcDir = baseDir + "/src";
var solution = srcDir + "/MahApps.Metro.sln";
var publishDir = baseDir + "/Publish";
var testResultsDir = Directory(baseDir + "/TestResults");

var styler = Context.Tools.Resolve("xstyler.exe");
var stylerFile = baseDir + "/Settings.XAMLStyler";

public class BuildData
{
    public string Configuration { get; }
    public Verbosity Verbosity { get; }
    public bool IsLocalBuild { get; set; }
    public bool IsPullRequest { get; set; }
    public bool IsDevelopBranch { get; set; }
    public bool IsReleaseBranch { get; set; }
    public GitVersion GitVersion { get; set; }
    public DirectoryPath MSBuildPath { get; }
    public FilePath MSBuildExe { get; }

    public BuildData(
        string configuration,
        Verbosity verbosity,
        DirectoryPath latestMSBuildInstallationPath
    )
    {
        Configuration = configuration;
        Verbosity = verbosity;

        MSBuildPath = latestMSBuildInstallationPath.Combine("./MSBuild/Current/Bin");
        MSBuildExe = MSBuildPath.CombineWithFilePath("./MSBuild.exe");
    }

    public void SetGitVersion(GitVersion gitVersion)
    {
        GitVersion = gitVersion;
        
        IsDevelopBranch = StringComparer.OrdinalIgnoreCase.Equals("develop", GitVersion.BranchName);
        IsReleaseBranch = StringComparer.OrdinalIgnoreCase.Equals("main", GitVersion.BranchName);
    }
}

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup<BuildData>(ctx =>
{
    if (!IsRunningOnWindows())
    {
        throw new NotImplementedException($"{repoName} will only build on Windows because it's not possible to target WPF and Windows Forms from UNIX.");
    }

    Information(Figlet(repoName));

    var gitVersionPath = Context.Tools.Resolve("dotnet-gitversion.exe");

    Information("GitVersion             : {0}", gitVersionPath);

    var buildData = new BuildData(
        configuration: Argument("configuration", "Release"),
        verbosity: Argument("verbosity", Verbosity.Minimal),
        latestMSBuildInstallationPath: VSWhereLatest(new VSWhereLatestSettings { IncludePrerelease = true })
    )
    {
        IsLocalBuild = BuildSystem.IsLocalBuild,
        IsPullRequest =
            (BuildSystem.GitHubActions.IsRunningOnGitHubActions && BuildSystem.GitHubActions.Environment.PullRequest.IsPullRequest)
            || (BuildSystem.AppVeyor.IsRunningOnAppVeyor && BuildSystem.AppVeyor.Environment.PullRequest.IsPullRequest)
    };

    // Set build version
    // if (buildData.IsLocalBuild == false || buildData.Verbosity == Verbosity.Verbose)
    // {
    //     GitVersion(new GitVersionSettings { ToolPath = gitVersionPath, OutputType = GitVersionOutput.BuildServer });
    // }
    buildData.SetGitVersion(GitVersion(new GitVersionSettings { ToolPath = gitVersionPath, OutputType = GitVersionOutput.Json }));

    Information("MSBuild                : {0}", buildData.MSBuildExe);
    Information("Branch                 : {0}", buildData.GitVersion.BranchName);
    Information("Configuration          : {0}", buildData.Configuration);
    Information("IsLocalBuild           : {0}", buildData.IsLocalBuild);
    Information("Informational   Version: {0}", buildData.GitVersion.InformationalVersion);
    Information("SemVer          Version: {0}", buildData.GitVersion.SemVer);
    Information("AssemblySemVer  Version: {0}", buildData.GitVersion.AssemblySemVer);
    Information("MajorMinorPatch Version: {0}", buildData.GitVersion.MajorMinorPatch);
    Information("NuGet           Version: {0}", buildData.GitVersion.NuGetVersion);

    if (FileExists(buildData.MSBuildExe) == false)
    {
        throw new NotImplementedException("You need at least Visual Studio 2019 to build this project.");
    }

    return buildData;
});

Teardown(ctx =>
{
});

///////////////////////////////////////////////////////////////////////////////
// TASKS
///////////////////////////////////////////////////////////////////////////////

Task("Clean")
    .ContinueOnError()
    .Does(() =>
{
    var directoriesToDelete = GetDirectories("./**/obj")
        .Concat(GetDirectories("./**/bin"))
        .Concat(GetDirectories("./**/Publish"));
    DeleteDirectories(directoriesToDelete, new DeleteDirectorySettings { Recursive = true, Force = true });
});

Task("Restore")
    .Does<BuildData>(data =>
{
    NuGetRestore(solution, new NuGetRestoreSettings { MSBuildPath = data.MSBuildPath.ToString() });
});

Task("Build")
    .Does<BuildData>(data =>
{
    var msBuildSettings = new MSBuildSettings {
        Verbosity = data.Verbosity
        , ToolPath = data.MSBuildExe
        , Configuration = data.Configuration
        , ArgumentCustomization = args => args.Append("/m").Append("/nr:false") // The /nr switch tells msbuild to quite once it�s done
        , BinaryLogger = new MSBuildBinaryLogSettings() { Enabled = data.IsLocalBuild }
    };
    MSBuild(solution, msBuildSettings
            .SetMaxCpuCount(0)
            .WithProperty("Description", "MahApps.Metro, a toolkit for creating Metro / Modern UI styled WPF applications.")
            .WithProperty("Version", data.IsReleaseBranch ? data.GitVersion.MajorMinorPatch : data.GitVersion.NuGetVersion)
            .WithProperty("AssemblyVersion", data.GitVersion.AssemblySemVer)
            .WithProperty("FileVersion", data.GitVersion.AssemblySemFileVer)
            .WithProperty("InformationalVersion", data.GitVersion.InformationalVersion)
            .WithProperty("ContinuousIntegrationBuild", data.IsReleaseBranch ? "true" : "false")
            );
});

Task("Pack")
    .ContinueOnError()
    .Does<BuildData>(data =>
{
    EnsureDirectoryExists(Directory(publishDir));

    var msBuildSettings = new MSBuildSettings {
        Verbosity = data.Verbosity
        , ToolPath = data.MSBuildExe
        , Configuration = data.Configuration
    };

    var project = "./src/MahApps.Metro/MahApps.Metro.csproj";
    MSBuild(project, msBuildSettings
      .WithTarget("pack")
      .WithProperty("NoBuild", "true")
      .WithProperty("IncludeBuildOutput", "true")
      .WithProperty("PackageOutputPath", publishDir)
      .WithProperty("RepositoryBranch", data.GitVersion.BranchName)
      .WithProperty("RepositoryCommit", data.GitVersion.Sha)
      .WithProperty("Description", "The goal of MahApps.Metro is to allow devs to quickly and easily cobble together a 'Modern' UI for their WPF apps (>= .Net 4.5), with minimal effort.")
      .WithProperty("Version", data.IsReleaseBranch ? data.GitVersion.MajorMinorPatch : data.GitVersion.NuGetVersion)
      .WithProperty("AssemblyVersion", data.GitVersion.AssemblySemVer)
      .WithProperty("FileVersion", data.GitVersion.AssemblySemFileVer)
      .WithProperty("InformationalVersion", data.GitVersion.InformationalVersion)
    );
});

void SignFiles(IEnumerable<FilePath> files, string description)
{
    var vurl = EnvironmentVariable("azure-key-vault-url");
    if(string.IsNullOrWhiteSpace(vurl)) {
        Error("Could not resolve signing url.");
        return;
    }

    var vcid = EnvironmentVariable("azure-key-vault-client-id");
    if(string.IsNullOrWhiteSpace(vcid)) {
        Error("Could not resolve signing client id.");
        return;
    }

    var vctid = EnvironmentVariable("azure-key-vault-tenant-id");
    if(string.IsNullOrWhiteSpace(vctid)) {
        Error("Could not resolve signing client tenant id.");
        return;
    }

    var vcs = EnvironmentVariable("azure-key-vault-client-secret");
    if(string.IsNullOrWhiteSpace(vcs)) {
        Error("Could not resolve signing client secret.");
        return;
    }

    var vc = EnvironmentVariable("azure-key-vault-certificate");
    if(string.IsNullOrWhiteSpace(vc)) {
        Error("Could not resolve signing certificate.");
        return;
    }

    var filesToSign = string.Join(" ", files.Select(f => MakeAbsolute(f).FullPath));

    var processSettings = new ProcessSettings {
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        Arguments = new ProcessArgumentBuilder()
            .Append("sign")
            .Append(filesToSign)
            .AppendSwitchQuoted("--file-digest", "sha256")
            .AppendSwitchQuoted("--description", description)
            .AppendSwitchQuoted("--description-url", "https://github.com/MahApps/MahApps.Metro")
            .Append("--no-page-hashing")
            .AppendSwitchQuoted("--timestamp-rfc3161", "http://timestamp.digicert.com")
            .AppendSwitchQuoted("--timestamp-digest", "sha256")
            .AppendSwitchQuoted("--azure-key-vault-url", vurl)
            .AppendSwitchQuotedSecret("--azure-key-vault-client-id", vcid)
            .AppendSwitchQuotedSecret("--azure-key-vault-tenant-id", vctid)
            .AppendSwitchQuotedSecret("--azure-key-vault-client-secret", vcs)
            .AppendSwitchQuotedSecret("--azure-key-vault-certificate", vc)
    };

    using(var process = StartAndReturnProcess("tools/AzureSignTool", processSettings))
    {
        process.WaitForExit();

        if (process.GetStandardOutput().Any())
        {
            Information($"Output:{Environment.NewLine}{string.Join(Environment.NewLine, process.GetStandardOutput())}");
        }

        if (process.GetStandardError().Any())
        {
            Information($"Errors occurred:{Environment.NewLine}{string.Join(Environment.NewLine, process.GetStandardError())}");
        }

        // This should output 0 as valid arguments supplied
        Information("Exit code: {0}", process.GetExitCode());
    }
}

Task("Sign")
    .WithCriteria<BuildData>((context, data) => !data.IsPullRequest)
    .ContinueOnError()
    .Does(() =>
{
    var files = GetFiles("./src/MahApps.Metro/bin/**/*/MahApps.Metro.dll");
    SignFiles(files, "MahApps.Metro, a toolkit for creating Metro / Modern UI styled WPF applications.");

    files = GetFiles("./src/MahApps.Metro.Samples/**/bin/**/*.exe");
    SignFiles(files, "Demo application of MahApps.Metro, a toolkit for creating Metro / Modern UI styled WPF applications.");
});

Task("SignNuGet")
    .WithCriteria<BuildData>((context, data) => !data.IsPullRequest)
    .ContinueOnError()
    .Does(() =>
{
    if (!DirectoryExists(Directory(publishDir)))
    {
        return;
    }

    var vurl = EnvironmentVariable("azure-key-vault-url");
    if(string.IsNullOrWhiteSpace(vurl)) {
        Error("Could not resolve signing url.");
        return;
    }

    var vcid = EnvironmentVariable("azure-key-vault-client-id");
    if(string.IsNullOrWhiteSpace(vcid)) {
        Error("Could not resolve signing client id.");
        return;
    }

    var vcs = EnvironmentVariable("azure-key-vault-client-secret");
    if(string.IsNullOrWhiteSpace(vcs)) {
        Error("Could not resolve signing client secret.");
        return;
    }

    var vc = EnvironmentVariable("azure-key-vault-certificate");
    if(string.IsNullOrWhiteSpace(vc)) {
        Error("Could not resolve signing certificate.");
        return;
    }

    var nugetFiles = GetFiles(publishDir + "/*.nupkg");
    foreach(var file in nugetFiles)
    {
        Information($"Sign file: {file}");
        var processSettings = new ProcessSettings {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            Arguments = new ProcessArgumentBuilder()
                .Append("sign")
                .Append(MakeAbsolute(file).FullPath)
                .Append("--force")
                .AppendSwitchQuoted("--file-digest", "sha256")
                .AppendSwitchQuoted("--timestamp-rfc3161", "http://timestamp.digicert.com")
                .AppendSwitchQuoted("--timestamp-digest", "sha256")
                .AppendSwitchQuoted("--azure-key-vault-url", vurl)
                .AppendSwitchQuotedSecret("--azure-key-vault-client-id", vcid)
                .AppendSwitchQuotedSecret("--azure-key-vault-client-secret", vcs)
                .AppendSwitchQuotedSecret("--azure-key-vault-certificate", vc)
        };

        using(var process = StartAndReturnProcess("tools/NuGetKeyVaultSignTool", processSettings))
        {
            process.WaitForExit();

            if (process.GetStandardOutput().Any())
            {
                Information($"Output:{Environment.NewLine}{string.Join(Environment.NewLine, process.GetStandardOutput())}");
            }

            if (process.GetStandardError().Any())
            {
                Information($"Errors occurred:{Environment.NewLine}{string.Join(Environment.NewLine, process.GetStandardError())}");
            }

            // This should output 0 as valid arguments supplied
            Information("Exit code: {0}", process.GetExitCode());
        }
    }
});

Task("Zip")
    .Does<BuildData>(data =>
{
    EnsureDirectoryExists(Directory(publishDir));

    Zip("./src/MahApps.Metro.Samples/MahApps.Metro.Demo/bin/" + data.Configuration, publishDir + "/MahApps.Metro.Demo-v" + data.GitVersion.NuGetVersion + ".zip");
    Zip("./src/MahApps.Metro.Samples/MahApps.Metro.Caliburn.Demo/bin/" + data.Configuration, publishDir + "/MahApps.Metro.Caliburn.Demo-v" + data.GitVersion.NuGetVersion + ".zip");
});

Task("Tests")
    .ContinueOnError()
    .Does<BuildData>(data =>
{
    CleanDirectory(testResultsDir);

    var settings = new DotNetCoreTestSettings
        {
            Configuration = data.Configuration,
            NoBuild = true,
            NoRestore = true,
            ResultsDirectory = testResultsDir,
            Verbosity = DotNetCoreVerbosity.Normal
        };

    DotNetCoreTest("./src/Mahapps.Metro.Tests/Mahapps.Metro.Tests.csproj", settings);
});

Task("StyleXaml")
    .Description("Ensures XAML Formatting is Clean")
    .Does(() =>
{
    Func<IFileSystemInfo, bool> exclude_Dir =
        fileSystemInfo => !fileSystemInfo.Path.Segments.Contains("obj") && !fileSystemInfo.Path.ToString().Contains("Styles/Themes");

    var files = GetFiles(srcDir + "/**/*.xaml", new GlobberSettings { Predicate = exclude_Dir });
    Information("\nChecking " + files.Count() + " file(s) for XAML Structure");
    StartProcess(styler, "-f \"" + string.Join(",", files.Select(f => f.ToString())) + "\" -c \"" + stylerFile + "\"");
});

Task("CreateRelease")
    .WithCriteria<BuildData>((context, data) => !data.IsPullRequest)
    .Does<BuildData>(data =>
{
    var token = EnvironmentVariable("GITHUB_TOKEN");
    if (string.IsNullOrEmpty(token))
    {
        throw new Exception("The GITHUB_TOKEN environment variable is not defined.");
    }

    GitReleaseManagerCreate(token, "MahApps", repoName, new GitReleaseManagerCreateSettings {
        Milestone         = data.GitVersion.MajorMinorPatch,
        Name              = data.GitVersion.AssemblySemFileVer,
        Prerelease        = data.IsDevelopBranch,
        TargetCommitish   = data.GitVersion.BranchName,
        WorkingDirectory  = "."
    });
});

///////////////////////////////////////////////////////////////////////////////
// TASK TARGETS
///////////////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .IsDependentOn("StyleXaml")
    .IsDependentOn("Build")
    .IsDependentOn("Tests")
    ;

Task("ci")
    .IsDependentOn("Default")
    .IsDependentOn("Sign")
    .IsDependentOn("Pack")
    .IsDependentOn("SignNuGet")
    .IsDependentOn("Zip")
    ;

///////////////////////////////////////////////////////////////////////////////
// EXECUTION
///////////////////////////////////////////////////////////////////////////////

RunTarget(target);
