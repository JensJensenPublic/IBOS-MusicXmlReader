using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MusicXmlReaderModel;

// https://msdn.microsoft.com/en-us/library/hh994769%28v=vs.110%29.aspx?f=255&MSPPError=-2147217396
// Describes good practice for designing with Access Keys and Shortcut Keys


// MuseScore shortcuts are describet in
// https://musescore.org/da/handbook/nodeindtastning#keyboard

// https://support.microsoft.com/en-us/kb/126449 contains the following list of standard Windows keyboard shortcuts
//F1: Help
//CTRL+ESC: Open Start menu
//ALT+TAB: Switch between open programs
//ALT+F4: Quit program
//SHIFT+DELETE: Delete item permanently
//Windows Logo+L: Lock the computer(without using CTRL+ALT+DELETE)
//Windows program key combinations
//CTRL+C: Copy
//CTRL+X: Cut
//CTRL+V: Paste
//CTRL+Z: Undo
//CTRL+B: Bold
//CTRL+U: Underline
//CTRL+I: Italic
//Mouse click/keyboard modifier combinations for shell objects
//SHIFT+right click: Displays a shortcut menu containing alternative commands
//SHIFT+double click: Runs the alternate default command(the second item on the menu)
//ALT+double click: Displays properties
//SHIFT+DELETE: Deletes an item immediately without placing it in the Recycle Bin
//General keyboard-only commands
//F1: Starts Windows Help
//F10: Activates menu bar options
//SHIFT+F10 Opens a shortcut menu for the selected item(this is the same as right-clicking an object
//CTRL+ESC: Opens the Start menu (use the ARROW keys to select an item)
//CTRL+ESC or ESC: Selects the Start button(press TAB to select the taskbar, or press SHIFT+F10 for a context menu)
//CTRL+SHIFT+ESC: Opens Windows Task Manager
//ALT+DOWN ARROW: Opens a drop-down list box
//ALT+TAB: Switch to another running program(hold down the ALT key and then press the TAB key to view the task-switching window)
//SHIFT: Press and hold down the SHIFT key while you insert a CD-ROM to bypass the automatic-run feature
//ALT+SPACE: Displays the main window's System menu (from the System menu, you can restore, move, resize, minimize, maximize, or close the window)
//ALT+- (ALT+hyphen): Displays the Multiple Document Interface(MDI) child window's System menu (from the MDI child window's System menu, you can restore, move, resize, minimize, maximize, or close the child window)
//CTRL+TAB: Switch to the next child window of a Multiple Document Interface(MDI) program
//ALT+underlined letter in menu: Opens the menu
//ALT+F4: Closes the current window
//CTRL+F4: Closes the current Multiple Document Interface(MDI) window
//ALT+F6: Switch between multiple windows in the same program(for example, when the Notepad Find dialog box is displayed, ALT+F6 switches between the Find dialog box and the main Notepad window)
//Shell objects and general folder/Windows Explorer shortcuts
//For a selected object: 
//F2: Rename object
//F3: Find all files
//CTRL+X: Cut
//CTRL+C: Copy
//CTRL+V: Paste
//SHIFT+DELETE: Delete selection immediately, without moving the item to the Recycle Bin
//ALT+ENTER: Open the properties for the selected object
//To copy a file
//Press and hold down the CTRL key while you drag the file to another folder.
//To create a shortcut
//Press and hold down CTRL+SHIFT while you drag a file to the desktop or a folder.
//General folder/shortcut control
//F4: Selects the Go To A Different Folder box and moves down the entries in the box (if the toolbar is active in Windows Explorer)
//F5: Refreshes the current window.
//F6: Moves among panes in Windows Explorer
//CTRL+G: Opens the Go To Folder tool (in Windows 95 Windows Explorer only)
//CTRL+Z: Undo the last command
//CTRL+A: Select all the items in the current window
//BACKSPACE: Switch to the parent folder
//SHIFT+click+Close button: For folders, close the current folder plus all parent folders
//Windows Explorer tree control
//Numeric Keypad*: Expands everything under the current selection
//Numeric Keypad +: Expands the current selection
//Numeric Keypad -: Collapses the current selection.
//RIGHT ARROW: Expands the current selection if it is not expanded, otherwise goes to the first child
//LEFT ARROW: Collapses the current selection if it is expanded, otherwise goes to the parent
//Properties control
//CTRL+TAB/CTRL+SHIFT+TAB: Move through the property tabs
//Accessibility shortcuts
//Press SHIFT five times: Toggles StickyKeys on and off
//Press down and hold the right SHIFT key for eight seconds: Toggles FilterKeys on and off
//Press down and hold the NUM LOCK key for five seconds: Toggles ToggleKeys on and off
//Left ALT+left SHIFT+NUM LOCK: Toggles MouseKeys on and off
//Left ALT+left SHIFT+PRINT SCREEN: Toggles high contrast on and off
//Microsoft Natural Keyboard keys
//Windows Logo: Start menu
//Windows Logo+R: Run dialog box
//Windows Logo+M: Minimize all
//SHIFT+Windows Logo+M: Undo minimize all
//Windows Logo+F1: Help
//Windows Logo+E: Windows Explorer
//Windows Logo+F: Find files or folders
//Windows Logo+D: Minimizes all open windows and displays the desktop
//CTRL+Windows Logo+F: Find computer
//CTRL+Windows Logo+TAB: Moves focus from Start, to the Quick Launch toolbar, to the system tray (use RIGHT ARROW or LEFT ARROW to move focus to items on the Quick Launch toolbar and the system tray)
//Windows Logo+TAB: Cycle through taskbar buttons
//Windows Logo+Break: System Properties dialog box
//Application key: Displays a shortcut menu for the selected item
//Microsoft Natural Keyboard with IntelliType software installed
//Windows Logo+L: Log off Windows
//Windows Logo+P: Starts Print Manager
//Windows Logo+C: Opens Control Panel
//Windows Logo+V: Starts Clipboard
//Windows Logo+K: Opens Keyboard Properties dialog box
//Windows Logo+I: Opens Mouse Properties dialog box
//Windows Logo+A: Starts Accessibility Options(if installed)
//Windows Logo+SPACEBAR: Displays the list of Microsoft IntelliType shortcut keys
//Windows Logo+S: Toggles CAPS LOCK on and off
//Dialog box keyboard commands
//TAB: Move to the next control in the dialog box
//SHIFT+TAB: Move to the previous control in the dialog box
//SPACEBAR: If the current control is a button, this clicks the button.If the current control is a check box, this toggles the check box. If the current control is an option, this selects the option.
//ENTER: Equivalent to clicking the selected button (the button with the outline)
//ESC: Equivalent to clicking the Cancel button
//ALT+underlined letter in dialog box item: Move to the corresponding item




