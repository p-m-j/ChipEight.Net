namespace ChipEight.Net.Hardware
{
    public class Stack
    {
        private const int MaxSize = 24;

        public short SP { get; private set; }
        public ushort[] Entries { get; }
        public ushort Head => Entries[SP];

        public Stack()
        {
            SP = -1;
            Entries = new ushort[MaxSize];
        }

        public void Push(ushort value)
        {
            SP++;
            Entries[SP] = value;
        }

        public ushort Pop()
        {
            var value = Head;
            SP--;
            return value;
        }

        public void Clear()
        {
            SP = -1;
            Entries.Clear();
        }
    }
}
