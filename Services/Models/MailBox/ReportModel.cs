using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Models.MailBox
{
    public  class MailBoxReportModel
    {
        public string CaseNo { get; set; }
        public string ClassName { get; set; }
        public string depName { get; set; }
        public string OriginalDeptName { get; set; }
        public string Subject { get; set; }
        public DateTime? AcceptDate { get; set; }

        public string Status { get; set; }

        public DateTime? ReplyDate { get; set; }

        //public DateTime? ReplySource2Date { get; set; }

        public string ReplySource { get; set; }

    }

    public class CaseApplyModel2
    {
        /// <summary>
        /// 流水碼
        /// </summary>
        public int CaseApplySN { get; set; }
        /// <summary>
        /// 標題
        /// </summary>
        public string Subject { get; set; }
        /// <summary>
        /// 內文
        /// </summary>
        public string CaseContent { get; set; }
        /// <summary>
        /// 申請者姓名
        /// </summary>
        public string ApplyUser { get; set; }
        /// <summary>
        /// 申請者信箱
        /// </summary>
        public string ContactEmail { get; set; }
        /// <summary>
        /// 區域號碼
        /// </summary>
        public string TelAreacode { get; set; }
        /// <summary>
        /// 家裡電話
        /// </summary>
        public string Tel { get; set; }
        /// <summary>
        /// 分機號碼
        /// </summary>
        public string TelExtension { get; set; }
        /// <summary>
        /// 手機號碼
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 案件編號- yyyyMMDDhhZZZZ (ZZZZ為流水碼)每小時最大可以受理9999
        /// </summary>
        public string CaseNo { get; set; }
        /// <summary>
        /// 案件驗證碼 隨機碼
        /// </summary>
        public string CasePwd { get; set; }

        /// <summary>
        /// 使用者 確認 案件成立時間
        /// </summary>
        public DateTime? AcceptDate { get; set; }
        public string depName { get; set; }

        public string DepartmentID { get; set; }

        public string ReplyCaseNo { get; set; }

        public string ReplyCasePwd { get; set; }

        /// <summary>
        /// 案件編號
        /// </summary>
        public string CaseApplyClassCaseNo { get; set; }

    }
}