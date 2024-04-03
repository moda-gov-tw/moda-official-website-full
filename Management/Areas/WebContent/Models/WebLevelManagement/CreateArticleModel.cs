using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utility;

namespace Management.Areas.WebContent.Models.WebLevelManagement
{
    public class CreateArticleModel
    {
        public List<string> LevelBreadcrumb { get; set; }
        public string WebLevelSN { get; set; }

        public string WeblevelType { get; set; }

        public List<WebLevelModuleModel> LevelMenu { get; set; }

        public List<WebLevelModuleModel> LevelMenu2 { get; set; }

        public List<WebLevelModuleModel> LevelMenu3 { get; set; }

        public List<WebLevelModuleModel> Condition { get; set; }

        public string sysUserSysDepartmentID { get; set; }

    }
}
