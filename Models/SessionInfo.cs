using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace gamma_wsapi.Models
{
    public class APISessionInfo
    {
        [JsonConstructor]
        public APISessionInfo()
        {
        }

        public APISessionInfo(SqlDataReader reader)
        {
            Name = reader["name"].ToString();
            Comment = reader["comment"].ToString();
            Livetime = Convert.ToDouble(reader["livetime"]);
            SpectrumCount = Convert.ToInt32(reader["speccnt"]);
        }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("comment")]
        public string Comment { get; set; }

        [JsonProperty("livetime")]
        public double Livetime { get; set; }

        [JsonProperty("spectrum_count")]
        public int SpectrumCount { get; set; }
    }
}