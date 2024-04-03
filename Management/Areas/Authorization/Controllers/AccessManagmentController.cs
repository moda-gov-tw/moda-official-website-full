using Management.ManagementUtility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Services.Authorization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Utility;
using Utility.Models.Authorization;

namespace Management.Areas.Authorization.Controllers
{
    [Area("Authorization")]
    public class AccessManagmentController : BaseController
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        public AccessManagmentController(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        public IActionResult Index()
        {
            ViewData["Breadcrumb"] = CommonUtility.Breadcrumb(11);
            var chkUM = CheckUserMenu(11);
            if (!chkUM.chk)
            {
                return RedirectToAction("ErrorCome", "Home", new { area = "" });
            }

            return View();
        }
        /// <summary>
        /// 權限表查詢
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="groupsn"></param>
        /// <param name="departmentid"></param>
        /// <returns></returns>
        public IActionResult List(string userid = "", string groupsn = "", string departmentid = "")
        {
            var list = AccessManagmentService.GetExcel1(new UserGroupSysSectionModel() { UserID = userid, GroupSN = groupsn, DepartmentID = departmentid });
            var viewModel = new Management.Areas.Authorization.Models.AccessManagment.ListModel();
            viewModel.userGroupSysSectionModels = list;
            return View(viewModel);
        }

        /// <summary>
        /// 權限表匯出
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="groupsn"></param>
        /// <param name="departmentid"></param>
        /// <returns></returns>
        public FileContentResult ExcelReport(string userid = "", string groupsn = "", string departmentid = "")
        {
            var fileName = $@"使用者功能權限清單{DateTime.UtcNow.AddHours(8).ToString("yyyyMMdd")}.xlsx";
            try
            {
                var path = _hostingEnvironment.WebRootPath;
                var excelDatas = new MemoryStream();
                var list = AccessManagmentService.GetExcel2(new UserGroupSysSectionModel() { UserID = userid, GroupSN = groupsn, DepartmentID = departmentid }).OrderBy(x => x.UserID).ThenBy(x => x.DepartmentName).ToList();
                var filePath = $@"{path}/Temp/ExampleReport.xlsx";
                var Info = new List<string>() {
                $@"建表日期{DateTime.UtcNow.AddHours(8).ToString("yyyy-MM-dd HH:mm")}",
                $@"建表人{UserData?.sysUser.UserName}",
                $@"資料筆數{list.Count()}" };
                var DetailTitle = new List<string>() {
                "權限區分",
                "單位名稱",
                "帳號/名稱",
                "狀態",
                "程式名稱",
                "節點名稱",
                "節點維護","內容編修","權限設定"
            };

                var EModel = new ExcelModel();
                EModel.Title = "MODA使用者權限清單";
                EModel.Info = Info;
                EModel.DetailTitle = DetailTitle;
                EModel.ExcelDetails = list.Select(x => new ExcelDetailModel()
                {
                    a = x.GroupIsEnable == "0" ? x.GroupName + "(停用)" : x.GroupName,
                    b = x.DepartmentName,
                    c = x.UserID + "/" + x.UserName,//x.SectionTitle,
                    d = (x.IsEnable == "1" ? "啟用" : (x.IsEnable == "0" ? "停用" : "刪除")),
                    e = x.SectionTitle,
                    f = x.LevelPath,
                    g = x.ModulePath.Length > 0 ? "V" : "",
                    h = x.AtricPath.Length > 0 ? "V" : "",
                    i = x.AuthPath.Length > 0 ? "V" : ""
                }
                ).ToList();
                excelDatas = Utility.Output.ExampleReport(EModel, filePath);

                return File(excelDatas.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{fileName}");
            }
            catch (Exception ex)
            {
                IList<string> errmsgs = new List<string>();
                string error = "";
                error = ex.Message;
                errmsgs.Add(error);
                return File(System.Text.Encoding.UTF8.GetBytes(error), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{fileName}");
            }
        }
    }
}
