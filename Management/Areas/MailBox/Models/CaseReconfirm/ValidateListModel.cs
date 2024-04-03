using DBModel;
using Services.Models.MailBox;
using System.Collections.Generic;
using Utility;

namespace Management.Areas.MailBox.Models.CaseReconfirm
{
    public class ValidateListModel
    {
        public List<CaseApplyValidate> CaseValidateList { get; set; }
        public DefaultPager defaultPager { get; set; }
    }
}
