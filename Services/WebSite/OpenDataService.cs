using DBModel;
using Services.Models.WebSite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.WebSite
{
    public class OpenDataService
    {
        public static List<OpenDataModel.News> GetWEBNews() 
        {
            List<OpenDataModel.News> webNews = new List<OpenDataModel.News>();

            using (var db = new MODAContext())
            {
                try
                {
                    var news = from n in db.WEBNews
                               where n.Module == "NEWS" && n.IsEnable == "1"
                               select new OpenDataModel.News
                               {
                                   Lang = n.Lang,
                                   Title = n.Title,
                                   SubTitle = n.SubTitle,
                                   Description = n.Description
                               };

                    webNews = news.ToList();
                }
                catch (Exception)
                {

                }
            }

            return webNews; 
        }
    }
}
