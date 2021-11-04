using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrailleMusicDecoder
{
    class MusicXmlBuilderStateUnknown : MusicXmlBuilderState
    {
        const string message = "Called unexpectedly";

        public override MusicXmlBuilderStateEnum GetState()
        {
            LogCF(message);
            return MusicXmlBuilderStateEnum.Unknown;
        }

        public override MusicXmlBuilderState ApplyNextInput(InputInterpretation input)
        {
            LogCF(message);
            return this;
        }

        public override void AddPart(InputInterpretation input)
        {
            LogCF(message);
        }

        public static MusicXmlBuilderStateUnknown Create(MusicXmlBuilder musicXmlBuilder)
        {
            return new MusicXmlBuilderStateUnknown(musicXmlBuilder);
        }

        private MusicXmlBuilderStateUnknown(MusicXmlBuilder musicXmlBuilder) : base(musicXmlBuilder)
        {
            LogCF("");
        }
    }
}
