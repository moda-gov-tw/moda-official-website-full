using DBModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Models
{
    public class sysUserModel
    {
        /// <summary>
        /// 確認是否正確
        /// </summary>
        public bool check { get; set; } = true;
        /// <summary>
        /// 是否超過90天
        /// </summary>
        public bool isOver90days { get; set; } = false;

        /// <summary>
        /// 重設密碼的網址
        /// </summary>
        public string resetOver90days { get; set; }

        /// <summary>
        /// 登入者訊息
        /// </summary>
        public SysUser sysUser { get; set; }

        /// <summary>
        /// 訊息
        /// </summary>
        public string message { get; set; }

        public List<SysWebSite> sysWebSites { get; set; }

        /// <summary>
        /// 登入者部門流水碼
        /// </summary>
        public int? sysUserSysDepartmentSN { get; set; }

        public string WebSiteID { get; set; }

        /// <summary>
        /// Menu 
        /// </summary>
        public List<vw_SysSection> menu { get; set; }

        public int errorCount { get; set; } = 0;
        public string type { get; set; }

        /// <summary>
        /// 群組
        /// </summary>
        public List<SysGroup> sysGroups { get; set; }

        public List<AuthSysGroupWebLevel> webLevelAccessForGroups { get; set; }

        public bool GodMode { get; set; } = false;


    }



}
