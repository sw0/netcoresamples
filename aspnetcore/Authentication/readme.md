[TOC]

# Understanding of Authentication and Authorization
## Initial Create
Create ASP.NET Core Web Application with Empty template

## Authentication and Authorization
Startup.ConfigureServices:
    services.AddAuthentication("cookie-auth")
           .AddCookie("cookie-auth", cfg => {
                cfg.Cookie.Name = "auth-cookie";
                cfg.LoginPath = "/Home/Login";
           });

Startup.Configure:
   app.UseAuthentication();
   app.UseAuthorization();

Controller:
    Claim, ClaimsIdentity, ClaimsPrincipal
    ClaimTypes.Name, Role, MobileNumber, Email, etc.

## Authentication and Authorization, Add Registration, add Database
ConfigureServices:
```
    services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(Configure.GetConnectionString("Default")));

    services.AddIdentity<IdentityUser, IdentityRole>(cfg => {})
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

```

Controllers:
```
    UserManager<IdentityUser>, SignInManager<IdentityUser>
```

Database: 
```
select * from [dbo].[AspNetRoleClaims]
select * from [dbo].[AspNetRoles]
select * from [dbo].[AspNetUserClaims]
select * from [dbo].[AspNetUserLogins]
select * from [dbo].[AspNetUserRoles]
select * from [dbo].[AspNetUsers]
select * from [dbo].[AspNetUserTokens]
```

# OAuth with JWT
## References
### package
* Microsoft.AspNetCore.Authentication.JwtBearer

### classes
* [System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames](http://tools.ietf.org/html/rfc7519#section-4) or [ID Token](https://openid.net/specs/openid-connect-core-1_0.html#IDToken)
* [System.IdentityModel.Tokens.Jwt.JwtSecurityToken](https://docs.microsoft.com/en-us/previous-versions/visualstudio/dn464189%28v%3dvs.114%29)
* [System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler](https://docs.microsoft.com/en-us/dotnet/api/system.identitymodel.tokens.jwt.jwtsecuritytokenhandler?view=azure-dotnet)
* [Microsoft.IdentityModel.Tokens.SigningCredentials](https://docs.microsoft.com/en-us/dotnet/api/system.identitymodel.tokens.signingcredentials?view=netframework-4.8)
* [SecurityKey](https://docs.microsoft.com/en-us/dotnet/api/system.identitymodel.tokens.securitykey?view=netframework-4.8)

## Code sample
Startup.ConfigureServices:
```
            services.AddAuthentication("OAuth")
                .AddJwtBearer("OAuth", options =>
                {
#if OAUTH_ALLOW_URI_TOKEN
                    //Add Event here to accept token in Query
                    options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents()
                    {
                        OnMessageReceived = context => {
                            if (context.Request.Query.ContainsKey("access_token"))
                            {
                                var token = context.Request.Query["access_token"];

                                context.Token = token;
                            }
                            return Task.CompletedTask;
                        }
                    };
#endif

                    //add TokenValidationParameters to use our customed security key
                    var secret = Encoding.UTF8.GetBytes(AppConsts.Secret);
                    var key = new SymmetricSecurityKey(secret);
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidIssuer = AppConsts.Issuer,
                        ValidAudience = AppConsts.Audience,
                        IssuerSigningKey = key,
                    };
                });
```
