using System;

namespace ChipEight.Net.Hardware
{
    public class Memory
    {
        private readonly byte[] _memory = new byte[4096];

        public byte this[int i]
        {
            get
            {
                if(i < 0 || i > 4095)
                    throw new ApplicationException($"Invalid memory address {i}");
                return _memory[i];
            }

            set
            {
                if (i < 0 || i > 4095)
                    throw new ApplicationException($"Invalid memory address {i}");
                _memory[i] = value;
            }
        }

        public void Clear()
        {
            _memory.Clear();
        }
    }
}
