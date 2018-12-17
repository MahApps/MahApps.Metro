///////////////////////////////////////////////////////////////////////////////
// TOOLS / ADDINS
///////////////////////////////////////////////////////////////////////////////

#tool GitVersion.CommandLine
#tool gitreleasemanager
#tool xunit.runner.console
#tool vswhere
#addin Cake.Figlet
#addin Cake.Paket

///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
if (string.IsNullOrWhiteSpace(target))
{
    target = "Default";
}

var configuration = Argument("configuration", "Release");
if (string.IsNullOrWhiteSpace(configuration))
{
    configuration = "Release";
}

var verbosity = Argument("verbosity", Verbosity.Normal);
if (string.IsNullOrWhiteSpace(configuration))
{
    verbosity = Verbosity.Normal;
}

///////////////////////////////////////////////////////////////////////////////
// PREPARATION
///////////////////////////////////////////////////////////////////////////////

var repoName = "MahApps.Metro";
var local = BuildSystem.IsLocalBuild;

// Set build version
if (local == false
    || verbosity == Verbosity.Verbose)
{
    GitVersion(new GitVersionSettings { OutputType = GitVersionOutput.BuildServer });
}
GitVersion gitVersion = GitVersion(new GitVersionSettings { OutputType = GitVersionOutput.Json });

var latestInstallationPath = VSWhereLatest(new VSWhereLatestSettings { IncludePrerelease = true });
//var msBuildPath = latestInstallationPath.CombineWithFilePath("./MSBuild/15.0/Bin/MSBuild.exe");
var msBuildPath = latestInstallationPath?.CombineWithFilePath("./MSBuild/Current/Bin/MSBuild.exe");

var isPullRequest = AppVeyor.Environment.PullRequest.IsPullRequest;
var branchName = gitVersion.BranchName;
var isDevelopBranch = StringComparer.OrdinalIgnoreCase.Equals("develop", branchName);
var isReleaseBranch = StringComparer.OrdinalIgnoreCase.Equals("master", branchName);
var isTagged = AppVeyor.Environment.Repository.Tag.IsTag;

// Directories and Paths
var solution = "./src/MahApps.Metro.sln";
var publishDir = "./Publish";

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup(ctx =>
{
    // Executed BEFORE the first task.

    if (!IsRunningOnWindows())
    {
        throw new NotImplementedException($"{repoName} will only build on Windows because it's not possible to target WPF and Windows Forms from UNIX.");
    }

    Information(Figlet(repoName));

    Information("Informational   Version: {0}", gitVersion.InformationalVersion);
    Information("SemVer          Version: {0}", gitVersion.SemVer);
    Information("AssemblySemVer  Version: {0}", gitVersion.AssemblySemVer);
    Information("MajorMinorPatch Version: {0}", gitVersion.MajorMinorPatch);
    Information("NuGet           Version: {0}", gitVersion.NuGetVersion);
    Information("IsLocalBuild           : {0}", local);
    Information("Branch                 : {0}", branchName);
    Information("Configuration          : {0}", configuration);
    Information("MSBuildPath            : {0}", msBuildPath);
});

Teardown(ctx =>
{
   // Executed AFTER the last task.
});

///////////////////////////////////////////////////////////////////////////////
// TASKS
///////////////////////////////////////////////////////////////////////////////

Task("Clean")
    .ContinueOnError()
    .Does(() =>
{
    var directoriesToDelete = GetDirectories("./**/obj").Concat(GetDirectories("./**/bin")).Concat(GetDirectories("./**/Publish"));
    DeleteDirectories(directoriesToDelete, new DeleteDirectorySettings { Recursive = true, Force = true });
});

Task("Restore")
    .Does(() =>
{
    var msBuildSettings = new MSBuildSettings {
        Verbosity = Verbosity.Minimal,
        ToolPath = msBuildPath,
        ToolVersion = MSBuildToolVersion.Default,
        Configuration = configuration,
        // Restore = true, // only with cake 0.28.x
        ArgumentCustomization = args => args.Append("/m")
    };

    MSBuild(solution, msBuildSettings.WithTarget("restore"));
});

Task("Build")
    .Does(() =>
{
    var msBuildSettings = new MSBuildSettings {
        Verbosity = Verbosity.Normal,
        ToolPath = msBuildPath,
        ToolVersion = MSBuildToolVersion.Default,
        Configuration = configuration,
        // Restore = true, // only with cake 0.28.x     
        ArgumentCustomization = args => args.Append("/m")
    };

    MSBuild(solution, msBuildSettings
            .SetMaxCpuCount(0)
            .WithProperty("Description", "A toolkit for creating Metro / Modern UI styled WPF apps.")
            .WithProperty("Version", isReleaseBranch ? gitVersion.MajorMinorPatch : gitVersion.NuGetVersion)
            .WithProperty("AssemblyVersion", gitVersion.AssemblySemVer)
            .WithProperty("FileVersion", gitVersion.AssemblySemFileVer)
            .WithProperty("InformationalVersion", gitVersion.InformationalVersion)
            );
});

