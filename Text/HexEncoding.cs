namespace GsmGadgetry.Text
{
    using System;
    using System.Text;

    public class HexEncoding : Encoding
    {
        private class HexDecoder : Decoder
        {
            public override void Convert(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex, int charCount, bool flush, out int bytesUsed, out int charsUsed, out bool completed)
            {
                charCount = GetChars(bytes, byteIndex, byteCount, chars, charIndex);
                
                bytesUsed = byteCount;
                charsUsed = charCount;
                completed = true;
            }

            public int GetCharCount(byte[] bytes, int index, int count)
            {
                int length = count - index;
                if (length > bytes.Length)
                {
                    length = bytes.Length;
                }

                return length * 2;
            }

            public int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
            {
                int charCount = GetCharCount(bytes, byteIndex, byteCount);
                for (int i = 0; i < byteCount; i++)
                {
                    chars[charIndex + i * 2] = GetHexChar(bytes[byteIndex + i], true);
                    chars[charIndex + i * 2 + 1] = GetHexChar(bytes[byteIndex + i], false);
                }

                return charCount;
            }

            private static char GetHexChar(int octet, bool highBits)
            {
                int semiOctet = (highBits ? (octet >> 4) : octet) & 0x0F;

                switch (semiOctet)
                {
                    case 0x0F:
                        return 'F';

                    case 0x0E:
                        return 'E';

                    case 0x0D:
                        return 'D';

                    case 0x0C:
                        return 'C';

                    case 0x0B:
                        return 'B';

                    case 0x0A:
                        return 'A';

                    default:
                        return semiOctet.ToString()[0];
                }
            }
        }

        private static HexEncoding _encoding;
        private HexDecoder _decoder;

        public static Encoding Hex
        {
            get { return _encoding ?? (_encoding = new HexEncoding()); }
        }

        public override Decoder GetDecoder()
        {
            return _decoder ?? (_decoder = new HexDecoder()); 
        }

        public int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
        {
            throw new NotSupportedException();
        }

        public int GetCharCount(byte[] bytes, int index, int count)
        {
            var decoder = (HexDecoder)GetDecoder();
            return decoder.GetCharCount(bytes, index, count);
        }

        public override char[] GetChars(byte[] bytes)
        {
            int charCount = GetCharCount(bytes, 0, bytes.Length);

            var chars = new char[charCount];
            GetChars(bytes, 0, bytes.Length, chars, 0);

            return chars;
        }

        public int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
        {
            var decoder = (HexDecoder)GetDecoder();
            return decoder.GetChars(bytes, byteIndex, byteCount, chars, charIndex);
        }
    }
}
