using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Models
{
    public class CaseApplyClassModel
    {
        /// <summary>
        /// 意見分類
        /// </summary>
        public string CaseName { get; set; }
        /// <summary>
        /// 承辦單位
        /// </summary>
        public string depName { get; set; }
        /// <summary>
        /// 新增時間
        /// </summary>
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// 修改時間
        /// </summary>
        public DateTime ProcessDate { get; set; }
        /// <summary>
        /// 最後編輯者
        /// </summary>
        public string ProcessUserID { get; set; }
        /// <summary>
        /// 流水碼
        /// </summary>
        public int CaseApplyClassSN { get; set; }
        /// <summary>
        /// 狀態
        /// </summary>
        public string IsEnable { get; set; }
        /// <summary>
        /// 民意信箱分類
        /// </summary>
        public string SysCategoryKey { get; set; }
        /// <summary>
        /// 意見分類代號
        /// </summary>
        public string CaseNo { get; set; }
        /// <summary>
        /// 子分類
        /// </summary>
        public string CaseType { get; set; }
        /// <summary>
        /// 辦理天數
        /// </summary>
        public string HandleDate { get; set; }
        /// <summary>
        /// 承辦人員
        /// </summary>
        public string CaseTo { get; set; }
    }

    public class MailToModel 
    {
        public int CaseApplyClassSN { get; set; }

        public string MailToName { get; set; }
    }
}
