using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace Services.Models.WebSite
{
    public class WebSiteListModel
    {
        /// <summary>
        /// News列表頁
        /// </summary>
        public class NewsList 
        {
            /// <summary>
            /// 分頁模組
            /// </summary>
            public DefaultPager pager { get; set; }
            /// <summary>
            /// 新聞列表
            /// </summary>
            public List<VMNews> News { get; set; }
        }

        /// <summary>
        /// News View Model
        /// </summary>
        public class VMNews
        {
            /// <summary>
            /// 公布日期
            /// </summary>
            public string PublishDate { get; set; }
            /// <summary>
            /// 標題
            /// </summary>
            public string Title { get; set; }
            /// <summary>
            /// 副標題
            /// </summary>
            public string SubTitle { get; set; }
            /// <summary>
            /// 頁面連結
            /// </summary>
            public string URL { get; set; }

            /// <summary>
            /// 檔案類型
            /// </summary>
            public string File { get; set; } = "";

        }
    }
}
