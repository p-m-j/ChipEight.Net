namespace ChipEight.Net.Hardware
{
    public class Registers
    {
        private readonly byte[] _v = new byte[17];

        public byte this[int i]
        {
            get => _v[i];
            set => _v[i] = value;
        }

        public void Clear()
        {
            _v.Clear();
        }
    }
}
