using DBModel;
using Services.Models.ModaMailBox;
using Utility;

namespace ModaMailBox.Models
{
    public class ConfirmMailModel
    {
        public CaseApply? CaseApply { get; set; }

        public List<CasesModel>?  casesModels { get; set; }

        //取token值回上頁
        public CaseApplyValidate? CaseApplyValidate { get; set; }

        public List<Files.SaveFileModel>? CaseFiles { get; set; }

        /// <summary>
        /// 意見分類
        /// </summary>
        public List<SysCategory>? SysCategory { get; set; }
        /// <summary>
        /// 意見分類大項目是否啟用
        /// </summary>
        public List<SysCategory>? ParentClass { get; set; }
    }
}
