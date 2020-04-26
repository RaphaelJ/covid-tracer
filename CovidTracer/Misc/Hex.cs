// Copyright 2020 Raphael Javaux
//
// This file is part of CovidTracer.
//
// CovidTracer is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// CovidTracer is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with CovidTracer. If not, see<https://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Text;

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

        /** Returns the tracer key as a string with a separators to improve
         * readability. */
        static public string ToHumanReadableString(
            byte[] value,
            int groupSize = 4,
            string sepatator = " "
            )
        {
            var asString = ToString(value);

            var builder = new StringBuilder();

            for (int i = 0; i < asString.Length; ++i) {
                if (i > 0 && i % groupSize == 0) {
                    builder.Append(sepatator);
                }
                builder.Append(asString[i]);
            }

            return builder.ToString();
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
