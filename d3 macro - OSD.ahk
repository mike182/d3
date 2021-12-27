; This is an AutoHotKey script designed to be used with Diablo III
;
; INFO - setting hotkeys
; set the hotkey by changing the text BEFORE the double colons
; example>>>       HOTKEY_HERE::randomText()       <<<example
; ! = alt key
; + = shift key
; ^ = crtl key
; # = windows key
; EXAMPLE      ^!f::randomText()     would activate macro by pressing crtl + alt + f at the same time
; EXAMPLE      q::randomText()       would actiavte macro by just pressing the q key


#Persistent
#SingleInstance Force
#NoEnv
process, close, Agent.exe
GameBarPresenceWriter.exe
process, close, RemindersServer.exe

; init
SetKeyDelay 75, 50
SetTimer, Spam, 100

; var
FirstKeyPress := true 
ScriptRun := true
EmptyKey := "."
EmptyRemap := "..."
ScriptOn := "******"
SpaceRemap := "..."
salvageDelay := 1	; Set delay when using auto salvage (lower = faster | higher = slower)

debugText := "Running"

; Gui Menu
Gui, Menu:Margin, 15, 9
Gui, Menu:+AlwaysOnTop
Gui, Menu:Font, s10, Consolas
Gui, Menu:Color, Black
Gui, Menu:Font, cRed
Gui, Menu:Add, Text, Section, D3 MACROS
Gui, Menu:Add, Text, xs, =========
Gui, Menu:add, Button, ys vScriptRun gScriptRun, Running
Gui, Menu:Add, CheckBox, xs vBuildRat gBuildRat,   rat   |  hexing macro
Gui, Menu:Add, CheckBox, xs vBuildZnec gBuildZnec, znec  |  force move
Gui, Menu:Add, CheckBox, xs vBuildDh gBuildDh,     dh/mk |  force stand still (LShift)
Gui, Menu:Add, Text, Section, NumLock
Gui, Menu:Add, CheckBox, ys vKeyNumLockOne gKeyNumLockOne, N1
Gui, Menu:Add, CheckBox, ys vKeyNumLockTwo gKeyNumLockTwo, N2
Gui, Menu:Add, CheckBox, ys vKeyNumLockThree gKeyNumLockThree, N3
Gui, Menu:Add, CheckBox, ys vKeyNumLockFour gKeyNumLockFour, N4
Gui, Menu:Add, Text, xs Section, KeySpam
Gui, Menu:Add, CheckBox, ys vKeySpamOne gKeySpamOne, S1
Gui, Menu:Add, CheckBox, ys vKeySpamTwo gKeySpamTwo, S2
Gui, Menu:Add, CheckBox, ys vKeySpamThree gKeySpamThree, S3
Gui, Menu:Add, CheckBox, ys vKeySpamFour gKeySpamFour, S4
Gui, Menu:Add, Text, xs Section, • NumLock/Spam Activation - [E]
Gui, Menu:Add, Text, xs Section, • Clicker   ---  [Alt + Clic]
Gui, Menu:Add, Text, xs Section, • Clicker   ---  [Alt + Right Clic]
Gui, Menu:Show, x 1500 y 200, AutoSize , d3 macro

; Gui OSD
Gui, Margin, 1, 1
CustomColor = EEAA99  ; Can be any RGB color (it will be made transparent below).
Gui +LastFound +AlwaysOnTop -Caption +ToolWindow  ; +ToolWindow avoids a taskbar button and an alt-tab menu item.
Gui, Color, %CustomColor%
Gui, Font, s20, Consolas
Gui, Font, cWhite
Gui, Font, s15 q3
Gui, Add, Text, Section vMyText cLime, xxxxxxxx  ; XX & YY serve to auto-size the window.
Gui, Add, Text, ys, 
Gui, Add, Text, ys vSpaceRemap cLime, xxxxxxxx  ; XX & YY serve to auto-size the window.
Gui, Add, Text, xs Section vNumLock1 cLime, x  ; XX & YY serve to auto-size the window.
Gui, Add, Text, ys vNumLock2 cLime, x  ; XX & YY serve to auto-size the window.
Gui, Add, Text, ys vNumLock3 cLime, x  ; XX & YY serve to auto-size the window.
Gui, Add, Text, ys vNumLock4 cLime, x  ; XX & YY serve to auto-size the window.
Gui, Add, Text, ys, 
Gui, Add, Text, ys, NumLock
Gui, Add, Text, xs Section vSPAM1 cLime, x  ; XX & YY serve to auto-size the window.
Gui, Add, Text, ys vSpam2 cLime, x  ; XX & YY serve to auto-size the window.
Gui, Add, Text, ys vSpam3 cLime, x  ; XX & YY serve to auto-size the window.
Gui, Add, Text, ys vSpam4 cLime, x  ; XX & YY serve to auto-size the window.
Gui, Add, Text, ys, 
Gui, Add, Text, ys, Spam
Gui, Add, Text, xs Section vScriptActive cLime, ..........  ; XX & YY serve to auto-size the window.
; Make all pixels of this color transparent and make the text itself translucent (150):
WinSet, TransColor, %CustomColor% 150
SetTimer, UpdateOSD, 200
Gosub, UpdateOSD  ; Make the first update immediate rather than waiting for the timer.
Gosub, KeyNumLockOne
Gosub, KeyNumLockTwo
Gosub, KeyNumLockThree
Gosub, KeyNumLockFour
Gosub, KeySpamOne
Gosub, KeySpamTwo
Gosub, KeySpamThree
Gosub, KeySpamFour
Gui, Show, x1600 y960 NoActivate  ; NoActivate avoids deactivating the currently active window.
return

