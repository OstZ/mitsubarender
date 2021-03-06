!include "MUI.nsh"
!include "LogicLib.nsh"
!include "nsProcess.nsh"

;Definitions
!define PRODUCT_NAME "Mitsuba Render"
!define PRODUCT_VERSION "1.0"
!define PRODUCT_SR "0"
!define PRODUCT_PUBLISHER "TDM Solutions SL"
!define PRODUCT_WEB_SITE "http://tdmsolutions.github.io/mitsubarender/"
!define PRODUCT_UNINST_KEY "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}"
!define PRODUCT_UNINST_ROOT_KEY "HKLM"
!define PLUGIN_UUID "24b6e560-183d-4898-bf4c-e1d721f46258"
!define SHORT_APP_NAME "Mitsuba Render"
!define SUPPORT_EMAIL "support@tdmsolutions.com"
!define PLUGIN_FILENAME "MitsubaRender.rhp"
!define DOTNET_VERSION "4"

;Name and file
Name "${PRODUCT_NAME} ${PRODUCT_VERSION}"
OutFile "MistubaRender_1.0_(2014-03-13).exe"
InstallDir "$PROGRAMFILES64\${PRODUCT_NAME} ${PRODUCT_VERSION}"

; Claves de registro de Rhino
!define RHINO_ROOT_KEY "HKLM"
!define RHINO_5_KEY64 "Software\McNeel\Rhinoceros\5.0x64\"
!define RHINO_5_KEY32 "Software\McNeel\Rhinoceros\5.0\"

;ShowInstDetails show
ShowUnInstDetails show


;Get installation folder from registry if available
InstallDirRegKey HKCU "Software\${PRODUCT_NAME} ${PRODUCT_VERSION}" ""

BrandingText /TRIMCENTER "TDM Solutions SL"
;--------------------------------
;Interface Settings
!define MUI_ABORTWARNING
!define OMUI_THEME "CD-Clean-Clayoo"
  
; MUI Settings / Icons
!define MUI_LANGDLL_ALLLANGUAGES

; In the moment of writing this, NSIS don't support well Vista icons with PNG compression.
; We provide both, compressed and uncompressed (-nopng) icons.
!define MUI_ICON "C:\dev\mitsubarender\install\art\mitsuba.ico"
!define MUI_UNICON "C:\dev\mitsubarender\install\art\mitsuba.ico"
 
; MUI Settings / Header
!define MUI_HEADERIMAGE
!define MUI_HEADERIMAGE_RIGHT
!define MUI_HEADERIMAGE_BITMAP "art\header.bmp"
!define MUI_HEADERIMAGE_UNBITMAP "art\header.bmp"
 
; MUI Settings / Wizard		
!define MUI_WELCOMEFINISHPAGE_BITMAP "art\wizard.bmp"
!define MUI_UNWELCOMEFINISHPAGE_BITMAP "art\wizard.bmp"  

;--------------------------------
;Pages
!insertmacro MUI_PAGE_WELCOME
!insertmacro MUI_PAGE_LICENSE "$(license)"
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_PAGE_FINISH
!insertmacro MUI_UNPAGE_WELCOME
!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES
!insertmacro MUI_UNPAGE_FINISH

; Uninstaller pages
!insertmacro MUI_UNPAGE_INSTFILES
;--------------------------------
;Languages idiomas soportados:
!insertmacro MUI_LANGUAGE "English"
!insertmacro MUI_LANGUAGE "Spanish"
!insertmacro MUI_LANGUAGE "Japanese"
!insertmacro MUI_LANGUAGE "German"
!insertmacro MUI_LANGUAGE "Italian"
!insertmacro MUI_LANGUAGE "French"
!insertmacro MUI_LANGUAGE "SimpChinese"
!insertmacro MUI_LANGUAGE "TradChinese"
!insertmacro MUI_LANGUAGE "Czech"
!insertmacro MUI_LANGUAGE "Korean"

LicenseLangString license ${LANG_ENGLISH} "agreement/en-us.rtf"
LicenseLangString license ${LANG_SPANISH} "agreement/en-us.rtf"
LicenseLangString license ${LANG_JAPANESE} "agreement/en-us.rtf"
LicenseLangString license ${LANG_GERMAN} "agreement/en-us.rtf"
LicenseLangString license ${LANG_ITALIAN} "agreement/en-us.rtf"
LicenseLangString license ${LANG_FRENCH} "agreement/en-us.rtf"
LicenseLangString license ${LANG_SIMPCHINESE} "agreement/en-us.rtf"
LicenseLangString license ${LANG_TRADCHINESE} "agreement/en-us.rtf"
LicenseLangString license ${LANG_KOREAN} "agreement/en-us.rtf"
LicenseLangString license ${LANG_CZECH} "agreement/en-us.rtf"

