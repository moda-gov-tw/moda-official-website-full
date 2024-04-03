using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Models
{
    public class BilingualIOpenDataModel
    {
        /// <summary>
        /// 序號
        /// </summary>
        public int? SeqNo { get; set; }
        /// <summary>
        /// 類別 一般法規/法規詞彙
        /// </summary>
        public string? Category { get; set; }
        /// <summary>
        /// 中文名稱
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// 英文名稱
        /// </summary>
        public string? EngName { get; set; }
        /// <summary>
        /// 中文內文
        /// </summary>
        public string? Content { get; set; }
        /// <summary>
        ///英文內文
        /// </summary>
        public string? EngContent { get; set; }
        /// <summary>
        /// 中文法規連結
        /// </summary>
        public string? StatURL { get; set; }
        /// <summary>
        /// 英文法規連結
        /// </summary>
        public string? EngStatURL { get; set; }

        public int? ArticleSort { get; set; }

    }
}
