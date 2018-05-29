/*	
	Gamma WSAPI - Web service for gamma spectrum reachback
    Copyright (C) 2018  Norwegian Radiation Protection Authority

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
// Authors: Dag robole,

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace gamma_wsapi.Models
{
    public class APISpectrum
    {
        [JsonProperty("session_name")]
        public string SessionName { get; set; }

        [JsonProperty("session_index")]
        public int SessionIndex { get; set; }

        [JsonProperty("start_time")]
        public string StartTime { get; set; }

        [JsonProperty("latitude")]
        public double Latitude { get; set; }

        [JsonProperty("longitude")]
        public double Longitude { get; set; }

        [JsonProperty("altitude")]
        public double Altitude { get; set; }

        [JsonProperty("track")]
        public double Track { get; set; }

        [JsonProperty("speed")]
        public double Speed { get; set; }

        [JsonProperty("climb")]
        public double Climb { get; set; }

        [JsonProperty("livetime")]
        public double Livetime { get; set; }

        [JsonProperty("realtime")]
        public double Realtime { get; set; }

        [JsonProperty("num_channels")]
        public int NumChannels { get; set; }

        [JsonProperty("channels")]
        public string Channels { get; set; }

        [JsonProperty("doserate")]
        public double Doserate { get; set; }
    }
}