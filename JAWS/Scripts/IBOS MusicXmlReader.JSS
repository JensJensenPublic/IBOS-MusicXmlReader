Script SayBottomLine ()
var handle hWnd,
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

Script SayScriptFileName ()
SayString("IBOS MusicXmlReader.JSS")
EndScript