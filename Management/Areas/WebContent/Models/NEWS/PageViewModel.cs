using DBModel;
using Services.Models;
using Services.Models.WebSite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Management.Areas.WebContent.Models.WebLevelManagement
{
    public class PageViewModel
    {
        public class News
        {
            public  WebNewsDetailModel webNewsDetailModel { get; set; } = new WebNewsDetailModel();
            /// <summary>
            /// WebNewsDetailModel
            /// </summary>
            public class WebNewsDetailModel
            {
                //基本資訊，EX標題、內容、內容模式、URL.....
                public WEBNews BasicData;
                //相關檔案、圖片
                public Attachment attachment;

                public List<WebLink> webLinks;

                public struct Attachment
                {
                    /// <summary>
                    /// 網頁模式
                    /// </summary>
                    public Page page;

                    /// <summary>
                    /// 檔案下載模式
                    /// </summary>
                    public Download download;
                }

                public struct Page
                {
                    /// <summary>
                    /// 相關檔案
                    /// </summary>
                    public List<WebFileAndGroupIDModel> Files;

                    /// <summary>
                    /// 相關圖檔
                    /// </summary>
                    public List<WebFileAndGroupIDModel> Imgs;

                }

                public struct Download
                {
                    /// <summary>
                    /// 單檔下載
                    /// </summary>
                    public WebFileAndGroupIDModel File;
                }
            }
            public WebLevel parentData { get; set; }
            public WebLevel localWebLevel { get; set; }
            public List<PAGELISTModel> brotherData { get; set; }
            public string DepName { get; set; }

            public List<WebLevelModel> Breadcrumb { get; set; }
        }
        //
    }


}
