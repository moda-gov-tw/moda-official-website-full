using DBModel;
using System;
using System.Collections.Generic;
using Utility;

namespace Management.ManagementUtility
{
    public enum MailType
    {
        /// <summary>
        /// 首次
        /// </summary>
        pwdfirst,
        /// <summary>
        /// 重置
        /// </summary>
        pwdreset,
    }

    public class MailUtility
    {
        /// <summary>
        /// 判斷是否正式環境
        /// </summary>
        static bool Official = AppSettingHelper.GetAppsetting("IsOfficialMail") == "1" ? true : false;


        /// <summary>
        /// 關於密碼的
        /// </summary>
        /// <param name="mailType"></param>
        /// <param name="path"></param>
        /// <param name="sysUser"></param>
        public static void Sendpwd(MailType mailType, string path, out Exception outex, SysUser sysUser = null)
        {
            var subject = "";
            var Url = string.Format(AppSettingHelper.GetAppsetting("LocalUrl") + "/Home/");
            var info = "";
            var mailTemplate = "";
            switch (mailType)
            {
                case MailType.pwdfirst:
                    subject = "MODA全球資訊網後端管理系統─帳號啟用認證信";
                    Url = $@"{Url}first?u={CommonUtility.AesEncrypt(sysUser.UserID)}&key={sysUser.p_w_d}";
                    info = $@"
提醒您，首次登入請先設定密碼以啟用帳號，步驟如下： <br>
Step1
點擊帳號啟用網址連結<br>
 <a href='{Url}'>點擊進入</a> <br><br>
 <p style='color:red' >※ 請用Chrome或Edge</p>
Step2<br>
進行啟始密碼設定<br>
<p style='color:red' >※ 密碼長度應至少12碼以上，並且混合大小寫英文字母、數字及特殊字元，</p>
一個複雜度符合安全要求的密碼應至少包含：<br>
一個大寫英文字母<br>
一個小寫英文字母<br>
一個數字<br>
一個特殊字元，如! @ # $ % & 等<br><br>

Step3<br>
登入數位發展部全球資訊網後端管理系統<br><br>

若有問題，請聯繫資訊處。
";

                    break;
                case MailType.pwdreset:
                    subject = "MODA全球資訊網後端管理系統─重新設定密碼";
                    Url = $@"{Url}reset?u={Utility.AES.AesEncrypt(sysUser.UserID, "MODA")}&key={sysUser.p_w_d}";
                    info = $@"
重新設定密碼步驟如下，請於10分鐘內完成密碼重設作業： <br>
Step1
點擊密碼重設網址連結：<br>
 <a href='{Url}'>點擊進入</a> <br><br>
<p style='color:red' >※ 請用Chrome或Edge</p>
Step2<br>
輸入新密碼<br>
<p style='color:red' >※ 密碼長度應至少12碼以上，並且混合大小寫英文字母、數字及特殊字元，</p>
一個複雜度符合安全要求的密碼應至少包含：<br>
一個大寫英文字母<br>
一個小寫英文字母<br>
一個數字<br>
一個特殊字元，如! @ # $ % & 等<br><br>

Step3<br>
登入數位發展部全球資訊網後端管理系統<br><br>

若有問題，請聯繫資訊處。
";
                    break;
            }

            var filePath = $@"{path}/MailTemplate/pwd.html";
            try
            {
                mailTemplate = System.IO.File.ReadAllText(filePath);
                //不能單傳KEY比對 因為有可能重複
                mailTemplate = mailTemplate
                   .Replace("[to]", sysUser.UserName)
                   .Replace("[subject]", subject)
                   .Replace("[userid]", sysUser.UserID)
                   .Replace("[info]", info)
                  ;
            }
            catch { }


            MailInfoModel mailInfo = new MailInfoModel()
            {
                ToMail = sysUser.Email.Trim(),
                Subject = subject,
                Body = mailTemplate,

            };
            Utility.Mail.Send(mailInfo, out Exception exception);
            outex = new Exception();
            // Utility.Mail.Send(sysUser.Email.Trim(), subject, mailTemplate, Official, ref outex);
        }

