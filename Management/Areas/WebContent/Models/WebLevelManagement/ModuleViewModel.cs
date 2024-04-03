using DBModel;
using System.Collections.Generic;
using Utility;
using static Utility.Files;

namespace Management.Areas.WebContent.Models.WebLevelManagement
{
    public class ModuleViewModel
    {
        public List<string> LevelBreadcrumb { get; set; }
        /// <summary>
        /// 主表資料
        /// </summary>
        public WebLevel mainWebLevel { get; set; }
        /// <summary>
        /// 各語系資料
        /// </summary>
        public WebLevel webLevelData { get; set; }
        /// <summary>
        /// 檔案
        /// </summary>
        public List<CommonFileModel> commonFileModels { get; set; }
        /// <summary>
        /// 類型
        /// </summary>
        public List<WebLevelModuleModel> LevelMenu { get; set; }

        /// <summary>
        /// 類型
        /// </summary>
        public string levelMenuString { get; set; }

        /// <summary>
        /// 列表
        /// </summary>
        public List<WebLevelModuleModel> LevelMenu2 { get; set; }
        /// <summary>
        /// 列表
        /// </summary>
        public string typeNameString { get; set; }

        /// <summary>
        /// 模型
        /// </summary>
        public List<WebLevelModuleModel> LevelMenu3 { get; set; }
        /// <summary>
        /// 版型
        /// </summary>
        public List<WebLevelModuleModel> LevelMenu5 { get; set; }

        public List<WebLevelModuleModel> Condition { get; set; }

        public string sysUserSysDepartmentID { get; set; }
        /// <summary>
        /// 客製化顯示/隱藏
        /// </summary>
        public  DivClassName DivClassName { get; set; } = new DivClassName();

        /// <summary>
        /// 司 關聯頁
        /// </summary>
        public List<WebLevel> DepsysCategories { get; set; } = new List<WebLevel>();


    }

    /// <summary>
    /// class 假如需要隱藏添加 d-none
    /// </summary>
    public class DivClassName
    {
        /// <summary>
        /// 列表類型
        /// </summary>
        public string div_LevelMenu2 { get; set; } = "";
        /// <summary>
        /// 搜尋條件
        /// </summary>
        public string div_Search { get; set; } = "";
        /// <summary>
        /// 版型
        /// </summary>
        public string div_LevelMenu5 { get; set; } = "";
        /// <summary>
        /// 司關聯選單
        /// </summary>
        public string div_DEPT { get; set; } = "";

        /// <summary>
        /// 顯示於FatFooter
        /// </summary>
        public string div_FatFooter { get; set; } = "";
        /// <summary>
        /// 顯示於主選單
        /// </summary>
        public string div_MainMenu { get; set; } = "";
        /// <summary>
        ///RSS 
        /// </summary>
        public string div_RSS { get; set; } = "";
        /// <summary>
        /// 顯示於左側選單
        /// </summary>
        public string div_LeftMenuShow { get; set; } = "";
        /// <summary>
        /// 排序
        /// </summary>
        public string div_Sort { get; set; } = "";
        /// <summary>
        /// 分類
        /// </summary>
        public string div_custom { get; set; } = "";
        /// <summary>
        /// SEO
        /// </summary>
        public string div_SEO { get; set; } = "";

    }
}
