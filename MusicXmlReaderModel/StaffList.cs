using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXmlReaderModel
{
    public class StaffList
    {
        private List<Staff> staffs = new List<Staff>();
        /// <summary>
        /// Represents the staffs shown in the graphic representation.
        /// Typically one staff per part, bur for instance piano and organ use two staffs, one for left hand and one for right hand.
        /// </summary>
        public List<Staff> Staffs { get { return staffs; } }
        /// <summary>
        private List<Staff> allStaffs = new List<Staff>();
        /// <summary>
        /// Contains all staffs shown in the graphic representation, plus a "TUTTI" staff representing all parts and special parts "SA", "TB" and "SATB"
        /// </summary>
        public List<Staff> AllStaffs { get { return allStaffs; } } // Also includes TuttiStaff
        private MetaInformation metaInformation;
        public MetaInformation MetaInformation { get { return metaInformation; } }

        private static List<string> sopranoNames = new List<string> { "SOPRANO", "SOPRAN", "S" };
        private static List<string> altoNames = new List<string> { "ALTO", "ALT", "A" };
        private static List<string> tenorNames = new List<string> { "TENOR", "TEN", "T" };
        private static List<string> basNames = new List<string> { "BASSO", "BAS", "B" };
        private List<List<string>> saNames = new List<List<string>> { sopranoNames, altoNames };
        private List<List<string>> tbNames = new List<List<string>> { tenorNames, basNames };
        // And finally, primarily for testing:
        private List<List<string>> satbNames = new List<List<string>> { sopranoNames, altoNames ,tenorNames, basNames };


        private const bool fromTop = true;
        private const bool fromBottom = false;

        private StaffList()
        { }

        private StaffList(List<NoteElement> notesForPart)
        {
            //// Divide the notes of the part accordnig to staff number
            foreach (NoteElement n in notesForPart)
            {
                Staff staffToUse = null;
                foreach (Staff staff in staffs)
                {
                    if (staff.StaffNumber == n.Staff)
                    {
                        staffToUse = staff;
                        break;
                    }
                }
                if (null == staffToUse)
                {
                    // We need to add a new staff
                    staffToUse = Staff.Create(n.PartNumber,n.Staff,false,this);
                    staffs.Add(staffToUse);
                }
                staffToUse.Notes.NoteElements.Add(n);
            }
            // Sort the notes in each part according to pitch !
#warning todo Sort the notes in each part according to pitch !

        }

        private StaffList(PartDescriptionList partDescriptionList,MetaInformation metaInformation)
        {
            this.metaInformation = metaInformation; // Title, composer, etc

            //            foreach (PartDescription part in partDescriptionList.parts)
            // We need the index so use for instead of foreach !
            for (int partNumber = 0; partNumber < partDescriptionList.parts.Count; partNumber++)
            {
                PartDescription part = partDescriptionList.parts[partNumber];
                int minStaffNumber = int.MaxValue;
                int maxStaffNumber = int.MinValue;
                int nElements = 0; // Not really needed.
                foreach (Element element in part.Elements)
                {
                    nElements++;
                    int staffNumber = element.GetStaffNumber();
                    if (staffNumber >= 0) // Negative staffnumber marks that no staffNumber exists for this element
                    {
                        minStaffNumber = Math.Min(staffNumber, minStaffNumber);
                        maxStaffNumber = Math.Max(staffNumber, maxStaffNumber);
                    }
                }
                // Logger.LogCF(string.Format(": PartId={0} minStaff={1} maxStaff={2} nElements={3}", part.Id, minStaffNumber, maxStaffNumber, nElements));

                // Create and add all staffs for this part (typically 1 staff per part except for Piano, Organ, Harp etc)
                bool isPartOfGrandStaff = minStaffNumber != maxStaffNumber;
                for (int i = minStaffNumber; i <= maxStaffNumber; i++)
                {
                    Staff staff = Staff.Create(partNumber, i, isPartOfGrandStaff,this);
                    staffs.Add(staff);
                    // Logger.LogCF(string.Format(": Added staff with staffNumber={0}  partId={1}", i, part.Id));
                }
            }
        }

        private StaffList(string formattedString,MetaInformation metaInformtion,string name)
        {
            this.metaInformation = metaInformtion;
            Staff theOnlyStaff = Staff.Create(formattedString,this,name);
            theOnlyStaff.Enabled = true;
            staffs.Add(theOnlyStaff);
        }

        public string MusicBrailleFilenameAttribute
        {
            get
            {
                int numberOfStaffs = staffs.Count;
                switch (numberOfStaffs)
                {
                    case 0: return "";
                    case 1: return staffs[0].PartId;
                    default: return "";
                }
            }
        }


        public void AddMetaInformationDetails(PartlistElement partList)
        {
#warning ToDo Move to Staff class 
            try
            {
                foreach (Staff staff in staffs)
                {
                    staff.ScorePartElement = partList.GetPartFromNumber(staff.PartNumber);
                    // Logger.LogCF(string.Format(": {0}", staff.ToDebugString()));
                }
            }
            catch (Exception e)
            {
                Logger.LogCFOnce(string.Format(": Exception.Message={0}",e.Message));
            }

        }

        private int GetLength(List<BrailleBuilder> bbs)
        {
            int totalLength = 0;
            foreach (BrailleBuilder bb in bbs)
            {
                totalLength += bb.ToBrailleString().Length;
            }
            return totalLength;
        }



    /// <summary>
    /// Filles in with all BrailleMusic information
    /// </summary>
    /// <param name="events">The list of timestamped EventDescriptions fully describing all events in the score</param>
    /// <param name="userSettings">The current conditions for generating BrailleMusic representation</param>
    /// <returns></returns>
    public bool Init(EventDescriptionList events, UserSettings userSettings)
        {
            bool result = true;

            // Set up for Music Braille. No normal text 
            // Do not tamper with the Braille settings: Keep user's choise !
            bool existingSpeechSettings = userSettings.MusicAsSpeech;  // Save the existing setting of the top node
            bool existingMusicBrailleSettings = userSettings.MusicAsMusicBraille;
            userSettings.MusicAsSpeech = false; // Turn off all reader settings by turning off the top node.
            userSettings.MusicAsMusicBraille = true; // Turn on all MusicBraille settings by turning on the top node. But keep the detailed setings as user wants!!!

            // For each staff generate a list of timestamped BrailleBuilders, representing the Braille Music representation.
            // If only one part is selected, only the staffs for that part will contain information
            // The timestamps make it possible to format sets of staffs while synchronizating at measure level or beat level.
            try
            {
                EventDescription currentEventDescription;
                foreach (Staff staff in this.Staffs)
                {
                    Logger.LogCF(string.Format(": Part={0} Staff={1}", staff.PartName,staff.StaffNumber));
                    //userSettings.StaffNumber = staff.StaffNumber; // Ignore all other staff numbers
                    userSettings.SelectedStaffs = new List<StaffSelector>();
                    userSettings.SelectedStaffs.Add(StaffSelector.Create(staff.PartId, staff.StaffNumber));  // Ignore all other staff numbers
                    List<BrailleBuilder> brailleBuilders = new List<BrailleBuilder>(); // For this particular staff in this particular part


                    currentEventDescription = events.firstEventDescription;
                    while(null != currentEventDescription)
                    {
                        BrailleBuilder bb = currentEventDescription.ToBraille(false, true, out currentEventDescription); // (false,true) <=> ( "Separate parts", "Use interval notation")
                        brailleBuilders.Add(bb);
                    }
                    staff.BrailleMusicBrailleBuilders = brailleBuilders;
                }                        //Logger.LogCF(string.Format(": Part={0} Staff={1} is represented by {2} BrailleBuilders. TotalLength={3}", staff.PartId, staff.StaffNumber, staff.BrailleMusicBrailleBuilders.Count, GetLength(brailleBuilders)));

                allStaffs.AddRange(staffs);

                //  Also support a list of ALL staffs, including the normal staffs, the TUTTI staff and any special staffs, such as SA and TB
                userSettings.SelectedStaffs = null; // Add all enabled parts
                AddPseudoStaff(events, "TUTTI");

                //  Init a psoudostaff containing
                //   A  soprano part, (identified with "SOPRANO", "SOPRAN" or "S") 
                //   An alto part,    (identified with "ALTO", "ALT" or "A") 
                userSettings.SelectedStaffs = GetSelectedStaffs(saNames, fromTop); // Select Soprano and Alto
                ConditionallyAddPseudoStaff(events, "SA", userSettings.SelectedStaffs);

                //  Init a psoudostaff containing
                //   A bas   part,    (identified with "BAS", "B")
                //   A tenor part, (identified with "TENOR", "TEN" or "T") 
                userSettings.SelectedStaffs = GetSelectedStaffs(tbNames, fromBottom); // Select tenor and bas
                ConditionallyAddPseudoStaff(events, "BT", userSettings.SelectedStaffs);   // The use of "BT" instead of "TB" indicates that intervals are computed from the Bas note

                //  Only used for testing:
                //  Init a psoudostaff containing a soprano part,  an alto part,  a tenor part and  A bas   part
                userSettings.SelectedStaffs = GetSelectedStaffs(satbNames, fromBottom); // Select Soprano and Alto and Tenor and Bas
                ConditionallyAddPseudoStaff(events, "SATB", userSettings.SelectedStaffs);          
            }
            catch (Exception e)
            {
                // Be sure to restore userSettings !!
                Logger.LogCFE(e);
                result = false;
            }
            userSettings.SelectedStaffs = null; // Stop selecting a specific staff !
            userSettings.MusicAsSpeech = existingSpeechSettings; // Restore speech settings
            userSettings.MusicAsMusicBraille = existingMusicBrailleSettings; // Restore Music Braille settings

            return result;

        }

        // Start new

        /// <summary>
        /// Unconditionally add a pseudo staff to the current Stafflist
        /// </summary>
        /// <param name="events">The EventDescriptionList to use for generating the pseudostaf</param>
        /// <param name="name">The name of the pseudoStaff, for instance "TUTTI", "SA", "TB" or "SATB"</param>
        private void AddPseudoStaff(EventDescriptionList events, string name)
        {
            Staff staff = Staff.Create(name, this);
            Logger.LogCF(string.Format(": {0}", name));
            staff.Enabled = true;
            InitPseudoStaff(staff, events.firstEventDescription);
            allStaffs.Add(staff);
        }

        /// <summary>
        /// Conditionally Add a pseudo staff to the current StaffList
        /// </summary>
        /// <param name="events">The EventDescriptionList to use for generating the pseudostaff</param>
        /// <param name="staffName">The name of the pseudoStaff, for instance "SA", "TB" or "SATB" </param>
        /// <param name="selectedStaffs">The list of (real) staffs to include in the new pseudoStaff</param>
        private void ConditionallyAddPseudoStaff(EventDescriptionList events, string staffName, List<StaffSelector> selectedStaffs)
        {
            // We only create the pseudostaff if it consists of at least 2 (enabled) real staffs !
            if ((null != selectedStaffs) && (selectedStaffs.Count >= 2))
            {
                AddPseudoStaff(events, staffName);
            }
        }

        // End new 

        private List<StaffSelector> GetSelectedStaffs(List<List<string>> names, bool fromTop)
        {
            List<StaffSelector> result = new List<StaffSelector>();
            foreach (Staff staff in this.Staffs)
            {
                foreach (List<string> alii in names)
                {
                    // alii contains the various names for a part, for instance ("SOPRANO", "SOPRAN", "S")
                    if (alii.Contains(staff.PartName.ToUpper()))
                    {
                        result.Add(StaffSelector.Create(staff.PartId, staff.StaffNumber, fromTop));
                    }

                }
            }
            // We may choose other criteria, for instance that not all parts were represented.
            if (0 == result.Count) return null;
            return result;
        }

        private void InitPseudoStaff(Staff pseudoStaff, EventDescription currentEventDescription)
        {
            // Generate Music Braille
            List<BrailleBuilder> brailleBuilders = new List<BrailleBuilder>(); // For all staffs in all selected parts      
            while (null != currentEventDescription)
            {
                BrailleBuilder bb = currentEventDescription.ToBraille(true, true, out currentEventDescription); // (true,true) <=> ( "Merged parts", "Use interval notation")
                brailleBuilders.Add(bb);
            }

            pseudoStaff.BrailleMusicBrailleBuilders = brailleBuilders;
            Logger.LogCF(string.Format(": Staff {0} is represented by {1} BrailleBuilders. TotalLength={2}", pseudoStaff.Name, pseudoStaff.BrailleMusicBrailleBuilders.Count, GetLength(brailleBuilders)));
        }




        /// <summary>
        /// Unpacks the information held as BrailleBuilders to text representation.
        /// </summary>
        public void Unpack(int charsPerLine, int linesPerForm)
        {
            foreach (Staff staff in this.AllStaffs)
            {
                staff.Unpack(charsPerLine, linesPerForm);
            }

        }

        /// <summary>
        /// Unpacks the information held as BrailleBuilders to debugger representation.
        /// </summary>
        public void Merge(int charsPerLineline, int linesPerForm)
        {
            foreach (Staff staff in this.AllStaffs)
            {
                staff.Merge(charsPerLineline,linesPerForm);
            }

        }

        public void Sort()
        {
            Staffs.Sort(Compare);
        }

        private int Compare(Staff x, Staff y)
        {
            return x.StaffNumber - y.StaffNumber;
        }


        /// <summary>
        /// Creates a stafflist for a single EventDescription
        /// </summary>
        /// <param name="notes"></param>
        /// <returns></returns>
        public static StaffList Create(List<NoteElement> notes)
        {
            return new StaffList(notes);
        }

        /// <summary>
        /// Creates the global staffList for the whole score
        /// </summary>
        /// <param name="partDescriptionList"></param>
        public static StaffList Create(PartDescriptionList partDescriptionList,MetaInformation metaInformation)
        {
            return new StaffList(partDescriptionList,metaInformation);
        }


        /// <summary>
        /// Creates a global stafflist for the whole score. FOR BACKWARD COMPATIBILITY with version 3.0 !
        /// </summary>
        /// <param name="formattedString"></param>
        /// <returns></returns>
        public static StaffList Create(string formattedString,MetaInformation metaInformation, string name)
        {
            return new StaffList(formattedString,metaInformation,name);
        }
    }
}
