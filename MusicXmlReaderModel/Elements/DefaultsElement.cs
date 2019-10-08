using System;
using System.Xml;
using System.Globalization;
using System.Diagnostics; // For finding calling method
using System.Reflection;  // For finding calling method

namespace MusicXmlReaderModel
{


    // https://usermanuals.musicxml.com/MusicXML/Content/EL-MusicXML-defaults.htm


    /// <summary>
    /// Contains information about the graphical formatting 
    /// This could be useful even for a blind user in a learning-situation where other
    /// (sighted) students may refer to systems and page numbers.
    /// </summary>
    public class DefaultsElement : Element
    {

        private ScalingElement scaling;
        private SystemLayoutElement systemLayout;
        private StaffLayoutElement staffLayout;
        private AppearanceElement appearance;
        private PageLayoutElement pageLayout;
        private EmptyFontElement musicFont;
        private EmptyFontElement wordFont;
        private LyricFontElement lyricFont;
        private LyricLanguageElement lyricLanguage;



        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        private DefaultsElement()
        { }


        /// <summary>
        /// Private constructor, used by the Crate() method
        /// </summary>
        /// <param name="node"></param>
        private DefaultsElement(XmlNode node)
        {
            // Dig out Attributes:
            foreach (XmlAttribute a in node.Attributes)
            {
                switch (a.Name)
                {
                    default:
                        //Logger.LogCFOnce(string.Format(": Unknown attribute. Name='{0}' Value='{1}'",  a.Name, a.Value));
                        LogFormatter.Log(once | unknown, a);
                        break;
                }
            }


            // Dig out elements
            foreach (XmlNode n in node.ChildNodes)
            {
                switch (n.Name)
                {
                    case "scaling":   scaling = ScalingElement.Create(n); break;
                    case "page-layout": pageLayout = PageLayoutElement.Create(n); break;
                    case "system-layout": systemLayout = SystemLayoutElement.Create(n); break;
                    case "staff-layout": staffLayout = StaffLayoutElement.Create(n); break;
                    case "appearance": appearance = AppearanceElement.Create(n); break; 
                    case "music-font": musicFont = EmptyFontElement.Create(n); break;
                    case "word-font": wordFont = EmptyFontElement.Create(n); break;
                    case "lyric-font": lyricFont = LyricFontElement.Create(n); break; 
                    case "lyric-language": lyricLanguage = LyricLanguageElement.Create(n); break;
                    default:
                        //Logger.LogCFOnce(string.Format(": Unknown element. Name='{0}' InnerText='{1}'", n.Name,n.InnerText));
                        LogFormatter.Log(once | unknown, n);
                        break;
                }
            }

            // LogFormatter.Log(once,  string.Format("DefaultsElement({0})",  ToDebugString())); // Only during initial debugging !
        }

        /// <summary>
        /// Only for debugging ! No localization !
        /// </summary>
        /// <returns></returns>
        public string ToDebugString()
        {
            string attributes = "";
            string elements = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8} ",
            (null != scaling) ? scaling.ToDebugString() + " " : "", // 0
            (null != pageLayout) ? pageLayout.ToDebugString() + " " : "", // 1
            (null != systemLayout) ? systemLayout.ToDebugString() + " " : "", // 2 ;
            (null != staffLayout) ? staffLayout.ToDebugString() + " " : "", // 3
            (null != appearance) ? appearance.ToDebugString() + " " : "", // 4
            (null != musicFont) ? musicFont.ToDebugString()+ " " : "", // 5
            (null != wordFont)  ? wordFont.ToDebugString() + " " : "", // 6
            (null != lyricFont) ? lyricFont.ToDebugString() + " ": "", // 7
            (null != lyricLanguage) ? lyricLanguage.ToDebugString() + " ": ""); // 8
            return attributes + elements;

        }

        public static DefaultsElement Create(XmlNode node)
        {
            return new DefaultsElement(node);
        }

        public static DefaultsElement Create(XmlNode node, bool supported)
        {
            if (supported) return new DefaultsElement(node);        
            LogFormatter.Log(always | unsupported, node);
            return null;
        }

        public override string ToString()
        {
            return "";
        }

    }
}

