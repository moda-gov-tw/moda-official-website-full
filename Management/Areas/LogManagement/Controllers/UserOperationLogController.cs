using DBModel;
using Management.Areas.LogManagement.Models.ErrorLog;
using Management.ManagementUtility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Services.Models.WebManagement;
using Services.WebManagement;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Utility;

namespace Management.Areas.LogManagement.Controllers
{
    [Area("LogManagement")]
    public class UserOperationLogController : BaseController
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        public UserOperationLogController(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        /// <summary>
        /// 首頁
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            ViewData["Breadcrumb"] = CommonUtility.Breadcrumb(17);
            if (!CheckUserMenu(17).chk) { return RedirectToAction("ErrorCome", "Home", new { area = "" }); }
            return View();
        }
        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="sd"></param>
        /// <param name="ed"></param>
        /// <param name="userid"></param>
        /// <param name="ip"></param>
        /// <param name="websiteid"></param>
        /// <param name="key"></param>
        /// <param name="sn"></param>
        /// <param name="p"></param>
        /// <param name="DisplayCount"></param>
        /// <returns></returns>
        public IActionResult List(string sd, string ed, string userid = "", string ip = "", string websiteid = "", string key = "", string sn = "", string departmentID = "", int p = 1, int DisplayCount = 15)
        {
            string str = Convert.ToDateTime(sd).ToString("yyyy-MM-dd 00:00:00").ToString();
            string end = Convert.ToDateTime(ed).ToString("yyyy-MM-dd 23:59:59").ToString();

            DefaultPager pager = new DefaultPager
            {
                DisplayCount = DisplayCount,
                p = p
            };

            var data = OperationStatisticsService.GetLogData(str, end, ref pager, 3, userid, ip, websiteid, key, sn, departmentID);

            foreach (var msg in data)
            {
                try
                {
                    var PreviousData = Services.WebManagement.OperationStatisticsService.GetLogByPrevious(msg.CreatedDate.ToString("yyyy-MM-dd HH:mm:ss"), msg.SourceSN, msg.SourceTable);
                    if (PreviousData != null && PreviousData.Action2.ToUpper() != "DELETE" && msg.Action2.ToUpper() != "DELETE")
                    {
                        var list = Management.ManagementUtility.LogUtility.LogAnalyze(PreviousData.MessageInput, msg.MessageInput, msg.SourceTable, PreviousData.Action2, msg.Action2);
                        if (list != null && list.Count > 0)
                        {
                            msg.MessageInput = string.Join("\n", list);
                        }
                        else
                        {
                            msg.MessageInput = "";
                        }
                    }
                    else
                    {
                        msg.MessageInput = "";
                    }
                }
                catch (Exception ex)
                {
                    Utility.LogExpansion.Write("D:\\Log", ex.Message);
                    msg.MessageInput = "";
                }

                var webpath = msg.WebPath;

                if (!string.IsNullOrWhiteSpace(webpath))
                {
                    var arrayWebpath = webpath.Split('/').Distinct();
                    webpath = string.Join("/", arrayWebpath);
                    msg.WebPath = webpath;
                }
            }

            Models.UserOperationLog.ListModel listModel = new Models.UserOperationLog.ListModel
            {
                defaultPager = pager,
                operationStatisticsModels = data
            };

            return View(listModel);
        }
        /// <summary>
        /// 匯出報表
        /// </summary>
        /// <param name="sd"></param>
        /// <param name="ed"></param>
        /// <param name="userid"></param>
        /// <param name="ip"></param>
        /// <param name="path"></param>
        /// <param name="websiteid"></param>
        /// <param name="key"></param>
        /// <param name="sn"></param>
        /// <returns></returns>
        //public IActionResult Export(string sd, string ed, string userid = "", string ip = "", string path = "", string websiteid = "", string key = "", string sn = "", string departmentID = "")
        //{
        //    try
        //    {
        //        string str = Convert.ToDateTime(sd).ToString("yyyy-MM-dd 00:00:00").ToString();
        //        string end = Convert.ToDateTime(ed).ToString("yyyy-MM-dd 23:59:59").ToString();
        //        var fpath = _hostingEnvironment.WebRootPath;
        //        var excelDatas = new MemoryStream();
        //        var DataList = OperationStatisticsService.GetLogData2(str, end, 3, userid, ip, path, websiteid, key, sn, departmentID);//更新資料
        //        var CountList = OperationStatisticsService.GetLogData3(str, end, 3, userid, ip, path, websiteid, key, sn, departmentID);//統計數
        //        var HelpOtherCountList = OperationStatisticsService.GetLogData4(str, end, websiteid);// 非本單位上稿統計
        //        var filePath = $@"{fpath}\Temp\ExampleReport.xlsx";
        //        var Info = new List<string>() {
        //        $@"建表日期: {DateTime.UtcNow.AddHours(8).ToString("yyyy-MM-dd HH:mm")}",
        //        $@"建表人: {UserData?.sysUser.UserName}",
        //        $@"月份：{sd}至{ed}"
        //        };

