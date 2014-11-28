; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "Wii U USB GCN adapter"
#define MyAppVersion "1.0"
#define MyAppPublisher "Matt Cunningham (Massive)"
#define MyAppURL "https://github.com/elmassivo/GCN-USB-Adapter"
#define MyAppExeName "GCNUSBFeeder.exe"

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{B3898604-95BA-4EBA-A8D7-C4C2BDC2712A}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={pf}\GCNadapter
DefaultGroupName={#MyAppName}
AllowNoIcons=yes
OutputBaseFilename=setup
SetupIconFile=E:\C#\GCNUSBFeeder\GCNUSBFeeder\icon.ico
Compression=lzma
SolidCompression=yes

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "E:\C#\GCNUSBFeeder\GCNUSBFeeder\bin\x86\Release\GCNUSBFeeder.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\C#\GCNUSBFeeder\GCNUSBFeeder\bin\x86\Release\GCNUSBFeeder.vshost.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\C#\GCNUSBFeeder\GCNUSBFeeder\bin\x86\Release\LibUsbDotNet.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\C#\GCNUSBFeeder\GCNUSBFeeder\bin\x86\Release\LibUsbDotNet.xml"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\C#\GCNUSBFeeder\GCNUSBFeeder\bin\x86\Release\vJoyInterface.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\C#\GCNUSBFeeder\GCNUSBFeeder\bin\x86\Release\vJoyInterfaceWrap.dll"; DestDir: "{app}"; Flags: ignoreversion
; NOTE: Don't use "Flags: ignoreversion" on any shared system files
Source: "E:\GCNController\LibUSB\device specification.htm"; DestDir: "{app}\LibUSB"; Flags: ignoreversion
Source: "E:\GCNController\LibUSB\install.bat"; DestDir: "{app}\LibUSB"; Flags: ignoreversion
Source: "E:\GCNController\LibUSB\uninstall.bat"; DestDir: "{app}\LibUSB"; Flags: ignoreversion
Source: "E:\GCNController\LibUSB\installer_x64.exe"; DestDir: "{app}\LibUSB"; Flags: ignoreversion
Source: "E:\GCNController\LibUSB\installer_x86.exe"; DestDir: "{app}\LibUSB"; Flags: ignoreversion
Source: "E:\GCNController\LibUSB\install-filter.exe"; DestDir: "{app}\LibUSB"; Flags: ignoreversion
Source: "E:\GCNController\LibUSB\libusb-win32-bin-README.txt"; DestDir: "{app}\LibUSB"; Flags: ignoreversion
Source: "E:\GCNController\LibUSB\WUP-028.cat"; DestDir: "{app}\LibUSB"; Flags: ignoreversion
Source: "E:\GCNController\LibUSB\WUP-028.inf"; DestDir: "{app}\LibUSB"; Flags: ignoreversion
Source: "E:\GCNController\vJoy_204_I220914.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\GCNController\ConfigJoysticks.bat"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\GCNController\UninstallJoysticks.bat"; DestDir: "{app}"

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\LibUSB\install.bat"; WorkingDir: "{app}\LibUSB"; Flags: shellexec waituntilterminated
Filename: "{app}\vJoy_204_I220914.exe"; WorkingDir: "{app}"; Flags: waituntilterminated
Filename: "{app}\ConfigJoysticks.bat"; WorkingDir: "{app}\vJoy"; Flags: shellexec waituntilterminated
Filename: "{app}\{#MyAppExeName}"; Flags: nowait postinstall skipifsilent; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"

[Messages]
english.WizardInfoBefore=Please make sure the Wii U Gamecube Adapter with no Controllers is plugged into your computer before running this installer.

[Dirs]
Name: "{app}\LibUSB"; Flags: deleteafterinstall
Name: "{app}\vJoy"; Flags: deleteafterinstall

[UninstallRun]
Filename: "{app}\LibUSB\uninstall.bat"; WorkingDir: "{app}\LibUSB"; Flags: shellexec
Filename: "{app}\UninstallJoysticks.bat"; WorkingDir: "{app}"; Flags: shellexec