; Check if Rhino is running
!macro CHECK_RHINO_RUNNING

	${nsProcess::FindProcess} "Rhino.exe" $R0
	StrCmp $R0 0 0 end

	    ${IF} $LANGUAGE = ${LANG_ENGLISH}
				MessageBox MB_ICONEXCLAMATION|MB_OK  "Please, close Rhinoceros before proceeding with the installation."
				${nsProcess::Unload}
				Abort "Rhino is running!"
		${ELSEIF} $LANGUAGE = ${LANG_SPANISH}
				MessageBox MB_ICONEXCLAMATION|MB_OK "Por favor, cierre Rhinoceros antes de continuar la instalación."
				${nsProcess::Unload}
				Abort "Rhino is running!"
        ${ELSEIF} $LANGUAGE = ${LANG_JAPANESE}
				MessageBox MB_ICONEXCLAMATION|MB_OK "ｲﾝｽﾄｰﾙを続ける前に、Rhinocerosを閉じてください。"
				${nsProcess::Unload}
				Abort "Rhino is running!"
	    ${ELSEIF} $LANGUAGE = ${LANG_GERMAN}
				MessageBox MB_ICONEXCLAMATION|MB_OK "Bitte schließen Sie Rhino, bevor Sie mit der Installation fortfahren."
				${nsProcess::Unload}
				Abort "Rhino is running!"
		${ELSEIF} $LANGUAGE = ${LANG_ITALIAN}
				MessageBox MB_ICONEXCLAMATION|MB_OK "Chiudere Rhino prima di procedere con l'installazione."
				${nsProcess::Unload}
				Abort "Rhino is running!"
		${ELSEIF} $LANGUAGE = ${LANG_FRENCH}
				MessageBox MB_ICONEXCLAMATION|MB_OK "S'il vous plaît, Rhinocéros à proximité avant de procéder à l'installation."
				${nsProcess::Unload}
				Abort "Rhino is running!"			 
        ${ENDIF}
	 
		end:
	${nsProcess::Unload}
	
!macroEnd


!ifndef ___X64__NSH___
!define ___X64__NSH___

!include LogicLib.nsh

!macro _RunningX64 _a _b _t _f
  !insertmacro _LOGICLIB_TEMP
  System::Call kernel32::GetCurrentProcess()i.s
  System::Call kernel32::IsWow64Process(is,*i.s)
  Pop $_LOGICLIB_TEMP
  !insertmacro _!= $_LOGICLIB_TEMP 0 `${_t}` `${_f}`
!macroend

!define RunningX64 `"" RunningX64 ""`

!macro DisableX64FSRedirection

  System::Call kernel32::Wow64EnableWow64FsRedirection(i0)

!macroend

!define DisableX64FSRedirection "!insertmacro DisableX64FSRedirection"

!macro EnableX64FSRedirection

  System::Call kernel32::Wow64EnableWow64FsRedirection(i1)

!macroend

!define EnableX64FSRedirection "!insertmacro EnableX64FSRedirection"

!endif # !___X64__NSH___


!macro REGISTER_RHINO_5_PLUGIN PLUGIN_UUID PLUGIN_PATH

 ${If} ${RunningX64}
		SetRegView 64
		WriteRegStr HKLM "${RHINO_5_KEY64}\Plug-ins\${PLUGIN_UUID}" "FileName" "${PLUGIN_PATH}"
		WriteRegStr HKLM "${RHINO_5_KEY64}\Plug-ins\${PLUGIN_UUID}" "Name" "${PRODUCT_NAME}"
${Else}
		SetRegView 32
		WriteRegStr HKLM "${RHINO_5_KEY32}\Plug-ins\${PLUGIN_UUID}" "FileName" "${PLUGIN_PATH}"
		WriteRegStr HKLM "${RHINO_5_KEY32}\Plug-ins\${PLUGIN_UUID}" "Name" "${PRODUCT_NAME}"
${EndIf}
!macroEnd

 Function .onInit
 
	; Definim el idioma
	!insertmacro MUI_RESERVEFILE_LANGDLL
	!insertmacro MUI_LANGDLL_DISPLAY	 
	;Comprovem si rhino esta funcionant
	!insertmacro CHECK_RHINO_RUNNING

FunctionEnd

