using DBModel;
using System.Collections.Generic;

namespace Management.Areas.MailBox.Models.CaseApplyClass
{
    public class DetailModel
    {
        public DBModel.CaseApplyClass caseApplyClass { get; set; }

        public List<CaseApplyClassTo> caseApplyClassTos { get; set; }

        public List<SysDepartment> sysDepartments { get; set; }
        /// <summary>
        /// 意見分類大項目是否啟用
        /// </summary>
        public List<SysCategory> ParentClass { get; set; }
    }
}
