
using Microsoft.AspNetCore.Mvc;

namespace WebSite.Controllers
{
    public class CommonController : BaseController
    {

        public CommonController( )
        {
        }

        public IActionResult Index()
        {
            return View();
        }

        public static string FBVideo(string key , int type)
        {
            switch (type)
            {
                case 0:
                    return $"https://youtu.be/{key}";

                case 1:
                    return $"https://i.ytimg.com/vi/{key}/hqdefault.jpg";
            }
            return "";
        
        }
    }
}
