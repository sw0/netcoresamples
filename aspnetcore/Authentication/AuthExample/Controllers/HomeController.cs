using AuthExample.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        UserManager<WebUser> _userManager;
        SignInManager<WebUser> _signInManager;

        public HomeController(UserManager<WebUser> userManager,
            SignInManager<WebUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

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

        //[Authorize(Roles = "admin, sales")]
        //public IActionResult SecretWithRole()
        //{
        //    ViewBag.UserName = User.Identity.Name;
        //    ViewBag.UserRole = User.FindFirst(ClaimTypes.Role)?.Value;

        //    return View();
        //}

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
            ViewData["ErrorMessage"] =  result.ToString();

            return View();

        }

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            await Task.CompletedTask;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string username, string password, string wechat, string unionId)
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
                await _signInManager.SignInAsync(user, false);

                return RedirectToAction("Index");
            }
            else
            {
                var error = result.Errors?.FirstOrDefault();

                if(error!=null)
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
    }
}
