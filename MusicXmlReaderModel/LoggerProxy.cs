namespace MusicXmlReaderModel
{
#if false
    class LoggerProxy : BrailleMusicDecoder.IBrailleMusicDecoderLogger
    {
        /// <summary>
        /// For interfacing to BrailleMusicDecoder
        /// </summary>
        /// <param name="s"></param>
        public void Log(string s)
        {
            Logger.Log(s);
        }

        public int GetDecoderOptions()
        {
            return (int) Logger.DecoderOptions;
        }

    }
#endif
}
