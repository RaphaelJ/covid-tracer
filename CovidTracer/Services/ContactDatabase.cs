using System;
using System.IO;

using SQLite;
using System.Collections.Generic;

using CovidTracer.Models.Keys;
using CovidTracer.Models.SQLite;
using CovidTracer.Models.Time;

namespace CovidTracer.Services
{
    /** Manages and matches the contacts keys (as discovered by the BLE scanning
     * service) and the reference cases (obtained by periodically querying
     * the backend) */
    public class ContactDatabase
    {
        /** The result of a know case comparison. Contains the set of close
         * contacts classified as positive and/or symptomatic. */
        public class ContactMatches
        {
            public readonly SortedSet<DateHour> Positives;
            public readonly SortedSet<DateHour> Symptomatics;

            public ContactMatches()
            {
                Positives = new SortedSet<DateHour>();
                Symptomatics = new SortedSet<DateHour>();
            }
        }

        public enum InfectionStatus
        {
            /** We didn't find any close contact. */
            Safe,

            /** We found contact with symptomatic cases. */
            Symptomatic,

            /** We found contact with Covid-19 positive cases. */
            Positive
        }

        const string DB_FILENAME = "covid_tracer.sqlite3";

        SQLiteConnection db;

        /** The currently detected positives and symptomatic contacts.
         *
         * Always lock the value when accessing it. */
        public ContactMatches Matches { get; protected set; }

        public delegate void MatchesChangeHandler(
            object sender, ContactMatches matches);
        /** Emitted when the `Matches` value change. */
        public event MatchesChangeHandler MatchesChange;

        public InfectionStatus CurrentInfectionStatus
        {
            get {
                lock (Matches) {
                    if (Matches.Positives.Count > 0) {
                        return InfectionStatus.Positive;
                    } else if (Matches.Symptomatics.Count > 0) {
                        return InfectionStatus.Symptomatic;
                    } else {
                        return InfectionStatus.Safe;
                    }
                }
            }
        }

        public delegate void CurrentInfectionStatusChangeHandler(
            object sender, InfectionStatus newStatus);
        /** Emitted when the `CurrentInfectionStatus` value change. */
        public event CurrentInfectionStatusChangeHandler
            CurrentInfectionStatusChange;

        public ContactDatabase()
        {
            db = new SQLiteConnection(GetDBPath());

            Matches = new ContactMatches();

            CreateTables();

            ComputeMatches();


            var date = DateHour.Now;

            // Create a timer with a two second interval.
            var aTimer = new System.Timers.Timer(10000);
            
            aTimer.Elapsed += (o, e) => {
                lock (Matches) {
                    Matches.Positives.Add(date);

                    MatchesChange?.Invoke(this, Matches);
                    CurrentInfectionStatusChange?.Invoke(this, CurrentInfectionStatus);
                }
                date = date.Next;
            };
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }

        /** Replaces the current case database with the provided new case set.
         */
        public void LoadCases(IEnumerable<Models.Case> cases)
        {
            db.RunInTransaction(() => {
                db.DropTable<Case>();

                foreach (var case_ in cases) {
                    // Creates an entry for every hourly derived key of the
                    // case.

                    var caseType = case_.Type == Models.CaseType.Positive
                        ? "positive" : "symptomatic";

                    for (int hour = 0; hour < 24; ++hour) {
                        var hourlyKey = case_.Key.DerivateHourlyKey(
                            case_.Day.WithHour(hour));

                        var sqlCase = new Case {
                            Key = hourlyKey.Value,
                            Type = caseType,
                            Year = case_.Day.Year,
                            Month = case_.Day.Month,
                            Day = case_.Day.Day,
                        };
                    }
                }
            });

            ComputeMatches();
        }

        /** Record a new device contact that happens at the moment. */
        public void NewContact(HourlyTracerKey key)
        {
            NewContact(key, DateHour.Now);
        }

        public void NewContact(HourlyTracerKey key, DateHour time)
        {
            Case matchingCase = null;

            // Adds the new case to the database.

            db.RunInTransaction(() => {
                var exists = db.Find<Contact>(c =>
                    c.Year == time.Year
                    && c.Month == time.Month
                    && c.Day == time.Day
                    && c.Hour == time.Hour
                    && c.Key == key.Value
                ) != null;

                if (!exists) {
                    Logger.Info($"New contact: {key} at {time}.");

                    db.Insert(new Contact {
                        Key = key.Value,
                        Year = time.Year,
                        Month = time.Month,
                        Day = time.Day,
                        Hour = time.Hour
                    });

                    // Gets any matching know case

                    matchingCase = db.Find<Case>(ca =>
                        ca.Key == key.Value
                        && ca.Year == time.Year
                        && ca.Month == time.Month
                        && ca.Day == time.Day
                    );
                }
            });

            // Classifies the newly added case

            if (matchingCase != null) {
                lock (Matches) {
                    if (matchingCase.Type == "positive") {
                        Matches.Positives.Add(time);
                    } else {
                        Matches.Symptomatics.Add(time);
                    }
                }

                MatchesChange?.Invoke(this, Matches);
                CurrentInfectionStatusChange?.Invoke(
                    this, CurrentInfectionStatus);
            }
        }

        public IDictionary<String, Object> GetStats()
        {
            var contactCount = db.ExecuteScalar<int>(
                "select count(*) from contacts;");
            var caseCount = db.ExecuteScalar<int>(
                "select count(*) from cases;");

            return new Dictionary<String, Object> {
                { "contacts.Count", contactCount },
                { "cases.Count", caseCount}
            };
        }

        protected string GetDBPath()
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Personal),
                DB_FILENAME);
        }

        protected void CreateTables()
        {
            db.CreateTable<Contact>();
            db.CreateTable<Case>();

            Contact.CreateIndex(db);
        }

        class SQLiteMatch // used in `ComputeMatches()`.
        {
            public string Type { get; set; } // 'symptomatic' or 'positive'
            public int Year { get; set; }
            public int Month { get; set; }
            public int Day { get; set; }
            public int Hour { get; set; }
        }

        /** Recomputes the match sets based on the current state of the contact
         * database. */
        protected void ComputeMatches()
        {   
            var sqlMatches = db.Query<SQLiteMatch>(
                "select ca.type, co.year, co.month, co.day, co.hour " +
                "from contacts as co " +
                "inner join cases as ca " +
                "   on ca.key = co.key " +
                "   and ca.year = co.year " +
                "   and ca.month = co.month " +
                "   and ca.day = co.day " +
                "group by ca.type, co.year, co.month, co.day, co.hour;"
            );

            lock (Matches) {
                Matches.Positives.Clear();
                Matches.Symptomatics.Clear();

                foreach (var sqlMatch in sqlMatches) {
                    var time = new DateHour(sqlMatch.Year, sqlMatch.Month,
                        sqlMatch.Day, sqlMatch.Hour);

                    if (sqlMatch.Type == "positive") {
                        Matches.Positives.Add(time);
                    } else {
                        Matches.Symptomatics.Add(time);
                    }
                }
            }

            MatchesChange?.Invoke(this, Matches);
            CurrentInfectionStatusChange?.Invoke(this, CurrentInfectionStatus);
        }
    }
}
