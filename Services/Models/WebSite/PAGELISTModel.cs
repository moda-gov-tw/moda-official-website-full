using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Models.WebSite
{
   public class PAGELISTModel
    {
        public int WebLevelSN { get; set; }
        public string Module { get; set; }
        public int ParentSN { get; set; }
        public string Title { get; set; }

        public int SortOrder { get; set; }
        public string WeblevelType { get; set; }

        public string ArticleType { get; set; }

        public int? WEBNewsSN { get; set; }

        public string WEBFileID { get; set; }

        public string Url { get;set; }

        public DateTime? StartDate { get; set; }

        public string target { get; set; } = "_self";

    }
}
