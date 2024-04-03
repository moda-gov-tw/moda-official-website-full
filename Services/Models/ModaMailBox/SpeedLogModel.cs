using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Models.ModaMailBox
{
    public class SpeedLogModel
    {
        public string CaseNo { get; set; }
        public string Action { get; set; }
        public string Success { get; set; }
        public string ApiStatus { get; set; }
        public string ApiMessage { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateUser { get; set; }
        public string Message { get; set; }
        public string Requset { get; set; }
    }
}
