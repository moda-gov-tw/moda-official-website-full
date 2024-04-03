using Services.Models.WebContent;
using System;
using System.Collections.Generic;
using System.Text;
using DBModel;

namespace Services.Models.WebSite
{
    /// <summary>
    /// WebTabModel
    /// </summary>
    public class WebTabModel
    {
        /// <summary>
        /// Tab頁籤資訊
        /// </summary>
        public TabData BasicData { get; set; }

        /// <summary>
        /// Tab頁籤對應資料列(最新N筆)
        /// </summary>
        public List<newsChildModel> TabNewList { get; set; }
        //public List<WEBNews> TabNewList { get; set; }

        public class TabData
        {
            #region
            /// <summary>
            /// DeptLevelSN
            /// </summary>
            public int WebLevelSN { get; set; }
            /// <summary>
            /// 語系
            /// </summary>
            public string Lang { get; set; }
            /// <summary>
            /// 流水號，起始值1，遞增值1
            /// </summary>
            public int WEBNewsSN { get; set; }
            /// <summary>
            /// 主標題
            /// </summary>
            public string Title { get; set; }
            /// <summary>
            /// 上架時間
            /// </summary>
            public DateTime? StartDate { get; set; }
            /// <summary>
            /// 下架時間
            /// </summary>
            public DateTime? EndDate { get; set; }
            /// <summary>
            /// 建立日期
            /// </summary>
            public DateTime CreatedDate { get; set; }
            /// <summary>
            /// 排序
            /// </summary>
            public int? SortOrder { get; set; }
            #endregion
            public int RelWebLevelMSN { get; set; }
            public string RelWebLevelModule { get; set; }
        }
    }
}

