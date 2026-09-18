<#
.SYNOPSIS
    Runs the tests once per target framework, each host on a desktop of its own.

.DESCRIPTION
    These are UI tests, and the things they need are not per process but per desktop: the
    foreground window, the keyboard focus and the mouse capture that keeps a popup up. Two hosts
    on one desktop take those from one another, and a test fails with nobody having touched it.
    Anything else on the desktop does the same, which is why a developer running the suite has
    their own typing land in an invisible test window.

    Windows hands a process the desktop it was started on, so every host gets one of its own here
    and the hosts can then run side by side. Where the window station will not give out desktops,
    which can happen to a build agent running under a service account, the frameworks run one
    after another on the desktop we already have instead. That is slower, and it is the same suite.
#>
param(
    [string] $Project = 'src/Mahapps.Metro.Tests/Mahapps.Metro.Tests.csproj',
    [string] $Configuration = 'Release',
    [string] $ResultsDirectory = 'TestResults',
    [string] $Verbosity = 'minimal',
    [string[]] $TargetFrameworks
)

$ErrorActionPreference = 'Stop'

Set-Location -LiteralPath $PSScriptRoot

Add-Type -TypeDefinition @'
using System;
using System.Runtime.InteropServices;

public static class MahAppsDesktop
{
    public const int GenericAll = 0x10000000;
    public const int CreateNewConsole = 0x00000010;

    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "CreateDesktopW")]
    public static extern IntPtr CreateDesktop(string desktop, IntPtr device, IntPtr deviceMode, int flags, int access, IntPtr attributes);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool CloseDesktop(IntPtr desktop);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct StartupInformation
    {
        public int cb;
        public string lpReserved;
        public string lpDesktop;
        public string lpTitle;
        public int dwX, dwY, dwXSize, dwYSize, dwXCountChars, dwYCountChars, dwFillAttribute, dwFlags;
        public short wShowWindow, cbReserved2;
        public IntPtr lpReserved2, hStdInput, hStdOutput, hStdError;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ProcessInformation
    {
        public IntPtr hProcess, hThread;
        public int dwProcessId, dwThreadId;
    }

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern bool CreateProcess(IntPtr application, string commandLine, IntPtr processAttributes, IntPtr threadAttributes,
                                            bool inheritHandles, int flags, IntPtr environment, string currentDirectory,
                                            ref StartupInformation startupInformation, out ProcessInformation information);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern int WaitForSingleObject(IntPtr handle, int milliseconds);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool GetExitCodeProcess(IntPtr handle, out int code);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool TerminateProcess(IntPtr handle, int code);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool CloseHandle(IntPtr handle);
}
'@

function Get-LastErrorMessage
{
    $code = [Runtime.InteropServices.Marshal]::GetLastWin32Error()
    $message = (New-Object System.ComponentModel.Win32Exception($code)).Message

    return "$code, $message"
}

function Get-TestArguments([string] $framework)
{
    # The blame collector cuts a run off after five minutes without a finished test, because a UI
    # test waiting on an event nobody raises waits forever, and what it writes next to the results
    # names the test that stopped moving. The dump type has to stay spelled out: left alone it is
    # full, and a full dump of the test host carries the signing secrets it inherited through its
    # environment.
    #
    # The results file is named after the framework, so that four of them starting in the same
    # second cannot land on the same name.
    return @(
        'test', $Project,
        '--framework', $framework,
        '--configuration', $Configuration,
        '--no-build', '--no-restore',
        '--logger', ('trx;LogFileName=' + $framework + '.trx'),
        '--results-directory', $ResultsDirectory,
        '--verbosity', $Verbosity,
        '--blame-hang', '--blame-hang-timeout', '5m', '--blame-hang-dump-type', 'mini'
    )
}

function ConvertTo-CommandLine([string] $program, [string[]] $arguments)
{
    $quoted = $arguments | ForEach-Object { if ($_ -match '[ ;]') { '"' + $_ + '"' } else { $_ } }

    return '"' + $program + '" ' + ($quoted -join ' ')
}

if (-not $TargetFrameworks)
{
    $projectFile = [xml](Get-Content -LiteralPath $Project)
    $TargetFrameworks = $projectFile.Project.PropertyGroup.TargetFrameworks | Where-Object { $_ }
}

# powershell -File hands every argument over as it stands, so a list of frameworks arrives as one
# string, and so does the one the project declares
$TargetFrameworks = ($TargetFrameworks -split '[;,]') | ForEach-Object { $_.Trim() } | Where-Object { $_ }

