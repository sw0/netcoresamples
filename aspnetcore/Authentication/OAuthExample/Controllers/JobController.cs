using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAuthExample.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class JobController : ControllerBase
    {
        static List<string> Jobs = new List<string> {
               "Cook",
               "Driver"
            };

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(Jobs);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Post(string job)
        {
            Jobs.Add(job);

            return Created($"api/job/{job}", job);
        }
    }
}
