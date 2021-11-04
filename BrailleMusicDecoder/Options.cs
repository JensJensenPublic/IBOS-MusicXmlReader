using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrailleMusicDecoder
{

    /// <summary>
    /// Simple container for all options defined for the Decoder.
    /// </summary>
    public class Options
    {
        private RegionalOptionsEnum regionalOptions = RegionalOptionsEnum.UnKnown;
        public RegionalOptionsEnum RegionalOptions { get { return regionalOptions; } }

        private StringFormatOptions stringFormatOptions = StringFormatOptions.none;
        public StringFormatOptions StringFormatOptions { get { return stringFormatOptions; } }

        private InputCategoryEnum visibleCategories = InputCategoryEnum.None;
        public InputCategoryEnum VisibleCategoryies { get { return visibleCategories; } }

        private InputCategoryEnum visibleCategoryNames = InputCategoryEnum.None;
        public InputCategoryEnum VisibleCategoryNames { get { return visibleCategoryNames; } }

        //private Decoder.DevelopmentOptionEnum developmentOptions;
        //public Decoder.DevelopmentOptionEnum DevelopmentOptions { get { return developmentOptions; } }

        private bool developerMode;
        public bool DeveloperMode { get { return developerMode; } }

        private Options(RegionalOptionsEnum regionalOptions, StringFormatOptions stringFormatOptions, InputCategoryEnum visibleCategories, InputCategoryEnum visibleCategoryNames,  bool developerMode)
        {
            this.regionalOptions = regionalOptions;
            this.stringFormatOptions = stringFormatOptions;
            this.visibleCategories = visibleCategories;
            this.visibleCategoryNames = visibleCategoryNames;
            this.developerMode = developerMode;
        }

        public static Options Create(RegionalOptionsEnum regionalOptions, StringFormatOptions stringFormatOptions, InputCategoryEnum visibleCategories, InputCategoryEnum visibleCategoryNames,  bool developerMode)
        {
            return new Options(regionalOptions, stringFormatOptions, visibleCategories,visibleCategoryNames, developerMode);
        }
        

    }
}
