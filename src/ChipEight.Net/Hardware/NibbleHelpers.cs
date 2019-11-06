using System;

namespace ChipEight.Net.Hardware
{
    public enum WordParts
    {
        High,
        Low
    }

    public enum NibbleParts
    {
        Upper,
        Lower
    }

    public static class NibbleHelpers
    {
        public static byte GetNibble(this byte @byte, NibbleParts whichNibble)
        {
            if (whichNibble == NibbleParts.Lower)
                return (byte)(@byte & 0x0F);
            return (byte)((@byte & 0xF0) >> 4);
        }

        public static byte GetByte(this ushort word, WordParts whichByte)
        {
            if (whichByte == WordParts.Low)
                return (byte) (word & 0x00FF);
            return (byte)((word & 0xFF00) >> 8);
        }

        /// <summary>
        /// Get nibble by index where 1 is most significant nibble, 4 is least significant nibble
        /// </summary>
 
        public static byte GetNibble(this ushort word, int index)
        {
            switch (index)
            {
                case 1:
                    return word.GetNibble(WordParts.High, NibbleParts.Upper);
                case 2:
                    return word.GetNibble(WordParts.High, NibbleParts.Lower);
                case 3:
                    return word.GetNibble(WordParts.Low, NibbleParts.Upper);
                case 4:
                    return word.GetNibble(WordParts.Low, NibbleParts.Lower);
                default:
                    throw new NotImplementedException();
            }
        }

        public static byte GetNibble(this ushort word, WordParts whichByte, NibbleParts whichNibble)
        {
            return word.GetByte(whichByte).GetNibble(whichNibble);
        }
    }
}
