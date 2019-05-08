using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataController : ControllerBase
    {
        // POST api/data
        [HttpPost]
        public ActionResult<IEnumerable<string>> Data([FromBody] string data)
        {

            return new string[] { "value1", "value2" };
        }
    }
}
