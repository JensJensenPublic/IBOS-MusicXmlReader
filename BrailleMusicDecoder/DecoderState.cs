namespace BrailleMusicDecoder
{
    public class DecoderState
    {

        private DecoderStateMachine.StateEnum myStateEnum;
        public DecoderStateMachine.StateEnum MyStateEnum { get { return myStateEnum; } }
        private DecoderState previousDecoderState;
        public DecoderState PreviousDecoderState { get { return previousDecoderState; } }

        public override string ToString()
        {
            return MyStateEnum.ToString();
        }

        //Constructor
        public DecoderState(DecoderStateMachine.StateEnum stateEnum, DecoderState previousDecoderState)
        {
            myStateEnum = stateEnum;
            this.previousDecoderState = previousDecoderState;
        }
    }
}
