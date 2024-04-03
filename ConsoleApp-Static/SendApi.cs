using DBModel;
using Services.Authorization;
using Services.ModaMailBox;
using static Utility.Files;
using static Utility.MailBox.Api;
using Utility.MailBox;
using System.ComponentModel.Design;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using System.Collections;
using static Utility.CommFun2.Status;

namespace ConsoleApp
{
    public class SendApi
    {
        public static string? SpeedAPIMore { get; set; }
        public static string? ManagementUrl { get; set; }
        public static void MailBoxApi(string logFile , ref List<string>msg)
        {
            Utility.Mail.sysAdmin = LogService.GetErroEmailAccount();

            var mailboxData = Services.WEBScheduleService.GetWEBSchedule("mailbox");

            switch (SpeedAPIMore)
            {
                case "create": SendAgain(logFile, ref msg); break;
                case "search": SendSearchApi(logFile, ref msg); break;
            }
        }
        /// <summary>
        /// 發送 新增case / 再次新增失敗的case
        /// </summary>
        static void SendAgain(string logFile, ref List<string> msg)
        {
            msg.Add("開始執行");
            var BigData = MailBoxService.GetCassApply();
            msg.Add($"需要拋送給公文系統案件數量：{BigData.Count()}");
            var errorList = new List<string>();
            foreach (var b in BigData)
            {
                if (MailBox.SendAPI(b, out string errorMsg))
                {
                    msg.Add($"拋送(成功)：{b.addNewCaseModel1.CompanyCaseNo}");
                }
                else
                {
                    msg.Add($"拋送(失敗)：{b.addNewCaseModel1.CompanyCaseNo}");
                    msg.Add($"拋送(錯誤訊息)：{errorMsg}");
                    errorList.Add(errorMsg);
                };
            }
            if (errorList.Count() > 0)
            {
                MailBoxService.SendErrorCreateApi(errorList);
            }
            msg.Add("執行結束");
        }
        /// <summary>
        /// 查詢結果
        /// </summary>
        static void SendSearchApi(string logFile , ref List<string> msg)
        {
            msg.Add("開始執行");
            var BigData = MailBoxService.GetSearchCaseList();
            var ReturnCount = 0;
            var ErrorCount = new List<string>();
            var FileErrorCount = new List<string>();
            msg.Add($"需要查詢公文系統案件數量：{BigData.Count()}");
            foreach (var b in BigData)
            {
                if (MailBox.SearchAPI(b, "Console", out bool isReturn, out bool isClose, out string error, out string retMsg))
                {
                    msg.Add($@"查詢結果({retMsg})：{b.SearchCaseModel.CaseNo}");
                    if (isReturn)
                    {
                        MailBoxService.SendReSetTellUser2(ManagementUrl, b.CaseApplySN);
                        ReturnCount++;
                    }
                }
                else 
                {
                    msg.Add( $@"查詢結果(失敗)：{b.SearchCaseModel.CaseNo}");
                    msg.Add($@"查詢結果(錯誤資訊)：{error}");
                    if (error.Contains("取得檔案發生異常")) 
                    {
                        FileErrorCount.Add(b.SearchCaseModel.CaseNo);
                    }
                    else 
                    {
                        ErrorCount.Add(b.SearchCaseModel.CaseNo);
                    }
                }
            }

            if (ErrorCount.Count > 0 || FileErrorCount.Count > 0) 
            {
                var mailTo = LogService.GetErroEmailAccount();
                MailBoxSendMail.SendConsoleErrorMail(ErrorCount, FileErrorCount, mailTo ,out string errorMsg);
            }
            msg.Add("執行結束");
        }

 
    }
}