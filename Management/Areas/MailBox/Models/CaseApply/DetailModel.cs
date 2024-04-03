using DBModel;
using Services.Models.MailBox;
using Services.Models.ModaMailBox;
using System;
using System.Collections.Generic;
using static Utility.Files;

namespace Management.Areas.MailBox.Models.CaseApply
{
    public class DetailModel
    {
        public DBModel.CaseApply caseApply { get; set; }
        /// <summary>
        /// 民眾附件
        /// </summary>
        public List<DBModel.WEBFile> wEBFiles { get; set; }
        /// <summary>
        /// 回覆附件
        /// </summary>
        public List<DBModel.WEBFile> wEBFiles2 { get; set; }
        /// <summary>
        /// 系統回覆附件
        /// </summary>
        public List<CommonFileModel> wEBFiles3 { get; set; }
        public CasesModel casesModel { get; set; }

        /// <summary>
        /// 系統親回 log
        /// </summary>
        public List<ReplyModel> replyModels { get; set; }
        /// <summary>
        /// 改分log
        /// </summary>
        public List<ResetLogModel> ResetLog { get; set; }

        public ReplySource ReplySource { get; set; }

        public string OriginalDeptName { get; set; }

        public string OriginalClassName { get; set; }
        /// <summary>
        /// 公文銷案紀錄
        /// </summary>
        public List<CaseApplySpeedApiLogModel> ReturnLog { get; set; }
    }

    /// <summary>
    /// 回覆model
    /// </summary>
    public class ReplyModel
    {
        public string caseApplySN { get; set; }
        public string replySource2 { get; set; }
        public string UserID { get; set; }
        public DateTime? dateTime { get; set; }

        public List<ReplyFileModel> files { get; set; }
    }

    public class ReplyFileModel
    { 
        public string webFileID { get; set; }
        public string fileTitle { get; set; }
    }
}