namespace MusicXmlReader
{

    /// <summary>
    /// Class for defining all keyboard shortcuts at on single place instead of scattering them around the code
    /// </summary>
    public static class ShortcutHandler
    {
        // The following static variables list the shortcut keys used in this program
        // This list must be carefully maintained in order to keep track of all Keyboard Shortcuts used
        // and may also be used as a base for End User documentaton.
        // Note that the same values may also be set up by the autogenerated code in MainForm.Designer.cs

        // FileToolStripMenuItem: 
        public const Keys openMusicXmlFile =    Keys.Control | Keys.O;
        public const Keys openMusicXmlFileUsingDefaultSettings = Keys.Control | Keys.Shift | Keys.O; 
        public const Keys openRecentMusicXmlFileToolStripMenuItem = Keys.Control | Keys.Alt | Keys.O; 
        public const Keys exitApplication =     Keys.Alt | Keys.F4;

        // EditToolStripMenuItem:
        // Note: from version 1.0.6.1 these menu items are also selectable throught the "&" mechanism in the Text property
        //public const Keys editAllItems = Keys.Control | Keys.A; // Expand all items and select tree root // We want to reserve the global CONTROL A for "select All" !
        public const Keys editFilter = Keys.Control | Keys.F; // Focus on Filter, but keep expansion and selection
        public const Keys editMusic = Keys.Control | Keys.M; // Expand Music and selest Music tree
        public const Keys editText = Keys.Control | Keys.T; // Expand Text and selest Text tree
        public const Keys editBraille = Keys.Control | Keys.B; // Expand Braille and selest Braille tree
        // public const Keys editParts = Keys.Control | Keys.S; // Expand Parts and selest Music Parts      // We want to reserve the global CONTROL S for "Save" !
        // public const Keys editDetails = Keys.Control | Keys.D; // Expand Detail and select Music Details // 
        // Shortcut keys (Set up in the Edit menu) for activating the ParameterInputForm
        public const Keys CommandRepeat = Keys.R | Keys.Control; // Repeat from Measure to Measure
        public const Keys CommandGoto = Keys.G | Keys.Control; // GoTo Measure
        public const Keys CommandTempo = Keys.N | Keys.Control; // % of Normal Tempo 
 
        // ViewToolStripMenuItem: 
        //
        // ToolsToolStripMenuItem: 
        //
        // HelpToolStripMenuItem: 
        //

        // Start / Stop of Autoplay
        public const Keys startPlaying =    Keys.P;               // Removed CTRL in 3.5 because we want to use  "CTRL+P" for Print
        public const Keys stopPlaying  =    Keys.Shift | Keys.P;  // Removed CTRL in 3.5 because we want to use  "CTRL+SHIFT+P" for Print
        public const Keys togglePlaying =   Keys.Space;


        public const Keys PrintOnWindowsPrinter = Keys.Control | Keys.P;
        public const Keys PrintUsingExternalProgram = Keys.Control | Keys.Shift |Keys.P;


