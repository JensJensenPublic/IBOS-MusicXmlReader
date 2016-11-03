using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MusicXmlReaderModel;

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
    public class ShortcutHandler
    {
        // The following static variables list the shortcut keys used in this program
        // This list must be carefully maintained in order to keep track of all Keyboard Shortcuts used
        // and may also be used as a base for End User documentaton.
        // Note that the same values may also be set up by the autogenerated code in MainForm.Designer.cs

        // FileToolStripMenuItem: 
        static public readonly Keys openMusicXmlFile = ((Keys)((Keys.Control | Keys.O)));
        static public readonly Keys exitApplication = ((Keys)((Keys.Alt | Keys.F4)));

        // EditToolStripMenuItem:
        static public readonly Keys editAllItems= ((Keys)((Keys.Control | Keys.A)));
        static public readonly Keys editMusic   = ((Keys)((Keys.Control | Keys.M)));
        static public readonly Keys editText    = ((Keys)((Keys.Control | Keys.T)));
        static public readonly Keys editBraille = ((Keys)((Keys.Control | Keys.B)));
        static public readonly Keys editParts   = ((Keys)((Keys.Control | Keys.S)));
        static public readonly Keys editDetails = ((Keys)((Keys.Control | Keys.D)));


        // ViewToolStripMenuItem: 
        //
        // ToolsToolStripMenuItem: 
        //
        // HelpToolStripMenuItem: 
        //

        // Start / Stop of Autoplay
        static public readonly Keys startPlaying = ((Keys)((Keys.Control | Keys.S)));
        static public readonly Keys[] stopPlaying = new Keys[] { Keys.Space, Keys.Enter };

        private ShortcutHandler()
        { }

        private MainForm mainForm;
        private Model model;

        private ShortcutHandler(MainForm mainForm, Model model)
        {
            this.mainForm = mainForm;
            this.model = model;
        }

        static public ShortcutHandler Create(MainForm mainForm, Model model)
        {
            return new ShortcutHandler(mainForm, model);
        }

        //  KeyData:
        //  A System.Windows.Forms.Keys representing the key code for the key that was pressed,
        //  combined with modifier flags that indicate which combination of CTRL, SHIFT,
        //  and ALT keys was pressed at the same time


        public bool IsStartPlayingShortcut(KeyEventArgs e)
        {
            return (e.KeyData == startPlaying);
        }

        public bool IsStopPlayingShortcut(KeyEventArgs e)
        {
            foreach (Keys key in stopPlaying)
            {
                if (e.KeyData == key)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