        //        var WEBLevelSN = DataList.GroupBy(u => new { u.ParentSN, u.SourceTable });

        //        var WEBLevelData = new List<dynamic>();

        //        foreach (var d in WEBLevelSN)
        //        {
        //            if (d.Key.ParentSN != null)
        //            {
        //                dynamic DyObj = new System.Dynamic.ExpandoObject();
        //                var data = Services.Authorization.WebLevelManagementService.GetWebLevel(int.Parse(d.Key.ParentSN.ToString()));
        //                DyObj.WebLevelSN = data.WebLevelSN;
        //                DyObj.Title = data.Title;
        //                DyObj.Module = data.Module;
        //                DyObj.Lang = data.Lang;
        //                WEBLevelData.Add(DyObj);
        //            }
        //        }

        //        var DetailTitle = new List<string>() {
        //        "編號",
        //        "帳號",
        //        "使用者名稱",
        //        "上稿單位",
        //        "發布單位",
        //        "IP",
        //        "異動狀態",
        //        "異動紀錄",
        //        "完整路徑",
        //        "資料編號",
        //        "時間"
        //        };

        //        var EModel = new ExcelModel();
        //        EModel.Info = Info;
        //        EModel.DetailTitle = DetailTitle;

        //        var CreateSheet = Services.CommonService.GetSysWebSiteLang(websiteid).Select(x => x.Lang).ToList();
        //        CreateSheet.Add("統計");
        //        CreateSheet.Add("代操作");
        //        var WEBSite = Services.WebSite.HomeService.GetSysWebSite().Where(x => x.WebSiteID == websiteid).Select(x => x.Title).First();

        //        foreach (var data in DataList.OrderByDescending(x => x.CreatedDate))
        //        {
        //            try
        //            {
        //                var PreviousData = OperationStatisticsService.GetLogByPrevious(data.CreatedDate.ToString("yyyy-MM-dd HH:mm:ss"), data.SourceSN, data.SourceTable);
        //                if (PreviousData != null && PreviousData.Action2.ToUpper() != "DELETE" && data.Action2.ToUpper() != "DELETE")
        //                {
        //                    var MSG = LogUtility.LogAnalyze(PreviousData.MessageInput, data.MessageInput, data.SourceTable, PreviousData.Action2, data.Action2);
        //                    data.MessageInput = (MSG != null && MSG.Count > 0) ? String.Join(", ", MSG) : "";
        //                }
        //                else
        //                {
        //                    data.MessageInput = "";
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                Utility.LogExpansion.Write("D:\\Log", ex.Message);
        //                data.MessageInput = "";
        //            }
        //        }

        //        var dataList = DataList.OrderByDescending(x => x.CreatedDate).Select(x => new ExcelDetailModel()
        //        {
        //            a = x.UserID,
        //            b = x.UserName,
        //            c = x.DepartmentName,
        //            d = x.DepartmentID == null ? "" : Services.Authorization.DepartmentManagementService.GetDepartmentBySysDepartmentID(x.DepartmentID, websiteid).Where(u => u.Lang == x.Lang).FirstOrDefault()?.DepartmentName,
        //            e = x.ProcessIPAddress,
        //            f = x.Action2.ToUpper() == "UPDATE" ? "更新資料" : x.Action2.ToUpper() == "RETURNED" ? "送審退回" : x.Action2.ToUpper() == "DELETE" ? "刪除資料" : x.Action2.ToUpper() == "SELECT" ? "查詢資料" : "新增資料",
        //            g = x.MessageInput,
        //            h = x.Webpath,
        //            i = x.SourceSN.ToString(),
        //            j = x.CreatedDate.ToString("yyyy-MM-dd HH:mm:ss"),
        //            k = x.Lang,
        //            l = x.ParentSN.ToString(),
        //            m = x.SourceTable,
        //        }).ToList();

