﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace DBModel
{
    public partial class WEBOpenDataMain
    {
        /// <summary>
        /// 流水號
        /// </summary>
        public int WEBOpenDataMainSN { get; set; }
        /// <summary>
        /// 標題
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 副標題
        /// </summary>
        public string SubTitle { get; set; }
        /// <summary>
        /// 網站編號
        /// </summary>
        public string WebSiteID { get; set; }
        /// <summary>
        /// 群組編號
        /// </summary>
        public string SysGroupID { get; set; }
        /// <summary>
        /// 語系
        /// </summary>
        public string Lang { get; set; }
        /// <summary>
        /// 備註
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 內容
        /// </summary>
        public string ContentText { get; set; }
        /// <summary>
        /// 資料庫名稱
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// SQL條件
        /// </summary>
        public string SQLParameter { get; set; }
        /// <summary>
        /// 類別
        /// </summary>
        public int? ModuleType { get; set; }
        /// <summary>
        /// 聯絡方式
        /// </summary>
        public string ContactInfo { get; set; }
        /// <summary>
        /// 聯絡人
        /// </summary>
        public string ContacPerson { get; set; }
        /// <summary>
        /// 筆數
        /// </summary>
        public int? Count { get; set; }
        /// <summary>
        /// 是否需要XML
        /// </summary>
        public int? IsXML { get; set; }
        /// <summary>
        /// 是否需要JSON
        /// </summary>
        public int? IsJSON { get; set; }
        /// <summary>
        /// 是否需要CSV
        /// </summary>
        public int? IsCSV { get; set; }
        /// <summary>
        /// 是否移除標籤
        /// </summary>
        public int? IsRemoveTag { get; set; }
        /// <summary>
        /// 編碼格式
        /// </summary>
        public string EncodingType { get; set; }
        /// <summary>
        /// 認證類型
        /// </summary>
        public string AuthType { get; set; }
        /// <summary>
        /// 開始時間
        /// </summary>
        public DateTime? StartDate { get; set; }
        /// <summary>
        /// 結束時間
        /// </summary>
        public DateTime? EndDate { get; set; }
        /// <summary>
        /// 建立時間
        /// </summary>
        public DateTime? DateCreated { get; set; }
        /// <summary>
        /// 維護時間
        /// </summary>
        public DateTime? ProcessDate { get; set; }
        /// <summary>
        /// 維護人
        /// </summary>
        public string ProcessUserID { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int? SortOrder { get; set; }
        /// <summary>
        /// 狀態
        /// </summary>
        public string IsEnable { get; set; }
        /// <summary>
        /// 授權說明網址
        /// </summary>
        public string URL { get; set; }
        /// <summary>
        /// 更新說明
        /// </summary>
        public string Refresh { get; set; }
        /// <summary>
        /// 計費方式
        /// </summary>
        public string ChargeType { get; set; }
        /// <summary>
        /// 部門ID
        /// </summary>
        public string DepartmentID { get; set; }

    }
}