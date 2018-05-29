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
    public class APISession
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("ip_address")]
        public string IPAddress { get; set; }

        [JsonProperty("comment")]
        public string Comment { get; set; }

        [JsonProperty("livetime")]
        public double Livetime { get; set; }

        [JsonProperty("detector_data")]
        public string DetectorData { get; set; }
    }
}