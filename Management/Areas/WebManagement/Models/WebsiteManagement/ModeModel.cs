using DBModel;
using static Utility.Files;
using System.Collections.Generic;

namespace Management.Areas.WebManagement.Models.WebsiteManagement
{
    public class ModeModel
    {
        public List<SysWebSiteLang> sysWebSiteLangs { get; set; }

        public List<WEBFile> wEBFiles { get; set; }

        public List<CommonFileModel> commonFileModels { get; set; }

        public List<WebSiteExtend> webSiteExtends { get; set; }
    }
}
