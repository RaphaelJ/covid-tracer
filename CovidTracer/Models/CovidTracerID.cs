using System;
using System.Text;
using Xamarin.Essentials;

namespace CovidTracer.Models
{
    /** Uniquely identifies the app instance using a randomly generated
     * alphanumerical string. */
    public /* Singleton */ class CovidTracerID
    {
        public const int LEN = 8;
        public const string ALPHABET = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        const string PREFERENCE_KEY = "covid_tracer_id";

        public readonly string Value;

        /** Fetches the previously generated ID from the preferences, or
         * generates it if it does not exist yet. */
        static public CovidTracerID GetInstance()
        {
            // FIXME Should be protected by a lock.
            if (Preferences.ContainsKey(PREFERENCE_KEY)) {
                return new CovidTracerID(Preferences.Get(PREFERENCE_KEY, null));
            } else {
                var id = new CovidTracerID();
                Preferences.Set(PREFERENCE_KEY, id.Value);
                return id;
            }
        }

        /** Creates a new randomly generated ID. */
        protected CovidTracerID()
        {
            Random random = new Random();

            var value = new char[LEN];
            for (int i = 0; i < LEN; ++i) {
                value[i] = ALPHABET[random.Next(ALPHABET.Length)];
            }

            Value = new String(value);
        }

        protected CovidTracerID(string value_)
        {
            Value = value_;
        }

        public byte[] AsBytes()
        {
            return ASCIIEncoding.ASCII.GetBytes(Value);
        }
    }
}
