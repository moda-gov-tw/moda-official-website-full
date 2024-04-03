using Services.Models.MailBox;
using System.Collections.Generic;
using Utility;

namespace Management.Areas.MailBox.Models.CaseApply
{
    public class ListModel
    {
        public List<CaseApplyModel> GetCases { get; set; }
        public DefaultPager defaultPager { get; set; }
    }
}
