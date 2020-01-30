[TOC]

# Initial Create
Create ASP.NET Core Web Application with Empty template

# Authentication and Authorization
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

# Authentication and Authorization, Add Registration, add Database
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