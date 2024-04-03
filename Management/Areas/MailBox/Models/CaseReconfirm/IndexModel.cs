using Services.Models.ModaMailBox;
using System.Collections.Generic;

namespace Management.Areas.MailBox.Models.CaseReconfirm
{
    public class IndexModel
    {
        /// <summary>
        /// 民意信箱分類
        /// </summary>
        public List<CasesModel> CaseClassList { get; set; }
    }
}