Task("Pack")
    .ContinueOnError()
    .Does(() =>
{
    EnsureDirectoryExists(Directory(publishDir));

    var msBuildSettings = new MSBuildSettings {
        Verbosity = Verbosity.Normal,
        ToolPath = msBuildPath,
        ToolVersion = MSBuildToolVersion.Default,
        Configuration = configuration
    };
    var project = "./src/MahApps.Metro/MahApps.Metro.csproj";

    MSBuild(project, msBuildSettings
      .WithTarget("pack")
      .WithProperty("PackageOutputPath", "../../" + publishDir)
      .WithProperty("RepositoryBranch", branchName)
      .WithProperty("RepositoryCommit", gitVersion.Sha)
      .WithProperty("Description", "The goal of MahApps.Metro is to allow devs to quickly and easily cobble together a 'Modern' UI for their WPF apps (>= .Net 4.5), with minimal effort.")
      .WithProperty("Version", isReleaseBranch ? gitVersion.MajorMinorPatch : gitVersion.NuGetVersion)
      .WithProperty("AssemblyVersion", gitVersion.AssemblySemVer)
      .WithProperty("FileVersion", gitVersion.AssemblySemFileVer)
      .WithProperty("InformationalVersion", gitVersion.InformationalVersion)
    );
});

Task("Zip")
    .Does(() =>
{
    EnsureDirectoryExists(Directory(publishDir));

    Zip("./src/MahApps.Metro.Samples/MahApps.Metro.Demo/bin/" + configuration, publishDir + "/MahApps.Metro.Demo-v" + gitVersion.NuGetVersion + ".zip");
    Zip("./src/MahApps.Metro.Samples/MahApps.Metro.Caliburn.Demo/bin/" + configuration, publishDir + "/MahApps.Metro.Caliburn.Demo-v" + gitVersion.NuGetVersion + ".zip");
});

Task("Tests")
    .ContinueOnError()
    .Does(() =>
{
    XUnit2(
        "./src/Mahapps.Metro.Tests/bin/" + configuration + "/**/*.Tests.dll",
        new XUnit2Settings { ToolTimeout = TimeSpan.FromMinutes(5) }
    );
});

Task("CreateRelease")
    .WithCriteria(() => !isTagged)
    .WithCriteria(() => !isPullRequest)
    .Does(() =>
{
    var username = EnvironmentVariable("GITHUB_USERNAME");
    if (string.IsNullOrEmpty(username))
    {
        throw new Exception("The GITHUB_USERNAME environment variable is not defined.");
    }

    var token = EnvironmentVariable("GITHUB_TOKEN");
    if (string.IsNullOrEmpty(token))
    {
        throw new Exception("The GITHUB_TOKEN environment variable is not defined.");
    }

    GitReleaseManagerCreate(username, token, "MahApps", repoName, new GitReleaseManagerCreateSettings {
        Milestone         = gitVersion.MajorMinorPatch,
        Name              = gitVersion.AssemblySemFileVer,
        Prerelease        = isDevelopBranch,
        TargetCommitish   = branchName,
        WorkingDirectory  = "."
    });
});

Task("ExportReleaseNotes")
    .Does(() =>
{
    var username = EnvironmentVariable("GITHUB_USERNAME");
    if (string.IsNullOrEmpty(username))
    {
        throw new Exception("The GITHUB_USERNAME environment variable is not defined.");
    }

    var token = EnvironmentVariable("GITHUB_TOKEN");
    if (string.IsNullOrEmpty(token))
    {
        throw new Exception("The GITHUB_TOKEN environment variable is not defined.");
    }

    EnsureDirectoryExists(Directory(publishDir));
    GitReleaseManagerExport(username, token, "MahApps", repoName, publishDir + "/releasenotes.md", new GitReleaseManagerExportSettings {
        TagName         = gitVersion.SemVer
    });
});

///////////////////////////////////////////////////////////////////////////////
// TASK TARGETS
///////////////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .IsDependentOn("Build")
    .IsDependentOn("Tests");

Task("appveyor")
    .IsDependentOn("Default")
    .IsDependentOn("Pack")
    .IsDependentOn("Zip");

///////////////////////////////////////////////////////////////////////////////
// EXECUTION
///////////////////////////////////////////////////////////////////////////////

RunTarget(target);