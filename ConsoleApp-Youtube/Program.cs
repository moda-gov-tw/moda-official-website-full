using ConsoleApp;
using DBModel;
using Microsoft.Extensions.Configuration;
using Services;
using Utility;

#region  appsetting
IConfiguration config = new ConfigurationBuilder()
.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
.Build();
string start = config["start"];
string logFile = config["LogFile"];
string StaticFile = config["StaticFile"];
#region Mail
var MailTypeList = new List<string>() { "Default", "MailBox" };
var MailItems = new List<string>() {
"Mail:xxx:Type",
"Mail:xxx:Server",
"Mail:xxx:UserName",
"Mail:xxx:Password",
"Mail:xxx:From",
"Mail:xxx:DisplayName",
"Mail:xxx:Port",
"Mail:xxx:SSL",
"Mail:xxx:IsAccountPWD",
};
var MailSettingData = new List<DefaultMailSettingModel>();
foreach (var t in MailTypeList)
{
    try
    {
        var mailSettingModel = new DefaultMailSettingModel();

        foreach (var item in MailItems)
        {
            var itemName = item.Split(":")[item.Split(":").Length - 1];
            var itemValue = config[item.Replace("xxx", t)];
            var propertyInfo = mailSettingModel.GetType().GetProperty(itemName);
            var _item = Convert.ChangeType(itemValue, propertyInfo.PropertyType);
            mailSettingModel.GetType().GetProperty(itemName).SetValue(mailSettingModel, _item);
        }
        MailSettingData.Add(mailSettingModel);
    }
    catch (Exception ex)
    {

    }
}
Utility.Mail.MailSetting = MailSettingData;
Utility.Mail.sysAdmin = config["Mail:sysAdmin"];
Utility.Mail.MailSSL = bool.Parse(config["MailSSL"] ?? "true");

#endregion
#region DB
string needEncryption = config["needEncryption"];
string AESKey = config["AESKey"];
string DB_ConnectionString = config.GetConnectionString("MODA");
var _SqlDecrypt = "";
if (needEncryption == "1") { _SqlDecrypt = Utility.AES.AesDecrypt(DB_ConnectionString, AESKey); }
else { _SqlDecrypt = DB_ConnectionString; }
var TrustServerCertificate = "TrustServerCertificate=true;";
Services.MODAContext.DB_ConnectionString = _SqlDecrypt + TrustServerCertificate;
#endregion
#endregion
var txtMsg = new List<string>();
var strDate = DateTime.UtcNow.AddHours(8);
var speedAPIMore = start.ToLower() != "mailbox" ? "" : string.IsNullOrWhiteSpace(SendApi.SpeedAPIMore) ? "" : $" {SendApi.SpeedAPIMore}";

switch (start.ToLower())
{
    case "static":
        #region StaticHelper
        ConsoleApp.StaticHelper.gitPush = config["static:git:push"];
        ConsoleApp.StaticHelper.gitExecute = "";
        ConsoleApp.StaticHelper.DemoDNS = config["static:DemoDNS"];
        ConsoleApp.StaticHelper.ResetHours = config["static:ResetHours"];
        ConsoleApp.StaticHelper.WebSiteUrl = config["static:WebSiteUrl"];
        ConsoleApp.StaticHelper.IsOfficial = config["static:IsOfficial"];
        #endregion
        StaticHelper.Start(logFile , ref txtMsg); break;
    case "mailbox":
        #region SendApi
        SendApi.SpeedAPIMore = config["mailbox:SpeedAPIMode"];
        SendApi.ManagementUrl = config["mailbox:ManagementUrl"];
        Utility.MailBox.Api.Url = config["mailbox:SpeedAPI"];
        Utility.MailBox.Api.MailBoxUrl = config["mailbox:MailBoxUrl"];
        Utility.MailBox.Api.FileServiceUrl = config["mailbox:FileServiceApi"];
        Services.CommonService.WebAPIUrl = config["mailbox:WEBAPI"];
        Services.ModaMailBox.MailBoxSendMail.MailBoxUrl = config["mailbox:MailBoxUrl"];
        var ClientID = config["mailbox:ClientID"];
        var ClientSecret = config["mailbox:ClientSecret"];
        Utility.MailBox.Api.toekenClass = new Utility.MailBox.Api.ToekenModel() { ClientId = ClientID, ClientSecret = ClientSecret };
        #endregion
        speedAPIMore = SendApi.SpeedAPIMore;
        SendApi.MailBoxApi(logFile , ref txtMsg); break;
    case "youtube":
        string DemoDNS = config["youtube:DemoDNS"];
        YouTubeApi.GetData(logFile, DemoDNS, ref txtMsg); break;
    default: break;
}

var endDate = DateTime.UtcNow.AddHours(8);
var UseTime = @$"執行時間：{CommFun.DateDiff(strDate, endDate)}";

txtMsg.Add(UseTime);
txtMsg.Add("結束");

var sinfo = txtMsg.Select(x => $@"{DateTime.UtcNow.AddHours(8).ToString("yyyy-MM-dd HH:mm:ss")}，執行：{start.ToLower()}{speedAPIMore}" + x).ToList();



Utility.LogExpansion.Write(logFile, sinfo);
var WEBScheduleServiceData = new WEBSchedule()
{
    Name = start.ToLower().Trim(),
    UseTime = UseTime,
    ProcessDate = strDate
};
WEBScheduleService.Update(WEBScheduleServiceData);
Environment.Exit(0);






