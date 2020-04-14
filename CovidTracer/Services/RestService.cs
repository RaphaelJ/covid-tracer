using System;
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
                var end = symptomsOnset.AddDays(-SYMPTOMS_TO_VIRUS_NEGATIVE);

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
                    "is_tested", isTested ? "yes" : "no"
                ),
                new KeyValuePair<string, string>("comment", comment),
            };

            foreach (var key in keys) {
                queryParams.Add(new KeyValuePair<string, string>(
                    "keys[].date", key.Item1.ToString()
                ));
                queryParams.Add(new KeyValuePair<string, string>(
                    "keys[].value", key.Item2.ToString()
                ));
            }

            var content = new FormUrlEncodedContent(queryParams.ToArray());
            return await Client.PostAsync($"{ROOT}/notify", content);
        }
    }
}
