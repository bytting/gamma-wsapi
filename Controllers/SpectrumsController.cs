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
using System.Net;
using System.Net.Http;
using System.Web.Http;
using gamma_wsapi.Models;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;

namespace gamma_wsapi.Controllers
{    
    public class SpectrumsController : ApiController
    {
        [HttpGet]
        public IEnumerable<APISpectrum> GetSpectrums()
        {
            // /spectrums

            List<APISpectrum> specList = new List<APISpectrum>();

            try
            {
                string ConnectionString = ConfigurationManager.ConnectionStrings["GammaStoreConnectionString"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("select * from spectrum order by session_name, session_index", conn);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())                        
                            specList.Add(new APISpectrum(reader));
                    }
                }
            }
            catch
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }

            return specList;
        }

        [HttpGet]
        public IEnumerable<APISpectrum> GetSpectrums(string from, string to)
        {
            // /spectrums?from=19990101_120101&to=20020101_120101

            DateTime dtFrom, dtTo;

            try
            {
                dtFrom = DateTime.ParseExact(from, "yyyyMMdd_HHmmss", CultureInfo.InvariantCulture);
                dtTo = DateTime.ParseExact(to, "yyyyMMdd_HHmmss", CultureInfo.InvariantCulture);
            }
            catch
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            List<APISpectrum> specList = new List<APISpectrum>();

            try
            {
                string ConnectionString = ConfigurationManager.ConnectionStrings["GammaStoreConnectionString"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("select * from spectrum where start_time between @From and @To order by session_name, session_index", conn);
                    cmd.Parameters.Add("@From", SqlDbType.DateTime).Value = dtFrom;
                    cmd.Parameters.Add("@To", SqlDbType.DateTime).Value = dtTo;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            specList.Add(new APISpectrum(reader));
                    }
                }
            }
            catch
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }

            return specList;
        }

        [HttpGet]
        public IEnumerable<APISpectrum> GetSpectrums(string name)
        {
            // /spectrums/01012000_120101

            List<APISpectrum> specList = new List<APISpectrum>();

            try
            {
                string ConnectionString = ConfigurationManager.ConnectionStrings["GammaStoreConnectionString"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("select * from spectrum where session_name like @SessionName order by session_index", conn);
                    cmd.Parameters.AddWithValue("@SessionName", name);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())                        
                            specList.Add(new APISpectrum(reader));
                    }
                }
            }
            catch
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }

            return specList;
        }

        [HttpGet]
        public IEnumerable<APISpectrum> GetSpectrums(string name, int minIdx)
        {
            // /spectrums/01012000_120101?minIdx=12

            List<APISpectrum> specList = new List<APISpectrum>();

            try
            {
                string ConnectionString = ConfigurationManager.ConnectionStrings["GammaStoreConnectionString"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("select * from spectrum where session_name like @SessionName and session_index >= @MinIndex order by session_index", conn);
                    cmd.Parameters.AddWithValue("@SessionName", name);
                    cmd.Parameters.AddWithValue("@MinIndex", minIdx);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())                        
                            specList.Add(new APISpectrum(reader));
                    }
                }
            }
            catch
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }

            return specList;
        }

        [HttpGet]
        public IEnumerable<APISpectrum> GetSpectrums(string name, int minIdx, int maxCnt)
        {
            // /spectrums/01012000_120101?minIdx=12&maxCnt=100

            if(maxCnt < 0)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            List<APISpectrum> specList = new List<APISpectrum>();

            try
            {
                string ConnectionString = ConfigurationManager.ConnectionStrings["GammaStoreConnectionString"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("select top(" + maxCnt.ToString() + ") * from spectrum where session_name like @SessionName and session_index >= @MinIndex order by session_index", conn);
                    cmd.Parameters.AddWithValue("@SessionName", name);
                    cmd.Parameters.AddWithValue("@MinIndex", minIdx);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            specList.Add(new APISpectrum(reader));
                    }
                }
            }
            catch
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }

            return specList;
        }

        [HttpGet]
        public IHttpActionResult GetSpectrum(string name, int id)
        {
            // /spectrums/01012000_120101/1

            APISpectrum spectrum = null;

            try
            {
                string ConnectionString = ConfigurationManager.ConnectionStrings["GammaStoreConnectionString"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("select * from spectrum where session_name like @SessionName and session_index = @SessionIndex", conn);
                    cmd.Parameters.AddWithValue("@SessionName", name);
                    cmd.Parameters.AddWithValue("@SessionIndex", id);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (!reader.HasRows)
                            return NotFound();

                        reader.Read();
                        spectrum = new APISpectrum(reader);
                    }
                }
            }
            catch
            {
                return StatusCode(HttpStatusCode.InternalServerError);
            }
            
            return Ok(spectrum);
        }

        [HttpPost]
        public IHttpActionResult PostSpectrum([FromBody]APISpectrum spectrum)
        {
            // /spectrums

            try
            {
                string ConnectionString = ConfigurationManager.ConnectionStrings["GammaStoreConnectionString"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("select count(*) from spectrum where session_name like @SessionName and session_index = @SessionIndex", conn);
                    cmd.Parameters.AddWithValue("@SessionName", spectrum.SessionName);
                    cmd.Parameters.AddWithValue("@SessionIndex", spectrum.SessionIndex);
                    object o = cmd.ExecuteScalar();
                    if(o == null || o == DBNull.Value)                    
                        return StatusCode(HttpStatusCode.InternalServerError);

                    int c = Convert.ToInt32(o);
                    if (c == 0)
                    {
                        cmd.CommandText = "insert into spectrum values (@SessionName, @SessionIndex, @StartTime, @Latitude, @Longitude, @Altitude, @Track, @Speed, @Climb, @Livetime, @Realtime, @NumChannels, @Channels, @Doserate)";
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@SessionName", spectrum.SessionName);
                        cmd.Parameters.AddWithValue("@SessionIndex", spectrum.SessionIndex);
                        DateTime dt = DateTime.Parse(spectrum.StartTime, null, DateTimeStyles.RoundtripKind);
                        cmd.Parameters.Add("@StartTime", SqlDbType.DateTime).Value = dt;
                        cmd.Parameters.AddWithValue("@Latitude", spectrum.Latitude);
                        cmd.Parameters.AddWithValue("@Longitude", spectrum.Longitude);
                        cmd.Parameters.AddWithValue("@Altitude", spectrum.Altitude);
                        cmd.Parameters.AddWithValue("@Track", spectrum.Track);
                        cmd.Parameters.AddWithValue("@Speed", spectrum.Speed);
                        cmd.Parameters.AddWithValue("@Climb", spectrum.Climb);
                        cmd.Parameters.AddWithValue("@Livetime", spectrum.Livetime);
                        cmd.Parameters.AddWithValue("@Realtime", spectrum.Realtime);
                        cmd.Parameters.AddWithValue("@NumChannels", spectrum.NumChannels);
                        cmd.Parameters.AddWithValue("@Channels", spectrum.Channels);
                        cmd.Parameters.AddWithValue("@Doserate", spectrum.Doserate);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch
            {
                return StatusCode(HttpStatusCode.InternalServerError);
            }

            return Ok();
        }
    }
}
