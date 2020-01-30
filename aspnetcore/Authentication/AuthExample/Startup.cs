#define USE_MEM_DB
#define AUTH_SAMPLE_4  //1, 2, 3, 4, 5
using AuthExample.AuthorizationRequirements;
using AuthExample.Data;
using AuthExample.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Security.Claims;


namespace AuthExample
{
    public class Startup
    {
        IConfiguration Configuration { get; set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(config =>
            {
#if USE_MEM_DB
                config.UseInMemoryDatabase("memory");
#else
                var connectionString = Configuration.GetConnectionString("AuthExampleConnection");

                config.UseSqlServer(connectionString);
#endif
            });

            services.AddIdentity<WebUser, IdentityRole>(cfg =>
            {
                cfg.Password.RequireDigit = false;
                cfg.Password.RequireLowercase = false;
                cfg.Password.RequireNonAlphanumeric = false;
                cfg.Password.RequireUppercase = false;
                cfg.Password.RequiredLength = 6;
            })
                //you no need this AspNetUserManager<WebUser> if you don't want to operate use's Role,Claims
                //and use UserManager<WebUser> instead directly in Controller, it's registred already
                //in IServiceCollection.AddIdentity<Tuser,TRole>
                .AddUserManager<AspNetUserManager<WebUser>>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(cfg =>
            {
                cfg.Cookie.Name = "auth-example";
                cfg.LoginPath = "/Home/Login";
                cfg.AccessDeniedPath = "/Home/AccessDenied";
                cfg.LogoutPath = "/Home/Logout";
            });

            services.AddAuthentication("AuthExample")
                .AddCookie("AuthExample", opt =>
                {
                    opt.Cookie.Name = "auth-example";
                    opt.LoginPath = "/Home/Login";
                    opt.AccessDeniedPath = "/Home/AccessDenied";
                    opt.LogoutPath = "/Home/Logout";
                });

#region optional for advanced authorization configuration
            services.AddAuthorization(cfg =>
            {
                #region auth sample 0
                //sample 0,  [Authorize(Policy = "Claims.DOB")]
                cfg.AddPolicy("Claims.DOB", policyBuilder =>
                {
                    policyBuilder.RequireClaim(ClaimTypes.DateOfBirth);
                }); 
                #endregion
#if  AUTH_SAMPLE_1
                #region auth sample 1
                //sample 1
                cfg.AddPolicy("Claims.DOB", policyBuilder =>
                {
                    policyBuilder.AddRequirements(new CustomRequireClaim(ClaimTypes.DateOfBirth));
                }); 
                #endregion

#elif AUTH_SAMPLE_2
                #region auth sample 2
                //sample 2, [Authorize(Policy = "Claims.DOB")]
                var defaultAuthBuilder = new AuthorizationPolicyBuilder();
                var defaultAuthPolicy = defaultAuthBuilder.RequireAuthenticatedUser()
                .RequireClaim(ClaimTypes.DateOfBirth)
                .Build();

                cfg.DefaultPolicy = defaultAuthPolicy;
                #endregion
#endif
            });

#if AUTH_SAMPLE_1
            services.AddScoped<IAuthorizationHandler, CustomRequireClaimHandler>();
#endif
#endregion

            services.AddControllersWithViews(cfg =>
            {
#if AUTH_SAMPLE_3  //global
                //way 1
                cfg.Filters.Add(new AuthorizeFilter());
#elif AUTH_SAMPLE_4
                //way 2
                var defaultAuthBuilder = new AuthorizationPolicyBuilder();

                var defaultPolicy = defaultAuthBuilder.RequireAuthenticatedUser()
                .Build();

                cfg.Filters.Add(new AuthorizeFilter(defaultPolicy));
#endif
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                //endpoints.MapGet("/", async context =>
                //{
                //    await context.Response.WriteAsync("Hello World!");
                //});
            });
        }
    }
}
