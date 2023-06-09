@echo off
echo. 2>build.log

set "version=1.0.0"
rem get the build number from git
for /f %%i in ('git rev-list --count HEAD') do set REVNR=%%i
echo The build number is %REVNR%
set buildNumber=%REVNR%%
rem the branch name
for /f %%i in ('git name-rev --name-only HEAD') do set BRANCHNAME=%%i
echo The branch name is %BRANCHNAME%

if %BRANCHNAME% == master (
	echo Building new release
	set "version=%version%.%buildNumber%"
) ELSE (
	echo Building development branch
	set "version=%version%.%buildNumber%.develop"
)


echo "You must publish the application in Visual Studio!!"

echo "Building installer for version jmfXps %version%"
rem build the installer
START "Create installer" /B /wait makensis.exe "/DVERSION=%version%" "xpsDifferInstaller.nsi" >> build.log

rem create directory in buildArtifacts directory named according to the release name
set releaseName=C:\SF_Mjukvara\buildArtifacts\setup_jmfXps-%version%
echo %releaseName%
if not exist %releaseName% mkdir %releaseName%
xcopy /y "C:\SF_Mjukvara\buildArtifacts\tmp\setup_jmfXps.exe" %releaseName%\