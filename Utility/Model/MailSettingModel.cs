using System;
using System.Collections.Generic;

namespace Utility
{
    public class MailInfoModel
    {
        /// <summary>
        /// MailTitle 類型 : 預設Default
        /// </summary>
        public string Type { get; set; } = "Default";
        /// <summary>
        /// 收件者
        /// </summary>
        public string ToMail { get; set; }
        /// <summary>
        /// 主旨
        /// </summary>
        public string Subject { get; set; }
        /// <summary>
        /// 內文
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// 本地檔案 - 檔案路徑<尚未測試>
        /// </summary>
        public string FilePath { get; set; } = "";
        /// <summary>
        /// 檔案使用 Stream 
        /// </summary>
        public List<MailFile> Files { get; set; } = null;

    }

    public class DefaultMailSettingModel
    {
        /// <summary>
        /// 類型
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// Mail
        /// </summary>
        public string Server { get; set; }
        /// <summary>
        /// 帳號
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 密碼
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 來源
        /// </summary>
        public string From { get; set; }
        /// <summary>
        /// 自訂寄信者名稱
        /// </summary>
        public string DisplayName { get; set; } = "";
        /// <summary>
        /// PORT -預設25
        /// </summary>
        public int Port { get; set; } = 25;
        /// <summary>
        /// 是否有SSL 預設true
        /// </summary>
        public bool SSL { get; set; } = true;
        /// <summary>
        /// 是否需要密碼 預設true
        /// </summary>
        public bool IsAccountPWD { get; set; } = true;

    }

    public class MailFile
    {
        public System.IO.Stream stream { get; set; }
        public string FileName { get; set; }
    }

}
