namespace GsmGadgetry.Text
{
    using System;
    using System.Text;

    public class Gsm7BitEncoding : Encoding
    {
        private class Gsm7BitEncoder
        {
            public int GetByteCount(char[] chars, int index, int count)
            {
                int length = chars.Length - index;

                if (length > count)
                {
                    length = count;
                }

                return length - length / 8;
            }

            public int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
            {
                int byteCount = GetByteCount(chars, charIndex, charCount);

                for (int i = 0; i < byteCount; i++)
                {
                                                 // { 0 1 2 3 4 5 6 7 8  9 10 11 12 13 14 15 16 }
                    int readIndex = i + (i / 7); // { 0 1 2 3 4 5 6 8 9 10 11 12 13 14 16 17 18 }
                    int space = (i % 7) + 1;     // { 1 2 3 4 5 6 7 1 2  3  4  5  6  7  1  2  3 }

                    int encodedByte = chars[charIndex + readIndex] & 0x7F;

                    // lose previously carried bits
                    encodedByte >>= space - 1;
                    encodedByte <<= space - 1;

                    // align left
                    encodedByte <<= 1;

                    // make space for carry bits
                    encodedByte >>= space;

                    if (readIndex < charCount - 1)
                    {
                        // get carry bits
                        int carryByte = chars[readIndex + 1] & 0x7F;
                        carryByte <<= (8 - space);
                        carryByte &= 0xFF;

                        // apply carry bits
                        encodedByte |= carryByte;
                    }

                    bytes[byteIndex + i] = (byte)encodedByte;
                }

                return byteCount;
            }
        }

        private static Gsm7BitEncoding _encoding;
        private Gsm7BitEncoder _encoder;

        public static Encoding Gsm7Bit
        {
            get { return _encoding ?? (_encoding = new Gsm7BitEncoding()); }
        }

        public override Decoder GetDecoder()
        {
            throw new NotSupportedException();
        }

        private Gsm7BitEncoder GetEncoder()
        {
            return _encoder ?? (_encoder = new Gsm7BitEncoder());
        }

        public int GetByteCount(char[] chars, int index, int count)
        {
            var encoder = GetEncoder();
            return encoder.GetByteCount(chars, index, count);
        }

        public override byte[] GetBytes(string text)
        {
            char[] chars = text.ToCharArray();
            int byteCount = GetByteCount(chars, 0, chars.Length);

            var bytes = new byte[byteCount];
            GetBytes(chars, 0, text.Length, bytes, 0);

            return bytes;
        }

        public int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
        {
            var encoder = GetEncoder();
            return encoder.GetBytes(chars, charIndex, charCount, bytes, byteIndex);
        }

        public int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
        {
            throw new NotSupportedException();
        }
    }
}
