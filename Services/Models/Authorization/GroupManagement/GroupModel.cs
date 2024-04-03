﻿using DBModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Models.Authorization
{
    public class GroupModel
    {
        /// <summary>
        /// 流水號，起始值1，遞增值1
        /// </summary>
        public int SysGroupSN { get; set; }
        /// <summary>
        /// 群組名稱
        /// </summary>
        public string GroupName { get; set; }
        /// <summary>
        /// 群組描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 是否啟用：1 啟用、0 停用
        /// </summary>
        public string IsEnable { get; set; }
        /// <summary>
        /// 是否可刪除：1 可、0 否，預設一般群組和系統管理群組不可刪除
        /// </summary>
        public string CanDelete { get; set; }
        /// <summary>
        /// 處理人員
        /// </summary>
        public string ProcessUserID { get; set; }
        /// <summary>
        /// 處理時間
        /// </summary>
        public DateTime? ProcessDate { get; set; }
        /// <summary>
        /// 處理人員IP
        /// </summary>
        public string ProcessIPAddress { get; set; }
        public string CreatedUserID { get; set; }
        /// <summary>
        /// 建立日期
        /// </summary>
        public DateTime? CreatedDate { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int? SortOrder { get; set; }

        public int UsersCount { get; set; }
        public int SectionCount { get; set; }
    }
}