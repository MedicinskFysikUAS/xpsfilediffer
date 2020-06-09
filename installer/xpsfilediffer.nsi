
!define ApplicationRootName "jmfXps"
!define ApplicationName "${ApplicationRootName}-${VERSION}"
!define CompanyName "SF_Mjukvara"
!define uninstallName "Avinstallera-${ApplicationName}"
!define InstallDir "C:\${CompanyName}"
!define ApplicationDir "${InstallDir}\${ApplicationName}"
!define installerOutDir "installer"
!define installerDirOutDirPath "${ApplicationDir}\${installerOutDir}"
!define installerDir "."

## The name of the exe must be setup_dcmkoll-3.3.0.306.exe to be able to run it
OutFile "..\buildArtifacts\setup_dcmkoll-3.3.0.306.exe"

RequestExecutionLevel user

# Uninstall previous versions
# -------------------------------------------------------------------------
Section "RemovePrevious"
## Find previous versions
FindFirst $0 $1 "${InstallDir}\${ApplicationRootName}*"

loop:
  StrCmp $1 "" done
  MessageBox MB_YESNO "En tidigare versioner av ${ApplicationRootName} finns redan installerad. Vill du avinstallera $1?" IDYES true IDNO false
  true:
	DetailPrint "${InstallDir}\$1\uninstaller.exe"
	ExecWait '"${InstallDir}\$1\uninstaller.exe"'
	Goto next
  false:
	Goto next
  next:
	FindNext $0 $1
	Goto loop
done:
FindClose $0

SectionEnd


# Installation section 
# -------------------------------------------------------------------------
Section "Install"

## Test:
##SetShellVarContext all

!define xpsfiledifferOutDir "xpsfilediffer"
!define xpsfiledifferDir "..\${xpsfiledifferOutDir}"
!define xpsfiledifferOutDirPath "${ApplicationDir}\${xpsfiledifferOutDir}"

!define publishFolderOutDir "publishFolder"
!define publishFolderDir "..\${publishFolderOutDir}"
!define publishFolderOutDirPath "${ApplicationDir}\${publishFolderOutDir}"

MessageBox MB_YESNO "En genvag till applikationen kommer att skapas pa skrivbordet for den anvandare som gor installationen. Man bor darfor inte kora denna installer som administrator.$\r$\n$\r$\nVill du fortsatta?" IDYES true IDNO false
true:
	Goto next
false:
	Quit

next:

# create a popup box, with an OK button and some text
MessageBox MB_OK "${ApplicationName} kommer att installeras under ${InstallDir}."

# Creates the installation directories
CreateDirectory "${InstallDir}"
CreateDirectory "${ApplicationDir}" 
CreateDirectory "${ApplicationDir}\${xpsfiledifferOutDir}" 
CreateDirectory "${ApplicationDir}\${publishFolderOutDir}" 
  
# Start menu
##CreateDirectory "$SMPROGRAMS\${CompanyName}"

FileOpen $0 "${ApplicationDir}\${ApplicationName}_installation.log" w
!define /date MYTIMESTAMP "%Y-%m-%d_%H:%M:%S"
FileWrite $0 "${MYTIMESTAMP} Starting installlation of ${ApplicationName}.${VERSION}"
FileClose $0

# Installation files
SetOutPath "${installerDirOutDirPath}"
File "${installerDir}\xpsDiff.ico"
## Note: The vbsScriptName must be created before creating the installer.
## There is a batch script for creating the vbs file. 
## The name of the vbs file is hard coded in that batch script.
#!define vbsScriptName "mkDesktopLink.vbs"
!define vbsScriptName "mkDesktopLink.vbs"
File "${installerDir}\${vbsScriptName}"
 
# xpsfilediffer files
SetOutPath "${xpsfiledifferOutDir}"
File "${installerDir}\xpsDiff.ico"

SetOutPath "${publishFolderOutDirPath}"
File "${publishFolderDir}\*.*"
File "${installerDir}\runXpsfilediffer.bat"
File "${installerDir}\runXpsfilediffer.ps1"

ExpandEnvStrings $0 %COMSPEC%


ExecWait '"$0" /C "${ApplicationDir}\${installerOutDir}\${vbsScriptName}" ${ApplicationName} ${xpsfiledifferOutDir}\runXpsfilediffer.bat ${installerDirOutDirPath}\xpsDiff.ico ${xpsfiledifferOutDir} %userprofile%'

# define uninstaller name
WriteUninstaller "${ApplicationDir}\uninstaller.exe"

# Show Success message.
MessageBox MB_OK "${ApplicationName} har installerats."
# end the section
SectionEnd


# Uninstallation section 
# -------------------------------------------------------------------------
Section "Uninstall"

MessageBox MB_YESNO "${ApplicationName} kommer att avinstalleras.$\r$\n$\r$\nVill du fortsatta?" IDYES true IDNO false
true:
	Goto next
false:
	Quit

next:
 
# Always delete uninstaller first
Delete "${ApplicationDir}\uninstaller.exe"

# Remove desktop link
Delete "$DESKTOP\${ApplicationName}.lnk"

# If all files have been installed into the Installation directory it is
# sufficient  to just remove this recursively
RMDir /r "${ApplicationDir}"

MessageBox MB_OK "${ApplicationName} har avinstallerats."
SectionEnd
