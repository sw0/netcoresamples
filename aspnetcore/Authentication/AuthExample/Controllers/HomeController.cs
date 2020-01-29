using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AuthExample.Controllers
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
            ViewBag.UserName = User.Identity.Name;
            ViewBag.UserRole = User.FindFirst(ClaimTypes.Role)?.Value;

            return View();
        }

        [Authorize(Roles = "admin, sales")]
        public IActionResult SecretWithRole()
        {
            ViewBag.UserName = User.Identity.Name;
            ViewBag.UserRole = User.FindFirst(ClaimTypes.Role)?.Value;

            return View();
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            ViewBag.UserName = User.Identity.Name;
            ViewBag.UserRole = User.FindFirst(ClaimTypes.Role)?.Value;
            return View();
        }

        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl ?? "/Home/Index";

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string role, string password, string returnUrl)
        {
            returnUrl = returnUrl ?? "/Home/Index";

            var jdClaims = new List<Claim> { 
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role),
                new Claim(ClaimTypes.DateOfBirth, "1980-01-01"),
                new Claim("JD-LOGO", "https://jd.com/logo.png"),
            };

            var jdId = new ClaimsIdentity(jdClaims, "JD Identity");

            var wxClaims = new List<Claim> {
                new Claim(ClaimTypes.Name, "wx-" + username),
                new Claim(ClaimTypes.MobilePhone, "18600010001"),
                new Claim("QRCODE", "https://wx.com/sdfs/sdf"),
            };

            var wxId = new ClaimsIdentity(wxClaims, "WeiXin Identity");

            var user = new ClaimsPrincipal(new[] { jdId, wxId});

            await HttpContext.SignInAsync(user);

            //return View();

            return Redirect(returnUrl);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();

            return RedirectToAction("Index");
        }
    }
}
