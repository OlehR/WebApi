using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SharedLib;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataController : ControllerBase
    {
        // POST api/data
        [HttpPost]
        public ActionResult<string> Data([FromBody] string data)
        {
            bool res;
            using (var con = new OracleDbConnection())
            {
                res = con.isConnection();
            }
            return res ? "1" : "0";

        }
    }
}
