using System.Collections.Generic;
using static Utility.Files;

namespace Management.Areas.MailBox.Models.CaseApplyPage
{
    public class DetailModel
    {
        /// <summary>
        /// 民意信箱頁面
        /// </summary>
        public DBModel.CaseApplyPage Page { get; set; }
        /// <summary>
        /// 頁面選項
        /// </summary>
        public List<DBModel.CaseApplyPageExtend> PageExtends { get; set; }
        /// <summary>
        /// 內嵌圖片
        /// </summary>
        public List<CommonFileModel> PageImgs  { get; set; }
    }
}
