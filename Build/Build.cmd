@echo on
call "%VS100COMNTOOLS%vsvars32.bat"
mkdir log\
mkdir lib\net40\

msbuild.exe /ToolsVersion:4.0 "..\MahApps.Metro\MahApps.Metro.csproj" /p:configuration=Release
..\Utilities\NuGet.exe pack %~dp0..\MahApps.Metro\MahApps.Metro.nuspec -OutputDirectory %~dp0
pause