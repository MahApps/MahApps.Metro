
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

// Define directories.
var buildDir = Directory("./bin");

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Setup(context =>
{
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
	var version = GitVersion();
	EnsureDirectoryExists("./Publish");
	PaketPack("./Publish", new PaketPackSettings { Version = version.NuGetVersion });
});

Task("Zip-Demos")
    //.WithCriteria(ShouldRunRelease())
    .Does(() =>
{
	var version = GitVersion();
	EnsureDirectoryExists("./Publish");
    Zip("./bin/MetroDemo/", "./Publish/MetroDemo-v" + version.NuGetVersion + ".zip");
    Zip("./bin/Caliburn.Metro.Demo/", "./Publish/Caliburn.MetroDemo-v" + version.NuGetVersion + ".zip");
});

Task("Unit-Tests")
    //.WithCriteria(ShouldRunRelease())
    .Does(() =>
{
    XUnit("./Mahapps.Metro.Tests/**/bin/" + configuration + "/*.Tests.dll",
        new XUnitSettings { ToolPath = "./packages/cake/xunit.runner.console/tools/net452/xunit.console.exe" });
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
