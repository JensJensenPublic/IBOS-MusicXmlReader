namespace MusicXmlReaderModel
{
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

    }
}
