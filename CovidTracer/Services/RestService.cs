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
using System.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

using CovidTracer.Models;
using CovidTracer.Models.Keys;
using CovidTracer.Models.Time;

namespace CovidTracer.Services
{
    /** Provides procedures to interact with the backend services. */
    public static class RestService
    {
        public const string ROOT =
            "https://covid-tracer-backend.herokuapp.com";

        public static readonly HttpClient Client = new HttpClient();

        /** Requests the current cases from the backend. */
        public static async Task<IList<Case>> Cases()
        {
            var resp = await Client.GetStringAsync($"{ROOT}/cases.json");

            var json = JsonValue.Parse(resp);

            var cases = new List<Case>();
            for (int i = 0; i < json["cases"].Count; ++i) {
                var jsonCase = json["cases"][i];

                var date = Date.ParseISO(jsonCase["date"]);
                var key = new DailyTracerKey(jsonCase["key"]);

                var type_ =
                      jsonCase["type"] == "positive"
                    ? CaseType.Positive
                    : CaseType.Symptomatic;

                cases.Add(new Case(key, type_, date));
            }

            return cases;
        }

        /** Notifies the backend service of a potential Covid-19 infection. */
        public static async Task<HttpResponseMessage> Notify(
            DateTime symptomsOnset, bool isTested, string comment)
        {
            // Generates the daily tracer keys that have been or will be used
            // during the infectious period, based on the symptoms onset.
            //
            // Use median incubation and virus positive periods according to
            // <https://www.atsjournals.org/doi/pdf/10.1164/rccm.202003-0524LE>

            var keys = new List<Tuple<Date, DailyTracerKey>>();

            {
                const int INCUBATION_PERIOD = 5;
                const int SYMPTOMS_TO_VIRUS_NEGATIVE = 11;

                var begin = symptomsOnset.AddDays(-INCUBATION_PERIOD);
                var end = symptomsOnset.AddDays(SYMPTOMS_TO_VIRUS_NEGATIVE);

                var masterKey = TracerKey.CurrentAppInstance();

                for (var dt = begin; dt < end; dt = dt.AddDays(1)) {
                    var date = new Date(dt);
                    var key = masterKey.DerivateDailyKey(date);

                    keys.Add(Tuple.Create(date, key));
                }
            }

            // Constructs and submit the POST request.
            //
            // Keys are encoded as a repeated HTTP field.

            var queryParams = new List<KeyValuePair<String, String>> {
                new KeyValuePair<string, string>(
                    "is_tested", isTested ? "true" : "false"
                ),
                new KeyValuePair<string, string>("comment", comment),
            };

            for (int i = 0; i < keys.Count; ++i) {
                queryParams.Add(new KeyValuePair<string, string>(
                    $"keys-{i}-date", keys[i].Item1.ToString()
                ));
                queryParams.Add(new KeyValuePair<string, string>(
                    $"keys-{i}-value", keys[i].Item2.ToString()
                ));
            }

            var content = new FormUrlEncodedContent(queryParams.ToArray());
            return await Client.PostAsync($"{ROOT}/notify", content);
        }
    }
}
