﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace DBModel
{
    public partial class WebFileExtend
    {
        /// <summary>
        /// 參考WebFile.WebFileSN
        /// </summary>
        public int WebFileExtendSN { get; set; }
        /// <summary>
        /// 檔案內容
        /// </summary>
        public string FileContentText { get; set; }
        /// <summary>
        /// 使用者保護   0-沒有(false)   1-有(true)
        /// </summary>
        public bool? IsProtect { get; set; }
    }
}