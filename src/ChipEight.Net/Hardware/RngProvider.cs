using System;

namespace ChipEight.Net.Hardware
{
    public class RngProvider : IRngProvider
    {
        private readonly Random _random;

        public RngProvider()
        {
            _random = new Random();
        }

        public byte Next()
        {
            return (byte) _random.Next(0, 255);
        }
    }
}
