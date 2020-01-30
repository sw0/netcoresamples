using AuthExample.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
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
        //UserManager<WebUser> _userManager;
        readonly AspNetUserManager<WebUser> _userManager;
        readonly SignInManager<WebUser> _signInManager;
        readonly RoleManager<IdentityRole> _roleManager;

        public HomeController(
            AspNetUserManager<WebUser> userManager,
            //UserManager<WebUser> userManager,
            SignInManager<WebUser> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Secret()
        {
            DecryptCookie();

            ViewBag.UserName = User.Identity.Name;
            ViewBag.UserRole = User.FindFirst(ClaimTypes.Role)?.Value;


            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            var claims = await _userManager.GetClaimsAsync(user);

            return View(claims);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddClaim(string type, string value)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            var claims = await _userManager.AddClaimAsync(user, new Claim(type, value));

            return RedirectToAction("Secret");
        }

        [HttpGet]
        [Authorize(Policy = "Claims.DOB")]
        public IActionResult SecretWithPolicy()
        {
            ViewBag.UserName = User.Identity.Name;
            ViewBag.UserRole = User.FindFirst(ClaimTypes.Role)?.Value;

            return View();
        }

        [HttpGet]
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

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl ?? "/Home/Index";

            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password, string returnUrl, string role = "user")
        {
            returnUrl = returnUrl ?? "/Home/Index";

            //var user = await _userManager.FindByNameAsync(username);

            var result = await _signInManager.PasswordSignInAsync(username, password, false, false);

            if (result.Succeeded)
            {
                return Redirect(returnUrl);
            }

#region claims, claimsIdentity, ClaimsPrincipal
            //var jdClaims = new List<Claim> {
            //    new Claim(ClaimTypes.Name, username),
            //    new Claim(ClaimTypes.Role, role ?? "user"),
            //    new Claim(ClaimTypes.DateOfBirth, "1980-01-01"),
            //    new Claim("JD-LOGO", "https://jd.com/logo.png"),
            //};

            //var jdId = new ClaimsIdentity(jdClaims, "JD Identity");

            //var wxClaims = new List<Claim> {
            //    new Claim(ClaimTypes.Name, "wx-" + username),
            //    new Claim(ClaimTypes.MobilePhone, "18600010001"),
            //    new Claim("QRCODE", "https://wx.com/sdfs/sdf"),
            //};

            //var wxId = new ClaimsIdentity(wxClaims, "WeiXin Identity");

            //var user = new ClaimsPrincipal(new[] { jdId, wxId });

            //await HttpContext.SignInAsync(user);

            //return Redirect(returnUrl);
#endregion


            ViewBag.ReturnUrl = returnUrl ?? "/Home/Index";
            ViewData["ErrorMessage"] = result.ToString();

            return View();

        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Register()
        {
            await Task.CompletedTask;

            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register(string username, string role,
            string password, string wechat, string unionId)
        {
            var user = new WebUser()
            {
                UserName = username,
                Wechat = wechat,
                UnionId = unionId,
            };

            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(role))
                {
                    var foundRole = await _roleManager.FindByNameAsync(role);
                    if (foundRole == null)
                    {
                        await _roleManager.CreateAsync(new IdentityRole() { Name = role });
                    }
                    await _userManager.AddToRoleAsync(user, role);
                }

                if (username == "slinwithdob")
                {
                    await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.DateOfBirth, "1986-01-01"));
                }

                await _signInManager.SignInAsync(user, false);

                return RedirectToAction("Index");
            }
            else
            {
                var error = result.Errors?.FirstOrDefault();

                if (error != null)
                {
                    ViewBag.ErrorMessage = $"{error.Code} -  {error.Description}";
                }
                else
                {
                    ViewBag.ErrorMessage = result.ToString();
                }
            }

            return View();
        }


        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            //await HttpContext.SignOutAsync();
            await _signInManager.SignOutAsync();

            return RedirectToAction("Index");
        }

        private void DecryptCookie()
        {

            //Get the encrypted cookie value
            string cookieName = "auth-example";
            string cookieSchema = IdentityConstants.ApplicationScheme;  //"Identity.Application"; 

            var httpContext = HttpContext;
            var opt = httpContext.RequestServices.GetRequiredService<IOptionsMonitor<CookieAuthenticationOptions>>();
            var cookie = opt.CurrentValue.CookieManager.GetRequestCookie(httpContext, cookieName);

            // Decrypt if found
            if (!string.IsNullOrEmpty(cookie))
            {
                var dataProtector = opt.CurrentValue.DataProtectionProvider.CreateProtector("Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationMiddleware", cookieSchema, "v2");

                var ticketDataFormat = new TicketDataFormat(dataProtector);
                var ticket = ticketDataFormat.Unprotect(cookie);

                var clains = ticket?.Principal?.Claims;
            }
        }
    }
}
