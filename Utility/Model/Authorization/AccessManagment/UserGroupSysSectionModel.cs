using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Models.Authorization
{
    public class UserGroupSysSectionModel
    {
        /// <summary>
        /// 帳號
        /// </summary>
        public string UserID { get; set; }
        /// <summary>
        /// 使用者名稱
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 群組名稱
        /// </summary>
        public string GroupName { get; set; }
        public string GroupIsEnable { get; set; }
        /// <summary>
        /// 群組SN
        /// </summary>
        public string GroupSN { get; set; }

        /// <summary>
        /// 選單權限
        /// </summary>
        public string SectionTitle { get; set; }

        /// <summary>
        /// 選單SN
        /// </summary>
        public string SectionSN { get; set; }

        /// <summary>
        /// 部門名稱
        /// </summary>
        public string DepartmentName { get; set; }

        /// <summary>
        /// 部門名稱
        /// </summary>
        public string DepartmentID { get; set; }

        public string IsEnable { get; set; }
        public string WebLevelTitle { get; set; }
        public string LevelPath { get; set; }
        public string ModulePath { get; set; }

        public string AtricPath { get; set; }
        public string AuthPath { get; set; }
    }
    
}