Section

	; Copiem el fitxer de sistema
	SetOutPath "$INSTDIR\System"
	File "Common\*.*"
	
	;Copiem els fitxers de Mitsuba
	SetOutPath "$INSTDIR\System\Mitsuba"
	File  /r  "mitsuba\Mitsuba 0.5.0\*.*"
	
  	;Registramos el plugin
	!insertmacro REGISTER_RHINO_5_PLUGIN ${PLUGIN_UUID} "$INSTDIR\System\${PLUGIN_FILENAME}"

	;Execute vcredist
    ExecWait '"$INSTDIR\System\vcredist_2013_x64.exe"'
			
SectionEnd


;//////////////////////////////////////////////
;///////////////////////////////////////////////
;///////////////////////////////////////////////
;///////////////////////////////////////////////
 

;--------------------------------
;Uninstaller Section


Section -AdditionalIcons
	SetShellVarContext all
	
 SectionEnd

Section -Post
  WriteUninstaller "$INSTDIR\System\uninst.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayName" "$(^Name)"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "UninstallString" "$INSTDIR\System\uninst.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayVersion" "${PRODUCT_VERSION}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "URLInfoAbout" "${PRODUCT_WEB_SITE}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "Publisher" "${PRODUCT_PUBLISHER}"
SectionEnd

Function un.onUninstSuccess
	;MessageBox MB_ICONEXCLAMATION|MB_OK $(UNINSTALL_COMPLETE)
FunctionEnd

Function un.onInit
!insertmacro MUI_UNGETLANGUAGE
;MessageBox MB_ICONQUESTION|MB_YESNO|MB_DEFBUTTON2 $(ASK_UNINSTALL)  IDYES +2
 ;Abort
FunctionEnd

Section "Uninstall"
  ;!insertmacro CHECK_RHINO_RUNNING
  SetShellVarContext all
  RMDir /r  "$SMPROGRAMS\${PRODUCT_NAME} ${PRODUCT_VERSION}"
  Delete "$INSTDIR\*.*"

  RMDir /r "$INSTDIR\System\x86"
  RMDir /r "$INSTDIR\System\x64"
  RMDir /r "$INSTDIR\System\cs"
  RMDir /r "$INSTDIR\System\de"
  RMDir /r "$INSTDIR\System\es"
  RMDir /r "$INSTDIR\System\fr"
  RMDir /r "$INSTDIR\System\it"
  RMDir /r "$INSTDIR\System\ja"
  RMDir /r "$INSTDIR\System\ko" 
  RMDir /r "$INSTDIR\System\zh-CN" 
  RMDir /r  "$INSTDIR\System\"
RMDir /r  "$INSTDIR\"

  DeleteRegKey ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}"

  RMDir "$INSTDIR"
  DeleteRegKey /ifempty HKCU "Software\${PRODUCT_NAME} ${PRODUCT_VERSION}"
  SetAutoClose true
