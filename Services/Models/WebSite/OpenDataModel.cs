using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Models.WebSite
{
    public class OpenDataModel
    {
        public class News 
        {
            /// <summary>
            /// 語言：TW、EN
            /// </summary>
            public string Lang { get; set; }
            /// <summary>
            /// 主標題
            /// </summary>
            public string Title { get; set; }
            /// <summary>
            /// 子標題
            /// </summary>
            public string SubTitle { get; set; }
            /// <summary>
            /// 描述
            /// </summary>
            public string Description { get; set; }
        }
    }
}
