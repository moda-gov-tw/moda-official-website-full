using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Models
{
    public class sysGroupToUserModel
    {
        /// <summary>
        /// 群組代碼
        /// </summary>
        public int SysGroupSN { get; set; }

        /// <summary>
        /// 群組名稱
        /// </summary>
        public string GroupName { get; set; }
        /// <summary>
        /// 說明
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 是否啟用
        /// </summary>
        public string IsEnable { get; set; }

        /// <summary>
        /// 是否有選取
        /// </summary>
        /// 
        public bool? IsSelect { get; set; }
        
         

    }
}
