@echo off
set "PathToPsScript=%~dp0runXpsfilediffer.ps1"

@echo off
rem @echo "Path to ps script: %PathToPsScript%"

@echo on
@echo "Kontrollerar utskriftsfiler ..."

@echo off
powershell -executionpolicy bypass -File %PathToPsScript%