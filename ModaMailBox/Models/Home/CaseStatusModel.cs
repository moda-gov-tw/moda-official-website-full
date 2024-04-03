using DBModel;

namespace ModaMailBox.Models
{
    public class CaseStatusModel
    {

        public CaseApply? caseApply { get; set; }

        /// <summary>
        /// 預設回覆內容
        /// </summary>
        public string? presetReply { get; set; }

    }
}
