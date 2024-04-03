using DBModel;
using Services.Models;
using System.Collections.Generic;
using Utility;

namespace Management.Areas.MailBox.Models.CaseApplyClass
{
    public class ListModel
    {
        public List<CaseApplyClassModel> caseApplyClassModel { get; set; }

        public DefaultPager defaultPager { get; set; }

        public List<SysCategory> sysCategories { get; set; }

        /// <summary>
        /// 意見分類大項目是否啟用
        /// </summary>
        public SysCategory ParentClass { get; set; }
    }
}
