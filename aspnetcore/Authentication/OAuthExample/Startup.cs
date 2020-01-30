#define OAUTH_ALLOW_URI_TOKEN
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace OAuthExample
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
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