GuiClose:
ExitApp
return

MenuGuiClose:
Gosub GuiClose
ExitApp
return

UpdateOSD:
GuiControl,, MyText, %debugText%
GuiControl,, SpaceRemap, %SpaceRemap%
GuiControl,, NumLock1, %NumLock1%
GuiControl,, NumLock2, %NumLock2%
GuiControl,, NumLock3, %NumLock3%
GuiControl,, NumLock4, %NumLock4%
GuiControl,, Spam1, %Spam1%
GuiControl,, Spam2, %Spam2%
GuiControl,, Spam3, %Spam3%
GuiControl,, Spam4, %Spam4%
GuiControl,, ScriptActive, %ScriptOn%
return

;; ----

;; rat build // Space to sidestep [hexing macro]
BuildRat:
ControlGet, IsChecked, Checked, , rat
SpaceRemap := (IsChecked ? "rat" : EmptyRemap)
Gui, Submit, NoHide
return

;; znec build // Space to force move
BuildZnec:
ControlGet, IsChecked, Checked, , znec
SpaceRemap := (IsChecked ? "znec" : EmptyRemap)
Gui, Submit, NoHide
return

;; god dh build // Space to force stand still  (LShift)
BuildDh:
ControlGet, IsChecked, Checked, , dh
SpaceRemap := (IsChecked ? "dh" : EmptyRemap)
Gui, Submit, NoHide
return

;; ----

KeyNumLockOne:
ControlGet, IsChecked, Checked, , N1
NumLock1 := (IsChecked ? 1 : EmptyKey)
Gui, Menu:Submit, NoHide
return

KeyNumLockTwo:
ControlGet, IsChecked, Checked, , N2
NumLock2 := (IsChecked ? 2 : EmptyKey)
Gui, Menu:Submit, NoHide
return

KeyNumLockThree:
ControlGet, IsChecked, Checked, , N3
NumLock3 := (IsChecked ? 3 : EmptyKey)
Gui, Menu:Submit, NoHide
return

KeyNumLockFour:
ControlGet, IsChecked, Checked, , N4
NumLock4 := (IsChecked ? 4 : EmptyKey)
Gui, Menu:Submit, NoHide
return

;; ----

KeySpamOne:
ControlGet, IsChecked, Checked, , S1
Spam1 := (IsChecked ? 1 : EmptyKey)
Gui, Menu:Submit, NoHide
return

KeySpamTwo:
ControlGet, IsChecked, Checked, , S2
Spam2 := (IsChecked ? 2 : EmptyKey)
Gui, Menu:Submit, NoHide
return

KeySpamThree:
ControlGet, IsChecked, Checked, , S3
Spam3 := (IsChecked ? 3 : EmptyKey)
Gui, Menu:Submit, NoHide
return

KeySpamFour:
ControlGet, IsChecked, Checked, , S4
Spam4 := (IsChecked ? 4 : EmptyKey)
Gui, Menu:Submit, NoHide
return

;; ----

ScriptActive:
Gui, Menu:Submit, NoHide
return

;; Runnig/Pause Button
ScriptRun:
{
	if (ScriptRun) {
		debugText := "Paused"
		GuiControl,, ScriptRun, Paused
		}
	else {
		debugText := "Running"
		GuiControl,, ScriptRun, Running
	}
	ScriptRun := !ScriptRun
	Gui, Menu:Submit, NoHide
	Suspend
}
return


