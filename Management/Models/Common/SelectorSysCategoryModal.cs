using DBModel;
using System.Collections.Generic;

namespace Management.Models.Common
{
    public class SelectorSysCategoryModal : definitionModel
    {
        //public string IdName = string.Empty;
        /// <summary>
        /// 父層資料
        /// </summary>
        public string parentKey { get; set; }
        /// <summary>
        /// 語系
        /// </summary>
        public string Lang { get; set; }

        public string WebSiteId { get; set; }

        /// <summary>
        /// 自己的KEY - 可以是多選
        /// </summary>
        public List<string> sysCategoryKeys { get; set; } = new List<string>();
        /// <summary>
        /// 子層資料
        /// </summary>
        //  public List<SysCategory> sysCategories { get; set; }

        /// <summary>
        /// 是否可多選
        /// </summary>
        public bool multiple = false;
        /// <summary>
        /// 標題class
        /// </summary>
        public string titleClass { get; set; } = string.Empty;
        /// <summary>
        /// 控制項class
        /// </summary>
        public string controlClass { get; set; } = string.Empty;
        /// <summary>
        /// 是否必填
        /// </summary>
        public bool required { get; set; } = false;

    }
}