        //        var countList = CountList.Select(x => new ExcelDetailModel()
        //        {
        //            a = x.Title,
        //            b = x.Webpath,
        //            c = x.DepartmentName,
        //            d = x.DepartmentID == null ? "" : Services.Authorization.DepartmentManagementService.GetDepartmentBySysDepartmentID(x.DepartmentID, websiteid).Where(u => u.Lang == x.Lang).FirstOrDefault()?.DepartmentName,
        //            e = x.StrDate.ToString("yyyy-MM-dd HH:mm:ss"),
        //            f = x.EndDate.ToString("yyyy-MM-dd HH:mm:ss"),
        //            g = x.InsertSUM.ToString(),
        //            h = x.UpdateSUM.ToString(),
        //            i = x.DeleteSUM.ToString(),
        //            j = x.SourceSN.ToString(),
        //            k = x.Lang
        //        }).ToList();
        //        var EModel2 = new ExcelModel();
        //        EModel2.DetailTitle = new List<string>() { "站台", "建立單位", "發布單位", "筆數" };

        //        EModel2.ExcelDetails = HelpOtherCountList.Select(x => new ExcelDetailModel()
        //        {
        //            a = x.WebSiteTitle,
        //            b = x.CreateDepName,
        //            c = x.ReleaseDepName,
        //            d = x.HelpOtherCount.ToString(),
        //        }).ToList();

        //        excelDatas = Utility.Output.SheetReport(filePath, EModel, dataList, countList, CreateSheet, WEBLevelData, WEBSite, EModel2);

        //        SetSession("file", Convert.ToBase64String(excelDatas.ToArray()));

        //        excelDatas.Dispose();

        //        return StatusResult(System.Net.HttpStatusCode.OK, "");
        //    }
        //    catch (Exception ex)
        //    {
        //        IList<string> errmsgs = new List<string>();
        //        string error = "";
        //        error = ex.Message;
        //        errmsgs.Add(error);

        //        SetSession("file", Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(error)));

        //        return StatusResult(System.Net.HttpStatusCode.BadRequest, "");
        //        //return File(System.Text.Encoding.UTF8.GetBytes(error), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{fileName}");
        //    }
        //}

