using Management.Models.Common;

namespace Management
{
    public class SelectorWebLevelTreeModel : definitionModel
    {
        /// <summary>
        /// 自身MainSN
        /// </summary>
        public int MainSN { get; set; }

        public int ParentSN { get; set; }

        /// <summary>
        /// WebSiteID
        /// </summary>
        public string WebSiteID { get; set; }

        public Utility.EnumWebLevelModuleLevel2 EnumWebLevelModuleLevel { get; set; }

    }
}
