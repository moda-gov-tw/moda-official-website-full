using Microsoft.Extensions.Primitives;
using ModaMailBox;
using ModaMailBox.Models;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Utility;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://*:80");
builder.Services.Configure<CookiePolicyOptions>(options =>
 {
     options.CheckConsentNeeded = context => false;
     options.MinimumSameSitePolicy = SameSiteMode.None;
 });
builder.Services.AddSession(options =>
{
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; 
    options.Cookie.Name = "mailbox";
    options.IdleTimeout = TimeSpan.FromHours(2); 
});
builder.Services.AddCookiePolicy(options =>
{
    options.HttpOnly =  Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.None;
    options.Secure = CookieSecurePolicy.Always;
    options.MinimumSameSitePolicy = SameSiteMode.Strict;
});
builder.Services.Configure<RouteOptions>(options =>
{
    options.LowercaseUrls = true;
});

builder.Services.AddAntiforgery(option =>
{
    option.Cookie.Name = "CUSTOMER-CSRF-COOKIE";
    option.FormFieldName = "CustomerFieldName";
    option.HeaderName = "CUSTOMER-CSRF-HEADER";
});

#region Session
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();
#endregion
#region 
Utility.MailBox.Scan.AntiVirusPath = builder.Configuration.GetValue<string>("virusscan");
var ClientID = builder.Configuration.GetValue<string>("ClientID");
var ClientSecret = builder.Configuration.GetValue<string>("ClientSecret");
Utility.MailBox.Api.Url = builder.Configuration.GetValue<string>("speedAPI");
Utility.MailBox.Api.FileServiceUrl = builder.Configuration.GetValue<string>("FileServiceApi");
Utility.MailBox.Api.toekenClass = new Utility.MailBox.Api.ToekenModel() { ClientId = ClientID, ClientSecret = ClientSecret };
#endregion

builder.Services.AddSingleton(HtmlEncoder.Create(UnicodeRanges.All));
#region SQL
var AESkey = builder.Configuration.GetValue<string>("AESKey");
var MODA0 = builder.Configuration.GetConnectionString("MODA");
var needEncryption = builder.Configuration.GetValue<string>("needEncryption");
var _SqlDecrypt = MODA0;
if (needEncryption == "1") { _SqlDecrypt = Utility.AES.AesDecrypt(MODA0, AESkey); }
var TrustServerCertificate = "TrustServerCertificate=true;";
Services.MODAContext.DB_ConnectionString = _SqlDecrypt + TrustServerCertificate;
#endregion
#region Mail
var MailSettingData = new List<DefaultMailSettingModel>();
var MailModelOptions = new AppsettingModel.MailModel();
builder.Configuration.GetSection("Mail").Bind(MailModelOptions);
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
#endregion
Services.ModaMailBox.MailBoxSendMail.MailBoxUrl = AppSettingHelper.GetAppsetting("localUrl");

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler(new ExceptionHandlerOptions()
    {
        ExceptionHandler = async context =>
        {
            context.Response.Redirect("/Home/Index");
        }
    });
    app.Use(async (context, next) =>
    {
        
        context.Response.Headers.Add("X-Frame-Options", "SAMEORIGIN");
        context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
        context.Response.Headers.Add("Referrer-Policy", "no-referrer");
        context.Response.Headers.Add("Content-Security-Policy", AppSettingHelper.GetAppsetting("Content-Security-Policy"));
        await next();
    });
}
app.UseExceptionHandler("/Home/Index");
app.UseStatusCodePagesWithRedirects("~/");
app.UseCookiePolicy();
app.UseSession();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
