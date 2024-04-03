using DBModel;
using System.Collections.Generic;
using static Utility.Files;

namespace Management.Areas.WebContent.Models
{
    public class NewCommonModel
    {
        public WEBNews webNews { get; set; }
        /// <summary>
        /// 首長行程
        /// </summary>
        public List<WEBNews> webNewsSchedule { get; set; }
        /// <summary>
        /// 檔案
        /// </summary>
        public List<CommonFileModel> commonFileModels { get; set; }
        /// <summary>
        /// 擴充
        /// </summary>
        public List<WEBNewsExtend> wEBNewsExtends { get; set; } = new List<WEBNewsExtend>();

        public List<AuthSysGroupWebLevel> AuthSysGroupWebLevels { get; set; }

        public string sysUserSysDepartmentID { get; set; }
        /// <summary>
        /// 依機關判斷主視覺模組顯示說明文字
        /// </summary>
        public string WebsiteID { get; set; }
    }
}
