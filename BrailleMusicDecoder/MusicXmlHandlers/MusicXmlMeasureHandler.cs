using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrailleMusicDecoder
{

    /// <summary>
    /// Handle the temporary disabling of generation of MusicXml Measures in a few cases:
    /// 1) After a FullMeasureInAccord
    /// 2) After a PartMeasureInAccord
    /// 3) After a "Musical Hyphen" (Danish: "Henvisningstegn"
    /// The generation of MusicXml Measures is enabled again on the first note or rest.
    /// </summary>
    class MusicXmlMeasureHandler
    {
        private bool enableNewMeasure = true;

        // For setting breakpoints easily !
        private void Modify(bool b)
        {
            enableNewMeasure = b;
        }

        // For setting breakpoints easily !
        private void Clear()
        {
            Modify(false);
        }

        // For setting breakpoints easily !
        private void Set()
        {
            Modify(true);
        }


        /// <summary>
        /// Returne true iff generation of NewMeasure on a SPACE input character is enabled.
        /// </summary>
        /// <returns></returns>
        public bool OnSpace()
        {
            return enableNewMeasure;
        }

        public void OnNote()
        {
            Set();
        }

        public void OnRest()
        {
            Set();
        }

        public void OnFullMeasureInAccord()
        {
            Clear(); 
        }

        public void OnPartMeasureInAccord()
        {
            Clear();
        }

        /// <summary>
        /// Danish: "Henvisningstegn"
        /// </summary>
        public void OnMusciHyphen()
        {
            Clear();
        }

        public void OnAnyHarmonyInput()
        {
            Set();
        }


        public void OnBeat()
        {
            Clear();
        }

        private MusicXmlMeasureHandler()
        {
            Clear();
        }

        public static MusicXmlMeasureHandler Create()
        {
            return new MusicXmlMeasureHandler();
        }
    }
}
