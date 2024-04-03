using DBModel;
using Newtonsoft.Json;
using NPOI.POIFS.Crypt.Dsig;
using Services.Authorization;
using Services.Models;
using Services.Models.MailBox;
using Services.Models.ModaMailBox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Security.AccessControl;
using Utility;
using Utility.MailBox;
using static Utility.CommFun2.Status;
using static Utility.Files;
using static Utility.MailBox.Api;

namespace Services.ModaMailBox
{
    public class MailBoxService
    {
        /// <summary>
        /// 取得站台基本設定
        /// </summary>
        /// <returns></returns>
        public static CaseApplyWeb GetCaseApplyWeb()
        {
            using (var db = new Services.MODAContext())
            {
                try
                {
                    return db.CaseApplyWeb.FirstOrDefault();
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 修改網站基本資料
        /// </summary>
        /// <param name="caseApply"></param>
        /// <param name="commonFileModels"></param>
        /// <returns></returns>
        public static bool EditGetApplyWeb(CaseApplyWeb caseApply, List<CommonFileModel> commonFileModels = null)
        {
            try
            {
                using (var db = new Services.MODAContext())
                {
                    var oldData = db.CaseApplyWeb.FirstOrDefault();
                    oldData.Title = caseApply.Title;
                    oldData.SEODescription = caseApply.SEODescription;
                    oldData.SEOKeywords = caseApply.SEOKeywords;
                    oldData.GACode = caseApply.GACode;
                    oldData.Satisfaction = caseApply.Satisfaction;
                    oldData.Footer = caseApply.Footer;
                    oldData.ProcessDate = caseApply.ProcessDate;
                    oldData.ProcessIPAddress = caseApply.ProcessIPAddress;
                    oldData.ProcessUserID = caseApply.ProcessUserID;
                    db.SaveChanges();
                    if (commonFileModels?.Count > 0)
                    {
                        var nowNewsFileName = commonFileModels.Select(x => x.fileNewName).ToList();

                        var DBAllFile = (from a in db.RelWebFileContent
                                         join b in db.WEBFile on a.WEBFileSN equals b.WEBFileSN
                                         where a.SourceTable == "CaseApplyWeb" &&
                                               a.SourceSN == oldData.CaseApplyWebSN
                                         select b).ToList();

                        var DBFileName = DBAllFile.Select(y => y.FileName).ToList();
                        //刪除的檔案
                        var NeedDeleteFiles = DBAllFile.Where(x => !nowNewsFileName.Contains(x.FileName)).ToList();
                        //新的檔案
                        var NewFiles = commonFileModels.Where(x => !DBFileName.Contains(x.fileNewName)).ToList();
                        // 刪除 先壓狀態
                        if (NeedDeleteFiles != null)
                        {
                            foreach (var file in NeedDeleteFiles)
                            {
                                file.IsEnable = "0";
                                var RelWebFileContentData = db.RelWebFileContent.First(x => x.WEBFileSN == file.WEBFileSN);
                                db.WEBFile.Update(file);
                                db.RelWebFileContent.Remove(RelWebFileContentData);
                                db.SaveChanges();
                            }
                        }
                        //新增
                        if (NewFiles != null)
                        {
                            foreach (var file in NewFiles)
                            {
                                var FileData = db.WEBFile.First(x => x.FileName == file.fileNewName);
                                var RelWebFileContentData = new RelWebFileContent()
                                {
                                    WEBFileSN = FileData.WEBFileSN,
                                    SourceTable = "CaseApplyWeb",
                                    SourceSN = oldData.CaseApplyWebSN,
                                    GroupID = file.GroupID.ToString(),
                                    CreatedUserID = oldData.ProcessUserID,
                                    CreatedDate = DateTime.UtcNow.AddHours(8),
                                    SortOrder = file.FileSort,
                                };
                                FileData.IsEnable = "1";
                                db.WEBFile.Update(FileData);
                                db.RelWebFileContent.Add(RelWebFileContentData);
                                db.SaveChanges();
                            }
                        }
                        //修改fileTitle
                        foreach (var file in commonFileModels)
                        {
                            var filedata = db.WEBFile.First(x => x.FileName == file.fileNewName);
                            filedata.FileTitle = file.fileTitle;
                            var RelWebFileContent = db.RelWebFileContent.FirstOrDefault(x => x.WEBFileSN == filedata.WEBFileSN);
                            RelWebFileContent.SortOrder = file.FileSort;

                            db.WEBFile.Update(filedata);
                            db.RelWebFileContent.Update(RelWebFileContent);
                            db.SaveChanges();

                        }
                    }
                    else
                    {
                        var DBAllFile = (from a in db.RelWebFileContent
                                         join b in db.WEBFile on a.WEBFileSN equals b.WEBFileSN
                                         where a.SourceTable == "CaseApplyWeb" &&
                                               a.SourceSN == oldData.CaseApplyWebSN
                                         select b).ToList();
                        // 刪除 先壓狀態
                        if (DBAllFile != null)
                        {
                            foreach (var file in DBAllFile)
                            {
                                file.IsEnable = "0";
                                var RelWebFileContentData = db.RelWebFileContent.First(x => x.WEBFileSN == file.WEBFileSN);
                                db.WEBFile.Update(file);
                                db.RelWebFileContent.Remove(RelWebFileContentData);
                                db.SaveChanges();
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static List<CaseApplyValidate> GetCaseValidate(string strDate, string endDate, ref DefaultPager pager, string keyword)
        {
            var _strDate = DateDateTime(strDate);
            var _endDate = DateDateTime(endDate, false);
            var data = new List<CaseApplyValidate>();
            using (var db = new MODAContext())
            {
                var _data1 = db.CaseApplyValidate.Where(x =>
                (string.IsNullOrWhiteSpace(strDate) ? 1 == 1 : x.CreateDate >= _strDate) &&
                (string.IsNullOrWhiteSpace(endDate) ? 1 == 1 : x.CreateDate <= _endDate) &&
                x.Status != ((int)Utility.MailBox.EnumCassApplyStatus.step3).ToString());

                var _data = (from a in _data1
                             where (string.IsNullOrWhiteSpace(keyword) ? 1 == 1 : (a.Email.Contains(keyword)))
                             select new CaseApplyValidate()
                             {
                                 Email = a.Email,
                                 CreateDate = a.CreateDate,
                                 EffectiveDate = a.EffectiveDate,
                                 Status = a.Status,
                             });

                var allCount = _data.Count();
                var list = _data.OrderByDescending(x => x.CreateDate).Skip((pager.p - 1) * pager.DisplayCount).Take(pager.DisplayCount).ToList();
                pager.TotalCount = allCount;
                pager.PageIndex = pager.p - 1;
                return list;
            }
        }

        /// <summary>
        /// 撈取民意信箱分類
        /// </summary>
        /// <returns></returns>
        public static List<SysCategory> GetSysCategory()
        {
            using (var db = new Services.MODAContext())
            {
                return db.SysCategory.Where(x => x.ParentKey.EndsWith("-10-2") && x.IsEnable == "1" && x.Lang == "zh-tw").ToList();
            }
        }
        /// <summary>
        /// 取的預設回復資訊
        /// </summary>
        /// <returns></returns>
        public static string GetPresetReply()
        {
            using (var db = new Services.MODAContext())
            {
                return db.SysCategory.FirstOrDefault(x => x.SysCategoryKey.EndsWith("Management-3-1") && x.IsEnable == "1" && x.Lang == "zh-tw")?.Value;
            }

        }

        public static List<SysCategory> GetParentClass()
        {
            using (var db = new Services.MODAContext())
            {
                return db.SysCategory.Where(x => x.SysCategoryKey.EndsWith("-10-1") && x.IsEnable == "1" && x.Lang == "zh-tw").ToList();
            }
        }

        public static List<CaseApplyClassModel> GetGroupList(string websiteId, string states, ref DefaultPager pager, int SysDepartmentSN = 0, string keyword = "", string CaseApplyClassSN = "", string IsEnable = "")
        {
            try
            {
                var _CaseApplyClassSN = 0;
                int.TryParse(CaseApplyClassSN, out _CaseApplyClassSN);
                using (var db = new MODAContext())
                {
                    var _CaseApplyClass = db.CaseApplyClass.Where(x =>
                    x.WebSiteID == websiteId &&
                    (string.IsNullOrWhiteSpace(states) ? 1 == 1 : x.IsEnable == states) &&
                    (SysDepartmentSN == 0 ? 1 == 1 : x.SysDepartmentSN == SysDepartmentSN) &&
                    (string.IsNullOrWhiteSpace(CaseApplyClassSN) ? 1 == 1 : x.CaseApplyClassSN == _CaseApplyClassSN) &&
                    (string.IsNullOrWhiteSpace(IsEnable) ? 1 == 1 : x.IsEnable == IsEnable)
                    );
                    var _SysDepartment = db.SysDepartment.Where(x => x.Lang == "zh-tw");

                    //ClassTo
                    var cTo = (from a in _CaseApplyClass
                               join b in db.CaseApplyClassTo on a.CaseApplyClassSN equals b.CaseApplyClassSN
                               select b)
                               .GroupBy(x => x.CaseApplyClassSN)
                               .Select(x => new MailToModel
                               {
                                   CaseApplyClassSN = x.Key ?? 0,
                                   MailToName = string.Join(",", x.Select(x => x.Name))
                               }).ToList();

                    var allList = (from a in _CaseApplyClass
                                   join b in _SysDepartment on a.SysDepartmentSN equals b.SysDepartmentSN into c
                                   from b in c.DefaultIfEmpty()
                                   select new CaseApplyClassModel()
                                   {
                                       CaseApplyClassSN = a.CaseApplyClassSN,
                                       CaseName = a.CaseName,
                                       depName = b.DepartmentName,
                                       CreatedDate = a.CreatedDate,
                                       ProcessDate = a.ProcessDate,
                                       IsEnable = a.IsEnable,
                                       ProcessUserID = a.ProcessUserID,
                                       CaseNo = a.CaseNo,
                                       CaseType = a.CaseType,
                                       SysCategoryKey = a.SysCategoryKey,
                                       HandleDate = a.HandleDate.ToString(),
                                   }).ToList();

                    allList = (from a in allList
                               join b in cTo on a.CaseApplyClassSN equals b.CaseApplyClassSN into c
                               from b in c.DefaultIfEmpty()
                               select new CaseApplyClassModel()
                               {
                                   CaseApplyClassSN = a.CaseApplyClassSN,
                                   CaseName = a.CaseName,
                                   depName = a.depName,
                                   CreatedDate = a.CreatedDate,
                                   ProcessDate = a.ProcessDate,
                                   IsEnable = a.IsEnable,
                                   ProcessUserID = a.ProcessUserID,
                                   CaseNo = a.CaseNo,
                                   CaseType = a.CaseType,
                                   SysCategoryKey = a.SysCategoryKey,
                                   HandleDate = a.HandleDate,
                                   CaseTo = b != null ? b.MailToName : ""
                               }).ToList();

                    if (!string.IsNullOrWhiteSpace(keyword))
                    {
                        allList = allList.Where(x => x.depName.Contains(keyword) || x.CaseName.Contains(keyword)).ToList();
                    }



                    var allCount = allList.Count();
                    var list = allList.OrderBy(x => x.CaseNo).Skip((pager.p - 1) * pager.DisplayCount).Take(pager.DisplayCount).ToList();
                    pager.TotalCount = allCount;
                    pager.PageIndex = pager.p - 1;
                    return list;
                }
            }
            catch (Exception ex)
            {
                LogService.CreateLogAction(new LogAction()
                {
                    Status = "0",
                    MessageResult = ex.ToString(),
                    ProcessIPAddress = "",
                    UserID = "",
                    WebSiteID = "",
                    WebPath = "",
                    ActionType = "1",
                    Action2 = "Select",
                    SourceTable = "CaseApplyClass",
                    Action = "GetGroupList",
                    Controller = "MailBoxService",
                    SourceSN = 0,
                    CreatedDate = DateTime.UtcNow.AddHours(8)
                });
            }
            return null;
        }

        public static List<CaseApplyModel> GetCaseApplyList(string websiteId, string strDate, string endDate, ref DefaultPager pager, int SysDepartmentSN = 0, int originalDepSN = 0, string keyword = "", string CaseApplyClassSN = "", string status = "")
        {
            var _strDate = DateDateTime(strDate);
            var _endDate = DateDateTime(endDate, false);
            var data = new List<CaseApplyModel>();
            var csw = GetClassStartWith(websiteId);
            using (var db = new MODAContext())
            {
                var _data1 = from a in db.CaseApply
                             join b in db.CaseApplyClass on a.CaseApplyClassSN equals b.CaseApplyClassSN
                             join c in db.SysDepartment.Where(x => x.Lang == "zh-tw" && x.IsEnable == "1") on a.DocDept equals c.DepartmentID into d
                             from e in d.DefaultIfEmpty()
                             where (e.WebSiteId == websiteId || a.WebSiteId == websiteId || b.CaseNo.StartsWith(csw)) &&
                               (string.IsNullOrWhiteSpace(strDate) ? 1 == 1 : a.AcceptDate >= _strDate) &&
                (string.IsNullOrWhiteSpace(endDate) ? 1 == 1 : a.AcceptDate <= _endDate) &&
                (string.IsNullOrWhiteSpace(CaseApplyClassSN) ? 1 == 1 : a.CaseApplyClassSN == int.Parse(CaseApplyClassSN))
                && a.AcceptDate != null && a.Status != EnumTpye.GetEnumNumberToSting(EnumCassApplyStatus.step11)
                             select a;
                ;
                //var _data1 = db.CaseApply.Where(x =>
                //x.WebSiteId == websiteId &&
                //(string.IsNullOrWhiteSpace(strDate) ? 1 == 1 : x.AcceptDate >= _strDate) &&
                //(string.IsNullOrWhiteSpace(endDate) ? 1 == 1 : x.AcceptDate <= _endDate) &&
                //(string.IsNullOrWhiteSpace(CaseApplyClassSN) ? 1 == 1 : x.CaseApplyClassSN == int.Parse(CaseApplyClassSN))
                //&& x.AcceptDate != null && x.Status != EnumTpye.GetEnumNumberToSting(EnumCassApplyStatus.step11));
                //Status
                if (status != "")
                {
                    var s = EnumTpye.GetEnum<MgrStatus>(status);

                    switch (s)
                    {
                        case MgrStatus.Accepted:
                            {
                                _data1 = _data1.Where(x => x.Status == EnumTpye.GetEnumNumberToSting(EnumCassApplyStatus.step7) ||
                                                           x.Status == EnumTpye.GetEnumNumberToSting(EnumCassApplyStatus.step9) ||
                                                           x.Status == EnumTpye.GetEnumNumberToSting(EnumCassApplyStatus.step10));
                            }
                            break;
                        case MgrStatus.Process:
                            {
                                _data1 = _data1.Where(x =>
                                x.Status == EnumTpye.GetEnumNumberToSting(EnumCassApplyStatus.step8) ||
                                x.Status == EnumTpye.GetEnumNumberToSting(EnumCassApplyStatus.step20)
                                );
                            }
                            break;
                        case MgrStatus.Closed:
                            {
                                _data1 = _data1.Where(x => x.Status == EnumTpye.GetEnumNumberToSting(EnumCassApplyStatus.step12) ||
                               x.Status == EnumTpye.GetEnumNumberToSting(EnumCassApplyStatus.step13) ||
                               x.Status == EnumTpye.GetEnumNumberToSting(EnumCassApplyStatus.step14) ||
                               x.Status == EnumTpye.GetEnumNumberToSting(EnumCassApplyStatus.step15));
                            }
                            break;
                        case MgrStatus.SpeedClosed:
                            {
                                _data1 = _data1.Where(x => (x.Status == EnumTpye.GetEnumNumberToSting(EnumCassApplyStatus.step12) ||
                                x.Status == EnumTpye.GetEnumNumberToSting(EnumCassApplyStatus.step14) ||
                                x.Status == EnumTpye.GetEnumNumberToSting(EnumCassApplyStatus.step15))
                                && x.ReplySource == EnumTpye.GetEnumNumberToSting(EnumReplySource.Speed));
                            }
                            break;
                        case MgrStatus.MgrClosed:
                            {
                                _data1 = _data1.Where(x => (x.Status == EnumTpye.GetEnumNumberToSting(EnumCassApplyStatus.step13) ||
                                   x.Status == EnumTpye.GetEnumNumberToSting(EnumCassApplyStatus.step14) ||
                                   x.Status == EnumTpye.GetEnumNumberToSting(EnumCassApplyStatus.step15))
                                   && x.ReplySource == EnumTpye.GetEnumNumberToSting(EnumReplySource.Mgr));
                            }
                            break;
                        case MgrStatus.Temp:
                            {
                                _data1 = _data1.Where(x => (x.Status == EnumTpye.GetEnumNumberToSting(EnumCassApplyStatus.step7) ||
                                 x.Status == EnumTpye.GetEnumNumberToSting(EnumCassApplyStatus.step8) ||
                                 x.Status == EnumTpye.GetEnumNumberToSting(EnumCassApplyStatus.step9) ||
                                 x.Status == EnumTpye.GetEnumNumberToSting(EnumCassApplyStatus.step10))
                                 && x.ReplySource == EnumTpye.GetEnumNumberToSting(EnumReplySource.Mgr));
                            }
                            break;
                        //case MgrStatus.ProcessMerge:
                        //    {
                        //        _data1 = _data1.Where(x => (x.Status == EnumTpye.GetEnumNumberToSting(EnumCassApplyStatus.step20)));
                        //    }
                        //    break;
                        case MgrStatus.SpeedClosedMerge:
                            {
                                _data1 = _data1.Where(x => (x.Status == EnumTpye.GetEnumNumberToSting(EnumCassApplyStatus.step21)));
                            }
                            break;
                        case MgrStatus.CheckClosedMerge:
                            {
                                _data1 = _data1.Where(x => (x.Status ==
                                    EnumTpye.GetEnumNumberToSting(EnumCassApplyStatus.step22) ||
                                 x.Status == EnumTpye.GetEnumNumberToSting(EnumCassApplyStatus.step23)));
                            }
                            break;
                        case MgrStatus.AutoClosed:
                            {
                                _data1 = _data1.Where(x => (x.Status ==
                                    EnumTpye.GetEnumNumberToSting(EnumCassApplyStatus.step16)));
                            }
                            break;
                    }
                }

                var _data = (from a in _data1
                             join b in db.CaseApplyClass on a.CaseApplyClassSN equals b.CaseApplyClassSN
                             join c in db.SysDepartment on b.SysDepartmentSN equals c.SysDepartmentSN
                             join d in db.SysDepartment on a.OriginalClassDeptSn equals d.SysDepartmentSN
                             where ((SysDepartmentSN == 0) ? 1 == 1 : c.SysDepartmentSN == SysDepartmentSN)
                             && ((originalDepSN == 0) ? 1 == 1 : a.OriginalClassDeptSn == originalDepSN)
                             && (string.IsNullOrWhiteSpace(keyword) ? 1 == 1 : (c.DepartmentName.Contains(keyword) || d.DepartmentName.Contains(keyword) || a.Subject.Contains(keyword) || a.ApplyUser.Contains(keyword) || a.CaseNo.Contains(keyword) || a.ReplyCaseNo.Contains(keyword) || a.CaseContent.Contains(keyword) || a.ContactEmail.Contains(keyword)))
                             select new CaseApplyModel()
                             {
                                 CaseNo = a.CaseNo,
                                 CaseName = b.CaseName,
                                 depName = CommonService.GetDeptTree(c.SysDepartmentSN),
                                 OriginalDeptName = CommonService.GetDeptTree(d.SysDepartmentSN),
                                 Subject = a.Subject,
                                 ContactEmail = a.ContactEmail,
                                 AcceptDate = a.AcceptDate,
                                 ReplySource = a.ReplySource,
                                 CaseApplySN = a.CaseApplySN,
                                 Status = a.Status,
                                 ReplyDate = a.ReplyDate
                             });

                var allCount = _data.Count();
                var list = _data.OrderByDescending(x => x.AcceptDate).Skip((pager.p - 1) * pager.DisplayCount).Take(pager.DisplayCount).ToList();
                pager.TotalCount = allCount;
                pager.PageIndex = pager.p - 1;
                return list;
            }
        }

        /// <summary>
        /// 查詢待改分清單
        /// </summary>
        public static List<CaseApplyModel> GetCaseApplyList2(string websiteId, ref DefaultPager pager, int SysDepartmentSN = 0, string keyword = "", string DocDpet = "")
        {
            var data = new List<CaseApplyModel>();
            using (var db = new MODAContext())
            {
                var _data = (from a in db.CaseApply
                             join c in db.SysDepartment.Where(x => x.Lang == "zh-tw" && x.IsEnable == "1" && x.ParentID == 0) on a.WebSiteId equals c.WebSiteId
                             join d in db.SysDepartment.Where(x => x.Lang == "zh-tw" && x.IsEnable == "1") on a.DocDept equals d.DepartmentID into e
                             from f in e.DefaultIfEmpty()
                             where
                             a.Status == EnumTpye.GetEnumNumberToSting(EnumCassApplyStatus.step11)
                             && (string.IsNullOrWhiteSpace(DocDpet) ? 1 == 1 : f.WebSiteId == DocDpet)
                             && (SysDepartmentSN == 0 ? 1 == 1 : c.SysDepartmentSN == SysDepartmentSN)
                             && (string.IsNullOrWhiteSpace(websiteId) ? 1 == 1 : a.WebSiteId == websiteId || c.WebSiteId == websiteId || f.WebSiteId == websiteId)
                                && (string.IsNullOrWhiteSpace(keyword) ? 1 == 1 :
                                    (a.CaseNo.Contains(keyword)
                                    || a.ReplyCaseNo.Contains(keyword)
                                    || a.Subject.Contains(keyword)
                                    || a.CaseContent.Contains(keyword)
                                    )
                                    )
                             select new CaseApplyModel()
                             {
                                 CaseNo = a.CaseNo,
                                 OriginalDeptName = c.DepartmentName,
                                 depName = f.DepartmentName ?? "",
                                 Subject = a.Subject,
                                 ContactEmail = a.ContactEmail,
                                 AcceptDate = a.AcceptDate,
                                 ReplySource = a.ReplySource,
                                 CaseApplySN = a.CaseApplySN,
                                 Status = a.Status,
                                 ReplyDate = a.ReplyDate

                             });

                var allCount = _data.Count();
                var list = _data.OrderByDescending(x => x.AcceptDate).Skip((pager.p - 1) * pager.DisplayCount).Take(pager.DisplayCount).ToList();

                pager.TotalCount = allCount;
                pager.PageIndex = pager.p - 1;
                return list;
            }
        }

        /// <summary>
        /// 案件重新設定分類群組
        /// </summary>
        /// <param name="CaseNo">案號</param>
        /// <param name="CaseApplyClassSN">分類代號</param>
        /// <param name="UserId">更新者</param>
        /// <returns></returns>
        public static bool UpdateCaseClass(CaseApply Case, out CaseApplyClass caseApplyClass, out SysCategory sysCategory)
        {
            caseApplyClass = null;
            sysCategory = null;
            try
            {

                using (var db = new MODAContext())
                {
                    var checkNeedSendSpeedApi = CheckNeedSendSpeedApi(Case.CaseApplyClassSN, out string msg, ref sysCategory, ref caseApplyClass);
                    if (!checkNeedSendSpeedApi)
                    {
                        Case.Status = EnumTpye.GetEnumNumberToSting(EnumCassApplyStatus.step16);
                        Case.ReplySource = "2";
                        Case.ReplySource2Date = DateTime.UtcNow.AddHours(8);
                        Case.ReplySource2 = msg;
                    }
                    //
                    db.Update(Case);
                    db.SaveChanges();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static List<MailBoxReportModel> GetReport(string websiteId, string strDate, string endDate, string strDate2, string endDate2, int _cassclass, int _dep, int originalDep, string replysource, ref DefaultPager pager)
        {
            using (var db = new MODAContext())
            {

                var _strDate = DateDateTime(strDate);
                var _endDate = DateDateTime(endDate, false);
                var _strDate2 = DateDateTime(strDate2);
                var _endDate2 = DateDateTime(endDate2, false);
                var allList = from a in db.CaseApply
                              join b in db.CaseApplyClass on a.CaseApplyClassSN equals b.CaseApplyClassSN
                              join c in db.SysDepartment on b.SysDepartmentSN equals c.SysDepartmentSN
                              join d in db.SysDepartment.Where(x => x.Lang == "zh-tw" && x.IsEnable == "1") on a.DocDept equals d.DepartmentID into e
                              from f in e.DefaultIfEmpty()
                              where b.WebSiteID == websiteId
                              && (string.IsNullOrWhiteSpace(strDate) ? 1 == 1 : a.AcceptDate >= _strDate)
                              && (string.IsNullOrWhiteSpace(endDate) ? 1 == 1 : a.AcceptDate <= _endDate)
                              && (string.IsNullOrWhiteSpace(strDate2) ? 1 == 1 : a.ReplyDate >= _strDate2)
                              && (string.IsNullOrWhiteSpace(endDate2) ? 1 == 1 : a.ReplyDate <= _endDate2)
                              && (_cassclass == 0 ? 1 == 1 : b.CaseApplyClassSN == _cassclass)
                              && (_dep == 0 ? 1 == 1 : c.SysDepartmentSN == _dep)
                              && (originalDep == 0 ? 1 == 1 : a.OriginalClassDeptSn == originalDep)
                              && (string.IsNullOrWhiteSpace(replysource) ? 1 == 1 : a.ReplySource == replysource)
                              select new MailBoxReportModel()
                              {
                                  CaseNo = a.CaseNo,
                                  ClassName = b.CaseName,
                                  AcceptDate = a.AcceptDate,
                                  OriginalDeptName = CommonService.GetDeptTree(a.OriginalClassDeptSn.Value),
                                  depName = a.ReplySource == EnumTpye.GetEnumNumberToSting(EnumReplySource.Speed) ? f.DepartmentName : CommonService.GetDeptTree(c.SysDepartmentSN),
                                  ReplyDate = (a.ReplySource == "2" ? a.ReplySource2Date : a.ReplySource == "1" ? a.ReplyDate : null),
                                  ReplySource = a.ReplySource,
                                  Status = a.Status,
                                  Subject = a.Subject
                              };
                var allCount = allList.Count();
                var list = allList.Skip((pager.p - 1) * pager.DisplayCount).Take(pager.DisplayCount).ToList();
                pager.TotalCount = allCount;
                pager.PageIndex = pager.p - 1;
                return list;
            }
        }

        public static bool CheckSurveyExists(int caseApplySN)
        {
            using (var db = new MODAContext())
            {
                var CaseApplySurvey = (from a in db.CaseApplySurvey
                                       where a.CaseApplySn == caseApplySN
                                       select a).FirstOrDefault();
                if (CaseApplySurvey != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static CaseApplySurvey SaveSurvey(CaseApplySurvey survey)
        {
            try
            {
                using (var db = new MODAContext())
                {
                    survey.CreateDate = DateTime.UtcNow.AddHours(8);
                    db.CaseApplySurvey.Add(survey);
                    db.SaveChanges();

                    return survey;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        static DateTime? MorthDateTime(string Date, bool haead = true)
        {
            var _date = new DateTime();
            if (string.IsNullOrWhiteSpace(Date)) return null;
            if (haead)
            {
                Date = Date + "-01 00:00:00";
                DateTime.TryParse(Date, out _date);
            }
            else
            {
                Date = Date + "-01 23:59:59";
                DateTime.TryParse(Date, out _date);
                _date = _date.AddMonths(1).AddDays(-1);
            }
            return _date;
        }

        static DateTime? DateDateTime(string Date, bool haead = true)
        {
            var _date = new DateTime();
            if (string.IsNullOrWhiteSpace(Date)) return null;
            if (haead)
            {
                Date = Date + " 00:00:00";
                DateTime.TryParse(Date, out _date);
            }
            else
            {
                Date = Date + " 23:59:59";
                DateTime.TryParse(Date, out _date);
            }
            return _date;
        }

        public static List<MailBoxReportModel> GetReport(string websiteId, string strDate, string endDate, string strDate2, string endDate2, int _cassclass, int _dep, int originalDep, string replysource)
        {
            using (var db = new MODAContext())
            {
                var _strDate = DateDateTime(strDate);
                var _endDate = DateDateTime(endDate, false);
                var _strDate2 = DateDateTime(strDate2);
                var _endDate2 = DateDateTime(endDate2, false);

                var allList = from a in db.CaseApply
                              join b in db.CaseApplyClass on a.CaseApplyClassSN equals b.CaseApplyClassSN
                              join c in db.SysDepartment on b.SysDepartmentSN equals c.SysDepartmentSN
                              join d in db.SysDepartment.Where(x => x.Lang == "zh-tw" && x.IsEnable == "1") on a.DocDept equals d.DepartmentID into e
                              from f in e.DefaultIfEmpty()
                              where b.WebSiteID == websiteId
                              && (string.IsNullOrWhiteSpace(strDate) ? 1 == 1 : a.AcceptDate >= _strDate)
                              && (string.IsNullOrWhiteSpace(endDate) ? 1 == 1 : a.AcceptDate <= _endDate)
                              && (string.IsNullOrWhiteSpace(strDate2) ? 1 == 1 : a.ReplyDate >= _strDate2)
                              && (string.IsNullOrWhiteSpace(endDate2) ? 1 == 1 : a.ReplyDate <= _endDate2)
                              && (_cassclass == 0 ? 1 == 1 : b.CaseApplyClassSN == _cassclass)
                              && (_dep == 0 ? 1 == 1 : c.SysDepartmentSN == _dep)
                              && (originalDep == 0 ? 1 == 1 : a.OriginalClassDeptSn == _dep)
                              && (string.IsNullOrWhiteSpace(replysource) ? 1 == 1 : a.ReplySource == replysource)
                              select new MailBoxReportModel()
                              {
                                  CaseNo = a.CaseNo,
                                  ClassName = b.CaseName,
                                  AcceptDate = a.AcceptDate,
                                  OriginalDeptName = CommonService.GetDeptTree(a.OriginalClassDeptSn.Value),
                                  depName = a.ReplySource == EnumTpye.GetEnumNumberToSting(EnumReplySource.Speed) ? f.DepartmentName : CommonService.GetDeptTree(c.SysDepartmentSN),
                                  ReplyDate = (a.ReplySource == "2" ? a.ReplySource2Date : a.ReplySource == "1" ? a.ReplyDate : null),
                                  ReplySource = a.ReplySource,
                                  Status = a.Status,
                                  Subject = a.Subject
                              };
                var allCount = allList.Count();
                var list = allList.ToList();

                return list;
            }
        }

        /// <summary>
        /// 撈取傳送至公文系統資料
        /// </summary>
        public static List<BigFileData> GetCassApply()
        {
            using (var db = new MODAContext())
            {
                var NewModels = new List<BigFileData>();
                var data = (from a in db.CaseApply
                            join b in db.CaseApplyClass on a.CaseApplyClassSN equals b.CaseApplyClassSN
                            join c in db.SysDepartment on b.SysDepartmentSN equals c.SysDepartmentSN
                            where (a.Status == EnumTpye.GetEnumNumberToSting(EnumCassApplyStatus.step7) ||
                                   a.Status == EnumTpye.GetEnumNumberToSting(EnumCassApplyStatus.step9) ||
                                   a.Status == EnumTpye.GetEnumNumberToSting(EnumCassApplyStatus.step10))
                            select new CaseApplyModel2()
                            {
                                CaseNo = a.CaseNo,
                                AcceptDate = a.AcceptDate,
                                ApplyUser = a.ApplyUser,
                                CaseApplySN = a.CaseApplySN,
                                CaseContent = a.CaseContent,
                                CasePwd = a.CasePwd,
                                ContactEmail = a.ContactEmail,
                                depName = c.DepartmentName,
                                Mobile = a.Mobile,
                                Subject = a.Subject,
                                Tel = a.Tel,
                                TelAreacode = a.TelAreacode,
                                TelExtension = a.TelExtension,
                                DepartmentID = c.DepartmentID,
                                CaseApplyClassCaseNo = b.CaseNo
                            }).ToList();

                foreach (var d in data)
                {
                    var dasModel = new BigFileData();
                    dasModel.CaseApplySN = d.CaseApplySN;
                    dasModel.addNewCaseModel1 = new AddNewCaseModel()
                    {
                        Subject = d.Subject,
                        Content = d.CaseContent,
                        ApplyUser = d.ApplyUser,
                        ContactEmail = d.ContactEmail,
                        Tel = d.Tel,
                        TelAreacode = d.TelAreacode,
                        TelExtensioncode = d.TelExtension,
                        ContactMphone = d.Mobile,
                        CompanyId = (d.DepartmentID.Substring(0, 1)),
                        CompanyCaseNo = d.CaseNo,
                        ItemTypeUid = d.CaseApplyClassCaseNo,
                        StartDate = d.AcceptDate.Value.ToString("yyyy/MM/dd HH:mm:ss")
                    };
                    var files = (from a in db.RelWebFileContent
                                 join b in db.WEBFile on a.WEBFileSN equals b.WEBFileSN
                                 where a.SourceTable == "CaseApply" && a.GroupID == "MailBox" && a.SourceSN == d.CaseApplySN
                                 select b).ToList();
                    if (files.Count() > 0)
                    {
                        var apiFile = files.Select(x => new FileModel()
                        {
                            FileName = x.FileName,
                            fileUrl = x.FileApiPath,
                            OriginalFileName = x.OriginalFileName,
                        }).ToList();
                        dasModel.fileModels = apiFile;
                    }
                    NewModels.Add(dasModel);
                }
                return NewModels;
            }
        }
        /// <summary>
        ///撈取單筆
        /// </summary>
        /// <param name="caseNo"></param>
        /// <returns></returns>
        public static List<BigFileData> GetCassApply(string caseNo)
        {
            using (var db = new MODAContext())
            {
                var NewModels = new List<BigFileData>();
                var data = (from a in db.CaseApply
                            join b in db.CaseApplyClass on a.CaseApplyClassSN equals b.CaseApplyClassSN
                            join c in db.SysDepartment on b.SysDepartmentSN equals c.SysDepartmentSN
                            where (a.CaseNo == caseNo)
                            select new CaseApplyModel2()
                            {
                                CaseNo = a.CaseNo,
                                AcceptDate = a.AcceptDate,
                                ApplyUser = a.ApplyUser,
                                CaseApplySN = a.CaseApplySN,
                                CaseContent = a.CaseContent,
                                CasePwd = a.CasePwd,
                                ContactEmail = a.ContactEmail,
                                depName = c.DepartmentName,
                                Mobile = a.Mobile,
                                Subject = a.Subject,
                                Tel = a.Tel,
                                TelAreacode = a.TelAreacode,
                                TelExtension = a.TelExtension,
                                DepartmentID = c.DepartmentID,
                                CaseApplyClassCaseNo = b.CaseNo
                            }).ToList();

                foreach (var d in data)
                {
                    var dasModel = new BigFileData();
                    dasModel.CaseApplySN = d.CaseApplySN;
                    dasModel.addNewCaseModel1 = new AddNewCaseModel()
                    {
                        Subject = d.Subject,
                        Content = d.CaseContent,
                        ApplyUser = d.ApplyUser,
                        ContactEmail = d.ContactEmail,
                        Tel = d.Tel,
                        TelAreacode = d.TelAreacode,
                        TelExtensioncode = d.TelExtension,
                        ContactMphone = d.Mobile,
                        CompanyId = (d.DepartmentID.Substring(0, 1)),
                        CompanyCaseNo = d.CaseNo,
                        ItemTypeUid = d.CaseApplyClassCaseNo,
                        StartDate = d.AcceptDate.Value.ToString("yyyy/MM/dd HH:mm:ss")
                    };
                    var files = (from a in db.RelWebFileContent
                                 join b in db.WEBFile on a.WEBFileSN equals b.WEBFileSN
                                 where a.SourceTable == "CaseApply" && a.GroupID == "MailBox" && a.SourceSN == d.CaseApplySN
                                 select b).ToList();
                    if (files.Count() > 0)
                    {
                        var apiFile = files.Select(x => new FileModel()
                        {
                            FileName = x.FileName,
                            fileUrl = x.FileApiPath,
                            OriginalFileName = x.OriginalFileName,

                        }).ToList();
                        dasModel.fileModels = apiFile;
                    }
                    NewModels.Add(dasModel);
                }
                return NewModels;
            }
        }


        /// <summary>
        /// 傳送API用 抓取回傳的資料
        /// </summary>
        /// <param name="caseNo">單筆資料CaseNo</param>
        /// <returns></returns>
        public static List<SearchModel> GetSearchCaseList(string caseNo = "", bool all = false)
        {
            var needSearhStatus = new List<string>() {
            EnumTpye.GetEnumNumberToSting(EnumCassApplyStatus.step8),
            EnumTpye.GetEnumNumberToSting(EnumCassApplyStatus.step20)
            };

            using (var db = new MODAContext())
            {
                var data = (from a in db.CaseApply
                            join b in db.CaseApplyClass on a.CaseApplyClassSN equals b.CaseApplyClassSN
                            join c in db.SysDepartment on b.SysDepartmentSN equals c.SysDepartmentSN
                            where (all || needSearhStatus.Contains(a.Status)) &&
                            (string.IsNullOrWhiteSpace(caseNo) ? 1 == 1 : a.CaseNo == caseNo)
                            select new CaseApplyModel2()
                            {
                                ReplyCaseNo = a.ReplyCaseNo,
                                ReplyCasePwd = a.ReplyCasePwd,
                                DepartmentID = c.DepartmentID,
                                CaseApplySN = a.CaseApplySN,
                            }).ToList();
                var s = data.Select(x => new SearchModel()
                {
                    CaseApplySN = x.CaseApplySN,
                    SearchCaseModel = new SearchCaseModel()
                    {
                        CaseNo = x.ReplyCaseNo,
                        CasePwd = x.ReplyCasePwd,
                        CompanyId = x.DepartmentID.Substring(0, 1)
                    }
                }).ToList();
                return s;
            }
        }

        /// <summary>
        /// 公文系統成案後改狀態
        /// </summary>
        /// <param name="CaseNo"></param>
        /// <param name="ReplyCaseNo"></param>
        /// <param name="ReplyCasePwd"></param>
        /// <returns></returns>
        public static bool UpdateCassApply(string CaseNo, string ReplyCaseNo, string ReplyCasePwd)
        {
            try
            {
                using (var db = new MODAContext())
                {
                    var data = db.CaseApply.FirstOrDefault(x => x.CaseNo == CaseNo);
                    if (!string.IsNullOrWhiteSpace(ReplyCaseNo))
                    {
                        data.ReplyCaseNo = ReplyCaseNo;
                        data.ReplyCasePwd = ReplyCasePwd;
                    }
                    data.ProcessDate = DateTime.UtcNow.AddHours(8);
                    data.Status = EnumTpye.GetEnumNumberToSting(EnumCassApplyStatus.step8);
                    db.SaveChanges();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static void UpdateCassApplyStatus(string CaseNo, Utility.MailBox.EnumCassApplyStatus status)
        {
            using (var db = new MODAContext())
            {
                var data = db.CaseApply.FirstOrDefault(x => x.CaseNo == CaseNo);
                data.ProcessDate = DateTime.UtcNow.AddHours(8);
                data.Status = status.ToString();
                db.SaveChanges();
            }
        }

        /// <summary>
        /// 更新回覆
        /// </summary>
        /// <param name="ReplyCaseNo"></param>
        /// <param name="ReplyCasePwd"></param>
        /// <param name="ReplyContent"></param>
        /// <param name="ReplyDate"></param>
        /// <param name="IsOver">是否結案</param>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static bool UpdateReplyCassApply(string ReplyCaseNo, string ReplyCasePwd, string ReplyContent, string ReplyDate = null, bool IsOver = true, string filepath = "", string docNo = "", string docDept = "", EnumCassApplyStatus finishStatus = EnumCassApplyStatus.step12)
        {
            var msg = "";
            try
            {
                using (var db = new MODAContext())
                {
                    //api可以更新的狀態
                    var canUpdateStatus = new List<string>() {
                        EnumTpye.GetEnumNumberToSting(EnumCassApplyStatus.step8),
                        EnumTpye.GetEnumNumberToSting(EnumCassApplyStatus.step20)
                    };
                    var oldData = db.CaseApply.FirstOrDefault(x => x.ReplyCaseNo == ReplyCaseNo && x.ReplyCasePwd == ReplyCasePwd);
                    if (canUpdateStatus.Contains(oldData.Status))
                    {
                        if (ReplyDate == null)
                        {
                            oldData.ReplyContent = ReplyContent;
                            if (finishStatus != EnumCassApplyStatus.step12)
                            {
                                oldData.Status = EnumTpye.GetEnumNumberToSting(finishStatus);
                                oldData.DocDept = docDept;
                                oldData.DocNo = docNo;
                            }
                        }
                        else
                        {
                            var replyDate = DateTime.UtcNow.AddHours(8);
                            DateTime.TryParse(ReplyDate, out replyDate);
                            if (IsOver)
                            {
                                oldData.Status = EnumTpye.GetEnumNumberToSting(finishStatus);
                                oldData.ReplySource = EnumTpye.GetEnumNumberToSting(EnumReplySource.Speed);
                                oldData.ReplyContent = ReplyContent;
                                oldData.DocDept = docDept;
                                oldData.DocNo = docNo;

                            }
                            else
                            {
                                oldData.Status = EnumTpye.GetEnumNumberToSting(EnumCassApplyStatus.step11);
                                oldData.DocDept = docDept;
                                oldData.DocNo = docNo;
                            }
                           oldData.ReplyDate = replyDate;
                           msg = $@"UpdateReplyCassApply : {JsonConvert.SerializeObject(oldData)}   ";
                        }
                        oldData.ReplyApiDate = DateTime.UtcNow.AddHours(8);
                        oldData.ProcessDate = DateTime.UtcNow.AddHours(8);
                        db.SaveChanges();

                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                msg += $@" | ERROR :  {ex.Message}";
                return false;
            }
        }


        public static bool UploadReplyFile(int CaseApplySN, List<FileMessage> fileMessages, ref bool already    )
        {
            var groupId = "SpeedApi";
            int sort = 1;
            string msg = "";
            try
            {
                using (var db = new MODAContext())
                {
                    var chk = db.RelWebFileContent.FirstOrDefault(x => x.SourceSN == CaseApplySN
                    && x.SourceTable == "CaseApply"
                    && x.GroupID == groupId
                    );
                    if (chk != null)
                    {
                        already = true;
                        return true;
                    }
                    if (fileMessages?.Count() > 0)
                    {
                        foreach (var f in fileMessages)
                        {
                            var replyFile = f.CommonFileModel;
                            var gid = Regular.GetRandomString(15, RegularType.notspecial);
                            var file = new WEBFile()
                            {
                                FilePath = $"{CommonService.WebAPIUrl}{gid}",
                                FileApiPath = replyFile.filePath,
                                WEBFileID = gid,
                                OriginalFileName = replyFile.fileOriginName,
                                FileName = replyFile.fileNewName ,
                                FileTitle = replyFile.fileNewName ,
                                FileSize = 10 ,
                                FileType = replyFile.fileExt,
                                IsEnable = "1",
                                CreatedDate = DateTime.UtcNow.AddHours(8),
                                ProcessDate = DateTime.UtcNow.AddHours(8),
                            };
                            msg += $@"WEBFile : {JsonConvert.SerializeObject(file)}   ";
                            db.WEBFile.Add(file);
                            db.SaveChanges();
                            msg += " OK ";
                            var relWebFileContent = new RelWebFileContent()
                            {
                                WEBFileSN = file.WEBFileSN,
                                SourceTable = "CaseApply",
                                SourceSN = CaseApplySN,
                                PageView = 0,
                                GroupID = groupId,
                                CreatedDate = DateTime.UtcNow.AddHours(8),
                                CreatedUserID = "system",
                                SortOrder = sort
                            };
                            sort++;
                            msg += $@" | RelWebFileContent : {JsonConvert.SerializeObject(relWebFileContent)}   ";
                            db.RelWebFileContent.Add(relWebFileContent);
                            db.SaveChanges();
                            msg += " OK \n\r";
                        }
                    }
                }

                return true;
            }
            catch (Exception  ex)
            {
                msg += $@" | ERROR :  {ex.Message}";
                return false;
            }
        }

        #region 前台


        /// <summary>
        /// 建立驗證信
        /// </summary>
        /// <param name="caseApplyValidate"></param>
        /// <returns></returns>
        public static bool CreateCaseApplyValidate(CaseApplyValidate caseApplyValidate)
        {
            using (var db = new Services.MODAContext())
            {
                try
                {
                    caseApplyValidate.Status = EnumTpye.GetEnumNumberToSting(EnumCassApplyStatus.step0);
                    db.CaseApplyValidate.Add(caseApplyValidate);
                    db.SaveChanges();
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 確認信箱驗證碼
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static CaseApplyValidate GetCaseApplyValidateBytoken(string token)
        {
            using (var db = new MODAContext())
            {
                return db.CaseApplyValidate.FirstOrDefault(x => x.Token == token);
            }
        }

        public static CaseApply GetCaseApplyByValidateSN(int ValidateSN)
        {
            using (var db = new MODAContext())
            {
                return db.CaseApply.FirstOrDefault(x => x.CaseValidateSN == ValidateSN);
            }
        }

        public static void UpdateCaseApplyValidate(CaseApplyValidate Validate)
        {
            using (var db = new MODAContext())
            {
                db.CaseApplyValidate.Update(Validate);
                db.SaveChanges();
            }
        }

        public static CaseApplyValidate GetCaseApplyValidate(int CAVsn)
        {
            using (var db = new MODAContext())
            {
                return db.CaseApplyValidate.FirstOrDefault(x => x.CaseValidateSN == CAVsn);
            }
        }

        /// <summary>
        /// 建立 民意信箱資料<<20230506 新增規則假如是特殊分類代號 直接回覆罐頭訊息>>
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool CreateCaseApply(CaseApply data, string addDays, out CaseApplyClass caseApplyClass, out SysCategory sysCategory)
        {
            caseApplyClass = null;
            sysCategory = null;
            using (var db = new MODAContext())
            {
                var caseno = GetNewCaseNo();
                try
                {
                    #region 判斷特殊規則  不能轉成數字的為特殊規則
                    var checkNeedSendSpeedApi = CheckNeedSendSpeedApi(data.CaseApplyClassSN, out string msg, ref sysCategory, ref caseApplyClass);
                    #endregion

                    var CaseApplyValidate = db.CaseApplyValidate.FirstOrDefault(x => x.CaseValidateSN == data.CaseValidateSN);
                    if (CaseApplyValidate == null)
                    {
                        return false;
                    }

                    var _addDays = 3;
                    int.TryParse(addDays, out _addDays);
                    var CasePwd = Utility.Regular.GetRandomString(12, Utility.RegularType.base58);
                    var EffectiveDate = DateTime.UtcNow.AddHours(8).AddDays(_addDays);
                    //
                    var Status = checkNeedSendSpeedApi == false ? EnumTpye.GetEnumNumberToSting(EnumCassApplyStatus.step16) : EnumTpye.GetEnumNumberToSting(EnumCassApplyStatus.step7);
                    //
                    var caseClass = db.CaseApplyClass.FirstOrDefault(x => x.CaseApplyClassSN == data.CaseApplyClassSN);
                    var WebsiteID = WebLevelManagementService.sysDepartments().FirstOrDefault(x => x.SysDepartmentSN == caseClass.SysDepartmentSN)?.WebSiteId;
                    data.CaseNo = caseno;
                    data.CasePwd = CasePwd;
                    data.EffectiveDate = EffectiveDate;
                    data.Status = Status;
                    data.AcceptDate = DateTime.UtcNow.AddHours(8);
                    data.CreateDate = DateTime.UtcNow.AddHours(8);
                    data.CreateUser = "";
                    data.ProcessDate = DateTime.UtcNow.AddHours(8);
                    data.ProcessUser = "";
                    data.ProcessIPAddress = "";
                    data.ReplySource = checkNeedSendSpeedApi == false ? "2" : "0";
                    data.WebSiteId = WebsiteID;
                    data.OriginalCaseApplyClassSn = caseClass?.CaseApplyClassSN ?? 0;
                    data.OriginalClassDeptSn = caseClass?.SysDepartmentSN ?? 0;
                    if (checkNeedSendSpeedApi == false)
                    {
                        data.ReplySource2Date = DateTime.UtcNow.AddHours(8);
                        data.ReplySource2 = msg;
                    }
                    db.CaseApply.Add(data);
                    //
                    db.SaveChanges();
                    //
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }
        /// <summary>
        /// 確認是否要發送API
        /// </summary>
        /// <param name="CaseApplyClassSN"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static bool CheckNeedSendSpeedApi(int CaseApplyClassSN, out string msg, ref SysCategory sysCategory, ref CaseApplyClass caseApplyClass)
        {
            using (var db = new MODAContext())
            {
                #region 判斷特殊規則 intCaseNo = 0 不能轉成數字的為特殊規則
                var caseApplyClassSN = CaseApplyClassSN;
                var intCaseNo = 0;
                var CaseApplyClass = db.CaseApplyClass.FirstOrDefault(x => x.CaseApplyClassSN == caseApplyClassSN);
                if (!string.IsNullOrWhiteSpace(CaseApplyClass?.SysCategoryKey))
                {
                    sysCategory = db.SysCategory.FirstOrDefault(x => x.SysCategoryKey == CaseApplyClass.SysCategoryKey && x.Lang == "zh-tw");
                }
                caseApplyClass = CaseApplyClass;
                var caseNo = CaseApplyClass?.CaseNo;
                if (int.TryParse(caseNo, out intCaseNo))
                {
                    msg = "";
                    return true;
                }
                else
                {
                    msg = db.SysCategory.FirstOrDefault(x => x.SysCategoryKey == "Management-3-2" && x.Lang == "zh-tw")?.Value;
                    return false;
                }
                #endregion

            }
        }

        /// <summary>
        /// 確認案件內容
        /// </summary>
        /// <param name="CaseNo"></param>
        /// <param name="CasePwd"></param>
        /// <returns></returns>
        public static bool ConfirmCase(ref CaseApply Case)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    Case.Status = EnumTpye.GetEnumNumberToSting(EnumCassApplyStatus.step7);
                    Case.AcceptDate = DateTime.UtcNow.AddHours(8);
                    Case.ProcessDate = DateTime.UtcNow.AddHours(8);
                    db.CaseApply.Update(Case);
                    db.SaveChanges();

                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="CaseApply"></param>
        /// <param name="ReplySource2"></param>
        /// <param name="ReplyFile"></param>
        /// <returns></returns>
        public static bool EditCaseApplyModeReply(CaseApply CaseApply, string ReplySource2, List<CommonFileModel> ReplyFile)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    CaseApply.ProcessDate = DateTime.UtcNow.AddHours(8);
                    db.CaseApply.Update(CaseApply);
                    db.SaveChanges();

                    if (ReplyFile?.Count > 0)
                    {
                        var nowNewsFileName = ReplyFile.Select(x => x.fileNewName).ToList();

                        var DBAllFile = (from a in db.RelWebFileContent
                                         join b in db.WEBFile on a.WEBFileSN equals b.WEBFileSN
                                         where a.SourceTable == "CaseApply" &&
                                               a.SourceSN == CaseApply.CaseApplySN
                                         select b).ToList();

                        var DBFileName = DBAllFile.Select(y => y.FileName).ToList();
                        //刪除的檔案
                        var NeedDeleteFiles = DBAllFile.Where(x => !nowNewsFileName.Contains(x.FileName)).ToList();
                        //新的檔案
                        var NewFiles = ReplyFile.Where(x => !DBFileName.Contains(x.fileNewName)).ToList();
                        // 刪除 先壓狀態
                        if (NeedDeleteFiles != null)
                        {
                            foreach (var file in NeedDeleteFiles)
                            {
                                file.IsEnable = "0";
                                var RelWebFileContentData = db.RelWebFileContent.First(x => x.WEBFileSN == file.WEBFileSN);
                                db.WEBFile.Update(file);
                                db.RelWebFileContent.Remove(RelWebFileContentData);
                                db.SaveChanges();
                            }
                        }
                        //新增
                        if (NewFiles != null)
                        {
                            foreach (var file in NewFiles)
                            {
                                var FileData = db.WEBFile.First(x => x.FileName == file.fileNewName);
                                var RelWebFileContentData = new RelWebFileContent()
                                {
                                    WEBFileSN = FileData.WEBFileSN,
                                    SourceTable = "CaseApply",
                                    SourceSN = CaseApply.CaseApplySN,
                                    GroupID = file.GroupID.ToString(),
                                    CreatedUserID = CaseApply.CreateUser,
                                    CreatedDate = DateTime.UtcNow.AddHours(8),
                                    SortOrder = file.FileSort,
                                };
                                FileData.IsEnable = "1";
                                db.WEBFile.Update(FileData);
                                db.RelWebFileContent.Add(RelWebFileContentData);
                                db.SaveChanges();
                            }
                        }
                        //修改fileTitle
                        foreach (var file in ReplyFile)
                        {
                            var filedata = db.WEBFile.First(x => x.FileName == file.fileNewName);
                            filedata.FileTitle = file.fileTitle;
                            var RelWebFileContent = db.RelWebFileContent.FirstOrDefault(x => x.WEBFileSN == filedata.WEBFileSN);
                            RelWebFileContent.SortOrder = file.FileSort;

                            db.WEBFile.Update(filedata);
                            db.RelWebFileContent.Update(RelWebFileContent);
                            db.SaveChanges();

                        }
                    }
                    else
                    {
                        var DBAllFile = (from a in db.RelWebFileContent
                                         join b in db.WEBFile on a.WEBFileSN equals b.WEBFileSN
                                         where a.SourceTable == "CaseApply" &&
                                               a.SourceSN == CaseApply.CaseApplySN
                                         select b).ToList();
                        // 刪除 先壓狀態
                        if (DBAllFile != null)
                        {
                            foreach (var file in DBAllFile)
                            {
                                file.IsEnable = "0";
                                var RelWebFileContentData = db.RelWebFileContent.First(x => x.WEBFileSN == file.WEBFileSN);
                                db.WEBFile.Update(file);
                                db.RelWebFileContent.Remove(RelWebFileContentData);
                                db.SaveChanges();
                            }
                        }
                    }

                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 撈取最後一筆申請案件
        /// </summary>
        public static CaseApply GetLastCase(string eMail)
        {
            using (var db = new MODAContext())
            {
                return db.CaseApply
                    .OrderByDescending(x => x.CaseApplySN)
                    .FirstOrDefault(x => x.ContactEmail == eMail);
            }
        }

        /// <summary>
        /// 撈取最後一筆處理中案件
        /// </summary>
        public static CaseApply GetLastInProgressCase(string eMail)
        {
            using (var db = new MODAContext())
            {
                var status = new[]{
                    EnumTpye.GetEnumNumberToSting(EnumCassApplyStatus.step7),
                    EnumTpye.GetEnumNumberToSting(EnumCassApplyStatus.step8),
                    EnumTpye.GetEnumNumberToSting(EnumCassApplyStatus.step9),
                    EnumTpye.GetEnumNumberToSting(EnumCassApplyStatus.step10),
                    EnumTpye.GetEnumNumberToSting(EnumCassApplyStatus.step11),
                };

                return db.CaseApply
                    .OrderByDescending(x => x.CaseApplySN)
                    .FirstOrDefault(x => status.Contains(x.Status) &&
                        x.ContactEmail == eMail
                );
            }
        }
        /// <summary>
        /// 取得案件資料
        /// </summary>
        /// <param name="CaseNo"></param>
        /// <param name="CasePwd"></param>
        /// <returns></returns>
        public static CaseApply GetCaseApply(string CaseNo, string CasePwd)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    return db.CaseApply.FirstOrDefault(x => x.CaseNo == CaseNo && x.CasePwd == CasePwd);
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// 取得資料
        /// </summary>
        /// <param name="CaseNo"></param>
        /// <returns></returns>
        public static CaseApply GetCaseApply(string CaseNo, string CasePwd = "", bool Replay = false)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    var chkData = db.CaseApply.FirstOrDefault(x => Replay == false ? (x.CaseNo == CaseNo) : (x.ReplyCaseNo == CaseNo && x.ReplyCasePwd == CasePwd));
                    if (chkData == null) return null;
                    return chkData;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 取得單筆資料
        /// </summary>
        /// <param name="CaseApplySN"></param>
        /// <returns></returns>
        public static CaseApply GetCaseApply(int CaseApplySN)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    var data = db.CaseApply.FirstOrDefault(x => x.CaseApplySN == CaseApplySN);
                    return data;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// 取得檔案
        /// </summary>
        /// <param name="CaseApplyClassSN"></param>
        /// <returns></returns>
        public static List<WEBFile> GetCaseApplyFiles(string SourceTable, int SN, string GroupID)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    var data = (from a in db.RelWebFileContent
                                join b in db.WEBFile on a.WEBFileSN equals b.WEBFileSN
                                where a.SourceTable == SourceTable
                                && a.SourceSN == SN
                                && a.GroupID == GroupID
                                select b).ToList();
                    return data;
                }
                catch (Exception)
                {

                    return null;
                }
            }
        }
        #endregion
        #region CaseApplyClass
        /// <summary>
        /// 取出目前的 意見分類 
        /// </summary>
        /// <returns></returns>
        public static List<CasesModel> GetCases(string IsEnable = "1")
        {
            using (var db = new MODAContext())
            {
                var _a = db.SysWebSite.ToList();
                var _b = db.CaseApplyClass.ToList();
                if (IsEnable == "1") _b = _b.Where(x => x.IsEnable == IsEnable).ToList();

                var data = (from a in _a
                            join b in _b on a.WebSiteID equals b.WebSiteID
                            orderby a.WebSiteID, b.SortOrder
                            select new CasesModel()
                            {
                                IsEnable = b.IsEnable,
                                WebSiteID = a.WebSiteID,
                                WebSiteName = a.Title,
                                CaseApplyClassSN = b.CaseApplyClassSN,
                                CaseName = b.CaseName,
                                CaseNameEN = b.CaseNameEN,
                                WebSiteSort = a.SortOrder.Value,
                                CaseApplySort = b.SortOrder,
                                SysCategoryKey = b.SysCategoryKey,
                                CaseNo = b.CaseNo,
                            }).ToList();
                return data;
            }
        }


        /// <summary>
        /// 取得單一資料
        /// </summary>
        /// <param name="CaseApplyClassSN"></param>
        /// <returns></returns>
        public static CasesModel GetCase(int CaseApplyClassSN)
        {
            using (var db = new MODAContext())
            {
                var data = (from a in db.SysWebSite.ToList()
                            join b in db.CaseApplyClass.ToList() on a.WebSiteID equals b.WebSiteID
                            join c in db.SysDepartment.ToList() on b.SysDepartmentSN equals c.SysDepartmentSN
                            where b.CaseApplyClassSN == CaseApplyClassSN
                            orderby a.WebSiteID, b.SortOrder
                            select new CasesModel()
                            {
                                WebSiteID = a.WebSiteID,
                                WebSiteName = a.Title,
                                CaseApplyClassSN = b.CaseApplyClassSN,
                                CaseName = b.CaseName,
                                CaseNameEN = b.CaseNameEN,
                                WebSiteSort = a.SortOrder.Value,
                                CaseApplySort = b.SortOrder,
                                DepName = CommonService.GetDeptTree(c.SysDepartmentSN),
                                SysCategoryKey = b.SysCategoryKey
                            }).FirstOrDefault();
                return data;

            }
        }
        /// <summary>
        /// 意見分類
        /// </summary>
        /// <param name="websiteId"></param>
        /// <param name="CaseApplyClassSN"></param>
        /// <returns></returns>
        public static CaseApplyClass GetCaseApplyClass(string websiteId, int CaseApplyClassSN = 0)
        {
            try
            {
                if (CaseApplyClassSN == 0) return null;
                using (var db = new MODAContext())
                {
                    return db.CaseApplyClass.FirstOrDefault(x => x.WebSiteID == websiteId && x.CaseApplyClassSN == CaseApplyClassSN);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
        /// <summary>
        /// 意見分類
        /// </summary>
        /// <param name="CaseApplyClassSN"></param>
        /// <returns></returns>
        public static CaseApplyClass GetCaseApplyClass(int CaseApplyClassSN = 0)
        {
            try
            {
                if (CaseApplyClassSN == 0) return null;
                using (var db = new MODAContext())
                {
                    return db.CaseApplyClass.FirstOrDefault(x => x.CaseApplyClassSN == CaseApplyClassSN);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 承辦人員
        /// </summary>
        /// <param name="CaseApplyClassSN"></param>
        /// <returns></returns>
        public static List<CaseApplyClassTo> GetCaseApplyClassTos(int CaseApplyClassSN = 0)
        {
            try
            {
                if (CaseApplyClassSN == 0) return null;
                using (var db = new MODAContext())
                {
                    return db.CaseApplyClassTo.Where(x => x.CaseApplyClassSN == CaseApplyClassSN).ToList();
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
        /// <summary>
        /// 儲存CaseApplyClass資料
        /// </summary>
        /// <param name="caseApplyClass"></param>
        /// <param name="caseApplyClassTos"></param>
        public static bool Save(CaseApplyClass caseApplyClass, List<CaseApplyClassTo> caseApplyClassTos)
        {
            try
            {
                using (var db = new MODAContext())
                {
                    if (caseApplyClass.CaseApplyClassSN == 0)
                    {
                        caseApplyClass.CaseNo = caseApplyClass.CaseNo.Trim();
                        db.CaseApplyClass.Add(caseApplyClass);
                        db.SaveChanges();
                        foreach (var _t in caseApplyClassTos)
                        {
                            _t.CaseApplyClassSN = caseApplyClass.CaseApplyClassSN;
                            _t.CreatedDate = DateTime.UtcNow.AddHours(8);
                            _t.SysDepartmentSN = caseApplyClass.SysDepartmentSN;
                            _t.ProcessDate = DateTime.UtcNow.AddHours(8);
                            _t.CreatedUserID = caseApplyClass.CreatedUserID;
                            _t.ProcessUserID = caseApplyClass.ProcessUserID;
                            _t.ProcessIPAddress = caseApplyClass.ProcessIPAddress;
                            _t.IsEnable = "1";
                            db.CaseApplyClassTo.Add(_t);
                        }

                        db.SaveChanges();
                    }
                    else
                    {
                        var oldData = db.CaseApplyClass.FirstOrDefault(x => x.CaseApplyClassSN == caseApplyClass.CaseApplyClassSN);
                        if (oldData != null)
                        {
                            oldData.CaseNo = caseApplyClass.CaseNo.Trim();
                            oldData.CaseName = caseApplyClass.CaseName;
                            oldData.CaseNameEN = caseApplyClass.CaseNameEN;
                            oldData.CaseType = caseApplyClass.CaseType;
                            oldData.SysDepartmentSN = caseApplyClass.SysDepartmentSN;
                            oldData.IsEnable = caseApplyClass.IsEnable;
                            oldData.HandleDate = caseApplyClass.HandleDate;
                            oldData.info = caseApplyClass.info;
                            oldData.SysCategoryKey = caseApplyClass.SysCategoryKey;
                            oldData.ProcessDate = caseApplyClass.ProcessDate;
                            oldData.ProcessIPAddress = caseApplyClass.ProcessIPAddress;
                            oldData.ProcessUserID = caseApplyClass.ProcessUserID;
                            var oldCaseApplyClassTo = db.CaseApplyClassTo.Where(x => x.CaseApplyClassSN == oldData.CaseApplyClassSN).ToList();
                            if (oldCaseApplyClassTo.Count() > 0)
                            {
                                db.CaseApplyClassTo.RemoveRange(oldCaseApplyClassTo);
                                db.SaveChanges();
                            }
                            foreach (var _t in caseApplyClassTos)
                            {
                                _t.CaseApplyClassSN = oldData.CaseApplyClassSN;
                                _t.CreatedDate = DateTime.UtcNow.AddHours(8);
                                _t.SysDepartmentSN = caseApplyClass.SysDepartmentSN;
                                _t.ProcessDate = DateTime.UtcNow.AddHours(8);
                                _t.CreatedUserID = caseApplyClass.CreatedUserID;
                                _t.ProcessUserID = caseApplyClass.ProcessUserID;
                                _t.ProcessIPAddress = caseApplyClass.ProcessIPAddress;
                                _t.IsEnable = "1";
                                db.CaseApplyClassTo.Add(_t);
                            }
                            db.SaveChanges();
                        }
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion

        /// <summary>
        /// 拋轉新增案子至公文系統-發生錯誤寄信
        /// </summary>
        /// <param name="errorList"></param>
        public static void SendErrorCreateApi(List<string> errorList)
        {
            var userList = LogService.GetErroEmailAccount();
            if (userList != null && userList.Count() > 0)
            {
                var users = string.Join(";", userList);
                var info = "請確認以下資訊<br/>";
                var error = string.Join("<br/> ", errorList);
                MailInfoModel mailInfo = new MailInfoModel()
                {
                    Type = "MailBox",
                    ToMail = users,
                    Subject = $@"數位發展部民意信箱─新增案件拋轉錯誤",
                    Body = $"{info}{error}",

                };
                Utility.Mail.Send(mailInfo, out Exception exception);
            }
        }
        public static void SendReSetTellUser(string localUrl)
        {
            var userList = MailBoxService.GetMailBoxResetList();
            if (userList != null && userList.Count() > 0)
            {
                var users = string.Join(";", userList);
                var detail = @$"
您好，<br><br>
原承辦機關：
有民意信箱案件自公文系統退回官網：<br>
請至民意信箱管理後台之「公文退件待改分」功能查看，<br>
並請重新指定案件之「意見分類」，以利分派給正確的單位處理。<br>
謝謝！<br>
數位發展部民意信箱小組 敬啟
";
                MailInfoModel mailInfo = new MailInfoModel()
                {
                    Type = "MailBox",
                    ToMail = users,
                    Subject = $@"數位發展部民意信箱─案件退回請重新指派",
                    Body = detail,

                };
                Utility.Mail.Send(mailInfo, out Exception exception);

                // Utility.Mail.SendMailBox(users, $@"數位發展部民意信箱─案件退回請重新指派", detail, true, out Exception outex);
            }

        }

        public static void SendReSetTellUser2(string localUrl, int CaseApplySN)
        {
            var userList = GetMailBoxResetList();
            using (var db = new MODAContext())
            {
                var data = db.CaseApply.FirstOrDefault(x => x.CaseApplySN == CaseApplySN);
                var caseData = db.CaseApplyClass.FirstOrDefault(x => x.CaseApplyClassSN == data.CaseApplyClassSN);
                var checkedcaseData = db.CaseApplyClass.Where(x => x.CaseNo == caseData.CaseNo).ToList();
                var WebSiteID = caseData.WebSiteID;
                if (checkedcaseData.Count > 1)
                {
                    var hotCaseWebSite = checkedcaseData.FirstOrDefault(x => x.WebSiteID != "MODA")?.WebSiteID;
                    if (hotCaseWebSite != null)
                    {
                        WebSiteID = hotCaseWebSite;
                    }
                }
                var webSiteData = db.SysWebSite.FirstOrDefault(x => x.WebSiteID == WebSiteID);
                var MailBoxPage = MailBoxService.GetCaseApplyPage("ResetMail");

                if (userList != null && userList.Count() > 0)
                {

                    var users = string.Join(";", userList);
                    var detail = MailBoxPage.PageContent;

                    detail = detail
                        .Replace("[Title]", webSiteData.Title)
                        .Replace("[CaseNo]", data.CaseNo)
                        .Replace("[Subject]", data.Subject)
                        .Replace("[LocalUrl]", localUrl);

                    MailInfoModel mailInfo = new MailInfoModel()
                    {
                        Type = "MailBox",
                        ToMail = users,
                        Subject = MailBoxPage.PageTitle,
                        Body = detail,

                    };
                    Utility.Mail.Send(mailInfo, out Exception exception);

                    // Utility.Mail.SendMailBox(users, $@"數位發展部民意信箱─案件退回請重新指派", detail, true, out Exception outex);
                }
            }


        }




        /// <summary>
        /// 新增api紀錄
        /// </summary>
        /// <param name="CaseModel"></param>
        /// <param name="ReplyModel"></param>
        /// <param name="CreatedUser"></param>
        public static void SpeedApiLog(returnAddNewCaseModel ReplyModel, int CaseApplySN, AddNewCaseModel addNewCaseModel, string CreatedUser)
        {
            var requestJson = Newtonsoft.Json.JsonConvert.SerializeObject(addNewCaseModel);

            CaseApplySpeedApiLog log = new CaseApplySpeedApiLog
            {
                CaseApplySn = CaseApplySN,
                Action = "AddNewCase",
                StatusCode = ReplyModel.StatusCode,
                Request = requestJson,
                Message = ReplyModel.Context,
                ApiStatus = ReplyModel.ApiStatus,
                ApiMessage = ReplyModel.Message,
                CreatedUserId = CreatedUser,
                CreatedDate = ReplyModel.SendDateTime,
            };
            SpeedApiLog(log);
        }

        public static void SpeedApiLog(returnFileModel ReplyFileModel, int CaseApplySN, string CreatedUser)
        {
            CaseApplySpeedApiLog log = new CaseApplySpeedApiLog
            {
                CaseApplySn = CaseApplySN,
                Action = "UploadAttachment",
                StatusCode = ReplyFileModel.StatusCode,
                Message = ReplyFileModel.Context,
                ApiStatus = ReplyFileModel.ApiStatus,
                ApiMessage = ReplyFileModel.Message,
                CreatedUserId = CreatedUser,
                CreatedDate = ReplyFileModel.SendDateTime,
            };
            SpeedApiLog(log);
        }

        public static void SpeedApiLog(returnSearchCaseModel ReplyModel, int CaseApplySN, SearchCaseModel searchCaseModel, string CreatedUser)
        {
            var requestJson = Newtonsoft.Json.JsonConvert.SerializeObject(searchCaseModel);

            var log = new CaseApplySpeedApiLog
            {
                CaseApplySn = CaseApplySN,
                Action = "SearchCase",
                StatusCode = ReplyModel.StatusCode,
                Request = requestJson,
                Message = ReplyModel.Context,
                ApiStatus = ReplyModel.ApiStatus,
                ApiMessage = ReplyModel.Message,
                CreatedUserId = CreatedUser,
                CreatedDate = ReplyModel.SendDateTime,
            };
            SpeedApiLog(log);
        }

        public static void SpeedApiLog(CaseApplySpeedApiLog speedApiLog)
        {
            try
            {
                using (var db = new MODAContext())
                {
                    db.CaseApplySpeedApiLog.Add(speedApiLog);
                    db.SaveChanges();
                }
            }
            catch (Exception)
            {

            }
        }
        /// <summary>
        /// 取的API回傳的退回資料
        /// </summary>
        /// <param name="CaseApplySN"></param>
        /// <returns></returns>
        public static List<CaseApplySpeedApiLog> GetReturnLog(int CaseApplySN)
        {
            using (var db = new MODAContext())
            {
                var data = db.CaseApplySpeedApiLog.Where(x =>
                x.CaseApplySn == CaseApplySN &&
                x.Action == "SearchCase" &&
                x.Message.Contains("公文已銷號")
                ).ToList();
                return data;
            }
        }
        public static List<SpeedLogModel> GetSpeedLog(string websiteId, string strDate, string endDate, string caseno, ref DefaultPager pager)
        {
            var allList = GetSpeedLogList(websiteId, strDate, endDate, caseno);

            var allCount = allList.Count();
            var list = allList.Skip((pager.p - 1) * pager.DisplayCount).Take(pager.DisplayCount).ToList();
            pager.TotalCount = allCount;
            pager.PageIndex = pager.p - 1;

            return list.ToList();
        }

        public static List<SpeedLogModel> GetSpeedLog(string websiteId, string strDate, string endDate, string caseno)
        {
            return GetSpeedLogList(websiteId, strDate, endDate, caseno);
        }

        private static List<SpeedLogModel> GetSpeedLogList(string websiteId, string strDate, string endDate, string caseno)
        {
            using (var db = new MODAContext())
            {
                var _strDate = DateDateTime(strDate);
                var _endDate = DateDateTime(endDate, false);
                var List = from a in db.CaseApplySpeedApiLog
                           join b in db.CaseApply on a.CaseApplySn equals b.CaseApplySN
                           where (string.IsNullOrWhiteSpace(websiteId) ? 1 == 1 : b.WebSiteId == websiteId) &&
                                 (string.IsNullOrWhiteSpace(strDate) ? 1 == 1 : a.CreatedDate >= _strDate) &&
                                 (string.IsNullOrWhiteSpace(endDate) ? 1 == 1 : a.CreatedDate <= _endDate) &&
                                 (string.IsNullOrWhiteSpace(caseno) ? 1 == 1 : (b.CaseNo == caseno || a.Message.Contains(caseno)))
                           select new SpeedLogModel
                           {
                               Action = ActionTxt(a.Action),
                               Success = StatusCodeTxt(a.StatusCode),
                               ApiStatus = ApiStatusTxt(a.ApiStatus),
                               ApiMessage = a.ApiMessage,
                               CaseNo = b.CaseNo,
                               CreateDate = a.CreatedDate,
                               CreateUser = CreateUserTxt(a.CreatedUserId),
                               Message = a.Message,
                               Requset = a.Request,
                           };

                return List.OrderByDescending(x => x.CreateDate).ToList();
            }
        }

        public static List<SurveyModel> GetSurvey(string websiteId, string strDate, string endDate, ref DefaultPager pager)
        {
            var allList = GetSurveyList(websiteId, strDate, endDate);

            var allCount = allList.Count();
            var list = allList.Skip((pager.p - 1) * pager.DisplayCount).Take(pager.DisplayCount).ToList();
            pager.TotalCount = allCount;
            pager.PageIndex = pager.p - 1;

            return list.ToList();
        }

        public static List<SurveyModel> GetSurvey(string websiteId, string strDate, string endDate)
        {
            return GetSurveyList(websiteId, strDate, endDate);
        }

        /// <summary>
        /// ~/MailBox/CaseApply/ReSet
        /// </summary>
        /// <param name="SysSectionSN"></param>
        /// <returns></returns>
        public static List<string> GetMailBoxResetList()
        {
            using (var db = new MODAContext())
            {
                var data = (from a in db.SysSection
                            join b in db.AuthSysGroupSysSection on a.SysSectionSN equals b.SysSectionSN
                            join c in db.SysGroup on b.SysGroupSN equals c.SysGroupSN
                            join d in db.RelSysUserGroup on c.SysGroupSN equals d.SysGroupSN
                            join e in db.SysUser on d.UserID equals e.UserID
                            where a.ActionPath == "~/MailBox/CaseApply/ReSet"
                            && e.UserSatus == "1"
                            select e.Email).ToList();
                return data;
            }
        }
        private static List<SurveyModel> GetSurveyList(string websiteId, string strDate, string endDate)
        {
            using (var db = new MODAContext())
            {
                var _strDate = DateDateTime(strDate);
                var _endDate = DateDateTime(endDate, false);
                var List = from a in db.CaseApplySurvey
                           join b in db.CaseApply on a.CaseApplySn equals b.CaseApplySN
                           where b.WebSiteId == websiteId &&
                                   (string.IsNullOrWhiteSpace(strDate) ? 1 == 1 : a.CreateDate >= _strDate) &&
                                   (string.IsNullOrWhiteSpace(endDate) ? 1 == 1 : a.CreateDate <= _endDate)
                           select new SurveyModel
                           {
                               CaseSatisfy = a.CaseSatisfy.ToString(),
                               CaseSolved = SolvedTxt(a.CaseSolved),
                               CaseDefect = DefectTxt(a.CaseDefect),
                               CaseProposal = a.CaseProposal,
                               CaseNo = b.CaseNo,
                               CreateDate = (DateTime)a.CreateDate,
                           };

                return List.OrderByDescending(x => x.CreateDate).ToList();
            }
        }

        public static Statistics GetStatistics(string websiteID, string strDate, string endDate)
        {
            using (var db = new MODAContext())
            {
                var finishStates = new List<string>() {
               "12","13","14"
                };

                var _strDate = DateDateTime(strDate);
                var _endDate = DateDateTime(endDate, false);

                var S = (from a in db.CaseApply
                         join b in db.CaseApplySurvey on a.CaseApplySN equals b.CaseApplySn into ab
                         from c in ab.DefaultIfEmpty()
                         where finishStates.Contains(a.Status)
                            && a.WebSiteId == websiteID
                            && (string.IsNullOrWhiteSpace(strDate) ? 1 == 1 : a.CreateDate >= _strDate)
                            && (string.IsNullOrWhiteSpace(endDate) ? 1 == 1 : a.CreateDate <= _endDate)
                         select new Statistics
                         {
                             CaseCount = 1,
                             SurveyCount = c != null ? 1 : 0,
                             SatisfyCount = c != null ? c.CaseSatisfy : 0,
                             SolvedCount_A = c != null && c.CaseSolved == "A" ? 1 : 0,
                             SolvedCount_B = c != null && c.CaseSolved == "B" ? 1 : 0,
                             SolvedCount_C = c != null && c.CaseSolved == "C" ? 1 : 0,
                             SolvedCount_D = c != null && c.CaseSolved == "D" ? 1 : 0,
                             DefectCount_A = c != null && c.CaseDefect.IndexOf("A") > -1 ? 1 : 0,
                             DefectCount_B = c != null && c.CaseDefect.IndexOf("B") > -1 ? 1 : 0,
                             DefectCount_C = c != null && c.CaseDefect.IndexOf("C") > -1 ? 1 : 0,
                             DefectCount_D = c != null && c.CaseDefect.IndexOf("D") > -1 ? 1 : 0,
                             DefectCount_E = c != null && c.CaseDefect.IndexOf("E") > -1 ? 1 : 0,
                             DefectCount_F = c != null && c.CaseDefect.IndexOf("F") > -1 ? 1 : 0,
                             DefectCount_G = c != null && c.CaseDefect.IndexOf("G") > -1 ? 1 : 0,
                             DefectCount_H = c != null && c.CaseDefect.IndexOf("H") > -1 ? 1 : 0,
                         }).GroupBy(x => x.CaseCount)
                         .Select(x => new Statistics
                         {
                             CaseCount = x.Sum(y => y.CaseCount),
                             SurveyCount = x.Sum(y => y.SurveyCount),
                             SatisfyCount = x.Sum(y => y.SatisfyCount),
                             SolvedCount_A = x.Sum(y => y.SolvedCount_A),
                             SolvedCount_B = x.Sum(y => y.SolvedCount_B),
                             SolvedCount_C = x.Sum(y => y.SolvedCount_C),
                             SolvedCount_D = x.Sum(y => y.SolvedCount_D),
                             DefectCount_A = x.Sum(y => y.DefectCount_A),
                             DefectCount_B = x.Sum(y => y.DefectCount_B),
                             DefectCount_C = x.Sum(y => y.DefectCount_C),
                             DefectCount_D = x.Sum(y => y.DefectCount_D),
                             DefectCount_E = x.Sum(y => y.DefectCount_E),
                             DefectCount_F = x.Sum(y => y.DefectCount_F),
                             DefectCount_G = x.Sum(y => y.DefectCount_G),
                             DefectCount_H = x.Sum(y => y.DefectCount_H),
                         }).FirstOrDefault();

                return S;
            }
        }

        public static List<CaseApplyModel> GetCaseReconfirm(string websiteId, string strDate, string endDate, ref DefaultPager pager, int SysDepartmentSN = 0, string keyword = "", string CaseApplyClassSN = "")
        {
            var _strDate = DateDateTime(strDate);
            var _endDate = DateDateTime(endDate, false);
            var data = new List<CaseApplyModel>();
            using (var db = new MODAContext())
            {
                var _data1 = db.CaseApply.Where(x =>
                x.WebSiteId == websiteId &&
                (string.IsNullOrWhiteSpace(strDate) ? 1 == 1 : x.AcceptDate >= _strDate) &&
                (string.IsNullOrWhiteSpace(endDate) ? 1 == 1 : x.AcceptDate <= _endDate) &&
                (string.IsNullOrWhiteSpace(CaseApplyClassSN) ? 1 == 1 : x.CaseApplyClassSN == int.Parse(CaseApplyClassSN)) &&
                (x.Status == EnumTpye.GetEnumNumberToSting(EnumCassApplyStatus.step5) || x.Status == EnumTpye.GetEnumNumberToSting(EnumCassApplyStatus.step6)));

                var _data = (from a in _data1
                             join b in db.CaseApplyClass on a.CaseApplyClassSN equals b.CaseApplyClassSN
                             join c in db.SysDepartment on b.SysDepartmentSN equals c.SysDepartmentSN
                             where ((SysDepartmentSN == 0) ? 1 == 1 : c.SysDepartmentSN == SysDepartmentSN)
                             && (string.IsNullOrWhiteSpace(keyword) ? 1 == 1 : (c.DepartmentName.Contains(keyword) || a.Subject.Contains(keyword) || a.ApplyUser.Contains(keyword) || a.CaseNo.Contains(keyword) || a.ReplyCaseNo.Contains(keyword) || a.ContactEmail.Contains(keyword) || a.CaseContent.Contains(keyword)))
                             && (string.IsNullOrWhiteSpace(strDate) ? 1 == 1 : a.CreateDate >= _strDate)
                             && (string.IsNullOrWhiteSpace(endDate) ? 1 == 1 : a.CreateDate <= _endDate)
                             select new CaseApplyModel()
                             {
                                 CaseNo = a.CaseNo,
                                 CaseName = b.CaseName,
                                 depName = c.DepartmentName,
                                 Subject = a.Subject,
                                 ContactEmail = a.ContactEmail,
                                 AcceptDate = a.AcceptDate,
                                 ReplySource = a.ReplySource,
                                 CaseApplySN = a.CaseApplySN,
                                 Status = a.Status,
                                 CreateDate = a.CreateDate,
                                 EffectiveDate = a.EffectiveDate
                             });

                var allCount = _data.Count();
                var list = _data.OrderByDescending(x => x.CreateDate).Skip((pager.p - 1) * pager.DisplayCount).Take(pager.DisplayCount).ToList();
                pager.TotalCount = allCount;
                pager.PageIndex = pager.p - 1;
                return list;
            }
        }

        /// <summary>
        /// 查詢改分Log
        /// </summary>
        /// <param name="caseApplySN"></param>
        /// <returns></returns>
        public static List<ResetLogModel> GetResetLog(int caseApplySN)
        {
            List<ResetLogModel> result = new();
            using (var db = new MODAContext())
            {
                var logs = db.LogAction.Where(x => x.Action == "ReSetCaseApply" && x.SourceSN == caseApplySN).ToList();

                foreach (var log in logs)
                {
                    try
                    {
                        var vs = log.MessageResult.Split(',');
                        var classFrom = vs.Length > 0 ? GetCase(int.Parse(vs[0])) : null;
                        var classTo = vs.Length > 1 ? GetCase(int.Parse(vs[1])) : null;
                        var replyCaseNo = vs.Length > 2 ? vs[2] ?? "" : "";

                        result.Add(new ResetLogModel
                        {
                            ResetSourse = "官網後台",
                            ResetFrom = classFrom?.DepName ?? "",
                            ResetFromClassName = classFrom?.CaseName ?? "",
                            ResetTo = classTo?.DepName ?? "",
                            ResetToClassName = classTo?.CaseName ?? "",
                            ResetDate = log.CreatedDate,
                            ReplyCaseNo = replyCaseNo
                        });
                    }
                    catch
                    {
                        result.Add(new ResetLogModel
                        {
                            ResetDate = log.CreatedDate
                        });
                    }
                }
            }

            return result;
        }
        /// <summary>
        /// 查詢公文系統退文(銷號)紀錄
        /// </summary>
        public static List<CaseApplySpeedApiLogModel> GetSpeedRetuenLog(int caseSN)
        {
            var Logs = MailBoxService.GetReturnLog(caseSN);
            var LogDatas = new List<CaseApplySpeedApiLogModel>();
            foreach (var log in Logs.OrderByDescending(x => x.SpeedApiLogSn))
            {
                var Message = Newtonsoft.Json.JsonConvert.DeserializeObject<returnSearchCaseModel>(log.Message);
                LogDatas.Add(new CaseApplySpeedApiLogModel()
                {
                    speedApiLogSn = log.SpeedApiLogSn,
                    returnCaseNo = Message.Data.CaseReplyStatus[0].CaseNo,
                    returnMessage = Message.Data.CaseProcessStatus.Status,
                    closedat = Message.Data.CaseProcessStatus.Closedat,
                    DocDept = CommonService.GetWebsiteDept().FirstOrDefault(x => x.DepartmentID == Message.Data.CaseReplyStatus[0].DocDept)?.DepartmentName ?? ""
                });
            }

            return LogDatas;
        }

        public static ReplySource GetReplySource(CaseApply caseApply)
        {
            ReplySource result = new();

            var repSource = EnumTpye.GetEnum<EnumReplySource>(caseApply.ReplySource ?? "0");
            result.ReplySourceName = EnumTpye.GetEnumDescription(repSource);
            switch (repSource)
            {
                case EnumReplySource.Speed:
                    result.ReplyDate = caseApply.ReplyDate.ToString();
                    result.ReplyDocName = CommonService.GetAllDepartments().FirstOrDefault(x => x.DepartmentID == caseApply.DocDept)?.DepartmentName;
                    result.ReplyUser = "";
                    break;
                case EnumReplySource.Mgr:
                    result.ReplyDate = caseApply.ReplySource2Date.ToString();
                    result.ReplyDocName = string.IsNullOrWhiteSpace(caseApply.ProcessUser) ? "系統自動回覆" : CommonService.GetDeptTree(CommonService.GetDeptByUser(caseApply.ProcessUser)?.SysDepartmentSN ?? 0);
                    result.ReplyUser = string.IsNullOrWhiteSpace(caseApply.ProcessUser) ? "系統自動回覆" : SYSUserService.GetUserData(caseApply.ProcessUser)?.UserName ?? "";
                    break;
                default:
                    return null;
            }

            return result;
        }

        #region 編輯頁面
        /// <summary>
        /// 抓取民意信箱頁面列表
        /// </summary>
        /// <returns></returns>
        public static List<CaseApplyPage> GetCaseApplyPages()
        {
            using var db = new MODAContext();
            return db.CaseApplyPage.OrderBy(x => x.SortOrder).ToList();
        }
        /// <summary>
        /// 抓取民意信箱頁面
        /// </summary>
        /// <param name="sn">頁面SN</param>
        /// <returns></returns>
        public static CaseApplyPage GetCaseApplyPage(int sn)
        {
            using var db = new MODAContext();
            return db.CaseApplyPage.FirstOrDefault(x => x.CaseApplyPageSn == sn);
        }
        /// <summary>
        /// 抓取民意信箱頁面
        /// </summary>
        /// <param name="actionName">頁面名稱</param>
        /// <returns></returns>
        public static CaseApplyPage GetCaseApplyPage(string actionName)
        {
            using var db = new MODAContext();
            return db.CaseApplyPage.FirstOrDefault(x => x.PageName == actionName);
        }
        /// <summary>
        /// 抓取民意信箱頁面擴充表
        /// </summary>
        /// <param name="pageSn">頁面SN</param>
        /// <returns></returns>
        public static List<CaseApplyPageExtend> GetCaseApplyPageExtends(int pageSn)
        {
            using var db = new MODAContext();
            return db.CaseApplyPageExtend.Where(x => x.CaseApplyPageSn == pageSn).OrderBy(x => x.SortOrder).ToList();
        }
        /// <summary>
        /// 修改民意信箱頁面
        /// </summary>
        /// <param name="page"></param>
        /// <param name="extends"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public static bool EditCaseApplyPage(CaseApplyPage page, List<CaseApplyPageExtend> extends, List<CommonFileModel> Imgs, out string errorMsg)
        {
            using var db = new MODAContext();
            List<string> errors = new();
            var dbPage = db.CaseApplyPage.FirstOrDefault(x => x.CaseApplyPageSn == page.CaseApplyPageSn);
            if (!(page == null || extends == null || dbPage == null))
            {
                if (string.IsNullOrEmpty(page.PageTitle))
                {
                    errors.Add("頁面標題為必填");
                }

                if (dbPage.ContentEnaled && string.IsNullOrEmpty(page.PageContent))
                {
                    errors.Add("頁面內容為必填");
                }

                if (extends.Count > 0)
                {
                    bool extendValid = true;
                    foreach (var extend in extends)
                    {
                        if (string.IsNullOrEmpty(extend.ExtendContent))
                        {
                            extendValid = false;
                        }
                    }
                    if (!extendValid) errors.Add("服務說明為必填");
                }
            }
            else
            {
                errors.Add("資料有誤");
            }

            if (errors.Count > 0)
            {
                errorMsg = string.Join(";", errors);
                return false;
            }
            else
            {
                dbPage.PageTitle = page.PageTitle;
                dbPage.PageContent = dbPage.ContentEnaled ? page.PageContent : null;
                dbPage.ProcessDate = DateTime.UtcNow.AddHours(8);
                dbPage.ProcessUser = page.ProcessUser;
                db.SaveChanges();

                if (extends.Count > 0)
                {
                    foreach (var extend in extends)
                    {
                        var dbExtend = db.CaseApplyPageExtend.FirstOrDefault(x => x.CaseApplyPageExtendSn == extend.CaseApplyPageExtendSn);
                        dbExtend.ExtendContent = extend.ExtendContent;
                        dbExtend.ProcessDate = DateTime.UtcNow.AddHours(8);
                        dbExtend.ProcessUser = extend.ProcessUser;
                        db.SaveChanges();
                    }
                }

                if (Imgs?.Count > 0)
                {
                    var nowNewsFileName = Imgs.Select(x => x.fileNewName).ToList();

                    var DBAllFile = (from a in db.RelWebFileContent
                                     join b in db.WEBFile on a.WEBFileSN equals b.WEBFileSN
                                     where a.SourceTable == "CaseApplyPage" &&
                                           a.SourceSN == page.CaseApplyPageSn
                                     select b).ToList();

                    var DBFileName = DBAllFile.Select(y => y.FileName).ToList();
                    //刪除的檔案
                    var NeedDeleteFiles = DBAllFile.Where(x => !nowNewsFileName.Contains(x.FileName)).ToList();
                    //新的檔案
                    var NewFiles = Imgs.Where(x => !DBFileName.Contains(x.fileNewName)).ToList();
                    // 刪除 先壓狀態
                    if (NeedDeleteFiles != null)
                    {
                        foreach (var file in NeedDeleteFiles)
                        {
                            file.IsEnable = "0";
                            var RelWebFileContentData = db.RelWebFileContent.First(x => x.WEBFileSN == file.WEBFileSN);
                            db.WEBFile.Update(file);
                            db.RelWebFileContent.Remove(RelWebFileContentData);
                            db.SaveChanges();
                        }
                    }
                    //新增
                    if (NewFiles != null)
                    {
                        foreach (var file in NewFiles)
                        {
                            var FileData = db.WEBFile.First(x => x.FileName == file.fileNewName);
                            var RelWebFileContentData = new RelWebFileContent()
                            {
                                WEBFileSN = FileData.WEBFileSN,
                                SourceTable = "CaseApplyPage",
                                SourceSN = page.CaseApplyPageSn,
                                GroupID = file.GroupID.ToString(),
                                CreatedUserID = page.ProcessUser,
                                CreatedDate = DateTime.UtcNow.AddHours(8),
                                SortOrder = file.FileSort,
                            };
                            FileData.IsEnable = "1";
                            db.WEBFile.Update(FileData);
                            db.RelWebFileContent.Add(RelWebFileContentData);
                            db.SaveChanges();
                        }
                    }
                    //修改fileTitle
                    foreach (var file in Imgs)
                    {
                        var filedata = db.WEBFile.First(x => x.FileName == file.fileNewName);
                        filedata.FileTitle = file.fileTitle;
                        var RelWebFileContent = db.RelWebFileContent.FirstOrDefault(x => x.WEBFileSN == filedata.WEBFileSN);
                        RelWebFileContent.SortOrder = file.FileSort;

                        db.WEBFile.Update(filedata);
                        db.RelWebFileContent.Update(RelWebFileContent);
                        db.SaveChanges();

                    }
                }
                else
                {
                    var DBAllFile = (from a in db.RelWebFileContent
                                     join b in db.WEBFile on a.WEBFileSN equals b.WEBFileSN
                                     where a.SourceTable == "CaseApplyPage" &&
                                           a.SourceSN == page.CaseApplyPageSn
                                     select b).ToList();
                    // 刪除 先壓狀態
                    if (DBAllFile != null)
                    {
                        foreach (var file in DBAllFile)
                        {
                            file.IsEnable = "0";
                            var RelWebFileContentData = db.RelWebFileContent.First(x => x.WEBFileSN == file.WEBFileSN);
                            db.WEBFile.Update(file);
                            db.RelWebFileContent.Remove(RelWebFileContentData);
                            db.SaveChanges();
                        }
                    }
                }
            }

            errorMsg = "";
            return true;
        }


        #endregion

        private static string ApiStatusTxt(string ApiStatus)
        {
            switch (ApiStatus)
            {
                case "1": return "成功";
                case "2": return "失敗";
                case "3": return "錯誤";

                default: return "";
            }
        }

        private static string StatusCodeTxt(string ApiStatus)
        {
            switch (ApiStatus)
            {
                case "OK": return "成功";
                default: return "失敗";
            }
        }

        private static string CreateUserTxt(string CreateUser)
        {
            switch (CreateUser)
            {
                case "MailBox": return "信箱前台";
                case "Console": return "信箱排程";
                default: return CreateUser;
            }
        }

        private static string ActionTxt(string Action)
        {
            switch (Action)
            {
                case "UploadAttachment": return "上傳檔案";
                case "AddNewCase": return "建立新案";
                case "SearchCase": return "查詢案件";
                case "DownloadAttachment": return "下載檔案";
                default: return Action;
            }
        }

        private static string SolvedTxt(string Solved)
        {
            switch (Solved)
            {
                case "A": return "完全解決";
                case "B": return "部份解決";
                case "C": return "沒有解決";
                case "D": return "提出建議";
                default: return Solved;
            }
        }

        /// <summary>
        /// 跨機關分類兩機關都需要看到，判斷分類代碼第一碼
        /// </summary>
        /// <param name="websiteID"></param>
        /// <returns></returns>
        private static string GetClassStartWith(string websiteID)
        {
            switch (websiteID)
            {
                case "MODA": return "0";
                case "ADI": return "1";
                case "ACS": return "2";
                default: return "9";
            }
        }

        private static string DefectTxt(string Defect)
        {
            Defect = Defect.Replace("A", "無");
            Defect = Defect.Replace("B", "未解決");
            Defect = Defect.Replace("C", "態度不佳");
            Defect = Defect.Replace("D", "時間太慢");
            Defect = Defect.Replace("E", "欠缺誠意");
            Defect = Defect.Replace("F", "引用錯誤");
            Defect = Defect.Replace("G", "推諉責任");
            Defect = Defect.Replace("H", "與實情不符");

            return Defect;
        }
        static string GetNewCaseNo()
        {
            var nt = DateTime.UtcNow.AddHours(8).ToString("yyyyMMdd");
            var rd = Utility.Regular.GetRandomString(6, RegularType.number);
            var chkNo = $@"{nt}{rd}";
            var chkNoItems = new List<string>();
            using (var db = new MODAContext())
            {
                var Items = db.CaseApply.Where(x => x.CaseNo.Substring(0, 8) == nt)?.Select(x => x.CaseNo).ToList();
                if (Items.Count() == 0) { return chkNo; }
                chkNoItems = Items;
            }
            ChkNewCaseNo(chkNoItems, ref chkNo);
            return chkNo;
        }
        static void ChkNewCaseNo(List<string> itmes, ref string no)
        {
            var chkNo = no;
            if (itmes.Any(x => x == chkNo))
            {
                var nt = DateTime.UtcNow.AddHours(8).ToString("yyyyMMdd");
                var rd = Utility.Regular.GetRandomString(6, RegularType.number);
                chkNo = $@"{nt}{rd}";
                ChkNewCaseNo(itmes, ref chkNo);
            }
        }

        public static void ChangeCaseStatus(CaseApply Case, string status)
        {
            try
            {
                using (var db = new MODAContext())
                {
                    var _Case = db.CaseApply.FirstOrDefault(x => x.CaseApplySN == Case.CaseApplySN);
                    if (_Case != null)
                    {
                        _Case.Status = status;
                        _Case.ProcessDate = DateTime.UtcNow.AddHours(8);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        public static void ChangeCaseValidateStatus(CaseApplyValidate CaseVailDate, string status)
        {
            try
            {
                using (var db = new MODAContext())
                {
                    var _Case = db.CaseApplyValidate.FirstOrDefault(x => x.CaseValidateSN == CaseVailDate.CaseValidateSN);
                    if (_Case != null)
                    {
                        _Case.Status = status;
                        _Case.ProcessDate = DateTime.UtcNow.AddHours(8);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception)
            {

            }
        }
    }
}
