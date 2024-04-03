using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Nancy;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Unicode;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

builder.Services.AddControllers(
    options => options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true
    );
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("1.0", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "API", Version = "1.0" });
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        c.IncludeXmlComments(xmlPath);
    });

builder.Services.AddCors(options =>
    {
        options.AddPolicy("CorsPolicy", policy =>
        {
            policy.WithOrigins(
                 config.GetSection("AllowOrigins").Get<string[]>()
                )
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
    });

builder.Services.AddSingleton(HtmlEncoder.Create(UnicodeRanges.All));
var cookieOptions = new CookieOptions
{
    Secure = true,
    HttpOnly = true,
    SameSite = SameSiteMode.None
};

var AESkey = builder.Configuration.GetValue<string>("AESKey");
var SqlDecrypt = builder.Configuration.GetConnectionString("MODA");
var needEncryption = builder.Configuration.GetValue<string>("needEncryption");
var _SqlDecrypt = SqlDecrypt;
if (needEncryption == "1")
{
    _SqlDecrypt = Utility.AES.AesDecrypt(SqlDecrypt, AESkey);
}
var TrustServerCertificate = "TrustServerCertificate=true;";
Services.MODAContext.DB_ConnectionString = _SqlDecrypt + TrustServerCertificate;

Services.CommonService.WebSiteUrl = builder.Configuration.GetValue<string>("WebSiteHost");
Services.CommonService.WebAPIUrl = builder.Configuration.GetValue<string>("WebAPIUrl");

builder.Services.AddMvc();

var app = builder.Build();

app.Use(async (context, next) =>
{
    context.Response.Cookies.Append("CookieKey", "CookieValue", cookieOptions);
    await next();
});

if (config.GetSection("AllowSwagger").Value == "true")
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/1.0/swagger.json", "API 1.0"));
}


app.UseStaticFiles();
app.UseCors("CorsPolicy");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapFallback(async (ctx) =>
{
    var phpath = Path.Join(app.Environment.WebRootPath, ctx.Request.Path);
    var name = Path.Combine(Path.GetDirectoryName(phpath)!, "404.html");
    ctx.Response.StatusCode = 200;
    ctx.Response.Redirect("404.html");
});

app.Run();
