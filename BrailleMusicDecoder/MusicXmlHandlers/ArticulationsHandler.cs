using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using MusicXmlReaderModel;

namespace BrailleMusicDecoder
{

    /// <summary>
    /// Collects all articulations related for a note.
    /// This is needed because articulations in MusicBraille are placed before the note.
    /// </summary>
    class ArticulationsHandler
    {

        // Derived from
        // https://usermanuals.musicxml.com/MusicXML/Content/EL-MusicXML-articulations.htm
        // Listed in the same order as in the above link
        private enum Articulation
        {
            accent ,
            breathmark, //-
            caesura,
            detachedlegato, //-
            doit,falloff,
            otherarticulation, //-
            plop,
            scoop,
            spiccato,
            staccatissimo,
            staccato,
            stress,
            strongaccent, //-
            tenuto,
            unstress
        }

        string ToString(Articulation articulation)
        {
           switch(articulation)
           {
                // some MusicXml strings contain a hyphen, which is not legal syntax !
                case Articulation.breathmark: return "breath-mark";
                case Articulation.detachedlegato: return "detached-lagato";
                case Articulation.otherarticulation: return "other-articulation";
                case Articulation.strongaccent: return "strong-accent";
            }
            string result = articulation.ToString("G"); // Use the "General" formatting
            return result;
        }

        private MusicXmlElementFactory musicXmlElementFactory;
        private List<Articulation> articulations; 


        public void Clear()
        {
            articulations = new List<Articulation>(); 
        }

        private Articulation GetArticulation(InputSubCategoryEnum subCategory)
        {
            switch (subCategory)
           {
                case InputSubCategoryEnum.ArticulationAccent: return Articulation.accent;
                case InputSubCategoryEnum.ArticulationPortamento: return Articulation.otherarticulation;// Not exact match
                case InputSubCategoryEnum.ArticulationPortato: return Articulation.otherarticulation; // Not exact match
                case InputSubCategoryEnum.ArticulationStaccato: return Articulation.staccato;
                case InputSubCategoryEnum.ArticulationStaccatoAccent: return Articulation.staccato; // Not exact match
                case InputSubCategoryEnum.ArticulationStaccattissimo: return Articulation.staccatissimo;
                case InputSubCategoryEnum.ArticulationTenuto: return Articulation.tenuto;
            }
            Logger.LogCF(string.Format(": Unexpected parameter: {0}", subCategory));

            return Articulation.otherarticulation;
        }


        public void Add(InputSubCategoryEnum subCategory)
        {
            Articulation articulation = GetArticulation(subCategory);
            articulations.Add(articulation);
        }

        private int Compare(Articulation a1, Articulation a2)
        {
            return (int)a1 - (int)a2; // Or maybe the other way around ?
        }


        public void AddArticulations(XmlNode currentNode, bool clear)
        {
            if (0 == articulations.Count) return;
            XmlNode notationsNode = currentNode.SelectSingleNode("notations");
            XmlNode articulationsNode = musicXmlElementFactory.Element("articulations");
            notationsNode.AppendChild(articulationsNode);

            articulations.Sort(Compare);

            foreach (Articulation articulation in articulations)
            {
                string s = ToString(articulation); // NOT articulation.ToString() !!
                articulationsNode.AppendChild(musicXmlElementFactory.Element(s));
            }

            if (clear)
            {
                this.Clear();
            }
        }


        private ArticulationsHandler(MusicXmlElementFactory musicXmlElementFactory)
        {
            this.musicXmlElementFactory = musicXmlElementFactory;
            articulations = new List<Articulation>();
        }

        public static ArticulationsHandler Create(MusicXmlElementFactory musicXmlElementFactory)
        {
            return new ArticulationsHandler(musicXmlElementFactory);
        }
    }
}