        /// <summary>
        /// 匯出報表改版
        /// </summary>
        /// <param name="sd"></param>
        /// <param name="ed"></param>
        /// <param name="userid"></param>
        /// <param name="ip"></param>
        /// <param name="path"></param>
        /// <param name="websiteid"></param>
        /// <param name="key"></param>
        /// <param name="sn"></param>
        /// <param name="departmentID"></param>
        /// <returns></returns>
        public IActionResult Export2(string sd, string ed, string userid = "", string ip = "", string path = "", string websiteid = "", string key = "", string sn = "", string departmentID = "")
        {
            try
            {
                #region 資料撈取&設定
                string str = Convert.ToDateTime(sd).ToString("yyyy-MM-dd 00:00:00").ToString();
                string end = Convert.ToDateTime(ed).ToString("yyyy-MM-dd 23:59:59").ToString();
                var fpath = _hostingEnvironment.WebRootPath;
                var excelDatas = new MemoryStream();
                var dataList = OperationStatisticsService.GetLogData2(str, end, 3, userid, ip, path, websiteid, key, sn, departmentID);//更新資料
                var countList = OperationStatisticsService.GetLogData3(str, end, 0, userid, ip, path, websiteid, key, sn, departmentID);//統計數
                var helpOtherCountList = OperationStatisticsService.GetLogData4(str, end, websiteid);// 非本單位上稿統計
                var depData = Services.Authorization.DepartmentManagementService.GetDepartmentList();
                var filePath = $@"{fpath}/Temp/ExampleReport.xlsx";
                #endregion
                var Info = new List<string>() {
                $@"建表日期: {DateTime.UtcNow.AddHours(8).ToString("yyyy-MM-dd HH:mm")}",
                $@"建表人: {UserData?.sysUser.UserName}",
                $@"月份：{sd}至{ed}"
                };
                var excelModels = new List<ExcelModel>();
                var countExcelDetails = new List<ExcelDetailModel>();
                if (dataList?.Count > 0)
                {
                    var detailTitle = new List<string>() {
                "編號",
                "節點",
                "帳號",
                "使用者名稱",
                "上稿單位",
                "發布單位",
                "IP",
                "異動狀態",
                "異動紀錄",
                "完整路徑",
                "資料編號",
                "時間"
                };
                    foreach (var data in dataList.GroupBy(x => x.Lang).Select(g => g).ToList())
                    {
                        var langer = data.Key == "zh-tw" ? "中文" : "英文";
                        var title = @$"{data.First().WebSiteTitle}({langer})";
                        var excelModel = new ExcelModel();
                        excelModel.Info = Info;
                        excelModel.Title = title;
                        excelModel.SheetTitle = title;
                        excelModel.DetailTitle = detailTitle;
                        List<ExcelDetailModel> excelDetailModels = new List<ExcelDetailModel>();
                        var icount = 1;
                        foreach (var detail in data.OrderByDescending(x => x.SourceSN).ThenByDescending(x => x.CreatedDate))
                        {
                            var webpath = detail.Webpath;
                            var b = "";
                            if (!string.IsNullOrWhiteSpace(webpath))
                            {
                                var arrayWebpath = webpath.Split('/').Distinct();
                                webpath = string.Join("/", arrayWebpath);
                                b = webpath.Split(@"/")[webpath.Split(@"/").Length - 2];
                            }
                            var excelDetailModel = new ExcelDetailModel();
                            excelDetailModel.a = icount.ToString();
                            excelDetailModel.b = b;
                            excelDetailModel.c = detail.UserID;
                            excelDetailModel.d = detail.UserName;
                            excelDetailModel.e = detail.DepartmentName;
                            excelDetailModel.f = string.IsNullOrWhiteSpace(detail.DepartmentID) ? "" : depData.FirstOrDefault(d => d.DepartmentID == detail.DepartmentID)?.DepartmentName;
                            excelDetailModel.g = detail.ProcessIPAddress;
                            excelDetailModel.h = detail.Action2.ToUpper() == "UPDATE" ? "更新資料" : detail.Action2.ToUpper() == "RETURNED" ? "送審退回" : detail.Action2.ToUpper() == "DELETE" ? "刪除資料" : detail.Action2.ToUpper() == "SELECT" ? "查詢資料" : "新增資料";
                            try
                            {
                                var PreviousData = OperationStatisticsService.GetLogByPrevious(detail.CreatedDate.ToString("yyyy-MM-dd HH:mm:ss"), detail.SourceSN, detail.SourceTable);
                                if (PreviousData != null && PreviousData.Action2.ToUpper() != "DELETE" && detail.Action2.ToUpper() != "DELETE")
                                {
                                    var MSG = LogUtility.LogAnalyze(PreviousData.MessageInput, detail.MessageInput, detail.SourceTable, PreviousData.Action2, detail.Action2);
                                    excelDetailModel.i = (MSG != null && MSG.Count > 0) ? String.Join(", ", MSG) : "";
                                }
                                else
                                {
                                    excelDetailModel.i = "";
                                }
                            }
                            catch (Exception ex)
                            {
                                Utility.LogExpansion.Write("D:\\Log", ex.Message);
                                excelDetailModel.i = "";
                            }
                            excelDetailModel.j = webpath;
                            excelDetailModel.k = detail.SourceSN.ToString();
                            excelDetailModel.l = detail.CreatedDate.ToString("yyyy-MM-dd HH:mm:ss");
                            icount++;
                            excelDetailModels.Add(excelDetailModel);
                        }
                        var _icount = 1;
                        foreach (var detail in excelDetailModels.OrderBy(x => x.b).ThenBy(x => x.k).ThenByDescending(x => x.l))
                        {
                            detail.a = (_icount++).ToString();
                        }
                        excelModel.ExcelDetails = excelDetailModels.OrderBy(x => x.b).ThenBy(x => x.k).ThenByDescending(x => x.l).ToList();
                        excelModels.Add(excelModel);
                    }
                }
                if (countList?.Count > 0)
                {
                    var detailTitle = new List<string>() { "編號", "資料標題", "上稿單位", "發布單位", "資料建立時間", "最後更新時間", "新增", "修改", "刪除", "資料編號" };
                    var levelList = countList.Where(x => x.SourceTable.ToLower() == "weblevel").ToList();
                    var newsList = countList.Where(x => x.SourceTable.ToLower() == "webnews").ToList();
                    var title = "統計";
                    if (levelList?.Count > 0)
                    {
                        var excelModel = new ExcelModel();
                        excelModel.Info = Info;
                        excelModel.Title = $"{title}(節點)";
                        excelModel.SheetTitle = $"{title}(節點)";
                        excelModel.DetailTitle = detailTitle;
                        excelModel.ExcelDetails = GetStatisticsData(levelList, depData);
                        excelModels.Add(excelModel);
                    }
                    if (newsList?.Count > 0)
                    {
                        var excelModel = new ExcelModel();
                        excelModel.Info = Info;
                        excelModel.Title = $"{title}(新聞)";
                        excelModel.SheetTitle = $"{title}(新聞)";
                        excelModel.DetailTitle = detailTitle;
                        countExcelDetails = GetStatisticsData(newsList, depData);
                        excelModel.ExcelDetails = countExcelDetails;
                        excelModels.Add(excelModel);
                    }
                }
                if (helpOtherCountList?.Count > 0)
                {
                    var title = "代操作新增(新聞)";
                    var DetailTitle = new List<string>() { "站台", "建立單位", "發布單位", "筆數" };
                    var excelModel = new ExcelModel();
                    excelModel.Info = Info;
                    excelModel.Title = title;
                    excelModel.SheetTitle = title;
                    excelModel.DetailTitle = DetailTitle;
                    excelModel.ExcelDetails = helpOtherCountList.Select(x => new ExcelDetailModel()
                    {
                        a = x.WebSiteTitle,
                        b = x.CreateDepName,
                        c = x.ReleaseDepName,
                        d = x.HelpOtherCount.ToString(),
                    }).ToList();
                    excelModels.Add(excelModel);
                }
                if (countExcelDetails?.Count > 0)
                {
                    var data = countExcelDetails.Where(x => x.c != x.d && int.Parse(x.h) > 0).GroupBy(x => new { x.c, x.d }).Select(g => g).ToList();
                    if (data?.Count > 0)
                    {
                        var title = "代操作更新(新聞)";
                        var detailTitle = new List<string>() { "站台", "更新單位", "發布單位", "筆數" };
                        var excelModel = new ExcelModel();
                        excelModel.Info = Info;
                        excelModel.Title = title;
                        excelModel.SheetTitle = title;
                        excelModel.DetailTitle = detailTitle;
                        var countExcelDetails2 = new List<ExcelDetailModel>();
                        foreach (var d2 in data) {
                            countExcelDetails2.Add(new ExcelDetailModel() {
                             a= websiteid.ToLower(),
                             b = d2.Key.c,
                             c= d2.Key.d,
                             d= d2.Sum(x => int.Parse(x.h)).ToString()
                        });
                        }
                        excelModel.ExcelDetails = countExcelDetails2;
                        excelModels.Add(excelModel);
                    }
                }
                excelDatas = Utility.Output.ExampleReport(excelModels, filePath);
                SetSession("file", Convert.ToBase64String(excelDatas.ToArray()));
                excelDatas.Dispose();
                return StatusResult(System.Net.HttpStatusCode.OK, "");
            }
            catch (Exception ex)
            {
                IList<string> errmsgs = new List<string>();
                string error = "";
                error = ex.Message;
                errmsgs.Add(error);

                SetSession("file", Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(error)));

                return StatusResult(System.Net.HttpStatusCode.BadRequest, "");
                //return File(System.Text.Encoding.UTF8.GetBytes(error), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{fileName}");
            }
        }

        static List<ExcelDetailModel> GetStatisticsData(List<OperationStatisticsModel3> operationStatisticsModel3s , List<SysDepartment>  sysDepartments)
        {
            int i = 1;
            var countExcelDetails = new List<ExcelDetailModel>();
            countExcelDetails = operationStatisticsModel3s.Select(x => new ExcelDetailModel()
            {
                a = (i++).ToString(),
                b =  x.SourceTable.ToLower() == "webnews" ? x.Title  : x.Webpath.Split("/")[x.Webpath.Split("/").Length-1] ,
                c = x.DepartmentName,
                d = String.IsNullOrWhiteSpace(x.DepartmentID) ? "" : sysDepartments.FirstOrDefault(d => d.DepartmentID == x.DepartmentID)?.DepartmentName,
                e = x.StrDate.ToString("yyyy-MM-dd HH:mm"),
                f = x.EndDate.ToString("yyyy-MM-dd HH:mm"),
                g = x.InsertSUM.ToString(),
                h = x.UpdateSUM.ToString(),
                i = x.DeleteSUM.ToString(),
                j = x.SourceSN.ToString()
            }).ToList();
            return countExcelDetails;
        }

        /// <summary>
        /// 下載
        /// </summary>
        /// <returns></returns>
        public IActionResult Download()
        {
            byte[] data = Convert.FromBase64String(GetSession<string>("file"));
            var fileName = $@"網站稽核紀錄{DateTime.UtcNow.AddHours(8):yyyyMMdd}.xlsx";
            SetSession("file", null);
            return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{fileName}");
        }
    }
}
