using DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utility;

namespace Management.Areas.WebContent.Models
{
    public class ArticleListModel
    {
        public List<string> LevelBreadcrumb { get; set; }
        public List<WEBNews>  wEBNews { get; set; }

        public DefaultPager defaultPager { get; set; }
        /// <summary>
        /// 排序List(所有News)
        /// </summary>
        public List<WEBNews> SortList { get; set; }

        public List<WEBNews> SortIstop { get; set; }
        public string Module { get; set; }

        public string SortMethod { get; set; }

        public List<AuthSysGroupWebLevel> AuthSysGroupWebLevels { get; set; }

        public List<LogAction> logActions { get; set; }

        public string sorttitle { get; set; } = "SortOrder";

        public string sorttype { get; set; } = "asc";

    }
}
