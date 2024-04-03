using DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Models.MailBox
{
    public class CaseApplyModel 
    {
        public string CaseNo { get; set; }

        public string CaseName { get; set; }

        public string Subject { get; set; }
        public string ContactEmail { get; set; }
        public DateTime? AcceptDate { get; set; }
        /// <summary>
        /// 承辦機關-單位
        /// </summary>
        public string depName { get; set; }
        /// <summary>
        /// 機關信箱-單位
        /// </summary>
        public string OriginalDeptName { get; set; }
        public string stateName { get; set; }

        public string Status { get; set; }

        public int CaseApplySN { get; set; }

        public int CaseApplyClassSN { get; set; }

        public string ReplySource { get; set; }

        public DateTime? ReplyDate { get; set; }
        
        public DateTime? CreateDate { get; set; }
        public DateTime? EffectiveDate { get; set; }
    }

    public class ResetLogModel 
    {
        /// <summary>
        /// 改分來源
        /// </summary>
        public string ResetSourse { get; set; }
        /// <summary>
        /// 改分自(單位)
        /// </summary>
        public string ResetFrom { get; set; }
        /// <summary>
        /// 改分前分類
        /// </summary>
        public string ResetFromClassName { get; set; }
        /// <summary>
        /// 改分至(單位)
        /// </summary>
        public string ResetTo { get; set; }
        /// <summary>
        /// 改分後分類
        /// </summary>
        public string ResetToClassName { get; set; }
        /// <summary>
        /// 改分時間
        /// </summary>
        public DateTime ResetDate { get; set; }
        /// <summary>
        /// 公文文號
        /// </summary>
        public string ReplyCaseNo { get; set; }
    }

    public class ReplySource 
    {
        /// <summary>
        /// 回覆來源
        /// </summary>
        public string ReplySourceName { get; set; }
        /// <summary>
        /// 承辦單位
        /// </summary>
        public string ReplyDocName { get; set; }
        /// <summary>
        /// 承辦人員
        /// </summary>
        public string ReplyUser { get; set; }
        /// <summary>
        /// 回覆案件時間
        /// </summary>
        public string ReplyDate { get; set; }
    }

    /// <summary>
    /// 退文記錄model
    /// </summary>
    public class CaseApplySpeedApiLogModel
    {
        public int speedApiLogSn { get; set; }
        public string returnCaseNo { get; set; }

        public string returnMessage { get; set; }

        public string closedat { get; set; }

        public string DocDept { get; set; }
    }
}
