using System;

namespace CovidTracer.Models
{
    public enum InfectionStatus
    {
        /** We didn't find any close contact. */
        Safe,

        /** We found contact with symptomatic cases. */
        Symptomatic,

        /** We found contact with Covid-19 positive cases. */
        Positive
    }
}
