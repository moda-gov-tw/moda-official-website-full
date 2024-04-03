using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Models.WebManagement
{
    public class CheckLinkModel
    {
        public class CheckingLink
        {
            public string SourceTable { get; set; }

            public int SourceSN { get; set; }

            /// <summary>
            /// 相關連結SN 回帶連結名稱
            /// </summary>
            public int ExtendSN { get; set; }

            public string URL { get; set; }

            /// <summary>
            /// 0-Cnt 1-NewsExtend 2-NewsArticleType2
            /// </summary>
            public int LinkType { get; set; }
            public string error { get; set; }
        }

        public class ExportLink 
        {
            public string Breadcrumb { get; set; }

            public string URL { get; set; }

            public string PageTitle { get; set; }

            public string WebURL { get; set; }

            public string DeptName { get; set; }

            public string Title { get; set; }

            public string error { get; set; }
        }
    }
}
