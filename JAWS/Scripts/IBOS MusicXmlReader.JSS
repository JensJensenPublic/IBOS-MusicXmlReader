
Include "hjconst.jsh"
Include "hjglobal.jsh"
include "MSAAConst.jsh"
include "UIA.jsh"
include "XMLDom.jsh"
include "FSIMouseSpeech.jsh"
Include "HjHelp.jsh" ; Help Topic Constants
include "locale.jsh"
Include "magic.jsh"
Include "magcodes.jsh"
include "WinStyles.jsh"; Win Style Bits Constants used by the GetWindowStyleBits function
Include "FSLog.jsh"

Script SayBottomLine ()
var handle hWnd,
	handle hChild,
	handle hNextWindow,
	int MaxBottom,
	handle  hMaxBottom,
    int iLeft,
	int iTop,
	int iRight,
	int iBottom,
	string WindowName

if DialogActive() then
	let hWnd = GetRealWindow(GetFocus())
else
	let hWnd = GetAppMainWindow(GetFocus())
EndIf

; Iterate through all children of the MainForm to find the bottom control
hNextWindow=GetFirstChild(hWnd)
let maxBottom = 0 
while hNextWindow != 0
	;SayAllInfo(hNextWindow)
	GetWindowRect(hNextWindow,iLeft,iRight,iTop,iBottom)	
	if iBottom > MaxBottom then	
		maxBottom = iBottom
		hMaxBottom = hNextWindow
	endif
	hNextWindow = GetNextWindow(hNextWindow)
EndWhile

; Say the contents of the bottom control
WindowName = GetWindowName(hMaxBottom)
SayString(WindowName)
EndScript

Script SayScriptName ()
SayString("Script for IBOS MusicXmlReader")
EndScript

; For debugging:
void function SayAllInfo(handle hWnd)
var
	int iLeft,
	int iTop,
	int iRight,
	int iBottom,
	int iLast,
	string WindowName
SayString("SayAllInfo for hWnd")
SayInteger(hWnd)
WindowName = GetWindowName(hWnd)
SayString("WindowName=")
SayString(WindowName)	
GetWindowRect(hWnd,iLeft,iRight,iTop,iBottom)
; Start debug info
SayString("Left=")
SayInteger(iLeft)
SayString("Right=")
SayInteger(iRight)
SayString("Top=")
SayInteger(iTop)
SayString("Bottom=")
SayInteger(iBottom)
; End debug info
return iBottom
EndFunction
