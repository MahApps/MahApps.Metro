function Get-ScriptDirectory
{
   $Invocation = (Get-Variable MyInvocation -Scope 1).Value
   Split-Path $Invocation.MyCommand.Path
}

# build the solution from scratch
$build_number = "0.11.0.0"
$version = "v4.0.30319"
$solution_dir = Join-Path (Get-ScriptDirectory) ..
$script = Join-Path (Get-ScriptDirectory) MahApps.Metro.proj
$proj = Join-Path (Get-ScriptDirectory) ..\MahApps.Metro\MahApps.Metro.csproj

. $env:windir\Microsoft.NET\Framework\$version\MSBuild.exe $script /t:Version /ToolsVersion:4.0 /p:SolutionDir=$solution_dir /p:BUILD_NUMBER=$build_number

. $env:windir\Microsoft.NET\Framework\$version\MSBuild.exe $proj /t:Rebuild /ToolsVersion:4.0 /p:StrongName=False /p:configuration=Release /m /v:q /p:BUILD_NUMBER=$build_number

$proj = Join-Path (Get-ScriptDirectory) ..\MahApps.Metro\MahApps.Metro.NET45.csproj

. $env:windir\Microsoft.NET\Framework\$version\MSBuild.exe $proj /t:Rebuild /ToolsVersion:4.0 /p:StrongName=False /p:configuration=Release /m /v:q /p:BUILD_NUMBER=$build_number

# package it up

$nuget = Join-Path (Get-ScriptDirectory) ..\Utilities\NuGet.exe
$nuspec = Join-Path (Get-ScriptDirectory) ..\MahApps.Metro\MahApps.Metro.nuspec

. $nuget pack $nuspec -OutputDirectory (Get-ScriptDirectory) -Version $build_number