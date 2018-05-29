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
using System.Data.SqlClient;
using System.Configuration;

namespace gamma_wsapi.Controllers
{    
    public class SessionsController : ApiController
    {
        [Route("sessions/names")]
        [HttpGet]
        public IEnumerable<String> GetSessionNames()
        {
            // /sessions/names

            List<String> nameList = new List<String>();

            try
            {
                string ConnectionString = ConfigurationManager.ConnectionStrings["GammaStoreConnectionString"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("select distinct name from session order by name", conn);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            nameList.Add(reader["name"].ToString());
                        }
                    }
                }
            }
            catch
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }

            return nameList;
        }

        [HttpGet]
        public IEnumerable<APISession> GetSessions()
        {
            // /sessions

            List<APISession> sessionList = new List<APISession>();

            try
            {
                string ConnectionString = ConfigurationManager.ConnectionStrings["GammaStoreConnectionString"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("select * from session order by name", conn);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            APISession sess = new APISession();
                            sess.Name = reader["name"].ToString();
                            sess.IPAddress = reader["ip_address"].ToString();
                            sess.Comment = reader["comment"].ToString();
                            sess.Livetime = Convert.ToDouble(reader["livetime"]);
                            sess.DetectorData = reader["detector_data"].ToString();

                            sessionList.Add(sess);
                        }
                    }
                }
            }
            catch
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }

            return sessionList;
        }
        
        [HttpGet]
        public IHttpActionResult GetSession(string name)
        {
            // /sessions/01012000_120101

            APISession sess = new APISession();

            try
            {
                string ConnectionString = ConfigurationManager.ConnectionStrings["GammaStoreConnectionString"].ConnectionString;                

                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("select * from session where name like @Name", conn);
                    cmd.Parameters.AddWithValue("@Name", name);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (!reader.HasRows)
                            return NotFound();

                        reader.Read();

                        sess.Name = reader["name"].ToString();
                        sess.IPAddress = reader["ip_address"].ToString();
                        sess.Comment = reader["comment"].ToString();
                        sess.Livetime = Convert.ToDouble(reader["livetime"]);
                        sess.DetectorData = reader["detector_data"].ToString();
                    }                    
                }
            }
            catch (Exception ex)
            {
                return StatusCode(HttpStatusCode.InternalServerError);
            }

            return Ok(sess);
        }
        
        [HttpGet]
        public IHttpActionResult GetSpectrum(string name, int id)
        {
            // /sessions/01012000_120101/1

            APISpectrum spec = new APISpectrum();

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

                        spec.SessionName = reader["session_name"].ToString();
                        spec.SessionIndex = Convert.ToInt32(reader["session_index"]);
                        spec.StartTime = reader["start_time"].ToString();
                        spec.Latitude = Convert.ToDouble(reader["latitude"]);
                        spec.Longitude = Convert.ToDouble(reader["longitude"]);
                        spec.Altitude = Convert.ToDouble(reader["altitude"]);
                        spec.Track = Convert.ToDouble(reader["track"]);
                        spec.Speed = Convert.ToDouble(reader["speed"]);
                        spec.Climb = Convert.ToDouble(reader["climb"]);
                        spec.Livetime = Convert.ToDouble(reader["livetime"]);
                        spec.Realtime = Convert.ToDouble(reader["realtime"]);
                        spec.NumChannels = Convert.ToInt32(reader["num_channels"]);
                        spec.Channels = reader["channels"].ToString();
                        spec.Doserate = Convert.ToDouble(reader["doserate"]);
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(HttpStatusCode.InternalServerError);
            }

            return Ok(spec);
        }

        [HttpPost]
        public IHttpActionResult PostSession([FromBody]APISession session)
        {
            try
            {
                string ConnectionString = ConfigurationManager.ConnectionStrings["GammaStoreConnectionString"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("insert into session values (@Name, @IPAddress, @Comment, @Livetime, @DetectorData)", conn);
                    cmd.Parameters.AddWithValue("@Name", session.Name);
                    cmd.Parameters.AddWithValue("@IPAddress", session.IPAddress);
                    cmd.Parameters.AddWithValue("@Comment", session.Comment);
                    cmd.Parameters.AddWithValue("@Livetime", session.Livetime);
                    cmd.Parameters.AddWithValue("@DetectorData", session.DetectorData);
                    cmd.ExecuteNonQuery();
                }
            }    
            catch(Exception ex)
            {
                return StatusCode(HttpStatusCode.InternalServerError);
            }

            return Ok();
        }
    }
}
