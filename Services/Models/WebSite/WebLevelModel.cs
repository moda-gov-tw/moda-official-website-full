using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Models.WebSite
{
    public class WebLevelModel
    {
        /// <summary>
        /// WebLevel
        /// </summary>
        public int WebLevelSN { get; set; }
        /// <summary>
        /// WebSiteID
        /// </summary>
        public string WebSiteID { get; set; }

        /// <summary>
        /// ParentSN
        /// </summary>
        public int ParentSN { get; set; }
        /// <summary>
        /// WeblevelType
        /// </summary>
        public string WeblevelType { get; set; }
        /// <summary>
        /// Module
        /// </summary>
        public string Module { get; set; }

        /// <summary>
        /// Lang
        /// </summary>
        public string Lang { get; set; }
        /// <summary>
        /// Title
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// SortOrder
        /// </summary>
        public int SortOrder { get; set; }
        /// <summary>
        /// IsEnable
        /// </summary>
        public string IsEnable { get; set; }
        /// <summary>
        /// ProcessDate
        /// </summary>
        public DateTime? ProcessDate { get; set; }

        public string MainMenuShow { get; set; }
        public int? MainSN { get; internal set; }

        public string DynamicURL { get; set; }

        public string target { get; set; }
    }
}
