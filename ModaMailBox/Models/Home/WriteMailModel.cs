using DBModel;

namespace ModaMailBox
{
    public class WriteMailModel
    {
        /// <summary>
        /// 訊息
        /// </summary>
        public string Msg { get; set; } = "";
        /// <summary>
        /// 驗證信箱資料
        /// </summary>
        public CaseApplyValidate? CaseApplyValidate { get; set; }

        /// <summary>
        /// 意見分類
        /// </summary>
        public List<Services.Models.ModaMailBox.CasesModel>? CasesModels { get; set; }

        public List<CasesModelViewItem>? CasesModelViewItems { get; set; }

        /// <summary>
        /// 選取到的
        /// </summary>
        public int CaseApplyClassSN = 0;

        public CaseApply? CaseApply { get; set; }

        /// <summary>
        /// 意見分類大項目
        /// </summary>
        public List<SysCategory>? SysCategory { get; set; }
        /// <summary>
        /// 意見分類大項目是否啟用
        /// </summary>
        public List<SysCategory>? ParentClass { get; set; }
    }
    public class CasesModelViewItem
    {
        /// <summary>
        /// SysCategoryKey     
        /// </summary>
        public string? sck { get; set; }
        /// <summary>
        /// WebSiteID          
        /// </summary>
        public string? wid { get; set; }
        /// <summary>
        /// CaseApplyClassSN   
        /// </summary>
        public int? sn { get; set; }
        /// <summary>
        /// CaseName           
        /// </summary>
        public string? cn { get; set; }

    }


}
