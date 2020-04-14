using System;
using System.Collections.Generic;
using CovidTracer.Models;

namespace CovidTracer.Services
{
    /** Cross-platform interface for a BLE server service. */
    public interface IBLEServer
    {
        /** Creates a new BLE service that advertize the provided read-only
         * characteristics.
         *
         * Characteristic values are obtained by calling the associated
         * characteristic's function.
         */
        void AddReadOnlyService(
            Guid serviceName, Dictionary<Guid, Func<byte[]>> characteristics);
    }
}
