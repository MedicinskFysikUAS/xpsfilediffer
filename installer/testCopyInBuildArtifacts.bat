@echo off
echo "start"
set version=3.3.0.400
echo %version%
set releaseName=..\buildArtifacts\setup_dcmKoll-%version%
echo %releaseName%
if not exist %releaseName% mkdir %releaseName%
xcopy /y ..\buildArtifacts\setup_dcmkoll-3.3.0.306.exe %releaseName%\