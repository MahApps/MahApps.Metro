@echo on
call "%VS100COMNTOOLS%vsvars32.bat"

msbuild.exe /ToolsVersion:4.0 "..\MahApps.Metro\MahApps.Metro.csproj" /p:StrongName=True /p:configuration=Release
msbuild.exe /ToolsVersion:4.0 "..\MahApps.Metro\MahApps.Metro.NET45.csproj" /p:StrongName=True /p:configuration=Release

..\Utilities\NuGet.exe pack %~dp0..\MahApps.Metro\MahApps.Metro.nuspec -OutputDirectory %~dp0
pause