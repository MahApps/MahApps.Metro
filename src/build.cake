///////////////////////////////////////////////////////////////////////////////
// TOOLS / ADDINS
///////////////////////////////////////////////////////////////////////////////

#tool paket:?package=GitVersion.CommandLine
#tool paket:?package=gitreleasemanager
#tool paket:?package=vswhere
#tool paket:?package=xunit.runner.console
#addin paket:?package=Cake.Figlet
#addin paket:?package=Cake.Paket

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

///////////////////////////////////////////////////////////////////////////////
// PREPARATION
///////////////////////////////////////////////////////////////////////////////

// Set build version
GitVersion(new GitVersionSettings { OutputType = GitVersionOutput.BuildServer });
GitVersion gitVersion = GitVersion(new GitVersionSettings { OutputType = GitVersionOutput.Json });

var latestInstallationPath = VSWhereProducts("*", new VSWhereProductSettings { Version = "[\"15.0\",\"16.0\"]" }).FirstOrDefault();
var msBuildPath = latestInstallationPath.CombineWithFilePath("./MSBuild/15.0/Bin/MSBuild.exe");

var local = BuildSystem.IsLocalBuild;
var isPullRequest = AppVeyor.Environment.PullRequest.IsPullRequest;
var isDevelopBranch = StringComparer.OrdinalIgnoreCase.Equals("develop", AppVeyor.Environment.Repository.Branch);
var isReleaseBranch = StringComparer.OrdinalIgnoreCase.Equals("master", AppVeyor.Environment.Repository.Branch);
var isTagged = AppVeyor.Environment.Repository.Tag.IsTag;

// Directories and Paths
var solution = "MahApps.Metro.sln";
var publishDir = "./Publish";

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup(ctx =>
{
    // Executed BEFORE the first task.

    if (!IsRunningOnWindows())
    {
        throw new NotImplementedException("MahApps.Metro will only build on Windows because it's not possible to target WPF and Windows Forms from UNIX.");
    }

    Information(Figlet("MahApps.Metro"));

    Information("Informational   Version: {0}", gitVersion.InformationalVersion);
    Information("SemVer          Version: {0}", gitVersion.SemVer);
    Information("AssemblySemVer  Version: {0}", gitVersion.AssemblySemVer);
    Information("MajorMinorPatch Version: {0}", gitVersion.MajorMinorPatch);
    Information("NuGet           Version: {0}", gitVersion.NuGetVersion);
    Information("IsLocalBuild           : {0}", local);
    Information("Configuration          : {0}", configuration);
});

Teardown(ctx =>
{
   // Executed AFTER the last task.
});

///////////////////////////////////////////////////////////////////////////////
// TASKS
///////////////////////////////////////////////////////////////////////////////

Task("Clean")
    //.ContinueOnError()
    .Does(() =>
{
    var directoriesToDelete = GetDirectories("./**/obj").Concat(GetDirectories("./**/bin"));
    DeleteDirectories(directoriesToDelete, new DeleteDirectorySettings { Recursive = true, Force = true });
});

Task("NuGet-Paket-Restore")
    .IsDependentOn("Clean")
    .Does(() =>
{
    PaketRestore();

    var msBuildSettings = new MSBuildSettings() { ToolPath = msBuildPath };
    MSBuild(solution, msBuildSettings.SetVerbosity(Verbosity.Normal).WithTarget("restore"));
});

Task("Update-SolutionInfo")
    .Does(() =>
{
	var solutionInfo = "./MahApps.Metro/Properties/AssemblyInfo.cs";
	GitVersion(new GitVersionSettings { UpdateAssemblyInfo = true, UpdateAssemblyInfoFilePath = solutionInfo});
});

Task("Build")
    .IsDependentOn("NuGet-Paket-Restore")
    .Does(() =>
{
  var msBuildSettings = new MSBuildSettings() { ToolPath = msBuildPath, ArgumentCustomization = args => args.Append("/m") };
  MSBuild(solution, msBuildSettings.SetMaxCpuCount(0)
                                   .SetVerbosity(Verbosity.Normal)
                                   //.WithRestore() only with cake 0.28.x
                                   .SetConfiguration(configuration)
                                   );
});

Task("Paket-Pack")
    .WithCriteria(() => !isPullRequest)
    .Does(() =>
{
    EnsureDirectoryExists(Directory(publishDir));
    PaketPack(publishDir, new PaketPackSettings {
        Version = isReleaseBranch ? gitVersion.MajorMinorPatch : gitVersion.NuGetVersion,
        BuildConfig = configuration
        });
});

Task("Zip-Demos")
    .WithCriteria(() => !isPullRequest)
    .Does(() =>
{
    EnsureDirectoryExists(Directory(publishDir));
    Zip("./MahApps.Metro.Samples/MahApps.Metro.Demo/bin/" + configuration, publishDir + "/MahApps.Metro.Demo-v" + gitVersion.NuGetVersion + ".zip");
    Zip("./MahApps.Metro.Samples/MahApps.Metro.Caliburn.Demo/bin/" + configuration, publishDir + "/MahApps.Metro.Caliburn.Demo-v" + gitVersion.NuGetVersion + ".zip");
});

Task("Unit-Tests")
    .Does(() =>
{
    XUnit2(
        "./Mahapps.Metro.Tests/bin/" + configuration + "/**/*.Tests.dll",
        new XUnit2Settings { ToolTimeout = TimeSpan.FromMinutes(5) }
    );
});

Task("CreateRelease")
    .WithCriteria(() => isReleaseBranch)
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

    GitReleaseManagerCreate(username, token, "MahApps", "MahApps.Metro", new GitReleaseManagerCreateSettings {
        Milestone         = gitVersion.MajorMinorPatch,
        Name              = gitVersion.MajorMinorPatch,
        Prerelease        = false,
        TargetCommitish   = "master",
        WorkingDirectory  = "../"
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
    GitReleaseManagerExport(username, token, "MahApps", "MahApps.Metro", publishDir + "/releasenotes.md", new GitReleaseManagerExportSettings {
        // TagName         = gitVersion.SemVer,
        TagName         = "1.5.0",
        TargetDirectory = publishDir,
        LogFilePath     = publishDir + "/grm.log"
    });
});

///////////////////////////////////////////////////////////////////////////////
// TASK TARGETS
///////////////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Build");

Task("appveyor")
    .IsDependentOn("Update-SolutionInfo")
    .IsDependentOn("Build")
    .IsDependentOn("Unit-Tests")
    .IsDependentOn("Paket-Pack")
    .IsDependentOn("Zip-Demos");

///////////////////////////////////////////////////////////////////////////////
// EXECUTION
///////////////////////////////////////////////////////////////////////////////

RunTarget(target);