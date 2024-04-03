using DBModel;
using Services.Models.ModaMailBox;
using System.Collections.Generic;

namespace Management.Areas.MailBox.Models.CaseApply
{
    public class IndexModel
    {
        public List<CasesModel> casesModels { get; set; }

        /// <summary>
        /// 意見分類
        /// </summary>
        public List<SysCategory> SysCategory { get; set; }

        /// <summary>
        /// 排程的時間
        /// </summary>
        public string WEBScheduleUseTime { get; set; }

        /// <summary>
        /// 查詢條件用 機關&部門代碼
        /// </summary>
        public List<SysDepartment> websiteDept { get; set; }
    }
}
