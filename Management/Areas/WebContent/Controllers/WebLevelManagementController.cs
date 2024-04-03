using DBModel;
using Management.Areas.WebContent.Models.WebLevelManagement;
using Management.ManagementUtility;
using Management.Models.Common;
using Microsoft.AspNetCore.Mvc;
using NPOI.POIFS.Properties;
using Services;
using Services.Authorization;
using Services.SystemManageMent;
using Services.WebManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using Utility;
using static Services.Youtube.YoutubeService;
using static Utility.Files;

namespace Management.Areas.WebContent
{
    [Area("WebContent")]
    public class WebLevelManagementController : BaseController
    {
        public IActionResult Index(string key = "")
        {
            var WeblevelType = Utility.EnumWeblevelType.WebLevelManagment;
            var Breadcrumb = "";
            var IsOpenASEKey = CodeManagementService.GetCategoryByCategoryKey("Management-5-1", "zh-tw");
            if (IsOpenASEKey?.Value == "1") {
                switch (CommonUtility.AesDecrypt(key))
                {
                    case "2": WeblevelType = Utility.EnumWeblevelType.WebHomeManagment; Breadcrumb = CommonUtility.Breadcrumb(7); break;
                    case "3": WeblevelType = Utility.EnumWeblevelType.WebHeaderFooterManagment; Breadcrumb = CommonUtility.Breadcrumb(19); break;
                    default: WeblevelType = Utility.EnumWeblevelType.WebLevelManagment; Breadcrumb = CommonUtility.Breadcrumb(6); break;
                }
            } else {
                switch (key)
                {
                    case "2": WeblevelType = Utility.EnumWeblevelType.WebHomeManagment; Breadcrumb = CommonUtility.Breadcrumb(7); break;
                    case "3": WeblevelType = Utility.EnumWeblevelType.WebHeaderFooterManagment; Breadcrumb = CommonUtility.Breadcrumb(19); break;
                    default: WeblevelType = Utility.EnumWeblevelType.WebLevelManagment; Breadcrumb = CommonUtility.Breadcrumb(6); break;
                }
            }
            
            ViewData["Breadcrumb"] = Breadcrumb;
            SetSession("Breadcrumb", Breadcrumb);
            var model = new IndexModel();
            model.levelForTrees = WebLevelManagementService.GetWebLevelList(WeblevelType, UserData.WebSiteID);
            model.GodMode = UserData.GodMode;
            model.authSysGroupWebLevels = UserData.webLevelAccessForGroups;
            model.WEBCP = WebLevelManagementService.GetWebNewList(model.levelForTrees.Select(X => X.WebLevelSN).ToList(), "CP");
            return View(model);
        }

