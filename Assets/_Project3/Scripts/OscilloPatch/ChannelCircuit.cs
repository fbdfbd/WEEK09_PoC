namespace Project3.OscilloPatch
{
    public sealed class ChannelCircuit
    {
        private readonly SignalPart[] slots;

        public int SlotCount => slots.Length;

        public ChannelCircuit(int slotCount)
        {
            slots = new SignalPart[slotCount];
        }

        public SignalPart GetPart(int slotIndex)
        {
            return slots[slotIndex];
        }

        public SignalPart SetPart(int slotIndex, SignalPart part)
        {
            SignalPart previous = slots[slotIndex];
            slots[slotIndex] = part;
            return previous;
        }

        public void Clear()
        {
            for (int index = 0; index < slots.Length; index++)
            {
                slots[index] = null;
            }
        }

        public Signal BuildSignal()
        {
            Signal signal = new Signal();

            foreach (SignalPart part in slots)
            {
                if (part == null)
                {
                    continue;
                }

                signal.Apply(part);
            }

            return signal;
        }
    }
}
