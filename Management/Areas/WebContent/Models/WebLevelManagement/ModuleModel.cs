using DBModel;
using System.Collections.Generic;
using Utility;
using static Utility.Files;

namespace Management.Areas.WebContent.Models.WebLevelManagement
{
    public class ModuleModel
    {
        public List<string> LevelBreadcrumb { get; set; }

        public string webLevelSN { get; set; }

        public string webLevelTitle { get; set; }

        /// <summary>
        /// 語系
        /// </summary>
        public string lang { get; set; }
        /// <summary>
        /// 語系選單
        /// </summary>
        public List<SysWebSiteLang> sysWebSiteLangs { get; set; }
        public WebLevel mainLevelData { get; set; }
        public List<CommonFileModel> commonFileModels { get; set; } = new List<CommonFileModel>();

        public List<ModuleViewModel> webLevelDatas { get; set; }

        public List<WebLevelModuleModel> template { get; set; }

        public List<WebLevelModuleModel> levelMenu2 { get; set; }

        public List<WebLevelModuleModel> levelMenu3 { get; set; }

        public List<WebLevelModuleModel> levelMenu5 { get; set; }

        public List<WebLevelModuleModel> Condition { get; set; }
    }
}
