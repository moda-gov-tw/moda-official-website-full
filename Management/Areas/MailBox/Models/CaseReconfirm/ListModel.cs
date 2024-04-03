using Services.Models.MailBox;
using System.Collections.Generic;
using Utility;

namespace Management.Areas.MailBox.Models.CaseReconfirm
{
    public class ListModel
    {
        public List<CaseApplyModel> CaseApplyList { get; set; }
        public DefaultPager defaultPager { get; set; }
    }
}
