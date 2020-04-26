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
using System.Collections.Generic;
using System.Threading;

namespace CovidTracer.iOS
{
    public class IOSBLEServer : Services.IBLEServer
    {
        public void AddReadOnlyService(
            Guid serviceName, Dictionary<Guid, Func<byte[]>> characteristics)
        {
            Logger.Error("BLE server not implemented on iOS.");
        }
    }
}
