using DBModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Models.WebContent
{
    public class WebLevelForTreeModel
    {

        public WebLevel webLevel { get; set; }
        public List<WebLevelForTreeModel> webChild { get; set; }
        /// <summary>
        /// 語系
        /// </summary>
        public string lang { get; set; }
        /// <summary>
        /// 語系選單
        /// </summary>
        public List<SysWebSite> sysWebSiteLangs { get; set; }


    }
}
