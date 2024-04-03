using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Models.Authorization
{
    public class EditSysUserGroupModel
    {
        /// <summary>
        /// 
        /// </summary>
        public int groupsn { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool isselect { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string UserID { get; set; }
        /// <summary>
        /// ip
        /// </summary>
        public string CreatedIPAddress { get; set; }

        public string CreatedUserID { get; set; }

    }
}
