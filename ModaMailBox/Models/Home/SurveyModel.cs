using DBModel;

namespace ModaMailBox
{
    public class SurveyModel
    {
        /// <summary>
        /// 訊息
        /// </summary>
        public string Msg { get; set; } = "";
        /// <summary>
        /// 案件
        /// </summary>
        public CaseApply? CaseApply { get; set; }
    }
}
