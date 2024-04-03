using DBModel;
using Services.Models.WebSite;
using System.Collections.Generic;
using Utility;

namespace Services.Models.WebSite
{
    public class NewsListModel : meteModel
    {
        public WebLevel WebLevel { get; set; }
        public List<WEBNewsListModel> list { get; set; }
        public DefaultPager pager { get; set; }

        public int levelSN { get; set; }

        public string str_Date { get; set; }

        public string end_Date { get; set; }

        public string txt { get; set; }

        public List<string> Conditions { get; set; }

        public List<WEBNewsListModel2> BigjsonData { get; set; }
        public string StrBigjsonData { get; set; }

        public List<WebLevelCustomizeTag> CustomizeTags { get; set; }

        public List<SysZipCode> sysZipCodes { get; set; }

        public bool foreverApi { get; set; } = false; 


    }
}
