using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Models.WebSite;
using Services.WebSite;
using Utility;
using WebSite.Models;
using WebSite.WebSiteUtility;

namespace WebSite.Controllers
{
    public class DeptController : BaseController
    {
        [Route("{WebSiteID?}/{Lang?}/Dept/{Key}")]
        public IActionResult Index(int Key, string Lang, string WebSiteID)
        {
            WebSiteID = WebSiteID ?? MainWebSite;
            Lang = Lang ?? MainLang;
            WEBSITEID = WebSiteID;
            LANG = Lang;
            //BaseController.WebSiteID = WebSiteID ??  BaseController.MainWebSite;
            //BaseController.Lang = Lang ?? BaseController.MainLang;
            DeptModel deptModel = new DeptModel();
            var webLevel = HomeService.getWebLevelSNByKey(Lang, Key);
            deptModel.webSiteBreadcrumbs = CommonService.GetWebSiteBreadcrumb(webLevel.Lang, webLevel.MainSN.Value);
            if (webLevel.Module != EnumTpye.GetEnumName(EnumWebLevelModuleLevel2.DEPT) || webLevel.Parameter == null || !int.TryParse(webLevel.Parameter ,out int _Parameter)) 
            { 
                return View(); 
            }
            //司首頁
            deptModel.Dept = HomeService.getWebLevelSNByKey(Lang, _Parameter);
            deptModel.LogoImg = HomeService.getLogoImg(deptModel.Dept?.MainSN).FirstOrDefault();
            deptModel.SysWebSiteLang = CommonService.GetSysWebSiteLang(WebSiteID, Lang);

            var lastTilte = string.Join('｜', deptModel.webSiteBreadcrumbs.Select(x => x.Title));
            //meta-og
            deptModel.ogData = new Services.Models.WebSite.ogModel()
            {
                title = lastTilte + "｜" + deptModel.SysWebSiteLang.Title,
                image = $"{WebSiteUrl}{deptModel.LogoImg?.FilePath}" ,
                image_type = deptModel.LogoImg?.FileType != null ? OpenGragh.getImageType(deptModel.LogoImg?.FileType) : "image/jpeg",
                description = OpenGragh.getFormattedDescription(deptModel.Dept?.ContentHeader)
            };

            //get image 
            OpenGragh.getImageSize(deptModel.LogoImg?.FilePath, out int Height, out int Width);
            deptModel.ogData.image_height = Height.ToString();
            deptModel.ogData.image_width = Width.ToString();
            

            if (deptModel.Dept != null) 
            {
                //第一層
                var levels = HomeService.getDeptChild(deptModel.Dept.MainSN.Value, deptModel.Dept.WebSiteID, deptModel.Dept.Lang);
                deptModel.ChildNodes = new List<WebSiteChildModel>();
                foreach (var level in levels.ToList())
                {
                    var child = new WebSiteChildModel();
                    child = HomeService.getChild(level);
                    deptModel.ChildNodes.Add(child);
                }
            }
            return View(deptModel);
        }
    }
}
