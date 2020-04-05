using System.Collections.Generic;

namespace CovidTracer.Models
{
    public /* Singleton */ class CaseDatabase
    {
        public readonly IDictionary<CovidTracerID, Case> Cases =
            new Dictionary<CovidTracerID, Case>();

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
            var id = new CovidTracerID("ABCDEFGH");
            Cases.Add(id,
                new Case(CaseType.Positive,
                    new Date(2020, 3, 30), new Date(2020, 4, 15)));

            Cases.Add(new CovidTracerID("12345678"),
                new Case(CaseType.Symptomatic,
                    new Date(2020, 3, 15), new Date(2020, 3, 30)));

            Cases.Add(new CovidTracerID("A1B2C3D4"),
                new Case(CaseType.Symptomatic,
                    new Date(2020, 3, 15), new Date(2020, 4, 1)));
        }

        /** Classifies the contact's encounter based on the current case
         * database. */
        public CaseType Classify(
            CovidTracerID contact, ContactEncounter encounter)
        {
            if (Cases.ContainsKey(contact)) {
                var case_ = Cases[contact];

                if (encounter.Intersect(case_)) {
                    return case_.Type;
                } else {
                    return CaseType.Safe;
                }
            } else {
                return CaseType.Safe;
            }
        }
    }
}
