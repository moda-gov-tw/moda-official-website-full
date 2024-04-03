﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace DBModel
{
    public partial class WebLevelCustomizeTag
    {
        /// <summary>
        /// 流水號，起始值1，遞增值1
        /// </summary>
        public int WebLevelCustomizeTagSn { get; set; }
        /// <summary>
        /// 單元序號[WebLevel]
        /// </summary>
        public int WebLevelSn { get; set; }
        /// <summary>
        /// 自訂標籤名稱
        /// </summary>
        public string TagName { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int SortOrder { get; set; }
        /// <summary>
        /// 是否啟用：1 啟用、0 停用     -99刪除
        /// </summary>
        public string IsEnable { get; set; }
        /// <summary>
        /// 建立人員
        /// </summary>
        public string CreatedUserId { get; set; }
        /// <summary>
        /// 建立日期
        /// </summary>
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// 處理人員
        /// </summary>
        public string ProcessUserId { get; set; }
        /// <summary>
        /// 處理時間
        /// </summary>
        public DateTime ProcessDate { get; set; }
        /// <summary>
        /// 處理人員IP
        /// </summary>
        public string ProcessIpaddress { get; set; }
    }
}