if (-not $TargetFrameworks)
{
    throw "no target frameworks to test: $Project declares none and none were passed"
}

if (-not (Test-Path -LiteralPath $ResultsDirectory))
{
    [void](New-Item -ItemType Directory -Path $ResultsDirectory)
}

$dotnet = (Get-Command dotnet).Source

# A desktop for every framework, or none at all: the first refusal sends the whole run down the
# road that needs no desktops, instead of leaving half the hosts sharing one.
$desktops = @{}
foreach ($framework in $TargetFrameworks)
{
    $name = 'MahAppsTests_' + ($framework -replace '[^A-Za-z0-9]', '_') + '_' + $PID
    $handle = [MahAppsDesktop]::CreateDesktop($name, [IntPtr]::Zero, [IntPtr]::Zero, 0, [MahAppsDesktop]::GenericAll, [IntPtr]::Zero)

    if ($handle -eq [IntPtr]::Zero)
    {
        Write-Host "A desktop of its own was refused for $framework ($(Get-LastErrorMessage)), so the frameworks run one after another."

        foreach ($open in $desktops.Values) { [void][MahAppsDesktop]::CloseDesktop($open.Handle) }
        $desktops = $null
        break
    }

    $desktops[$framework] = @{ Name = $name; Handle = $handle }
}

$exitCode = 0

if ($null -eq $desktops)
{
    foreach ($framework in $TargetFrameworks)
    {
        Write-Host "Testing $framework"

        & $dotnet @(Get-TestArguments $framework)

        if ($LASTEXITCODE -ne 0) { $exitCode = $LASTEXITCODE }
    }

    exit $exitCode
}

# cmd takes the redirection, which spares this script a set of inherited handles. With /s it
# strips the outer pair of quotes and leaves everything inside alone, so a path with a space in
# it survives the trip.
$running = @()
try
{
    foreach ($framework in $TargetFrameworks)
    {
        $desktop = $desktops[$framework]
        $log = Join-Path $ResultsDirectory ($framework + '.log')

        $startupInformation = New-Object MahAppsDesktop+StartupInformation
        $startupInformation.cb = [Runtime.InteropServices.Marshal]::SizeOf($startupInformation)
        $startupInformation.lpDesktop = $desktop.Name

        $information = New-Object MahAppsDesktop+ProcessInformation
        $commandLine = 'cmd.exe /s /c "' + (ConvertTo-CommandLine $dotnet (Get-TestArguments $framework)) + ' > "' + $log + '" 2>&1"'

        $started = [MahAppsDesktop]::CreateProcess([IntPtr]::Zero, $commandLine, [IntPtr]::Zero, [IntPtr]::Zero, $false,
                                                   [MahAppsDesktop]::CreateNewConsole, [IntPtr]::Zero, $PSScriptRoot,
                                                   [ref] $startupInformation, [ref] $information)

        if (-not $started)
        {
            throw "the test host for $framework could not be started on $($desktop.Name) ($(Get-LastErrorMessage))"
        }

        Write-Host "Testing $framework in process $($information.dwProcessId) on desktop $($desktop.Name)"

        $running += @{ Framework = $framework; Information = $information; Log = $log }
    }

    foreach ($run in $running)
    {
        [void][MahAppsDesktop]::WaitForSingleObject($run.Information.hProcess, -1)

        $code = 0
        [void][MahAppsDesktop]::GetExitCodeProcess($run.Information.hProcess, [ref] $code)
        if ($code -ne 0) { $exitCode = $code }

        # every host wrote to a file of its own, so the reading happens here, in one piece per
        # framework, which is the only way a run of four of them is worth reading afterwards
        Write-Host ''
        Write-Host "======== $($run.Framework) ========"
        if (Test-Path -LiteralPath $run.Log) { Get-Content -LiteralPath $run.Log | Write-Host }
        Write-Host "$($run.Framework) finished with exit code $code"
    }
}
finally
{
    foreach ($run in $running)
    {
        $code = 0
        [void][MahAppsDesktop]::GetExitCodeProcess($run.Information.hProcess, [ref] $code)
        if ($code -eq 259) { [void][MahAppsDesktop]::TerminateProcess($run.Information.hProcess, 1) }

        [void][MahAppsDesktop]::CloseHandle($run.Information.hThread)
        [void][MahAppsDesktop]::CloseHandle($run.Information.hProcess)
    }

    foreach ($desktop in $desktops.Values)
    {
        [void][MahAppsDesktop]::CloseDesktop($desktop.Handle)
    }
}

exit $exitCode
