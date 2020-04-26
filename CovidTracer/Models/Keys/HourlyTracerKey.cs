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

namespace CovidTracer.Models.Keys
{
    /** 160 bits key that will be broadcasted over Bluetooth with nearby users.
     * The key is derived every hour from the master `DailerTracerKey`. */
    public class HourlyTracerKey
    {
        public const int Length = 20; // Key length in bytes (limited by BLE).

        public readonly byte[] Value;

        public HourlyTracerKey(byte[] key)
        {
            if (key.Length != Length) {
                throw new Exception($"Key should be {Length * 8} bits long.");
            }

            Value = key;
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
