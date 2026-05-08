namespace Project3.OscilloPatch
{
    public sealed class SignalCompiler
    {
        public SignalPair Compile(ChannelCircuit xChannel, ChannelCircuit yChannel)
        {
            return new SignalPair(xChannel.BuildSignal(), yChannel.BuildSignal());
        }
    }
}
