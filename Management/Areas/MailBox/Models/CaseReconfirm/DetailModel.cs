using Services.Models.ModaMailBox;
using System.Collections.Generic;

namespace Management.Areas.MailBox.Models.CaseReconfirm
{
    public class DetailModel
    {
        public DBModel.CaseApply caseApply { get; set; }
        /// <summary>
        /// 民眾附件
        /// </summary>
        public List<DBModel.WEBFile> WebFileList { get; set; }
        /// <summary>
        /// 所選民意信箱分類
        /// </summary>
        public CasesModel CaseClass { get; set; }
    }
}
