﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace DBModel
{
    public partial class LogWebSite
    {
        /// <summary>
        /// 流水碼
        /// </summary>
        public int LogWebSiteSN { get; set; }
        /// <summary>
        /// WebLevelMSN
        /// </summary>
        public int? WebLevelSN { get; set; }
        /// <summary>
        /// WebNewsSN
        /// </summary>
        public int? WebNewsSN { get; set; }
        /// <summary>
        /// WebFileSN
        /// </summary>
        public int? WebFileSN { get; set; }
        /// <summary>
        /// GroupId
        /// </summary>
        public string GroupId { get; set; }
        public int? WebJournalSN { get; set; }
        public int? WebJournalArticleSN { get; set; }
        public int? WebBanknoteSN { get; set; }
        public string Ip { get; set; }
        /// <summary>
        /// CreateDate
        /// </summary>
        public DateTime CreateDate { get; set; }
    }
}