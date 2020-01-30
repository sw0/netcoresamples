using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace OAuthExample.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class TokenController : ControllerBase
    {
        [HttpGet]
        public IActionResult Generate()
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, "some_identity_id"),
                new Claim("teacher says", "he is good boy")
            };

            var secret = Encoding.UTF8.GetBytes(AppConsts.Secret);
            var key = new SymmetricSecurityKey(secret);

            var algorithm = SecurityAlgorithms.HmacSha256;

            var signingCredentials = new SigningCredentials(key, algorithm);

            var jwt = new JwtSecurityToken(AppConsts.Issuer, AppConsts.Audience, claims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddHours(12),
                signingCredentials);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(jwt);

            return Ok(new { access_token = tokenString });
        }

        [HttpPost]
        public IActionResult Unwind(string token)
        {
            var tokenx = new JwtSecurityTokenHandler().ReadJwtToken(token);

            //NO Signature validation here yet.

            return Ok(tokenx.ToString());
        }
    }
}
