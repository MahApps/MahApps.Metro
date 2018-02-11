
//////////////////////////////////////////////////////////////////////
// TOOLS / ADDINS
//////////////////////////////////////////////////////////////////////

#tool paket:?package=GitVersion.CommandLine
#tool paket:?package=gitreleasemanager
#tool paket:?package=xunit.runner.console
#addin paket:?package=Cake.Figlet
#addin paket:?package=Cake.Paket

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

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

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Set build version
GitVersion(new GitVersionSettings { OutputType = GitVersionOutput.BuildServer });
GitVersion gitVersion;

// Define directories.
var buildDir = Directory("./bin");

var username = "";
var password = "";

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Setup(context =>
{
    gitVersion = GitVersion(new GitVersionSettings { OutputType = GitVersionOutput.Json });
    Information("Informational Version: {0}", gitVersion.InformationalVersion);
    Information("SemVer Version: {0}", gitVersion.SemVer);

    Information(Figlet("MahApps.Metro"));
});

Task("Clean")
    .Does(() =>
{
    CleanDirectory(buildDir);
});

Task("Paket-Restore")
    .IsDependentOn("Clean")
    .Does(() =>
{
    PaketRestore();
});

Task("Update-SolutionInfo")
    .Does(() =>
{
	var solutionInfo = "./MahApps.Metro/MahApps.Metro/Properties/AssemblyInfo.cs";
	GitVersion(new GitVersionSettings { UpdateAssemblyInfo = true, UpdateAssemblyInfoFilePath = solutionInfo});
});

Task("Build")
    .IsDependentOn("Paket-Restore")
    .Does(() =>
{
    if(IsRunningOnWindows())
    {
      // Use MSBuild
      MSBuild("./MahApps.Metro.sln", settings => settings.SetMaxCpuCount(0).SetConfiguration(configuration));
    }
});

Task("Paket-Pack")
    //.WithCriteria(ShouldRunRelease())
    .Does(() =>
{
	EnsureDirectoryExists("./Publish");
	PaketPack("./Publish", new PaketPackSettings { Version = gitVersion.NuGetVersion });
});

Task("Zip-Demos")
    //.WithCriteria(ShouldRunRelease())
    .Does(() =>
{
	EnsureDirectoryExists("./Publish");
    Zip("./bin/MetroDemo/", "./Publish/MetroDemo-v" + gitVersion.NuGetVersion + ".zip");
    Zip("./bin/Caliburn.Metro.Demo/", "./Publish/Caliburn.MetroDemo-v" + gitVersion.NuGetVersion + ".zip");
});

Task("Unit-Tests")
    //.WithCriteria(ShouldRunRelease())
    .Does(() =>
{
    XUnit(
        "./Mahapps.Metro.Tests/**/bin/" + configuration + "/*.Tests.dll",
        new XUnitSettings { ToolPath = "./packages/cake/xunit.runner.console/tools/net452/xunit.console.exe" }
    );
});

Task("GetCredentials")
    .Does(() =>
{
    username = EnvironmentVariable("GITHUB_USERNAME_MAHAPPS");
    password = EnvironmentVariable("GITHUB_PASSWORD_MAHAPPS");
});

Task("CreateReleaseNotes")
    .Does(() =>
{
    EnsureDirectoryExists("./Publish");
    // GitReleaseManagerExport(username, password, "MahApps", "MahApps.Metro", "./Publish/releasenotes.md", new GitReleaseManagerExportSettings {
    //     TagName         = "1.5.0",
    //     TargetDirectory = "./Publish",
    //     LogFilePath     = "./Publish/grm.log"
    // });
    GitReleaseManagerCreate(username, password, "MahApps", "MahApps.Metro", new GitReleaseManagerCreateSettings {
        Milestone         = gitVersion.MajorMinorPatch,
        Name              = gitVersion.SemVer,
        Prerelease        = false,
        TargetCommitish   = "master",
        WorkingDirectory  = "../"
    });
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Build");

Task("appveyor")
    .IsDependentOn("Update-SolutionInfo")
    .IsDependentOn("Build")
    .IsDependentOn("Unit-Tests")
    .IsDependentOn("Paket-Pack")
    .IsDependentOn("Zip-Demos");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
