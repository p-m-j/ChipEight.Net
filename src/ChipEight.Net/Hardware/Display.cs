namespace ChipEight.Net.Hardware
{
    public class Display
    {
        private byte[] _display = new byte[64 * 32];

        public void Clear()
        {
            _display = new byte[64 * 32];
        }

        public bool DrawSprite(int x, int y, byte sprite)
        {
            var collision = false;
            collision |= SetPixel(x + 0, y, (sprite & 0b10000000) == 0b10000000);
            collision |= SetPixel(x + 1, y, (sprite & 0b01000000) == 0b01000000);
            collision |= SetPixel(x + 2, y, (sprite & 0b00100000) == 0b00100000);
            collision |= SetPixel(x + 3, y, (sprite & 0b00010000) == 0b00010000);
            collision |= SetPixel(x + 4, y, (sprite & 0b00001000) == 0b00001000);
            collision |= SetPixel(x + 5, y, (sprite & 0b00000100) == 0b00000100);
            collision |= SetPixel(x + 6, y, (sprite & 0b00000010) == 0b00000010);
            collision |= SetPixel(x + 7, y, (sprite & 0b00000001) == 0b00000001);
            return collision;
        }

        public bool SetPixel(int x, int y, bool on)
        {
            if(x < 0 || y < 0 || x > 63 || y > 31)
                return false; // Off screen, NOP, Emu should maybe support wrap flag.

            var yOffset = y * 64;

            var i = yOffset + x;

            var current = _display[i];
            var update = on ? 0x1 : 0x0;

            _display[i] = (byte) (current ^ update);

            return current == 0x1 && _display[i] == 0x00;
        }

        public bool GetPixel(int x, int y)
        {
            var yOffset = y * 64;
            var i = yOffset + x;

            return _display[i] == 0x1;
        }
    }
}
