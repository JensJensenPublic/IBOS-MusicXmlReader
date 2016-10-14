using System.Xml;
using System.Globalization;
using MusicXmlReaderModel;

namespace MusicXmlReaderUI
{

    enum CreatorTypeEnum { unknown, composer, poet, lyricist, arranger,translator };

    class CreatorElement : Element
    {

        private CreatorTypeEnum creatorType;
        string value = "";

        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        private CreatorElement()
        { }


        private CreatorTypeEnum GetCreatorType(string s) 
        {
            string functionName = "GetCreatorType";
            switch (s)
            {
                case "composer": return CreatorTypeEnum.composer;
                case "poet": return CreatorTypeEnum.poet;
                case "lyricist": return CreatorTypeEnum.lyricist;
                case "arranger": return CreatorTypeEnum.arranger;
                case "translator": return CreatorTypeEnum.translator;
                default:
                    Logger.LogOnce(string.Format("{0}: Unknown Creator Type= {1}", functionName, s));
                    return CreatorTypeEnum.unknown;
            }
        }


        /// <summary>
        /// Private constructor, used by the Crate() method
        /// </summary>
        /// <param name="node"></param>
        private CreatorElement(XmlNode node)
        {
           
            // Dig out attributes
            foreach (XmlAttribute a in node.Attributes)
            {
                switch (a.Name)
                {
                    case "type":
                        creatorType = GetCreatorType(a.Value);                  
                        break;
                }
            }

            this.value = node.InnerText;
        }

        public static CreatorElement Create(XmlNode node)
        {
            return new CreatorElement(node);
        }

        private string LocalizeCreatorType(CreatorTypeEnum creatorType)
        {
            string functionName = "LocalizeCreatorType";
            switch (creatorType)
            {
                case CreatorTypeEnum.arranger: return ResourcesForModel.CreatorElementType_Arranger;
                case CreatorTypeEnum.composer: return ResourcesForModel.CreatorElementType_Composer;
                case CreatorTypeEnum.lyricist: return ResourcesForModel.CreatorElementType_Lyricist;
                case CreatorTypeEnum.poet: return ResourcesForModel.CreatorElementType_Poet;
                case CreatorTypeEnum.translator: return ResourcesForModel.CreatorElementType_Translator;
                case CreatorTypeEnum.unknown:
                    Logger.LogOnce(string.Format("{0}: CreatorType is unknown", functionName ));
                    return ResourcesForModel.CreatorElementType_Unknown;
                default:
                    Logger.LogOnce(string.Format("{0}: Unsupported CreatorType={1}", functionName, creatorType.ToString()));
                    return ResourcesForModel.CreatorElementType_Unknown;
            }
        }

        
        public override string ToString() 
        {
            return string.Format("{0}: {1}",LocalizeCreatorType(creatorType),value );
        }



    }
}
