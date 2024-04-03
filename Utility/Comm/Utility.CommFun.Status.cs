using Nancy.Json;
using System;

namespace Utility.CommFun2
{

    public static class Status
    {
        public static Msg GetIsEnableDesc(string isenable, DateTime? str = null, DateTime? end = null)
        {
            Msg oRtn = new Msg();

            string rtn = SysConstTable.CntStatus.NoPublish;
            oRtn.status = ((int)Utility.SysConst.Show.NotDisplay).ToString();

            if (isenable == "1")   //發布
            {
                if (end != null && end < DateTime.UtcNow.AddHours(8)) //過期下架
                {
                    rtn = SysConstTable.CntStatus.OffShelf;
                }
                else //公開
                {
                    rtn = SysConstTable.CntStatus.Publish;
                    oRtn.status = Utility.SysConst.Show.Display.ToString();
                }
            }
            else if (isenable == "3")
            {
                rtn = SysConstTable.CntStatus.Reviewer;
            }
            else if (isenable == "-2")
            {
                rtn = SysConstTable.CntStatus.Returned;
            }
            else
            {
                rtn = SysConst.FindInDictionary(SysConst.IsEnable.Items(), isenable, isenable);
            }

            oRtn.desc = rtn;

            return oRtn;

        }


        public class Msg
        {
            public string desc { get; set; }
            public string status { get; set; }
        }

    }


}
