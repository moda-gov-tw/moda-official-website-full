using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Management.Models
{
    public class CommonWebNewsModel
    {

        /// <summary>
        /// WebLevelM-WebLevelSN
        /// </summary>
        public string WebLevelSN { get; set; }
        /// <summary>
        /// WebLevelM-WebSiteID
        /// </summary>
        public string WebSiteID { get; set; }
        /// <summary>
        /// WebLevelM-HierarchyID
        /// </summary>
        public string HierarchyID { get; set; }
        /// <summary>
        /// WebLevelM-ParentSN
        /// </summary>
        public string ParentSN { get; set; }
        /// <summary>
        /// WebLevelM-WeblevelType
        /// </summary>
        public string WeblevelType { get; set; }
        /// <summary>
        /// WebLevelM-Module
        /// </summary>
        public string Module { get; set; }
        /// <summary>
        /// WebLevelD-Lang
        /// </summary>
        public string Lang { get; set; }
        /// <summary>
        /// WebLevelD-Title
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// WebLevelD-SortOrder
        /// </summary>
        public string SortOrder { get; set; }

        public string MainSN { get; set; }

        public Boolean Disabled { get; set; }
    }
}