        public static void SendReviewer(WEBNews wEBNews, System.Data.DataTable sysUsers, out Exception outex)
        {
            string subject = "moda官網 上稿資料待覆核通知";
            List<string> email = new List<string>();
            string _email = string.Empty;
            for (var i = 0; i < sysUsers.Rows.Count; i++)
            {
                email.Add(sysUsers.Rows[i]["Email"].ToString());
            }

            if (email.Count > 0)
            {
                _email = string.Join(";", email.ToArray());
            }

            string info = "<div style='font-size:16px;'>您好：</div>";
            info += Environment.NewLine + "<div style='font-size:16px;'>您有待覆核網頁如下：</div>";
            info += Environment.NewLine + "<table style='font-size:16px' border ='1' cellpadding = '0' cellspacing = '0' width = '750'>";
            info += Environment.NewLine + "<tr>";
            info += Environment.NewLine + "<th style='font-size:16px;'>標題</th>";
            info += Environment.NewLine + "<th style='font-size:16px;'>資料位置</th>";
            info += Environment.NewLine + "</tr>";
            info += Environment.NewLine + "<tr>";
            info += Environment.NewLine + "<td style='font-size:16px;'>" + wEBNews.Title + "</td>";
            var LevelBreadcrumb = Services.CommonService.LevelBreadcrumb(wEBNews.WebLevelSN, wEBNews.WEBNewsSN);
            info += Environment.NewLine + "<td style='font-size:16px;'>" + string.Join(">", LevelBreadcrumb) + "</td>";
            info += Environment.NewLine + "</tr>";
            info += Environment.NewLine + "</table><br>";
            info += Environment.NewLine + "<div style='font-size:16px;'>請登入<a href=" + AppSettingHelper.GetAppsetting("LocalUrl") + ">全球資訊網後端管理系統</a>進行資料覆核，謝謝！</div><br>";
            info += Environment.NewLine + "<div style='font-size:16px;'>全球資訊網後台管理系統</div>";

            MailInfoModel mailInfo = new MailInfoModel()
            {
                ToMail = _email,
                Subject = subject,
                Body = info,
            };
            Utility.Mail.Send(mailInfo, out Exception exception);
            outex = new Exception();
            //Utility.Mail.Send(_email, subject, info, Official, ref outex);

        }

        public static void SendReviewerOK(WEBNews wEBNews, SysUser user, out Exception outex)
        {
            string subject = "moda官網 網頁覆核通過及發布通知";

            string info = "<div style='font-size:16px;'> 您好：</div>";
            info += Environment.NewLine + "<div style='font-size:16px;'>[" + wEBNews.Title + "]</div>";
            info += Environment.NewLine + "<div style='font-size:16px;'>已通過覆核並公布於網站上，</div>";
            info += Environment.NewLine + "<div style='font-size:16px;'>請至官網前台確認網頁內容。謝謝！</div><br>";
            info += Environment.NewLine + "<div style='font-size:16px;'>全球資訊網後端管理系統</div>";


            MailInfoModel mailInfo = new MailInfoModel()
            {
                ToMail = user.Email,
                Subject = subject,
                Body = info,
            };
            Utility.Mail.Send(mailInfo, out Exception exception);
            outex = new Exception();
            //Utility.Mail.Send(user.Email, subject, info, Official, ref outex);
        }

        public static void SendReturned(WEBNews wEBNews, SysUser user, out Exception outex)
        {
            string subject = "moda官網 上稿資料退回通知";

            string info = "<div style='font-size:16px;'>您好：</div>";
            info += Environment.NewLine + "<div style='font-size:16px;'>[" + wEBNews.Title + "]</div>";
            info += Environment.NewLine + "<div style='font-size:16px;'>覆核未通過，請您編修後再次送出覆核申請。謝謝！</div><br>";
            info += Environment.NewLine + "<div style='font-size:16px;'><a href=" + AppSettingHelper.GetAppsetting("LocalUrl") + ">全球資訊網後端管理系統</a></div><br>";

            outex = new Exception();
            //Utility.Mail.Send(user.Email, subject, info, Official, ref outex);
            MailInfoModel mailInfo = new MailInfoModel()
            {
                ToMail = user.Email,
                Subject = subject,
                Body = info,
            };
            Utility.Mail.Send(mailInfo, out Exception exception);
        }
    }
}
