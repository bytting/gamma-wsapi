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
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http.Filters;
using System.Web.Security;

namespace gamma_wsapi
{
    public class BasicAuthenticationAttribute : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(System.Web.Http.Controllers.HttpActionContext actionContext)
        {            
            if (actionContext.Request.Headers.Authorization == null)
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
            }
            else
            {
                try
                {
                    string authString = actionContext.Request.Headers.Authorization.Parameter;
                    string origString = Encoding.UTF8.GetString(Convert.FromBase64String(authString));

                    string[] items = origString.Split(new char[] { ':' });
                    if (items.Length != 2)
                    {
                        actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                    }
                    else
                    {
                        string username = items[0];
                        string password = items[1];

                        string ConnectionString = ConfigurationManager.ConnectionStrings["GammaStoreConnectionString"].ConnectionString;
                        using (SqlConnection conn = new SqlConnection(ConnectionString))
                        {
                            conn.Open();
                            SqlCommand cmd = new SqlCommand("select password from users where username like @Username", conn);
                            cmd.Parameters.AddWithValue("@Username", username);
                            object o = cmd.ExecuteScalar();
                            if (o == null || o == DBNull.Value)
                            {
                                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                            }
                            else
                            {
                                string dbpass = o.ToString();
                                if (dbpass != password)
                                    actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                            }
                        }
                    }
                }
                catch
                {
                    actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.InternalServerError);
                }
            }

            base.OnAuthorization(actionContext);
        }
    }
}