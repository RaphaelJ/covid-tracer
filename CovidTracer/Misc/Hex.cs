using System;
using System.Collections.Generic;

namespace CovidTracer.Misc
{
    /** Provides routines to convert hexadecimal string to and from bytes
     * arrays.*/
    public static class Hex
    {
        const string ALPHABET = "0123456789ABCDEF";

        static public string ToString(byte[] value)
        {
            return BitConverter.ToString(value).Replace("-", "");
        }

        static public byte[] FromString(string value)
        {
            if (value.Length % 2 != 0) {
                throw new Exception(
                    "`value` should contain an even number of characters.");
            }

            // Creates an index of hex. characters to byte values.
            var charMap = new Dictionary<char, byte>();
            for (byte i = 0; i < ALPHABET.Length; ++i) {
                charMap.Add(ALPHABET[i], i);
            }

            // Converts every pair of characters to single bytes.
            var bytes = new byte[value.Length / 2];
            for (int i = 0; i < bytes.Length; ++i) {
                int higherVal = charMap[value[i * 2]] << 4;
                int lowerVal = charMap[value[i * 2 + 1]];

                bytes[i] = (byte) (higherVal | lowerVal);
            }

            return bytes;
        }
    }
}
