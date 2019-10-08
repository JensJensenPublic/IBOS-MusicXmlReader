using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXmlReaderModel
{
    /// <summary>
    /// For selecting a specific staff in a specific part when generating BrailleMusic files.
    /// </summary>
    public class StaffSelector
    {
        private string partId;
        public string PartId { get { return partId; } }
        private int staff;
        public int Staff { get { return staff; } }
        private bool fromTop;
        public bool FromTop { get { return fromTop; } }

        private StaffSelector()
        { }

        private StaffSelector(string partId, int staff,bool fromTop)
        {
            this.partId = partId;
            this.staff = staff;
            this.fromTop = fromTop;
        }

        public bool Equals(string partId, int staff)
        {
            return ((0 == string.Compare(this.partId, partId)) && (this.staff == staff));
        }

        static public StaffSelector Create(string partId, int staff)
        {
            return new StaffSelector(partId, staff, false);
        }

        static public StaffSelector Create(string partId, int staff,bool fromTop)
        {
            return new StaffSelector(partId, staff, fromTop);
        }
    }

}
