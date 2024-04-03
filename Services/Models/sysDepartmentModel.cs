using DBModel;

namespace Services.Models
{
    public class sysDepartmentModel
    {
        /// <summary>
        /// 確認是否正確
        /// </summary>
        public bool check { get; set; } = true;

       
        public SysDepartment sysDepartment { get; set; }

        /// <summary>
        /// 訊息
        /// </summary>
        public string message { get; set; }
    }
}
