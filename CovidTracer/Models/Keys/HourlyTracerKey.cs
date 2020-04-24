using System;
using CovidTracer.Models.Time;

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
