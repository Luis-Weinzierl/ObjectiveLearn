; The name of the installer
Name "Objective: Learn"

; The file to write
OutFile "objectivelearnsetup.exe"

; Request application privileges for Windows Vista and higher
RequestExecutionLevel admin

; Build Unicode installer
Unicode True

; The default installation directory
InstallDir $PROGRAMFILES64\ObjectiveLearn

; Registry key to check for directory (so if you install again, it will 
; overwrite the old one automatically)
InstallDirRegKey HKLM "Software\LuisWeinzierl_ObjectiveLearn" "Install_Dir"

;--------------------------------

; Pages

Page components
Page directory
Page instfiles

UninstPage uninstConfirm
UninstPage instfiles

;--------------------------------

; The stuff to install
Section "ObjectiveLearn (required)"

  SectionIn RO
  
  ; Set output path to the installation directory.
  SetOutPath $INSTDIR
  
  ; Put file there
  File /r ".\ObjectiveLearn\bin\Wpf\Release\net6.0-windows\win-x64\*"
  
  ; Write the installation path into the registry
  WriteRegStr HKLM SOFTWARE\LuisWeinzierl_ObjectiveLearn "Install_Dir" "$INSTDIR"
  
  ; Write the uninstall keys for Windows
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\ObjectiveLearn" "DisplayName" "NSIS ObjectiveLearn"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\ObjectiveLearn" "UninstallString" '"$INSTDIR\uninstall.exe"'
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\ObjectiveLearn" "NoModify" 1
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\ObjectiveLearn" "NoRepair" 1
  WriteUninstaller "$INSTDIR\uninstall.exe"
  
SectionEnd

; Optional section (can be disabled by the user)
Section "Start Menu Shortcuts"

  CreateDirectory "$SMPROGRAMS\ObjectiveLearn"
  CreateShortcut "$SMPROGRAMS\ObjectiveLearn\Uninstall.lnk" "$INSTDIR\uninstall.exe"
  CreateShortcut "$SMPROGRAMS\ObjectiveLearn\ObjectiveLearn.lnk" "$INSTDIR\ObjectiveLearn.exe"

SectionEnd

;--------------------------------

; Uninstaller

Section "Uninstall"
  
  ; Remove registry keys
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\ObjectiveLearn"
  DeleteRegKey HKLM SOFTWARE\LuisWeinzierl_ObjectiveLearn

  ; Remove files and uninstaller
  Delete $INSTDIR

  ; Remove shortcuts, if any
  Delete "$SMPROGRAMS\ObjectiveLearn\*.lnk"

  ; Remove directories
  RMDir "$SMPROGRAMS\ObjectiveLearn"
  RMDir "$INSTDIR"

SectionEnd
