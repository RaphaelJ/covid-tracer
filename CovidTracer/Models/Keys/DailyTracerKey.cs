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
using System.Security.Cryptography;
using System.Text;

using CovidTracer.Models.Time;

namespace CovidTracer.Models.Keys
{
    /** 256 bits key that will be shared with the others users on a positive
     * infection. The key is derived every day from the master `TracerKey` and
     * is used to generate `HourlyTracerKey` every hour. */ 
    public class DailyTracerKey
    {
        public const int Length = 256 / 8; // Key length in bytes.

        public readonly byte[] Value;

        public DailyTracerKey(byte[] key)
        {
            if (key.Length != Length) {
                throw new Exception($"Key should be {Length * 8} bits long.");
            }

            Value = key;
        }

        public DailyTracerKey(string value)
            : this(Misc.Hex.FromString(value))
        {
        }

        public HourlyTracerKey DerivateHourlyKey(DateHour date)
        {
            using (var hmac = new HMACSHA256(Value)) {
                var dateBytes = ASCIIEncoding.ASCII.GetBytes(date.ToString());
                var hash = hmac.ComputeHash(dateBytes);

                // Truncates hourly key to fit in a BLE packet.
                var truncatedHash = new byte[HourlyTracerKey.Length];
                Array.Copy(hash, truncatedHash, HourlyTracerKey.Length);
                    
                return new HourlyTracerKey(truncatedHash);
            }
        }

        public override string ToString()
        {
            return Misc.Hex.ToString(Value);
        }

        public string ToHumanReadableString()
        {
            return Misc.Hex.ToHumanReadableString(Value);
        }
    }
}
