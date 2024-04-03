using DBModel;
using System.Collections.Generic;
using static Utility.Files;

namespace Management.Areas.WebContent.Models
{
    public class NewsDefaultModel
    {
        public WebLevel webLevel { get; set; }
        public WEBNews wEBNews { get; set; }

        public List<WEBFile> wEBFiles { get; set; } = new List<WEBFile>();

        public List<CommonFileModel> commonFileModels { get; set; } = new List<CommonFileModel>();

        public List<NewCommonModel> newCommonModels { get; set; } = new List<NewCommonModel>();

        public List<SysWebSiteLang> sysWebSiteLangs { get; set; } = new List<SysWebSiteLang>();

        public List<string> LevelBreadcrumb { get; set; }

        public string sysUserSysDepartmentID { get; set; }
    }
}
