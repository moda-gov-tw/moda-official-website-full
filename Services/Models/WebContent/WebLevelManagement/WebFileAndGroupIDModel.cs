using DBModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Models
{
    public class WebFileAndGroupIDModel 
    {
        public string WEBFileID { get; set; }
        public string OriginalFileName { get; set; }
        public string FileName { get; set; }
        public string FileTitle { get; set; }
        public string FilePath { get; set; }



        public int? FileSize { get; set; }
        public string FileType { get; set; }
        /// <summary>
        /// 是否發布：0 不公開，1 發布
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
        public string CreatedUserID { get; set; }
        /// <summary>
        /// 建立日期
        /// </summary>
        public DateTime? CreatedDate { get; set; }

        public string GroupID { get; set; }

        public int SortOrder { get; set; }
    }
}
