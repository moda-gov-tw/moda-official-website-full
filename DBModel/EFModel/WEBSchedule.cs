using System;

namespace DBModel
{
    public partial class WEBSchedule
    {
        /// <summary>
        /// 流水碼
        /// </summary>
        public int ScheduleSN { get; set; }
        /// <summary>
        /// 排程名稱
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// (預設) 可以控制間隔時間
        /// </summary>
        public int? Frequency { get; set; }
        /// <summary>
        /// 花費的時間
        /// </summary>
        public string UseTime { get; set; }
        /// <summary>
        /// (預設)開關
        /// </summary>
        public int? InEnable { get; set; }
        /// <summary>
        /// 更新時間
        /// </summary>
        public DateTime? ProcessDate { get; set; }
        /// <summary>
        /// 備註
        /// </summary>
        public string Info { get; set; }
    }
}
