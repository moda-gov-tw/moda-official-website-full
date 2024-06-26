﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace DBModel
{
    public partial class LogAction
    {
        /// <summary>
        /// 流水號，起始值1，遞增值1
        /// </summary>
        public int LogActionSN { get; set; }
        /// <summary>
        /// 使用者帳號
        /// </summary>
        public string UserID { get; set; }
        public string WebSiteID { get; set; }
        /// <summary>
        /// controller name
        /// </summary>
        public string Controller { get; set; }
        /// <summary>
        /// action/function name
        /// </summary>
        public string Action { get; set; }
        /// <summary>
        /// 0:Web, 1:Service, 2:Utility
        /// </summary>
        public string ActionType { get; set; }
        /// <summary>
        /// 狀態：1:成功、0:失敗
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 資料來源：WEBNEWS、WEBBanner、WebJournal、WebLevel、WEBFile 參考Utility.SysConst.SourceTable
        /// </summary>
        public string SourceTable { get; set; }
        public int? SourceSN { get; set; }
        /// <summary>
        /// 執行動作 Add、Del、Edit  參考Utility.SysConst.Action
        /// </summary>
        public string Action2 { get; set; }
        /// <summary>
        /// 輸入資料
        /// </summary>
        public string MessageInput { get; set; }
        /// <summary>
        /// 結果訊息
        /// </summary>
        public string MessageResult { get; set; }
        /// <summary>
        /// 紀錄修訂的網頁路徑
        /// 如：系統架構相關/網站管理/網站架構列表/首頁/研究報告查詢/99年委辦計畫一覽表
        /// </summary>
        public string WebPath { get; set; }
        public string DepartmentID { get; set; }
        /// <summary>
        /// 建檔日期
        /// </summary>
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// 登入者的IP
        /// </summary>
        public string ProcessIPAddress { get; set; }
    }
}