using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Formatters;
using System.Security.Claims;
using System.Web.Http;
using System.Web.Http.Cors;
using Api.Models;
using Newtonsoft.Json.Serialization;
using Muuzy.Class;

namespace Api.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class TestController : ApiController
    {
        [HttpGet]
        [Route("ping")]
        public IHttpActionResult NotSecured()
        {
            return this.Ok("All good. You don't need to be authenticated to call this.");
        }

        [Authorize]
        [HttpGet]
        [Route("secured/ping")]

        public IHttpActionResult Test()
        {
            QpiroJSON resp = new QpiroJSON();
            (resp.Data as List<object>).Add(ClaimsPrincipal.Current.Identity.Name);
            return this.Json<QpiroJSON>(resp);
        }
    }


}