        /// <summary>
        /// 編輯
        /// </summary>
        /// <param name="key">WebLevelDSN</param>
        /// <param name="lang"></param>
        /// <returns></returns>
        public IActionResult Module(string key, string lang = "", string websiteid = "")
        {

            var moduleModel = new ModuleModel();
            if (CommonUtility.UrlKey(ref key))
            {

                //初始化 之後檔案都這邊讀取
                SetSession("WEBFile", new List<CommonFileModel>());
                var mainKey = key;
                var moreLan = WebLevelManagementService.GetWebLevelByMainSN(int.Parse(mainKey), websiteid);
                var moduleViewData = new List<ModuleViewModel>();
                var fileData = new List<CommonFileModel>();
                var mainData = WebLevelManagementService.GetWebLevel(int.Parse(mainKey));
                var levelMenu = Utility.WebLevelModule.GetWebLevelMenu();
                var levelMenu2 = Utility.WebLevelModule.GetPageListModel();
                var levelMenu3 = mainData.WeblevelType == "1" ? Utility.WebLevelModule.GetList(EnumWeblevelType.WebLevelManagment) : Utility.WebLevelModule.GetList(EnumWeblevelType.WebHomeManagment);
                var levelMenu4 = WebLevelManagementService.GetWebLevelList(EnumWeblevelType.WebHomeManagment, "MODA");
                var template = Utility.WebLevelModule.GetTemplate(); //版型模組
                var levelMenu5 = Utility.WebLevelModule.GetTemplateList();//版型
                var Condition = Utility.WebLevelModule.Condition();//查詢
                var levelMenuStirng = "";
                var typeName = "";
                var module = Utility.EnumTpye.GetEnum<Utility.EnumWebLevelModuleLevel1>(mainData.Module);
                module = (module == Utility.EnumWebLevelModuleLevel1.error) ? EnumWebLevelModuleLevel1.NEWS : module;
                levelMenuStirng = Utility.EnumTpye.GetEnumName(module);
                typeName = Utility.EnumTpye.GetEnumNumberToSting(module);
                var sysCategories = new List<WebLevel>();
                #region div客製化 是否顯示
                var divClassName = new DivClassName();
                if (levelMenuStirng == Utility.EnumTpye.GetEnumName(Utility.EnumWebLevelModuleLevel1.CP)
                 || mainData.WeblevelType != Utility.EnumTpye.GetEnumNumberToSting(Utility.EnumWeblevelType.WebLevelManagment)
                 || mainData.Module == Utility.EnumTpye.GetEnumName(Utility.EnumWebLevelModuleLevel2.RSS)
                 || mainData.Module == Utility.EnumTpye.GetEnumName(Utility.EnumWebLevelModuleLevel2.DEPT)
                 )
                {
                    divClassName.div_LevelMenu2 = "d-none";
                }
                if ((mainData.Module == Utility.EnumTpye.GetEnumName(Utility.EnumWebLevelModuleLevel2.NEWS)
                    && mainData.WeblevelType == Utility.EnumTpye.GetEnumNumberToSting(Utility.EnumWeblevelType.WebLevelManagment))) { }
                else
                {
                    divClassName.div_Search = "d-none";
                }
                var chk = Services.Authorization.WebLevelManagementService.GetWebLevelByWebLevelSN(mainData.MainSN, 2, "");
                if (chk?.Count == 0)
                {
                    divClassName.div_LevelMenu5 = "d-none";
                }
                if (mainData.Module == Utility.EnumTpye.GetEnumName(EnumWebLevelModuleLevel2.DEPT)) { }
                else
                {
                    divClassName.div_DEPT = "d-none";
                }
                if (mainData.WeblevelType != EnumTpye.GetEnumNumberToSting(EnumWeblevelType.WebLevelManagment))
                {
                    divClassName.div_FatFooter = "d-none";
                    divClassName.div_MainMenu = "d-none";
                    divClassName.div_LeftMenuShow = "d-none";
                }
                if (((mainData.Module == Utility.EnumTpye.GetEnumName(Utility.EnumWebLevelModuleLevel2.NEWS) || mainData.Module == Utility.EnumTpye.GetEnumName(Utility.EnumWebLevelModuleLevel2.Schedule))
                    && mainData.WeblevelType == EnumTpye.GetEnumNumberToSting(EnumWeblevelType.WebLevelManagment))) { }
                else
                {
                    divClassName.div_RSS = "d-none";
                }
                if (mainData.Module == Utility.EnumTpye.GetEnumName(Utility.EnumWebLevelModuleLevel2.NEWS)) { }
                else
                {
                    divClassName.div_Sort = "d-none";
                    divClassName.div_custom = "d-none";
                }
                if (mainData.WeblevelType == EnumTpye.GetEnumNumberToSting(EnumWeblevelType.WebLevelManagment) && (mainData.Module != Utility.EnumTpye.GetEnumName(Utility.EnumWebLevelModuleLevel2.CP))){ } else {
                    divClassName.div_SEO = "d-none";
                }
                #endregion
                if (module == EnumWebLevelModuleLevel1.DEPT)
                {
                    sysCategories = WebLevelManagementService.GetWebLevelList(EnumWeblevelType.WebHomeManagment, "MODA")
                        .Where(x => x.Lang.Contains("zh-tw")
                        && x.Module == EnumTpye.GetEnumName(Utility.EnumWebLevelModuleLevel2.PAGELIST)
                        ).ToList();
                }
                if (moreLan != null)
                {
                    foreach (var singe in moreLan)
                    {
                        var files = CommonUtility.GetFileByDB(singe.WebLevelSN.ToString(), "WebLevel");
                        if (files != null)
                        {
                            foreach (var file in files)
                            {
                                file.lan = singe.Lang;
                            }
                            fileData.AddRange(files);
                            moduleModel.commonFileModels.AddRange(files);
                        }

                        moduleViewData.Add(new ModuleViewModel()
                        {
                            mainWebLevel = mainData,
                            webLevelData = singe,
                            commonFileModels = files,
                            LevelMenu = levelMenu,
                            LevelMenu2 = levelMenu2,
                            LevelMenu3 = levelMenu3,
                            LevelMenu5 = levelMenu5,
                            levelMenuString = levelMenuStirng,
                            typeNameString = typeName,
                            Condition = Condition,
                            sysUserSysDepartmentID = UserData.sysUser.DepartmentID,
                            DivClassName = divClassName,
                            DepsysCategories = sysCategories,
                        });
                    }
                    SetSession("WEBFile", fileData);
                }
                moduleModel.webLevelSN = key;
                moduleModel.webLevelDatas = moduleViewData;
                moduleModel.sysWebSiteLangs = moreLan.Count == 2 ? WebLevelManagementService.GetSysWebSiteLangs(UserData, "Module", int.Parse(key)) : WebLevelManagementService.GetSysWebSiteLangs(UserData, "Module", int.Parse(key)).Where(x => x.Lang != "en").ToList();
                moduleModel.mainLevelData = mainData;
                moduleModel.levelMenu2 = levelMenu2;
                moduleModel.levelMenu3 = levelMenu3;
                moduleModel.template = template;
                moduleModel.levelMenu5 = levelMenu5;
                moduleModel.Condition = Condition;
                //moduleModel.Customize = Customizes;
                moduleModel.LevelBreadcrumb = CommonService.LevelBreadcrumb(int.Parse(moduleModel.webLevelSN));
            }
            return View(moduleModel);
        }
        /// <summary>
        /// 修改WebLevel
        /// </summary>
        /// <param name="data">WebLevel DATA</param>
        /// <param name="module">WebLevelM.Module</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditModule(WebLevel data, string module, List<SelectTxt> customize, List<CommonFileModel> fileinfo = null)
        {
            try
            {
                var webLevelM = WebLevelManagementService.GetWebLevel(data.WebLevelSN);
                SetLogActionModel(webPath: "網站維護/" + OperationStatisticsService.GetWebLevelTree(webLevelM.ParentSN).FirstOrDefault()?.Path, Action2: Utility.Model.LoginModel.Action2.update, SourceTable: "WebLevel", SourceSN: data.WebLevelSN);
                Dictionary<int, string> dtcustomize = new Dictionary<int, string>();//分類
                var files = GetSession<List<CommonFileModel>>("WEBFile");
                fileinfo = fileinfo.Where(x => x.lan == data.Lang).ToList();
                files = files.Where(x => x.lan == data.Lang).ToList();
                if (fileinfo != null && files != null)
                {
                    foreach (var file in files)
                    {
                        var fileModel = fileinfo.FirstOrDefault(x => x.fileNewName == file.fileNewName);
                        if (fileModel != null)
                        {
                            file.fileTitle = fileModel.fileTitle;
                            file.FileSort = fileModel.FileSort;
                        }
                    }
                }
                data.WebSiteID = UserData.WebSiteID;
                data.ProcessUserID = UserData.sysUser.UserID;
                data.ProcessIPAddress = ContextModel.ProcessIpaddress;
                data.CreatedDate = DateTime.UtcNow.AddHours(8);
                data.ProcessDate = DateTime.UtcNow.AddHours(8);
                data.Title = data.Title == null ? "" : data.Title.Trim();
                data.Module = module.Trim();
                if (customize.Count > 0)
                {
                    var i = 0;
                    foreach (var dt in customize)
                    {
                        dtcustomize.Add((dt.val == null ? 99999 + i : int.Parse(dt.val)), dt.txt);
                        i++;
                    }
                }

                data.WeblevelType = webLevelM.WeblevelType;

                List<string> ckEditorurl = new List<string>();

                var checkMissData = Services.CheckModel.CheckedData.CheckWebLevelData(ref data, Utility.SysConst.Action.Edit, ref ckEditorurl);
                if (!checkMissData.chk)
                {
                    return StatusResult(System.Net.HttpStatusCode.InternalServerError, checkMissData.error);
                }
                //頁首頁尾Logo判斷
                if (webLevelM.WeblevelType == Utility.EnumTpye.GetEnumNumberToSting(EnumWeblevelType.WebHeaderFooterManagment) && (webLevelM.WebLevelKey == "footer" || webLevelM.WebLevelKey == "header"))
                {
                    var _title = "";
                    if (webLevelM.WebLevelKey == "header")
                        _title = "頁首Logo";
                    else
                        _title = "頁尾Logo";

                    if (files == null || files.Where(x => x.GroupID == "MSLI").Count() != 2)
                    {
                        return StatusResult(System.Net.HttpStatusCode.InternalServerError, $"請於｛{_title}｝上傳深、淺色共兩張圖檔");
                    }
                }

                string msg = "";
                var chkCustomizeTag = WebLevelManagementService.EditWebLevelCustomizeTag(data, dtcustomize, ref msg);
                if (!chkCustomizeTag)
                {
                    return StatusResult(System.Net.HttpStatusCode.InternalServerError, msg);
                }

                var chk = WebLevelManagementService.EditModule(data, module, files);
                WebLevelManagementService.SaveWebCntLink("WebLevel", data.WebLevelSN, ckEditorurl);
                logActionModel.SourceSN = data.WebLevelSN;
                return StatusResult(System.Net.HttpStatusCode.OK, "");
            }
            catch (Exception ex)
            {
                logActionModel.status = Utility.Model.LoginModel.Status.Error;
                logActionModel.response = ex.ToString();
                Log(logActionModel);
                return StatusResult(System.Net.HttpStatusCode.BadRequest, Utility.Model.LoginModel.ServieError);
            }
        }

