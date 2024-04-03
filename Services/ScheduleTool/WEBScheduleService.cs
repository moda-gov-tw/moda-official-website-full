using DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class WEBScheduleService
    {
        public static void Update(WEBSchedule wEBSchedule)
        {
            using (var db = new MODAContext())
            {
                var data = db.WEBSchedule.FirstOrDefault(x => x.Name == wEBSchedule.Name);
                if (data != null)
                {
                    data.UseTime = wEBSchedule.UseTime;
                    data.ProcessDate = wEBSchedule.ProcessDate;
                    db.SaveChanges();
                }
            }
        }
        /// <summary>
        /// 抓取排程資料
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public static WEBSchedule GetWEBSchedule(string Name)
        {
            using (var db = new MODAContext())
            {
                return db.WEBSchedule.FirstOrDefault(x => x.Name == Name);
            }
        }

        public static List<WEBSchedule> ScheduleData()
        {

            using (var db = new MODAContext())
            {
                return db.WEBSchedule.Where(x =>
                  x.InEnable ==1 && 
                  (
                  x.ProcessDate <= DateTime.UtcNow.AddHours(8).AddHours(-1) ||
                  x.ProcessDate == null)
                  ).ToList();
            }

        }

    }
}
