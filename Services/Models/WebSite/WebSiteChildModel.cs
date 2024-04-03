using DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Models.WebSite
{
    /// <summary>
    /// 首頁區塊共用Model
    /// </summary>
    public class WebSiteChildModel
    {
        public LevelViewModel LevelView { get; set; } = new LevelViewModel();

        public List<NewsViewModel> NewsViews { get; set; }

        public List<WebSiteChildModel> Childlevels { get; set; }
    }

    public class LevelViewModel
    {
        public WebLevel BasicLevel { get; set; } = new WebLevel();

        public string DynamicURL { get; set; }
    }

    public class NewsViewModel
    {
        public WEBNews BasicNews { get; set; }

        public WebFileAndGroupIDModel MainImg { get; set; }

        public WebFileAndGroupIDModel SubImg { get; set; }

        public List<TabNewsModel> TabNewsView { get; set; }

        public string DynamicURL { get; set; }
    }

    public class TabNewsModel
    {
        public TabNews TabNews { get; set; }

        public WebFileAndGroupIDModel Logo { get; set; }

        public List<NewsTag> NewsTags { get; set; }

        public string DynamicURL { get; set; }

        public string fileType { get; set; } = "";

        public string fileTitle { get; set; } = "";

        /// <summary>
        /// 政策計畫tab分組
        /// </summary>
        public string Mark { get; set; }
        /// <summary>
        /// 相關影音
        /// </summary>
        public string MediaKey { get; set; }

        /// <summary>
        /// 相關(檔案)
        /// </summary>
        public List<webfile> relatedFile { get; set; } = null;
        /// <summary>
        /// 相關(圖片)
        /// </summary>
        public List<webfile> relatedImg { get; set; } = null;
        /// <summary>
        /// 相關連結
        /// </summary>
        public List<webnewsextend> relatedlink { get; set; } = null;
        /// <summary>
        /// 相關影片
        /// </summary>
        public List<webnewsextend> relatedvideo { get; set; } = null;
        /// <summary>
        /// 相關法規
        /// </summary>
        public List<webnewsextend> relatedmoj { get; set; } = null;
    }

    public class TabNews 
    {
        public string Source { get; set; }

        public int SourceSN { get; set; }

        public string Title { get; set; }

        public string SubTitle { get; set; }

        public string ContentText { get; set; }

        public string URL { get; set; }

        public string ArticleType { get; set; }

        public int MainSN { get; set; }

        public string target { get; set; }

        public DateTime? strDate { get; set; }

        public DateTime? PublishDate { get; set; }

        public int SortOrder { get; set; }

        public bool IsTop { get; set; }
    }

    public class NewsTag 
    {
        public string WebSiteID { get; set; }

        public string Lang { get; set; }

        public string Value { get; set; }

        public string Key { get; set; }
    }
}
