namespace ChipEight.Net.Hardware
{
    public static class Chip8Extensions
    {
        public static void Clear(this byte[] zone)
        {
            for (var i = 0; i < zone.Length; i++)
                zone[i] = 0;
        }

        public static void Clear(this ushort[] zone)
        {
            for (var i = 0; i < zone.Length; i++)
                zone[i] = 0;
        }

        public static void Clear(this bool[] zone)
        {
            for (var i = 0; i < zone.Length; i++)
                zone[i] = false;
        }
    }
}
