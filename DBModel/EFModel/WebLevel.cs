﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace DBModel
{
    public partial class WebLevel
    {
        /// <summary>
        /// 流水號，起始值1，遞增值1
        /// </summary>
        public int WebLevelSN { get; set; }
        /// <summary>
        /// 自訂節點名稱&lt;同一網站不能一致&gt;
        /// </summary>
        public string WebLevelKey { get; set; }
        /// <summary>
        /// 父層序號
        /// </summary>
        public int ParentSN { get; set; }
        /// <summary>
        /// 網站編號
        /// </summary>
        public string WebSiteID { get; set; }
        /// <summary>
        /// 語系
        /// </summary>
        public string Lang { get; set; }
        /// <summary>
        /// 1:網站單元、2.首頁區塊、3.標題頁尾區塊
        /// </summary>
        public string WeblevelType { get; set; }
        /// <summary>
        /// 單元模組，代碼如下CP:單頁、NEWS：條列式(一般模組)、LINK： 相關連結、
        /// </summary>
        public string Module { get; set; }
        /// <summary>
        /// 參數
        /// </summary>
        public string Parameter { get; set; }
        /// <summary>
        /// 標題
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 簡介
        /// </summary>
        public string ContentText { get; set; }
        /// <summary>
        /// 顯示於FatFooter：1 顯示、0 不顯示
        /// </summary>
        public string FatFooterShow { get; set; }
        /// <summary>
        /// 顯示於主選單：1 顯示、0 不顯示
        /// </summary>
        public string MainMenuShow { get; set; }
        /// <summary>
        /// 顯示於子選單：1 顯示、0 不顯示(即環保署左選單)
        /// </summary>
        public string SubMemuShow { get; set; }
        /// <summary>
        /// 顯示左側選單:1 顯示、0 不顯示
        /// </summary>
        public string LeftMenuShow { get; set; }
        /// <summary>
        /// 是否提供RSS
        /// </summary>
        public string RSSShow { get; set; }
        /// <summary>
        /// 瀏覽/點閱數
        /// </summary>
        public int? PageView { get; set; }
        /// <summary>
        /// 上架時間
        /// </summary>
        public DateTime? StartDate { get; set; }
        /// <summary>
        /// 下架時間
        /// </summary>
        public DateTime? EndDate { get; set; }
        /// <summary>
        /// 內容區塊標頭
        /// </summary>
        public string ContentHeader { get; set; }
        /// <summary>
        /// 內容區塊頁尾
        /// </summary>
        public string ContentFooter { get; set; }
        /// <summary>
        /// 前台列表樣式
        /// </summary>
        public string ListType { get; set; }
        /// <summary>
        /// 排序方式:0 資料排序、1最新消息排序
        /// </summary>
        public string SortMethod { get; set; }
        /// <summary>
        /// 是否啟用：1 啟用、0 停用  -99 刪除
        /// </summary>
        public string IsEnable { get; set; }
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
        /// <summary>
        /// 建立人員
        /// </summary>
        public string CreatedUserID { get; set; }
        /// <summary>
        /// 建立日期
        /// </summary>
        public DateTime? CreatedDate { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int? SortOrder { get; set; }
        /// <summary>
        /// &lt;同筆資料不同語系有想同的MainSN&gt;可以想像GroupSN
        /// </summary>
        public int? MainSN { get; set; }
        /// <summary>
        /// 靜態化網址
        /// </summary>
        public string StatesUrl { get; set; }
        /// <summary>
        /// 版型樣式
        /// </summary>
        public string TemplateValue { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 查詢條件
        /// </summary>
        public string Condition { get; set; }
        /// <summary>
        /// SEODescription
        /// </summary>
        public string SEODescription { get; set; }
        /// <summary>
        /// SEOKeywords
        /// </summary>
        public string SEOKeywords { get; set; }

        /// <summary>
        /// 發布單位
        /// </summary>
        public string DepartmentID { get; set; }

        /// <summary>
        /// 額外擴充CSS
        /// </summary>
        public string AdditionalCSS { get; set; }


        /// <summary>
        /// 額外擴充Script
        /// </summary>
        public string AdditionalScript { get; set; }

    }
}