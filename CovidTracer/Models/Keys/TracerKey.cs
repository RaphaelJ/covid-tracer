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

using Xamarin.Essentials;

using CovidTracer.Models.Time;

namespace CovidTracer.Models.Keys
{
    /** Random 256-bits key generated the first time the app is loaded.
     *
     * This key is never shared. `DailyTracerKey`s are derived daily from this
     * key.
     */
    public class TracerKey
    {
        public const int Length = 256 / 8; // Key length in bytes.

        public readonly byte[] Value;

        /** Creates a `TracerKey` instance from an hexadecimal string. */
        public TracerKey(string key) :
            this(Misc.Hex.FromString(key))
        {
        }

        public TracerKey(byte[] key)
        {
            if (key.Length != Length) {
                throw new Exception($"Key should be {Length * 8} bits long.");
            }

            Value = key;
        }

        /** Creates a new randomly-generated `TracerKey` instance. */
        public TracerKey()
        {
            Value = new byte[Length];

            using (var rng = new RNGCryptoServiceProvider()) {
                rng.GetBytes(Value);
            }
        }

        /** Fetches the previously generated app ID from the preferences, or
         * generates it if it does not exist yet. */
        static public TracerKey CurrentAppInstance()
        {
            const string PREFERENCE_KEY = "tracer_key";

            // FIXME Should be protected by a lock.
            if (Preferences.ContainsKey(PREFERENCE_KEY)) {
                return new TracerKey(Preferences.Get(PREFERENCE_KEY, null));
            } else {
                var id = new TracerKey();
                Preferences.Set(PREFERENCE_KEY, id.ToString());
                return id;
            }
        }

        public DailyTracerKey DerivateDailyKey(Date date)
        {
            using (var hmac = new HMACSHA256(Value)) {
                var dateBytes = ASCIIEncoding.ASCII.GetBytes(date.ToString());
                return new DailyTracerKey(hmac.ComputeHash(dateBytes));
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
