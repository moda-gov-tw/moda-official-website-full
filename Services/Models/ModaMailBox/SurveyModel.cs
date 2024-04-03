using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Models.ModaMailBox
{
    public class SurveyModel
    {
        public string CaseNo { get; set; }

        public string CaseSatisfy { get; set; }

        public string CaseSolved { get; set; }

        public string CaseDefect { get; set; }

        public string CaseProposal { get; set; }

        public DateTime CreateDate { get; set; }
    }

    public class Statistics
    {
        public int CaseCount { get; set; } = 0;
        /// <summary>
        /// 收案數
        /// </summary>
        public int SurveyCount { get; set; } = 0;
        /// <summary>
        /// 滿意度加總
        /// </summary>
        public int SatisfyCount { get; set; } = 0;
        /// <summary>
        /// 完全解決
        /// </summary>
        public int SolvedCount_A { get; set; } = 0;
        /// <summary>
        /// 部份解決
        /// </summary>
        public int SolvedCount_B { get; set; } = 0;
        /// <summary>
        /// 沒有解決
        /// </summary>
        public int SolvedCount_C { get; set; } = 0;
        /// <summary>
        /// 提出建議
        /// </summary>
        public int SolvedCount_D { get; set; } = 0;
        /// <summary>
        /// 無
        /// </summary>
        public int DefectCount_A { get; set; } = 0;
        /// <summary>
        /// 未解決
        /// </summary>
        public int DefectCount_B { get; set; } = 0;
        /// <summary>
        /// 態度不佳
        /// </summary>
        public int DefectCount_C { get; set; } = 0;
        /// <summary>
        /// 時間太慢
        /// </summary>
        public int DefectCount_D { get; set; } = 0;
        /// <summary>
        /// 欠缺誠意
        /// </summary>
        public int DefectCount_E { get; set; } = 0;
        /// <summary>
        /// 引用錯誤
        /// </summary>
        public int DefectCount_F { get; set; } = 0;
        /// <summary>
        /// 推諉責任
        /// </summary>
        public int DefectCount_G { get; set; } = 0;
        /// <summary>
        /// 與實情不符
        /// </summary>
        public int DefectCount_H { get; set; } = 0;
    }
}
