using System.Collections.Generic;
using System.Diagnostics;

using CovidTracer.Models.Keys;
using CovidTracer.Models.Time;

namespace CovidTracer.Models
{
    /** Stores the CovidTracer contacts as detected by the Bluetooth scanning
     * service. */
    public /* Singleton */ class ContactDatabase
    {
        /** Previously encountered hourly identifiers. */
        public readonly Dictionary<HourlyTracerKey, SortedSet<DateHour>>
            Contacts = new Dictionary<HourlyTracerKey, SortedSet<DateHour>>();

        static private ContactDatabase instance = null;

        static public ContactDatabase GetInstance()
        {
            if (instance != null) { // FIXME Instance should be locked.
                return instance;
            } else {
                instance = new ContactDatabase();
                return instance;
            }
        }

        public ContactDatabase()
        {
        }

        /** Record a new device contact that happens at the moment. */
        public void NewContact(HourlyTracerKey key)
        {
            NewContact(key, DateHour.Now);
        }

        public void NewContact(HourlyTracerKey key, DateHour time)
        {
            Logger.Info($"New encounter: {key} at {time}.");

            lock (Contacts) {
                if (!Contacts.ContainsKey(key)) {
                    Contacts.Add(key, new SortedSet<DateHour>() { time });
                } else {
                    Contacts[key].Add(time);
                }
            }
        }
    }
}
