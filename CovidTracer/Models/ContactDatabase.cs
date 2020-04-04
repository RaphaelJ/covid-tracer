using System;
using System.Collections.Generic;

namespace CovidTracer.Models
{
    /** Stores the CovidTracer contacts as detected by the Bluetooth scanning
     * service. */
    public /* Singleton */ class ContactDatabase
    {
        readonly Dictionary<CovidTracerID, SortedSet<ContactEncounter>>
            contacts =
                new Dictionary<CovidTracerID, SortedSet<ContactEncounter>>();

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

        /** Record a new encounter that happens at the moment. */
        public void NewEncounter(CovidTracerID id)
        {
            var encounter = ContactEncounter.Now();

            Logger.Info($"New encounter: {id} at {encounter}");

            lock (contacts) {
                if (contacts.ContainsKey(id)) {
                    var prevEncounters = contacts[id];

                    if (!prevEncounters.Contains(encounter)) {
                        // Previously meet device, new encounter.
                        prevEncounters.Add(encounter);
                    }
                } else {
                    // First time we meet that device.
                    contacts.Add(
                        id, new SortedSet<ContactEncounter> { encounter });
                }
            }
        }
    }
}