        /// <summary>
        /// 子節點列表
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IActionResult ChildNodeList(string sorttitle, string sorttype, string key, int p = 1, int DisplayCount = 10)
        {
            if (CommonUtility.UrlKey(ref key) && int.TryParse(key, out int _key))
            {

                var WebLevelData = WebLevelManagementService.GetWebLevel(_key);
                var sortData = new List<int>();
                var model = new ChildNodeListModel();
                if (!string.IsNullOrWhiteSpace(sorttitle) && sorttitle != "undefined") model.sorttitle = sorttitle;
                if (!string.IsNullOrWhiteSpace(sorttype) && sorttype != "undefined") model.sorttype = sorttype;
                DefaultPager pager = new DefaultPager();
                pager.DisplayCount = DisplayCount;
                pager.p = p;
                model.ParentLevel = WebLevelData;
                model.webLevels = WebLevelManagementService.GetWebLevelByParentSN(model.sorttitle, model.sorttype, _key, ref sortData, ref pager);
                model.sortData = sortData;
                model.SortList = WebLevelManagementService.GetWebLevelByParentSN(_key);
                model.defaultPager = pager;
                model.LevelBreadcrumb = CommonService.LevelBreadcrumb(_key);
                model.CheckYoutubeApiKey = WebLevelManagementService.CheckYouTouApiKey(WebLevelData.MainSN.Value);
                return View(model);
            }

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GetYouTubeApiData()
        {
            try
            {
                Services.Youtube.YoutubeService.GetPlayListApiData(AppSettingHelper.GetAppsetting("DemoDNS"), out string msg, out string error, out List<OutYoutubeMgs> outYoutubeMgsList);
                return StatusResult(System.Net.HttpStatusCode.OK, msg);
            }
            catch (Exception ex)
            {
                logActionModel.status = Utility.Model.LoginModel.Status.Error;
                logActionModel.response = ex.ToString();
                Log(logActionModel);
                return StatusResult(System.Net.HttpStatusCode.BadRequest, "系統發生錯誤請聯絡工程師");
            }
        }
        #region 節點 新增/修改/刪除
        /// <summary>
        /// CreateArticle
        /// </summary>
        /// <returns></returns>
        public IActionResult CreateArticle(string key)
        {

            if (CommonUtility.UrlKey(ref key))
            {
                var webLevelM = WebLevelManagementService.GetWebLevel(int.Parse(key));
                CreateArticleModel createArticleModel = new CreateArticleModel();
                createArticleModel.WebLevelSN = key;
                createArticleModel.WeblevelType = webLevelM.WeblevelType;
                createArticleModel.LevelMenu = Utility.WebLevelModule.GetWebLevelMenu();
                createArticleModel.LevelMenu2 = Utility.WebLevelModule.GetPageListModel();
                createArticleModel.Condition = Utility.WebLevelModule.Condition();
                createArticleModel.sysUserSysDepartmentID = UserData.sysUser.DepartmentID;
                var LevelMenu3 = (webLevelM.WeblevelType == Utility.EnumTpye.GetEnumNumberToSting(EnumWeblevelType.WebLevelManagment)) ? Utility.WebLevelModule.GetList(EnumWeblevelType.WebLevelManagment) : Utility.WebLevelModule.GetList(EnumWeblevelType.WebHomeManagment);
                createArticleModel.LevelMenu3 = LevelMenu3;
                createArticleModel.LevelBreadcrumb = CommonService.LevelBreadcrumb(int.Parse(key));
                return View(createArticleModel);
            }
            return View();
        }
        /// <summary>
        /// 新增CreateArticle
        /// </summary>
        /// <param name="titile"></param>
        /// <param name="key"></param>
        /// <param name="weblevelType"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateArticle(string title, string key, string weblevelType, string listType, string WebLevelKey, string Condition, string DepartmentID)
        {
            try
            {
                SetLogActionModel(Action2: Utility.Model.LoginModel.Action2.insert, SourceTable: "WebLevel");

                //參數null設為空白
                if (string.IsNullOrWhiteSpace(title)) title = "";
                if (string.IsNullOrWhiteSpace(key)) key = "";
                if (string.IsNullOrWhiteSpace(weblevelType)) weblevelType = "";
                if (string.IsNullOrWhiteSpace(listType)) listType = "";
                key = System.Web.HttpUtility.HtmlDecode(key);
                if (CommonUtility.UrlKey(ref key))
                {
                    var ParentwebLevel = WebLevelManagementService.GetWebLevel(int.Parse(key));
                    logActionModel.webPath = "網站維護/" + OperationStatisticsService.GetWebLevelTree(ParentwebLevel.WebLevelSN).FirstOrDefault()?.Path;
                    var sysWebSitesLan = CommonService.GetSysWebSiteLang(UserData.WebSiteID);
                    var webLevelModel = new WebLevel()
                    {
                        WebSiteID = ParentwebLevel.WebSiteID,
                        WeblevelType = ParentwebLevel.WeblevelType,
                        Title = title.Trim(),
                        CreatedDate = DateTime.UtcNow.AddHours(8),
                        CreatedUserID = UserData.sysUser.UserID,
                        Module = weblevelType.Trim(),
                        ParentSN = ParentwebLevel.WebLevelSN,
                        SortOrder = 99,
                        ProcessDate = DateTime.UtcNow.AddHours(8),
                        ProcessIPAddress = ContextModel.ProcessIpaddress,
                        ProcessUserID = UserData.sysUser.UserID,
                        ListType = listType,
                        WebLevelKey = string.IsNullOrWhiteSpace(WebLevelKey) ? "" : WebLevelKey.Trim().ToLower(),
                        IsEnable = "0",
                        Condition = Condition,
                        DepartmentID = DepartmentID,
                    };

                    List<string> ckEditorurl = new List<string>();
                    var checkMissData = Services.CheckModel.CheckedData.CheckWebLevelData(ref webLevelModel, Utility.SysConst.Action.Add, ref ckEditorurl);
                    if (!checkMissData.chk)
                    {
                        return StatusResult(System.Net.HttpStatusCode.InternalServerError, checkMissData.error);
                    }
                    var chkWebLevelKey = WebLevelManagementService.CheckWebLevelKey(webLevelModel);
                    if (!chkWebLevelKey)
                    {
                        return StatusResult(System.Net.HttpStatusCode.InternalServerError, "節點代號重複，請重新輸入");
                    }
                    var webLevelMSN = WebLevelManagementService.InsertWebLevel(webLevelModel, sysWebSitesLan);


                    logActionModel.SourceSN = webLevelMSN;
                    //Log(logActionModel);
                    return StatusResult(System.Net.HttpStatusCode.OK, CommonUtility.GetUrlAesEncrypt(webLevelMSN.ToString()));
                }
                return StatusResult(System.Net.HttpStatusCode.OK, "");
            }
            catch (Exception ex)
            {
                logActionModel.status = Utility.Model.LoginModel.Status.Error;
                logActionModel.response = ex.ToString();
                Log(logActionModel);
                return StatusResult(System.Net.HttpStatusCode.BadRequest, Utility.Model.LoginModel.ServieError);
            }
        }

        /// <summary>
        /// NEW列表
        /// </summary>
        /// <param name="key">WebLevelSN</param>
        /// <returns></returns>
        public IActionResult Article(string key)
        {

            if (CommonUtility.UrlKey(ref key))
            {
                SetSession("MD_Datazh-tw", null);
                SetSession("MD_Dataen", null);
                ArticleModel article = new ArticleModel();
                var webLevelM = WebLevelManagementService.GetWebLevel(int.Parse(key));

               
                article.WebLevel = webLevelM;
                article.IsEnable = Utility.SysConst.IsEnable.Items(Utility.SysConst.IsEnable.BackendQryIsEnableItem);
                article.SysDepartments = Services.CommonService.GetDepartments();
                article.LevelBreadcrumb = CommonService.LevelBreadcrumb(int.Parse(key));
                switch (webLevelM.Module)
                {
                    case "CP": break;
                    default: return View(article);
                }
                return View(article);
            }

            return View();
        }

        /// <summary>
        /// ArticleList列表
        /// </summary>
        /// <param name="key"></param>
        /// <param name="keyword"></param>
        /// <param name="str"></param>
        /// <param name="end"></param>
        /// <param name="sts"></param>
        /// <param name="p"></param>
        /// <param name="DisplayCount"></param>
        /// <returns></returns>
        public IActionResult ArticleList(string sorttitle, string sorttype, string key = "", string keyword = "", string str = "", string end = "", string sts = "", string dep = "", string lan = "", int p = 1, int DisplayCount = 10)
        {
            var _key = 0;
            if (int.TryParse(key, out _key))
            {
                Models.ArticleListModel articleListModel = new();
                if (!string.IsNullOrWhiteSpace(sorttitle)) articleListModel.sorttitle = sorttitle;
                if (!string.IsNullOrWhiteSpace(sorttype)) articleListModel.sorttype = sorttype;
                List<WEBNews> lists = new List<WEBNews>();
                DefaultPager pager = new DefaultPager();
                pager.DisplayCount = DisplayCount;
                pager.p = p;
                var webLevelM = WebLevelManagementService.GetWebLevel(_key);
                var list = WebLevelManagementService.GetWebNews(articleListModel.sorttitle, articleListModel.sorttype, keyword, str, end, webLevelM, sts, dep, lan, ref pager);
                var sort = WebLevelManagementService.GetWebNews(webLevelM, lan);
                var IsTop = WebLevelManagementService.GetWebNewsForIsTop(webLevelM, lan);

                articleListModel.Module = webLevelM.Module;
                articleListModel.SortMethod = webLevelM.SortMethod;
                articleListModel.defaultPager = pager;
                articleListModel.wEBNews = list;
                articleListModel.SortList = sort;
                articleListModel.SortIstop = IsTop;
                articleListModel.AuthSysGroupWebLevels = UserData.webLevelAccessForGroups;
                articleListModel.logActions = LogService.GetAction(UserData.sysUser.UserID, "returned", _key).ToList();
                return View(articleListModel);
            }
            return View(null);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteAericle(string key)
        {
            try
            {
                SetLogActionModel(Action2: Utility.Model.LoginModel.Action2.delete, SourceTable: "WebLevel");

                if (CommonUtility.UrlKey(ref key) && int.TryParse(key, out int _key))
                {
                    logActionModel.webPath = "網站維護/" + OperationStatisticsService.GetWebLevelTree(_key).FirstOrDefault()?.Path;
                    logActionModel.SourceSN = _key;
                    WebLevelManagementService.DeleteAericle(_key, UserData.sysUser.UserID, UserData.sysUser.ProcessIPAddress);
                    //Log(logActionModel);
                    return StatusResult(System.Net.HttpStatusCode.OK, "");
                }
                else
                {
                    logActionModel.status = Utility.Model.LoginModel.Status.Error;
                    logActionModel.response = "請別亂輸入測試";
                    Log(logActionModel);
                    return StatusResult(System.Net.HttpStatusCode.BadRequest, "請別亂輸入測試");
                }
            }
            catch (Exception ex)
            {
                logActionModel.status = Utility.Model.LoginModel.Status.Error;
                logActionModel.response = ex.ToString();
                Log(logActionModel);
                return StatusResult(System.Net.HttpStatusCode.BadRequest, Utility.Model.LoginModel.ServieError);
            }
        }

        /// <summary>
        /// WebLevel子節點重新排序
        /// </summary>
        /// <param name="key">子節點WebLevelSN</param>
        /// <param name="sort">欲調整至序號</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ReArrange(string key, string sort)
        {
            if (CommonUtility.UrlKey(ref key) && int.TryParse(key, out int _key) && int.TryParse(sort, out int _sort))
            {
                WebLevelManagementService.ReArrangeByChild(_key, _sort, UserData.sysUser.UserID, UserData.sysUser.ProcessIPAddress);
                Log(logActionModel);
                return StatusResult(System.Net.HttpStatusCode.OK, "");
            }
            else
            {
                logActionModel.status = Utility.Model.LoginModel.Status.Error;
                logActionModel.response = "請別亂輸入測試";
                Log(logActionModel);
                return StatusResult(System.Net.HttpStatusCode.BadRequest, "請別亂輸入測試");
            }
        }

        #endregion
        #region Auth 權限
        /// <summary>
        /// 權限設定
        /// </summary>
        /// <returns></returns>
        public IActionResult Auth(string key, string lang = "")
        {
            var authModel = new AuthModel();
            if (CommonUtility.UrlKey(ref key))
            {
                authModel.sysGroups = WebLevelManagementService.GetNotOwnerSysGroups();
                authModel.webLevelSN = key;
                authModel.authSysGroupWebLevels = WebLevelManagementService.GetNotOwnerLevelAccessForGroupList(int.Parse(key));
                authModel.LevelBreadcrumb = CommonService.LevelBreadcrumb(int.Parse(key));
            }
            return View(authModel);
        }

        public IActionResult AuthList(string key, string q , int p = 1, int DisplayCount = 10)
        {
            var authListModel = new AuthListModel();
            if (CommonUtility.UrlKey(ref key))
            {
                DefaultPager pager = new DefaultPager();
                pager.DisplayCount = DisplayCount;
                pager.p = p;
                authListModel.sysGroups = WebLevelManagementService.GetNotOwnerSysGroups(q, ref pager);
                authListModel.authSysGroupWebLevels = WebLevelManagementService.GetNotOwnerLevelAccessForGroupList(int.Parse(key));
                authListModel.webLevelSN = key;
                authListModel.defaultPager = pager;
            }
            return View(authListModel);
        }

        /// <summary>
        /// 更新資料
        /// </summary>
        /// <param name="chk"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditAuth(bool chk, AuthSysGroupWebLevel data)
        {
            try
            {
                SetLogActionModel(webPath: "網站維護/" + OperationStatisticsService.GetWebLevelTree(data.WebLevelSN).FirstOrDefault()?.Path, Action2: Utility.Model.LoginModel.Action2.update, SourceTable: "AuthSysGroupWebLevel", SourceSN: data.AuthSysGroupWebLevelSN);

                data.CreatedUserID = UserData.sysUser.UserID;
                data.WebSiteID = UserData.WebSiteID;
                if (WebLevelManagementService.EditAuthSysGroupWebLevel(chk, data))
                {
                    //Log(logActionModel);
                    return StatusResult(System.Net.HttpStatusCode.OK, "");
                }
                else
                {
                    logActionModel.status = Utility.Model.LoginModel.Status.Error;
                    Log(logActionModel);
                    return StatusResult(System.Net.HttpStatusCode.BadRequest, "更新失敗");
                }
            }
            catch (Exception ex)
            {
                logActionModel.status = Utility.Model.LoginModel.Status.Error;
                logActionModel.response = ex.ToString();
                Log(logActionModel);
                return StatusResult(System.Net.HttpStatusCode.BadRequest, Utility.Model.LoginModel.ServieError);
            }
        }
        #endregion
        /// <summary>
        /// LINK
        /// </summary>
        /// <param name="key">WebLevelMSN</param>
        /// <param name="key2">WEBNewsSN</param>
        /// <returns></returns>
        public IActionResult NewsType(string key = "", string key2 = "", string key3 = "", string lan = "")
        {
            return RedirectToAction("Index", new { area = "WebContent", Controller = key3, key = key, key2 = key2, lan = lan });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangeTree(int mainSN, int parentSN)
        {
            try
            {
                WebLevelManagementService.MoveTree(parentSN, mainSN);
                return StatusResult(System.Net.HttpStatusCode.OK, "");
            }
            catch (Exception ex)
            {
                return StatusResult(System.Net.HttpStatusCode.BadRequest, "發生異常");
            }


        }

        /// <summary>
        /// 重新刷新tree
        /// </summary>
        /// <param name="key">ParentSN</param>
        /// <returns></returns>
        public IActionResult RefWebLevelTree(string key)
        {
            var _key = 0;
            CommonUtility.UrlKey(ref key);
            _key = int.Parse(key);

            var model = new IndexModel();
            var parentWeblevel = WebLevelManagementService.GetWebLevel(_key);
            model.levelForTrees = WebLevelManagementService.GetWebLevelList(Utility.EnumTpye.GetEnum<Utility.EnumWeblevelType>(parentWeblevel.WeblevelType), UserData.WebSiteID);
            model.GodMode = UserData.GodMode;
            model.authSysGroupWebLevels = UserData.webLevelAccessForGroups;
            var parrntSN = new List<int>();
            parrntSN.Add(_key);
            RefParentSN(model.levelForTrees, ref parrntSN, _key);
            model.ParentSNOnList = parrntSN;
            model.WEBCP = WebLevelManagementService.GetWebNewList(model.levelForTrees.Select(X => X.WebLevelSN).ToList(), "CP");
            return View("~/Areas/WebContent/Views/WebLevelManagement/WebLevelTree.cshtml", model);
        }

        private void RefParentSN(List<WebLevel> webLevels,  ref List<int> parrntSNList , int key)
        {
            var webLevel = webLevels.FirstOrDefault(x => x.WebLevelSN == key);
            if (webLevel != null)
            {
                parrntSNList.Add(webLevel.ParentSN);
                RefParentSN(webLevels, ref parrntSNList, webLevel.ParentSN);
            }
        }
    }
}