;; =========
;; Key Spam
;; =========

Spam:
if (Toggle && WinActive("Diablo III")) {
	KeysToSpam := 
	KeysToSpam .= (Spam1 == 1 ? 1 : "")
	KeysToSpam .= (Spam2 == 2 ? 2 : "")
	KeysToSpam .= (Spam3 == 3 ? 3 : "")
	KeysToSpam .= (Spam4 == 4 ? 4 : "")
	Send %KeysToSpam%
}
else
	Toggle := false
return


;; ===============
;; NumLock & Spam
;; ===============

#IfWinActive Diablo III
~e::
if (FirstKeyPress) {
	ScriptOn := "Active"
	SetNumLockState, On
	if (KeyNumLockOne)
		Send {Numpad1 Down}
	if (KeyNumLockTwo)
		Send {Numpad2 Down}
	if (KeyNumLockThree)
		Send {Numpad3 Down}
	if (KeyNumLockFour)
		Send {Numpad4 Down}
	SetNumLockState, Off
	Toggle := !Toggle ; Start Spam()
	FirstKeyPress := false
	;SoundBeep 1000
}
else {
	Toggle := false
	SetNumLockState, On
	Send {Numpad1 Up}{Numpad2 Up}{Numpad3 Up}{Numpad4 Up}
	SetNumLockState, Off
	FirstKeyPress := true
	ScriptOn := ""
	;SoundBeep, 500,
}
return

;; ==================================
;; Cancel NumLock and Toggle when tp
;; ==================================

~XButton2::
Toggle := false
SetNumLockState, On
sleep 50
Send {Numpad1 Up}
Send {Numpad2 Up}
Send {Numpad3 Up}
Send {Numpad4 Up}
sleep 50
SetNumLockState, Off
FirstKeyPress := true
ScriptOn := ""
if (A_ThisHotkey = "~Xbutton2") {
	sleep 500
	send, {XButton2}
}
return

;; ================================================
;; Remap Space to move
;; Remember to bind mousewheel click to force move
;; ================================================

#IfWinActive Diablo III
~space::
if (BuildDh) { ; Stand still to Space
	~space::LShift
}
else {
	SetControlDelay -1 ; May improve reliability and reduce side effects.
	While GetKeyState("space","P") {
		if (BuildRat) { ; Sidestep to Space
			ControlClick, x940 y507,Diablo III,,middle,1,NA
			Sleep, 115
			ControlClick, x980 y507,Diablo III,,middle,1,NA
			Sleep, 115
		}
		if (BuildZnec) { ; Force move to Space
			Click, middle
			sleep 20
		}
	}
	SetControlDelay 20 ; reset to default
}
return

;; =====================
;; DH - Hungering Arrow
;; =====================

; Left Click force attack for DH if Right Clic is Pushed
#IfWinActive Diablo III
~LButton::
if (BuildDh) {
	if getKeyState("RButton", "P") {
		SendInput, {LShift down}
		Click
		SendInput, {LShift up}
	}
}
return

;; ==========
;; Clickers
;; ==========

; Alt + Left Clic
#IfWinActive Diablo III
!LButton:: 
while GetKeyState("LButton", "P") {
    if (WinActive("Diablo III")) {
        Click
        Sleep, 5
    }
}
return


; Alt + Right Clic
#IfWinActive Diablo III
!RButton::
while GetKeyState("RButton", "P") {
	if (WinActive("Diablo III")) {
		Click, right
		Sleep, 5
	}
}
return

; =================
; salvage inventory
; =================

; Hotkey to salvage inventory
!q:: 
If (A_PriorHotKey = A_ThisHotKey and A_TimeSincePriorHotkey < 500)
	salvageStuff()
return

