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

void function ReadAllObjects(handle hWnd)
var
	string sObjList,
	int iObjCount,
	int i,
	string sName,
	int iType
SayString("All Objects for hWnd")
SayInteger(hWnd)
let sObjList = GetListOfObjects(hWnd,LIST_ITEM_SEPARATOR)
SayString(sObjList)
let iObjCount = StringSegmentCount(sObjList,LIST_ITEM_SEPARATOR)
SayString("Number of objects=")
SayInteger(iObjCount)
let i = 1
while i <= iObjCount
	let sName = StringSegment(sObjList,LIST_ITEM_SEPARATOR,i)
	GetObjectInfoByName(hWnd,sName,1,iType)
	SayString(sName)
	let i = i+1
EndWhile
EndFunction


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

string function GetBottomLineOfWindowJSJ()
var
	handle hWnd,
	string sWindowText,
	int iLeft,
	int iTop,
	int iRight,
	int iBottom,
	int iLast,
	int nSubType,
	int nState,
	string Value,
	string Description,
	string HotKey,
	string ContainerName,
	string WindowName
	
if DialogActive() then
	let hWnd = GetRealWindow(GetFocus())
	SayString("GetRealWindow returned ")
else
	let hWnd = GetAppMainWindow(GetFocus())
	SayString("GetAppMainWindow returned ")
EndIf

;SayString("hWnd=")
SayInteger(hWnd)

SayAllInfo(hWnd)

SayAllInfo(GetFirstChild(hWnd))



if GetObjectInfoByName(hWnd,"Statuslinie",1,nSubType,nState,Value,Description,HotKey,ContainerName) then
	SayString(Value)
else
	SayString("GetObjectInfoByName failed")
endIf

SayInteger(nSubType)
SayInteger(nState)
SayString(Value)
SayString(Description)
SayString(Hotkey)
SayString(ContainerName)





let sWindowText = GetTextInRect(iLeft,iTop,iRight,iBottom,0,IgnoreColor,IgnoreColor,1,0)
SayString(sWindowText)
if !sWindowText return "NULL" endIf
let iLast = StringSegmentCount(sWindowText,"\n")
SayString("Last=")
SayInteger(iLast)
return StringSegment(sWindowText,"\n",iLast)
EndFunction




Script HelloWorld ()
var handle hWnd,
	handle hChild,
	handle hNextChild,
	int MaxBottom,
	handle  hMaxBottom,
    int iLeft,
	int iTop,
	int iRight,
	int iBottom,
	string WindowName
SayString("Hello World")
if DialogActive() then
	let hWnd = GetRealWindow(GetFocus())
	SayString("GetRealWindow returned ")
else
	let hWnd = GetAppMainWindow(GetFocus())
	SayString("GetAppMainWindow returned ")
EndIf

SayString("hWnd=")
SayInteger(hWnd)

;SayAllInfo(hWnd)


hChild=GetFirstChild(hWnd)
;SayString("hChild=")
;SayInteger(hChild) 
;SayAllInfo(hChild)

hChild = GetFirstWindow(hChild)
;SayString("GetFirstWindow returned")
;SayInteger(hChild) 
;SayAllInfo(hChild)

;hNextChild = GetNextWindow(hChild)
;SayString("GetNextWindow.1 returned")
;SayInteger(hNextChild) 
;SayAllInfo(hNextChild)

;hNextChild = GetNextWindow(hNextChild)
;SayString("GetNextWindow.2 returned")
;SayInteger(hNextChild) 
;SayAllInfo(hNextChild)

let maxBottom = 0
let hNextChild = hChild 
while hNextChild != 0
	;SayAllInfo(hNextChild)
	GetWindowRect(hNextChild,iLeft,iRight,iTop,iBottom)	
	if iBottom > MaxBottom then	
		maxBottom = iBottom
		hMaxBottom = hNextChild
	endif
	hNextChild = GetNextWindow(hNextChild)
EndWhile

; Report info ablot the bottommost control
SayString("MaxBottom=")
Sayinteger(maxBottom)
SayString("MaxHandle=")
Sayinteger(hMaxBottom) 
;SayAllInfo(hMaxBottom)

WindowName = GetWindowName(hMaxBottom)
SayString("WindowName=")
SayString(WindowName)


EndScript
