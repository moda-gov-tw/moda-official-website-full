using DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utility;

namespace Management.Areas.WebContent.Models.WebLevelManagement
{
    public class ArticleModel
    {
        public List<string> LevelBreadcrumb { get; set; }
        /// <summary>
        /// 主表
        /// </summary>
        public WebLevel WebLevel { get; set; }
        
        /// <summary>
        /// 狀態
        /// </summary>
        public Dictionary<string, string> IsEnable;

        /// <summary>
        /// 選單_單位2
        /// </summary>
        public List<SysDepartment> SysDepartments { get; set; }

        /// <summary>
        /// ChkYoutubeApi
        /// </summary>
        public bool ChkYoutubeApi { get; set; } = false;
    }
}
