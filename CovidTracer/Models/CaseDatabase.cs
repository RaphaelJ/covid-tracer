using System;
using System.Collections.Generic;

using CovidTracer.Models.Keys;
using CovidTracer.Models.Time;

namespace CovidTracer.Models
{
    public /* Singleton */ class CaseDatabase
    {
        public readonly IDictionary<HourlyTracerKey, Case> Cases =
            new Dictionary<HourlyTracerKey, Case>();

        static private CaseDatabase instance = null;

        static public CaseDatabase GetInstance()
        {
            if (instance != null) { // FIXME Instance should be locked.
                return instance;
            } else {
                instance = new CaseDatabase();
                return instance;
            }
        }

        private CaseDatabase()
        {
        }

        /** Classifies the contact's encounter based on the current case
         * database. */
        public InfectionStatus Classify(HourlyTracerKey contact, DateHour time)
        {
            if (Cases.ContainsKey(contact)) {
                var case_ = Cases[contact];

                // FIXME We might relax this constraint a little bit by allowing
                // some overlap with the previous and next day.
                if (case_.Day == time.AsDate()) {
                    switch (case_.Type) {
                    case CaseType.Positive:
                        return InfectionStatus.Positive;
                    case CaseType.Symptomatic:
                        return InfectionStatus.Symptomatic;
                    default:
                        throw new Exception("Invalid case type.");
                    }
                } else {
                    return InfectionStatus.Safe;
                }
            } else {
                return InfectionStatus.Safe;
            }
        }
    }
}
