
using eCommerce.DataAccess;
using eCommerce.DataAccess.Data.Initializer;
using eCommerce.DataAccess.Data.Repository;
using eCommerce.DataAccess.Data.Repository.IRepository;
using eCommerce.Models;
using eCommerce.Utility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCommerce
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
            services.AddDbContext<ApplicationDbContext>(options
                => options
              .EnableSensitiveDataLogging(true)
              .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                .UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            
            services.AddDatabaseDeveloperPageExceptionFilter();
            services.AddIdentity<IdentityUser,IdentityRole>()
               
              .AddEntityFrameworkStores<ApplicationDbContext>()
             .AddDefaultUI()
              .AddDefaultTokenProviders();

            services.AddTransient<IEmailSender, EmailSender>();
         
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IDBInitializer, DBInitializer>();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(15);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            services.Configure<StripeSettings>(Configuration.GetSection("Stripe"));
          
            services.AddRazorPages();
            services.AddControllersWithViews().AddRazorRuntimeCompilation().AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);
            services.AddAuthentication().AddFacebook(facebookOptions => {
                facebookOptions.AppId = "601554724153789";
                facebookOptions.AppSecret = "60e891cc5728443dc684fd1f755bf6b7";
             
            });
            services.AddAuthentication().AddMicrosoftAccount(microsoftOPtions =>
            {
                microsoftOPtions.ClientId = "e5a4331a-dba7-474e-8081-be93d76507cb";
                microsoftOPtions.ClientSecret = "f0goW7v6I4~sJv.WNHx-eA~g25y6LYGWg2";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IDBInitializer dBInitializer)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSession();
            app.UseRouting();
            dBInitializer.Initialize();
            app.UseAuthentication();
            app.UseAuthorization();

         
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
            });
            //imamo mvc i razor pages 
            //  app.UseMvc();
            StripeConfiguration.ApiKey = Configuration.GetSection("Stripe")["SecretKey"];
        }
    }
}