        // Tempo control of AutoPlay
        public const Keys tempoIncrement = Keys.Control | Keys.PageUp;
        public const Keys tempoDecrement = Keys.Control | Keys.PageDown;


        // UserSettingsTreeview:


        // NOTE: It is difficult to find the right assignment of shortcuts for the desired functionality:
        // 1: Handle all nodes with same Name       For the time being Keys.Control | Keys.D0 / Keys.D1
        // 2: Handle all nodes with same parent      For the time being Keys.Control | Keys.Down / Keys.Up
        // 3: Handle all nodes with same parent and for each of them all nodes with same name: For the time being not implemented! 
        // Some shortcut values are unusable because they make the program crash during initialization, others because they simply don't work!

        public const Keys checkAll = Keys.Control | Keys.D1;   // Check all occurances of same part (or detail) for all settings
        public const Keys uncheckAll = Keys.Control | Keys.D0; // Uncheck all occurances of same part (or detail) for all settings
        public const Keys checkOthers = Keys.Control | Keys.Up; // Check other parts (or details) within same setting
        public const Keys uncheckOthers = Keys.Control | Keys.Down; // Uncheck other parts (or details) within same setting
//        public const Keys checkOthers = Keys.Shift | Keys.D1; // Check other parts (or details) within same setting
//        public const Keys uncheckOthers =  Keys.Shift | Keys.D2; // Uncheck other parts (or details) within same setting
        //public const Keys toggleAndCopy =   Keys.Control | Keys.Space;

        public const Keys listBoxFocus =    Keys.Control | Keys.L;

        // Use the "+" semantics known from a tree to start showing single notes 
        //public const Keys detailsOpenSingleNotesFromBottom  = Keys.Add;                 // Enter     Details mode and select the last detail, if available)
        //public const Keys detailsOpenSingleNotesFromTop     = Keys.Add | Keys.Shift;    // Enter     Details mode and select the first detail, if available

        // Alternatively use navigation keys to start showing single notes 

        public const Keys detailsTopDetail =        Keys.Home;    // Remain in Details mode and select the top detail  if available)
        public const Keys detailsBottomDetail =     Keys.End;     // Remain in Details mode and select the bottum detail, if available 
        public const Keys detailsNextEvent =        Keys.Right;   // Leave Details mode and select the next event
        public const Keys detailsPreviousEvent =    Keys.Left;    // Leave Details mode and select the previous event

        public const Keys NextMeasure =             Keys.Control | Keys.Right;
        public const Keys PreviousMeasure =         Keys.Control | Keys.Left;

        public const Keys NextEvent =               Keys.Right;
        public const Keys PreviousEvent =           Keys.Left;


        // MusicXmlReaders of use of ARROW keys with the 8 possible combination-keys:  CTRL ALT SHIFT when JAWS is running
        // No combination key.... UP/DOWN   
        // SHIFT................. NOT USED (Reserved for later use for selection)
        // ALT................... Show Status details
        // ALT + SHIFT........... NOT USED
        // CTRI.................. Show part details
        // CTRL + SHIFT.......... Show harmony details
        // CTRL + ALT............ NOT USED (Seems to be used by JAWS for some table functionality)
        // CTRL + ALT + SHIFT.... NOT USED (Seems to be used by JAWS for some table functionality)

        public const Keys DetailsNextPart = Keys.Down;
        public const Keys DetailsPreviousPart = Keys.Up;

        // For loading details
        public const Keys DetailsHarmonyTop =           Keys.Down | Keys.Control | Keys.Shift;  // Directly from the NoteList
        public const Keys DetailsPartsTop =             Keys.Down | Keys.Control;               // Directly from the NoteList: Split into parts
        public const Keys DetailsNotesTop =             Keys.Down ;  // Directly from the NoteList: Split directly in notes without respect to parts
        public const Keys DetailsStatusTop =            Keys.Down | Keys.Alt;                   // Directly from the NoteList
        public const Keys detailsSingleNotesFromTop =   Keys.Down | Keys.Control;               // From the Details list: Split selected part into notes.
        public const Keys DetailsInstruments =          Keys.I  | Keys.Control;
        public const Keys DetailsHarmonyBottom =        Keys.Up | Keys.Control | Keys.Shift;
        public const Keys DetailsPartsBottom =          Keys.Up | Keys.Control;
        public const Keys DetailsNotesBottom =          Keys.Up ;
        public const Keys DetailsStatusBottom =         Keys.Up | Keys.Alt;
        public const Keys detailsSingleNotesFromBottom= Keys.Up | Keys.Control; // When already viewing parts
        //public const Keys DetailsInstrumentsButtom= Keys.I | Keys.C;

        public const Keys StopAllNotesPlaying = Keys.Escape;

        public const Keys NoKeys = Keys.None; // Means that no shortcut key is defined



    }
}
