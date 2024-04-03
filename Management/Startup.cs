using Management.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Primitives;
using Services.Authorization;
using System;
using System.Collections.Generic;
using System.IO;
using Utility;

namespace Management
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
            services.AddSession();
            services.AddMvc();
            services.AddControllersWithViews();

            services.AddControllers();
            services.AddDistributedMemoryCache();
            services.AddHttpContextAccessor();
            #region Mail

            var MailSettingData = new List<DefaultMailSettingModel>();
            var MailModelOptions = new AppsettingModel.MailModel();
            Configuration.GetSection("Mail").Bind(MailModelOptions);
            if (MailModelOptions != null)
            {
                if (MailModelOptions.Default != null) MailSettingData.Add(MailModelOptions.Default);
                if (MailModelOptions.MailBox != null) MailSettingData.Add(MailModelOptions.MailBox);
                if (MailSettingData?.Count > 0) Utility.Mail.MailSetting = MailSettingData;
                Utility.Mail.IsOfficialMail = MailModelOptions.IsOfficialMail;
                Utility.Mail.MailServer = MailModelOptions.Default.Server;
                Utility.Mail.MailUserName = MailModelOptions.Default.UserName;
                Utility.Mail.MailPD = MailModelOptions.Default.Password;
                Utility.Mail.MailFrom = MailModelOptions.Default.From;
                Utility.Mail.MailFromDisplayName = MailModelOptions.Default.DisplayName;
                Utility.Mail.MailSSL = MailModelOptions.Default.SSL;
                Utility.Mail.MailPort = MailModelOptions.Default.Port;
                Utility.Mail.IsAccountPWD = MailModelOptions.Default.IsAccountPWD;
                Utility.Mail.sysAdmin = MailModelOptions.sysAdmin;
            }
            WebLevelManagementService.DemoDNS = AppSettingHelper.GetAppsetting("DemoDNS");
            // end 
            #endregion
            #region          
            var ClientID = AppSettingHelper.GetAppsetting("ClientID");
            var ClientSecret = AppSettingHelper.GetAppsetting("ClientSecret");
            Utility.MailBox.Api.Url = AppSettingHelper.GetAppsetting("speedAPI");
            Utility.MailBox.Api.FileServiceUrl = AppSettingHelper.GetAppsetting("FileServiceApi");
            Utility.MailBox.Api.toekenClass = new Utility.MailBox.Api.ToekenModel() { ClientId = ClientID, ClientSecret = ClientSecret };
            #endregion
            #region Session
            services.AddSession(options =>
            {
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; 
                options.Cookie.Name = "MODA";
                options.IdleTimeout = TimeSpan.FromMinutes(15); 

            });
            services.AddAntiforgery(option =>
            {
                option.Cookie.Name = "CUSTOMER-CSRF-COOKIE";
                option.FormFieldName = "CustomerFieldName";
                option.HeaderName = "CUSTOMER-CSRF-HEADER";
            });

            #endregion
            #region SQL 
            var AESkey = AppSettingHelper.GetAppsetting("AESKey");
            var sqlDecrypt = this.Configuration.GetConnectionString("MODA");
            var needEncryption = AppSettingHelper.GetAppsetting("NeedEncryption");
            if (needEncryption == "1")
            {
                sqlDecrypt = Utility.AES.AesDecrypt(sqlDecrypt, AESkey);
            }
            var TrustServerCertificate = "TrustServerCertificate=true;";
            Services.MODAContext.DB_ConnectionString = sqlDecrypt + TrustServerCertificate;
            #endregion
            #region httpOnly
            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                }).AddCookie(options =>
                {
                    options.Cookie.HttpOnly = true;
                });
            #endregion
            #region HSTS
            services.AddHsts(options =>
            {
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(365);
                options.Preload = true;
            });
            #endregion
            Services.Static.StaticLinkService.mainWebSiteID = AppSettingHelper.GetAppsetting("MainWebSiteID");
            Services.CommonService.WebSiteUrl = AppSettingHelper.GetAppsetting("WebSiteUrl");
            Services.CommonService.WebAPIUrl = AppSettingHelper.GetAppsetting("WEBAPI");
            Services.ModaMailBox.MailBoxSendMail.MailBoxUrl = AppSettingHelper.GetAppsetting("MailBoxUrl");

			
			services.AddHostedService<MemoryUsageMonitor>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.Use(async (context, next) =>
                {
                    context.Response.Headers.Add("X-Frame-Options", "SAMEORIGIN");
                    context.Response.Headers.Add("x-xss-protection", new StringValues("1; mode=block"));
                    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                    context.Response.Headers.Add("Referrer-Policy", "no-referrer");
                    //context.Response.Headers.Add("Content-Security-Policy", "default-src *;  img-src 'self' data:;frame-src *;script-src * 'unsafe-inline' 'unsafe-eval';style-src * 'unsafe-inline' 'unsafe-eval';frame-ancestors 'self'");
                    //  context.Response.Headers.Add("Content-Security-Policy", AppSettingHelper.GetAppsetting("Content-Security-Policy"));
                    await next();
                });
            }
            else
            {
                app.UseHttpsRedirection();
                app.UseHsts();
                app.Use(async (context, next) =>
                {
                    context.Response.Headers.Add("X-Frame-Options", "SAMEORIGIN");
                    context.Response.Headers.Add("x-xss-protection", new StringValues("1; mode=block"));
                    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                    context.Response.Headers.Add("Referrer-Policy", "no-referrer");
                    await next();
                });
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseSession();
            app.UseStaticFiles();

            #region
            if (AppSettingHelper.GetAppsetting("Environment") != "windows")
            {
                var staticPath = AppSettingHelper.GetAppsetting("StaticPath");
                app.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(
                        Path.Combine(env.ContentRootPath, $@"{staticPath}/copyright")),
                    RequestPath = "/copyright"
                });
            }
            #endregion

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                   name: "areaRoute",
                   pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
