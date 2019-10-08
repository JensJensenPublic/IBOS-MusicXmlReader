namespace MusicXmlReaderModel
{
    
    public abstract class Element : MusicXmlObject
    {
        public virtual string Caption { get { return ""; } }
        // abstract public Element Create(XmlNode node);
        // abstract public string Format();

        /// <summary>
        /// To be overwritten in Elements which have a staffNumber
        /// </summary>
        /// <returns></returns>
        public virtual int GetStaffNumber()
        {
            return -1;
        }


        // Some simple convenience constants for reducing amount of sourcecode
        protected const LogFormatter.LogOptions once = LogFormatter.LogOptions.Once;
        protected const LogFormatter.LogOptions always = LogFormatter.LogOptions.Always;
        protected const LogFormatter.LogOptions unsupported = LogFormatter.LogOptions.Unsupported;
        protected const LogFormatter.LogOptions unknown = LogFormatter.LogOptions.Unknown;
        protected const LogFormatter.LogOptions quiet = LogFormatter.LogOptions.Quiet;
        protected const LogFormatter.LogOptions dumb = LogFormatter.LogOptions.Dumb;
        protected const LogFormatter.LogOptions verbose = LogFormatter.LogOptions.Verbose;


    }
}
