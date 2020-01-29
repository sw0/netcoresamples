using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthExample.Data;
using AuthExample.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AuthExample
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(config =>
            {
                config.UseInMemoryDatabase("websystem");
            });

            services.AddIdentity<WebUser, IdentityRole>(cfg =>
            {
                cfg.Password.RequireDigit = false;
                cfg.Password.RequireLowercase = false;
                cfg.Password.RequireNonAlphanumeric = false;
                cfg.Password.RequireUppercase = false;
                cfg.Password.RequiredLength = 6;
            })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(cfg => {
                cfg.Cookie.Name = "auth-example";
                cfg.LoginPath = "/Home/Login";
                cfg.AccessDeniedPath = "/Home/AccessDenied";
                cfg.LogoutPath = "/Home/Logout";
            });

            //services.AddAuthentication("AuthExample")
            //    .AddCookie("AuthExample", opt =>
            //    {
            //        opt.Cookie.Name = "auth-example";
            //        opt.LoginPath = "/Home/Login";
            //        opt.AccessDeniedPath = "/Home/AccessDenied";
            //        opt.LogoutPath = "/Home/Logout";
            //    });

            services.AddControllersWithViews();

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
