@echo on
call "%VS100COMNTOOLS%vsvars32.bat"
mkdir log\
mkdir lib\net40\

msbuild.exe /ToolsVersion:4.0 "..\MahApps.Metro\MahApps.Metro.csproj" /p:OutDir=%~dp0lib\net40\

..\Utilities\NuGet.exe pack ..\MahApps.Metro.nuspec -BasePath "%~dp0\"
pause