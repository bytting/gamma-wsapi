using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace gamma_wsapi.Controllers
{
    public class SystemController : ApiController
    {
        [Route("system/ping")]
        [HttpGet]
        public IHttpActionResult Ping()
        {
            return Ok();
        }
    }
}