salvageStuff()
{
	SetMouseDelay, -1	; Sets the delay that will occur after each mouse movement or click.
	SetKeyDelay, -1		; Sets the delay that will occur after each keystroke

	setResUtilities()
	
	SendMode, Input
	
	Global salvageX, salvageY
	Global inventoryX, inventoryY
	Global nextItem, startItem, salvageDelay
	
	MouseClick, Left, salvageX, salvageY
	
	Loop, 54 
	{   
		MouseClick, Left, inventoryX, inventoryY
		; Sleep %salvageDelay%
		
		SendInput {Enter Down}
		SendInput {Enter Up}
		Sleep %salvageDelay%
		
		SendInput {Enter Down}
		SendInput {Enter Up}
		Sleep %salvageDelay%

		inventoryX := inventoryX + nextItem
		
		if (A_Index == 9)
		{
			inventoryY := inventoryY + nextItem
			inventoryX := startItem
		}
		if (A_Index == 18)
		{
			inventoryY := inventoryY + nextItem
			inventoryX := startItem
		}
		if (A_Index == 27)
		{
			inventoryY := inventoryY + nextItem
			inventoryX := startItem
		}
		if (A_Index == 36)
		{
			inventoryY := inventoryY + nextItem
			inventoryX := startItem
		}
		if (A_Index == 45)
		{
			inventoryY := inventoryY + nextItem
			inventoryX := startItem
		}       
	}
	Sleep %salvageDelay%
	SendInput, {Esc}
	SetMouseDelay, 10	; Sets back to default
	SetKeyDelay, 10		; Sets back to default
	return
}

setResUtilities()
{ 
	Global cubeBookX            := 577
	Global cubeBookY            := 1100
	Global cubeBookArrowRightX  := 1137
	Global cubeBookArrowRightY  := 1121
	Global cubeBookArrowLeftX   := 777
	Global cubeBookArrowLeftY   := 1123
	Global cubeBookFillX        := 969
	Global cubeBookFillY        := 1114
	Global cubeTransmuteX       := 314
	Global cubeTransmuteY       := 1100
	Global inventoryX           := 1975
	Global inventoryY           := 780
	Global nextItem             := 64
	Global startItem            := 1975
	Global salvageX             := 221
	Global salvageY             := 339
	Global exitgameX            := 344
	Global exitgameY            := 647
	Global mapbackX             := 1198
	Global mapbackY             := 173
	Global actoneX              := 980
	Global actoneY              := 822
	Global actonehomeX          := 1358
	Global actonehomeY          := 643
	Global obeliskX             := 2406
	Global obelisky             := 813
	Global grX                  := 363
	Global grY                  := 633
	Global gracceptX            := 372
	Global gracceptY            := 1128
	Global yellowItemCountX     := 165
	Global yellowItemCountY     := 1043
	Global yellowItemCraftX     := 419
	Global yellowItemCraftY     := 1043

	SysGet, screenSizeX, 0
	SysGet, screenSizeY, 1

	if (screenSizeX != 2560 || screenSizeY != 1440)
	{
		Global cubeX                *= (screenSizeX / 2560)
		Global cubeY                *= (screenSizeY / 1440)
		Global cubeBookX            *= (screenSizeX / 2560)
		Global cubeBookY            *= (screenSizeY / 1440)
		Global cubeBookArrowRightX  *= (screenSizeX / 2560)
		Global cubeBookArrowRightY  *= (screenSizeY / 1440)
		Global cubeBookArrowLeftX   *= (screenSizeX / 2560)
		Global cubeBookArrowLeftY   *= (screenSizeY / 1440)
		Global cubeBookFillX        *= (screenSizeX / 2560)
		Global cubeBookFillY        *= (screenSizeY / 1440)
		Global cubeTransmuteX       *= (screenSizeX / 2560)
		Global cubeTransmuteY       *= (screenSizeY / 1440)
		Global inventoryX           *= (screenSizeX / 2560)
		Global inventoryY           *= (screenSizeY / 1440)
		Global nextItem             *= (screenSizeX / 2560)
		Global startItem            *= (screenSizeX / 2560)
		Global salvageX             *= (screenSizeX / 2560)
		Global salvageY             *= (screenSizeY / 1440)
		Global exitgameX            *= (screenSizeY / 1440)
		Global exitgameY            *= (screenSizeY / 1440)
		Global actoneX              *= (screenSizeX / 2560)
		Global actoneY              *= (screenSizeY / 1440)
		Global actonehomeX          *= (screenSizeX / 2560)
		Global actonehomeY          *= (screenSizeY / 1440)
		Global obeliskX             *= (screenSizeX / 2560)
		Global obelisky             *= (screenSizeY / 1440)
		Global grX                  *= (screenSizeX / 2560)
		Global grY                  *= (screenSizeY / 1440)
		Global gracceptX            *= (screenSizeX / 2560)
		Global gracceptY            *= (screenSizeY / 1440)
		Global yellowItemCountX     *= (screenSizeX / 2560)
		Global yellowItemCountY     *= (screenSizeY / 1440)
		Global yellowItemCraftX     *= (screenSizeX / 2560)
		Global yellowItemCraftY     *= (screenSizeY / 1440)
	}
	return
}