using DBModel;
using System.Collections.Generic;
using static Utility.Files;

namespace Management.Areas.MailBox.Models.WebSite
{
    public class IndexModel
    {
        public CaseApplyWeb caseApplyWeb { get; set; }


        public List<CommonFileModel> commonFileModels { get; set; }

    }
}
