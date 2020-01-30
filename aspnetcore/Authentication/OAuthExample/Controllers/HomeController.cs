using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAuthExample.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult Secret()
        {
            return View();
        }

        /// <summary>
        /// decode the part of JWT
        /// </summary>
        /// <param name="part">first or second part of JWT</param>
        /// <returns></returns>
        public IActionResult Decode(string part)
        {
            var bytes = Convert.FromBase64String(part);

            return Ok(Encoding.UTF8.GetString(bytes));
        }
    }
}