SectionEnd
!macro OLDCheckDotNET DotNetReqVer
 MessageBox MB_ICONEXCLAMATION "Checking DotNet"
   !define DOTNET_URL "http://www.microsoft.com/downloads/info.aspx?na=90&p=&SrcDisplayLang=en&SrcCategoryId=&SrcFamilyId=0856eacb-4362-4b0d-8edd-aab15c5e04f5&u=http%3a%2f%2fdownload.microsoft.com%2fdownload%2f5%2f6%2f7%2f567758a3-759e-473e-bf8f-52154438565a%2fdotnetfx.exe"
  DetailPrint "Checking your .NET Framework version..."
  ;callee register save
  Push $0
  Push $1
  Push $2
  Push $3
  Push $4
  Push $5
  Push $6 ;backup of intsalled ver
  Push $7 ;backup of DoNetReqVer
 
  StrCpy $7 ${DotNetReqVer}
 
  System::Call "mscoree::GetCORVersion(w .r0, i ${NSIS_MAX_STRLEN}, *i r2r2) i .r1 ?u"
 
  ${If} $0 == 0
  	DetailPrint ".NET Framework not found, download is required for program to run."
    Goto DownloadDotNET
  ${EndIf}
 
  ;at this point, $0 has maybe v2.345.678.
  StrCpy $0 $0 $2 1 ;remove the starting "v", $0 has the installed version num as a string
  StrCpy $6 $0
  StrCpy $1 $7 ;$1 has the requested verison num as a string
 
  ;MessageBox MB_OKCANCEL "found $0" IDCANCEL GiveUpDotNET
    
  ;MessageBox MB_OKCANCEL "looking for $1" IDCANCEL GiveUpDotNET
 
  ;now let's compare the versions, installed against required <part0>.<part1>.<part2>.
  ${Do}
    StrCpy $2 "" ;clear out the installed part
    StrCpy $3 "" ;clear out the required part
 
    ${Do}
      ${If} $0 == "" ;if there are no more characters in the version
        StrCpy $4 "." ;fake the end of the version string
      ${Else}
        StrCpy $4 $0 1 0 ;$4 = character from the installed ver
        ${If} $4 != "."
          StrCpy $0 $0 ${NSIS_MAX_STRLEN} 1 ;remove that first character from the remaining
        ${EndIf}
      ${EndIf}
      
      ${If} $1 == ""  ;if there are no more characters in the version
        StrCpy $5 "." ;fake the end of the version string
      ${Else}
        StrCpy $5 $1 1 0 ;$5 = character from the required ver
        ${If} $5 != "."
          StrCpy $1 $1 ${NSIS_MAX_STRLEN} 1 ;remove that first character from the remaining
        ${EndIf}
      ${EndIf}
      ;MessageBox MB_OKCANCEL "installed $2,$4,$0 required $3,$5,$1" IDCANCEL GiveUpDotNET
      ${If} $4 == "."
      ${AndIf} $5 == "."
        ${ExitDo} ;we're at the end of the part
      ${EndIf}
 
      ${If} $4 == "." ;if we're at the end of the current installed part
        StrCpy $2 "0$2" ;put a zero on the front
      ${Else} ;we have another character
        StrCpy $2 "$2$4" ;put the next character on the back
      ${EndIf}
      ${If} $5 == "." ;if we're at the end of the current required part
        StrCpy $3 "0$3" ;put a zero on the front
      ${Else} ;we have another character
        StrCpy $3 "$3$5" ;put the next character on the back
      ${EndIf}
    ${Loop}
    ;MessageBox MB_OKCANCEL "finished parts: installed $2,$4,$0 required $3,$5,$1" IDCANCEL GiveUpDotNET
 
    ${If} $0 != "" ;let's remove the leading period on installed part if it exists
      StrCpy $0 $0 ${NSIS_MAX_STRLEN} 1
    ${EndIf}
    ${If} $1 != "" ;let's remove the leading period on required part if it exists
      StrCpy $1 $1 ${NSIS_MAX_STRLEN} 1
    ${EndIf}
 
    ;$2 has the installed part, $3 has the required part
    ${If} $2 S< $3
      IntOp $0 0 - 1 ;$0 = -1, installed less than required
      ${ExitDo}
    ${ElseIf} $2 S> $3
      IntOp $0 0 + 1 ;$0 = 1, installed greater than required
      ${ExitDo}
    ${ElseIf} $2 == ""
    ${AndIf} $3 == ""
      IntOp $0 0 + 0 ;$0 = 0, the versions are identical
      ${ExitDo}
    ${EndIf} ;otherwise we just keep looping through the parts
  ${Loop}
 
  ${If} $0 < 0
    DetailPrint ".NET Framework Version found: $6, but is older than the required version: $7"
    Goto DownloadDotNET
  ${Else}
    DetailPrint ".NET Framework Version found: $6, equal or newer to required version: $7."
    Goto NewDotNET
  ${EndIf}
 
NoDotNET:
    MessageBox MB_YESNOCANCEL|MB_ICONEXCLAMATION \
    ".NET Framework not installed.$\nRequired Version: $7 or greater.$\nDownload .NET Framework version from www.microsoft.com?" \
    /SD IDYES IDYES DownloadDotNET IDNO NewDotNET
    goto GiveUpDotNET ;IDCANCEL
OldDotNET:
    MessageBox MB_YESNOCANCEL|MB_ICONEXCLAMATION \
    "Your .NET Framework version: $6.$\nRequired Version: $7 or greater.$\nDownload .NET Framework version from www.microsoft.com?" \
    /SD IDYES IDYES DownloadDotNET IDNO NewDotNET
    goto GiveUpDotNET ;IDCANCEL
 
DownloadDotNET:

is32:
	
	ExecWait '$INSTDIR\dotnetfx.exe /q /c:"install /q"'
   goto newdotnet
is64:
ExecWait '$INSTDIR\NetFx64.exe /q /c:"install /q"'
  ;MessageBox MB_ICONEXCLAMATION "INSTALANDO 64"
  goto NewDotNet
 
GiveUpDotNET:
  Abort "Installation cancelled by user."
 
NewDotNET:
;  DetailPrint "Proceeding with remainder of installation."
  Pop $0
  Pop $1
  Pop $2
  Pop $3
  Pop $4
  Pop $5
  Pop $6 ;backup of intsalled ver
  Pop $7 ;backup of DoNetReqVer
!macroend

