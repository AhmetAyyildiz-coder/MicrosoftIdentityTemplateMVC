using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MicrosoftIdentityTemplate.CustomValidations;
using MicrosoftIdentityTemplate.Models;
using Microsoft.AspNetCore.Http;

namespace MicrosoftIdentityTemplate
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();


            //IdentityDbContext builder
            services.AddDbContext<CustomIdentityDbContext>(opt =>
                {
                    opt.UseSqlServer(Configuration.GetConnectionString("SqlConnection"));
                });

            //Identity Builder with dbcontext 
            services.AddIdentity<CustomIdentityUser, CustomIdentityRole>()
                /*.AddUserValidator<CustomUserValidation>()*/ //class bazýnda user validations
                .AddPasswordValidator<CustomIdentityPasswordValidations>() //class bazýnda password validations
                .AddErrorDescriber<CustomIdentityErrorDescriptor>() //class bazýnda hatalarýn türkçeleþtirilmesi
                .AddEntityFrameworkStores<CustomIdentityDbContext>();




            //Cookie bazý kimlik doðrulama için cookie ayarlarý 
            CookieBuilder cookieBuilder = new CookieBuilder();
            cookieBuilder.Name = "MyIdentityCookie";
            cookieBuilder.HttpOnly = false;
            cookieBuilder.SameSite = SameSiteMode.Lax;
            cookieBuilder.SecurePolicy = CookieSecurePolicy.SameAsRequest;

            services.ConfigureApplicationCookie(opt =>
            {
                opt.LoginPath = new PathString("/Home/Login");
                opt.Cookie = cookieBuilder;
                
                opt.SlidingExpiration = true; //cookie ömrünü uzatmak için alttaki deðere ek süre 
                                              //eklemek için bu property true yapýlmalýdýr.

                opt.ExpireTimeSpan = TimeSpan.FromMinutes(40);
            });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseDeveloperExceptionPage();
            app.UseStatusCodePages();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
