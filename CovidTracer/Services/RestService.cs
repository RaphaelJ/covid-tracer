using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using CovidTracer.Models;

namespace CovidTracer.Services
{
    public static class RestService
    {
        public static readonly string Root =
            "https://covid-tracer-backend.herokuapp.com";

        public static readonly HttpClient Client = new HttpClient();

        public static async Task<HttpResponseMessage> Notify(
            DateTime symptomsOnset, bool isTested, string comment)
        {
            var content = new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>(
                    "symptoms_onset", symptomsOnset.ToString("yyyy-MM-dd")
                ),
                new KeyValuePair<string, string>(
                    "is_tested", isTested ? "yes" : "no"
                ),
                new KeyValuePair<string, string>("comment", comment),
            });

            var id = CovidTracerID.GetCurrentInstance();
            return await Client.PostAsync($"{Root}/notify/{id.Value}", content);
        }
    }
}
