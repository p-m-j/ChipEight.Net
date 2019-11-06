namespace ChipEight.Net
{
    public static class SoundHelpers
    {
        public static short[] GenerateSquareWave(uint sampleRate, int duration)
        {
            var len = sampleRate * duration;
            var raw = new short[len];

            const int pitch = 1024;
            const short frequency = 10000;

            for (uint i = 0; i < len; i++)
            {
                raw[i] = (short)(((i / (sampleRate / pitch) / 2) % 2) == 0 ? frequency : -frequency);
            }

            return raw;
        }
    }
}
