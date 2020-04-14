using System;
using System.Security.Cryptography;
using System.Text;
using CovidTracer.Models.Time;

namespace CovidTracer.Models.Keys
{
    /** 160 bits key that will be shared with the others users on a positive
     * infection. The key is derived every day from the master `TracerKey` and
     * is used to generate `HourlyTracerKey` every hour. */ 
    public class DailyTracerKey
    {
        public const int Length = 20; // Key length in bytes (limited by BLE).

        public readonly byte[] Value;

        public DailyTracerKey(byte[] key)
        {
            if (key.Length != Length) {
                throw new Exception("Key should be 256 bits long.");
            }

            Value = key;
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
    }
}
