using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalizationAnalyzer
{
    class TranslationList
    {
        private List<Translation> translations;
        private CultureEnum culture;
        public CultureEnum Culture { get { return culture; } }
        private RessourcesEnum resource;
        public RessourcesEnum Resource { get { return resource; } }
        private TranslationList() { }
        private TranslationList(CultureEnum culture, RessourcesEnum resource, List<Translation> translations)
        {
            this.culture = culture;
            this.resource = resource;
            this.translations = translations;
        }

        static public TranslationList Create(CultureEnum culture, RessourcesEnum resource, List<Translation> translations)
        {
            return new TranslationList(culture, resource,translations);
        }

        private void Add(Translation translation)
        {
            this.translations.Add(translation);
        }

        public Translation GetTranslation(int i)
        {
            if (i < 0) return null;
            if (i >= translations.Count) return null;
            return translations[i];
        }

        public int NumberOfItems { get { return translations.Count; } }

        public override string ToString()
        {
            return string.Format("Culture='{0}' Resource='{1}' Count={2}", Analyzer.ToString(culture), Analyzer.ToTypeId(resource).TypeName.ToString() , this.translations.Count);
        }

    }
}
