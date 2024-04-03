using DBModel;
using Services.Models;
using System.Collections.Generic;
using Utility.Models.Authorization;
using Utility;
using System;
using System.Linq;

namespace Services.Authorization
{
    public class DepartmentManagementService
    {

        public static List<SysDepartment> GetDepartmentList()
        {
            using (var db = new MODAContext())
            {
                try
                {
                    return db.SysDepartment.Where(x => x.IsEnable != ((int)Utility.SysConst.IsEnable.Code.Del).ToString()).OrderBy(x => x.SortOrder).ToList();
                }
                catch (Exception ex)
                {
                    Utility.Mail.Error(ex.ToString());
                    LogService.CreateLogAction(new LogAction()
                    {
                        Status = "0",
                        MessageResult = ex.ToString(),
                        ProcessIPAddress = "",
                        UserID = "",
                        WebSiteID = "",
                        WebPath = "",
                        ActionType = "1",
                        Action2 = "select",
                        SourceTable = "SysDepartment",
                        Action = "GetDepartmentList",
                        Controller = "DepartmentManagementService",
                        SourceSN = 0,
                        CreatedDate = DateTime.UtcNow.AddHours(8)
                    });
                    return null;
                }
            }
        }

        public static List<SysDepartment> GetDepartmentByKeys(int? ParentID, string websiteid, string states, ref DefaultPager pager)
        {
            try
            {
                using (var db = new MODAContext())
                {
                    var list = db.SysDepartment.Where(x => 1 == 1
                    && (ParentID == null ? 1 == 1 : x.ParentID == ParentID)
                    && x.IsEnable != ((int)Utility.SysConst.IsEnable.Code.Del).ToString()
                    && x.WebSiteId == websiteid);
                    var allData = list.Count();
                    pager.TotalCount = allData;
                    pager.PageIndex = pager.p - 1;
                    //可以下ORDER BY 條件
                    var searchData = list.OrderBy(o => o.SortOrder).Skip((pager.p - 1) * pager.DisplayCount).Take(pager.DisplayCount).ToList();
                    return searchData;
                }
            }
            catch (Exception ex)
            {
                Utility.Mail.Error(ex.ToString());
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
                    SourceTable = "SysDepartment",
                    Action = "GetDepartmentByKeys",
                    Controller = "DepartmentManagementService",
                    SourceSN = 0,
                    CreatedDate = DateTime.UtcNow.AddHours(8)
                });
            }
            return null;
        }

        public static List<SysDepartment> GetDepartmentBySysDepartmentSN(int DepartmentSN)
        {
            try
            {
                using (var db = new MODAContext())
                {
                    return db.SysDepartment.Where(x => x.MainSN == DepartmentSN).ToList();

                }
            }
            catch (Exception ex)
            {
                Utility.Mail.Error(ex.ToString());
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
                    SourceTable = "SysDepartment",
                    Action = "GetDepartmentByKeys",
                    Controller = "DepartmentManagementService",
                    SourceSN = 0,
                    CreatedDate = DateTime.UtcNow.AddHours(8)
                });
            }
            return null;
        }

        public static List<SysDepartment> GetDepartmentBySysDepartmentID(string DepartmentID, string WebSiteID)
        {
            try
            {
                using (var db = new MODAContext())
                {
                    return db.SysDepartment.Where(x => x.DepartmentID == DepartmentID && x.WebSiteId == WebSiteID).ToList();

                }
            }
            catch (Exception ex)
            {
                Utility.Mail.Error(ex.ToString());
                LogService.CreateLogAction(new LogAction()
                {
                    Status = "0",
                    MessageResult = ex.ToString(),
                    ProcessIPAddress = "",
                    UserID = "",
                    WebSiteID = "",
                    WebPath = "",
                    ActionType = "1",
                    Action2 = "PageView",
                    SourceTable = "SysDepartment",
                    Action = "GetDepartmentByKeys",
                    Controller = "DepartmentManagementService",
                    SourceSN = 0,
                    CreatedDate = DateTime.UtcNow.AddHours(8)
                });
            }
            return null;
        }


        public static sysDepartmentModel Create(SysDepartment department, string webSiteID)
        {
            sysDepartmentModel sysDepartmentModel = new sysDepartmentModel();
            try
            {
                var sysWebSiteLangs = CommonService.GetSysWebSiteLang(webSiteID);
                using (var db = new MODAContext())
                {
                    var ad = db.SysDepartment.FirstOrDefault(x => x.DepartmentID == department.DepartmentID.Trim() && x.IsEnable != "-99");
                    if (ad != null)
                    {
                        sysDepartmentModel.check = false;
                        sysDepartmentModel.message = "部門代號已存在";
                        return sysDepartmentModel;
                    }
                    if (department.SortOrder == 0)
                    {
                        var parentID = department.ParentID.Value;
                        int SortMax = db.SysDepartment.Where(x => x.ParentID == parentID).Select(m => m.SortOrder).DefaultIfEmpty().Max();
                        department.SortOrder = SortMax + 1;
                    }
                    department.Lang = "zh-tw";
                    db.SysDepartment.Add(department);
                    db.SaveChanges();

                    //Update MainSN
                    department.MainSN = department.SysDepartmentSN;
                    db.SaveChanges();

                    //Add Langs
                    foreach (var lang in sysWebSiteLangs.Where(x => x.Lang != "zh-tw"))
                    {
                        var deplang = new SysDepartment
                        {
                            DepartmentID = department.DepartmentID,
                            ParentID = department.ParentID,
                            DepartmentName = department.DepartmentName,
                            ShortName = department.ShortName,
                            Description = department.Description,
                            IsEnable = department.IsEnable,
                            ProcessUserID = department.ProcessUserID,
                            ProcessDate = department.ProcessDate,
                            ProcessIPAddress = department.ProcessIPAddress,
                            CreatedDate = department.CreatedDate,
                            CreatedUserID = department.CreatedUserID,
                            SortOrder = department.SortOrder,
                            Lang = lang.Lang,
                            MainSN = department.SysDepartmentSN,
                            WebSiteId = department.WebSiteId
                        };

                        db.SysDepartment.Add(deplang);
                    }
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Utility.Mail.Error(ex.ToString());
                sysDepartmentModel.check = false;
                sysDepartmentModel.message = "新增失敗";
                LogService.CreateLogAction(new LogAction()
                {
                    Status = "0",
                    MessageResult = ex.ToString(),
                    ProcessIPAddress = department.ProcessIPAddress,
                    UserID = department.ProcessUserID,
                    WebSiteID = "",
                    WebPath = "",
                    ActionType = "1",
                    Action2 = "Add",
                    SourceTable = "SysDepartment",
                    Action = "Create",
                    Controller = "DepartmentManagementService",
                    SourceSN = 0,
                    CreatedDate = DateTime.UtcNow.AddHours(8)
                });
            }
            return sysDepartmentModel;
        }

        public static sysDepartmentModel CreateSubLang(SysDepartment department)
        {
            sysDepartmentModel sysDepartmentModel = new sysDepartmentModel();

            using (var db = new MODAContext())
            {
                try
                {
                    var main = db.SysDepartment.FirstOrDefault(x => x.SysDepartmentSN == department.MainSN);

                    department.DepartmentID = main.DepartmentID;
                    department.IsEnable = main.IsEnable;

                    var ad = db.SysDepartment.FirstOrDefault(x => x.DepartmentName == department.DepartmentName.Trim() && x.IsEnable != "-99");
                    if (ad != null)
                    {
                        sysDepartmentModel.check = false;
                        sysDepartmentModel.message = "部門名稱已存在";
                        return sysDepartmentModel;
                    }

                    db.SysDepartment.Add(department);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    Utility.Mail.Error(ex.ToString());
                    LogService.CreateLogAction(new LogAction()
                    {
                        Status = "0",
                        MessageResult = ex.ToString(),
                        ProcessIPAddress = "",
                        UserID = "",
                        WebSiteID = "",
                        WebPath = "",
                        ActionType = "1",
                        Action2 = "Add",
                        SourceTable = "SysDepartment",
                        Action = "CreateSubLang",
                        Controller = "DepartmentManagementService",
                        SourceSN = 0,
                        CreatedDate = DateTime.UtcNow.AddHours(8)
                    });

                    sysDepartmentModel.check = false;
                    sysDepartmentModel.message = "部門名稱已存在";
                    return sysDepartmentModel;
                }
            }

            return sysDepartmentModel;
        }

        public static sysDepartmentModel Edit(SysDepartment department)
        {
            sysDepartmentModel sysDepartmentModel = new sysDepartmentModel();
            try
            {
                using (var db = new MODAContext())
                {
                    if (department.SysDepartmentSN == department.MainSN)
                    {
                        var ad = db.SysDepartment.FirstOrDefault(x => x.DepartmentID == department.DepartmentID.Trim() && x.SysDepartmentSN != department.SysDepartmentSN && x.MainSN == x.SysDepartmentSN && x.WebSiteId == department.WebSiteId && x.IsEnable != "-99");
                        if (ad != null)
                        {
                            sysDepartmentModel.check = false;
                            sysDepartmentModel.message = "單位代號已存在";
                            return sysDepartmentModel;
                        }
                    }

                    var oldData = db.SysDepartment.FirstOrDefault(x => x.SysDepartmentSN == department.SysDepartmentSN);
                    if (oldData != null)
                    {
                        if (department.SysDepartmentSN == department.MainSN)
                        {
                            oldData.DepartmentID = department.DepartmentID.Trim();
                        }

                        oldData.DepartmentName = department.DepartmentName.Trim();
                        oldData.Description = department.Description;
                        oldData.IsEnable = department.IsEnable;
                        oldData.ProcessIPAddress = department.ProcessIPAddress;
                        oldData.ProcessUserID = department.ProcessUserID;
                        oldData.ProcessDate = DateTime.UtcNow.AddHours(8);
                        db.SysDepartment.Update(oldData);
                        db.SaveChanges();
                        sysDepartmentModel.check = true;
                        sysDepartmentModel.sysDepartment = oldData;
                    }

                    //中文修改英文部門代號
                    if (oldData.Lang == "zh-tw")
                    {
                        var otherLangData = db.SysDepartment.Where(x => x.MainSN == oldData.MainSN && x.Lang != oldData.Lang).ToList();
                        foreach (var langData in otherLangData)
                        {
                            langData.DepartmentID = department.DepartmentID.Trim();
                            db.SysDepartment.Update(langData);
                            db.SaveChanges();
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                Utility.Mail.Error(ex.ToString());
                sysDepartmentModel.check = false;
                sysDepartmentModel.message = "修改失敗";
                LogService.CreateLogAction(new LogAction()
                {
                    Status = "0",
                    MessageResult = ex.ToString(),
                    ProcessIPAddress = department.ProcessIPAddress,
                    UserID = department.ProcessUserID,
                    WebSiteID = "",
                    WebPath = "",
                    ActionType = "1",
                    Action2 = "Edit",
                    SourceTable = "SysDepartment",
                    Action = "Edit",
                    Controller = "DepartmentManagementService",
                    SourceSN = department.SysDepartmentSN,
                    CreatedDate = DateTime.UtcNow.AddHours(8)
                });
            }
            return sysDepartmentModel;
        }

        public static sysDepartmentModel Delete(int SysDepartmentSN)
        {
            sysDepartmentModel sysDepartmentModel = new sysDepartmentModel();

            try
            {
                using (var db = new MODAContext())
                {
                    var depDatas = db.SysDepartment.Where(x => x.MainSN == SysDepartmentSN).ToList();

                    foreach (var depData in depDatas)
                    {
                        //判斷此部門有沒有人員
                        var ad = db.SysUser.FirstOrDefault(x => x.DepartmentID == depData.DepartmentID);
                        if (ad != null)
                        {
                            sysDepartmentModel.check = false;
                            sysDepartmentModel.message = "部門尚有人員存在";
                            return sysDepartmentModel;
                        }
                        //判斷下層是否尚有部門
                        var ad2 = db.SysDepartment.FirstOrDefault(x => x.ParentID == SysDepartmentSN);
                        if (ad2 != null)
                        {
                            sysDepartmentModel.check = false;
                            sysDepartmentModel.message = "下層尚有部門";
                            return sysDepartmentModel;
                        }
                    }

                    //確認所有語系都沒有
                    foreach (var depData in depDatas)
                    {
                        depData.IsEnable = "-99";
                        depData.ProcessDate = DateTime.UtcNow.AddHours(8);
                        db.SysDepartment.Update(depData);
                    }

                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Utility.Mail.Error(ex.ToString());
                LogService.CreateLogAction(new LogAction()
                {
                    Status = "0",
                    MessageResult = ex.ToString(),
                    ProcessIPAddress = "",
                    UserID = "",
                    WebSiteID = "",
                    WebPath = "",
                    ActionType = "1",
                    Action2 = "Del",
                    SourceTable = "SysDepartment",
                    Action = "Delete",
                    Controller = "DepartmentManagementService",
                    SourceSN = 0,
                    CreatedDate = DateTime.UtcNow.AddHours(8)
                });

                sysDepartmentModel.check = false;
                sysDepartmentModel.message = ex.Message;
            }
            return sysDepartmentModel;
        }
        /// <summary>
        /// 部門停用
        /// 改成切換啟用/停用狀態
        /// </summary>
        /// <param name="department"></param>
        /// <returns></returns>
        public static bool StopDept(SysDepartment department)
        {
            using (var db = new MODAContext())
            {
                var Data = db.SysDepartment.FirstOrDefault(x => x.SysDepartmentSN == department.SysDepartmentSN);
                if (Data == null)
                {
                    return false;
                }
                else
                {
                    try
                    {
                        Data.IsEnable = Data.IsEnable == "0" ? "1" : "0";
                        Data.ProcessIPAddress = department.ProcessIPAddress;
                        Data.ProcessUserID = department.ProcessUserID;
                        Data.ProcessDate = DateTime.UtcNow.AddHours(8);
                        db.SysDepartment.Update(Data);
                        db.SaveChanges();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        Utility.Mail.Error(ex.ToString());
                        LogService.CreateLogAction(new LogAction()
                        {
                            Status = "0",
                            MessageResult = ex.ToString(),
                            ProcessIPAddress = "",
                            UserID = "",
                            WebSiteID = "",
                            WebPath = "",
                            ActionType = "1",
                            Action2 = "Del",
                            SourceTable = "SysDepartment",
                            Action = "StopDept",
                            Controller = "DepartmentManagementService",
                            SourceSN = 0,
                            CreatedDate = DateTime.UtcNow.AddHours(8)
                        });
                        return false;
                    }
                }
            }
        }

        public static List<SysUser> GetDeptUsers(string dep, ref DefaultPager pager)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    var list = db.SysUser.Where(x => x.DepartmentID == dep && x.UserSatus != "-99");
                    var allData = list.Count();
                    pager.TotalCount = allData;
                    pager.PageIndex = pager.p - 1;
                    //可以下ORDER BY 條件
                    var searchData = list.OrderBy(o => o.SortOrder).Skip((pager.p - 1) * pager.DisplayCount).Take(pager.DisplayCount).ToList();
                    return searchData;
                }
                catch (Exception ex)
                {
                    Utility.Mail.Error(ex.ToString());
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
                        SourceTable = "SysDepartment",
                        Action = "GetDeptUsers",
                        Controller = "DepartmentManagementService",
                        SourceSN = 0,
                        CreatedDate = DateTime.UtcNow.AddHours(8)
                    });

                    return null;
                }
            }
        }

        public static List<SysDepartment> GetDepartment(int key, string websiteid)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    var Data = db.SysDepartment.Where(x => x.ParentID == key && x.WebSiteId == websiteid);
                    Data = Data.Where(x => x.IsEnable != ((int)Utility.SysConst.IsEnable.Code.Del).ToString());
                    var searchData = Data.OrderBy(o => o.SortOrder).ToList();
                    return searchData;
                }
                catch (Exception ex)
                {
                    Utility.Mail.Error(ex.ToString());
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
                        SourceTable = "SysDepartment",
                        Action = "GetDepartment",
                        Controller = "DepartmentManagementService",
                        SourceSN = 0,
                        CreatedDate = DateTime.UtcNow.AddHours(8)
                    });
                    return null;
                }
            }
        }
        /// <summary>
        /// 部門重新排序
        /// </summary>
        /// <param name="key">SysDepartmentSN</param>
        /// <param name="sort">欲調整至序號</param>
        /// <param name="ProcessUserID"></param>
        /// <param name="ProcessIP"></param>
        public static void DeptReArrangeByChild(int key, int sort, string ProcessUserID, string ProcessIP)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    //向上找出父節點的所有子節點
                    var Tree = from m in db.SysDepartment.Where(m => m.SysDepartmentSN == key)
                               join d in db.SysDepartment.Where(m => m.IsEnable != "-99")
                                on new { m.ParentID } equals new { d.ParentID }
                               select d;

                    //找出當前節點的SortOrder
                    int originalSort = (from t in Tree
                                        where t.SysDepartmentSN == key
                                        select t.SortOrder).FirstOrDefault();
                    //插入新序號
                    foreach (var item in Tree)
                    {
                        if (sort < originalSort)
                        {
                            if (item.SortOrder >= sort)
                                item.SortOrder += 1;
                            if (item.SysDepartmentSN == key)
                                item.SortOrder = sort;
                        }
                        else
                        {
                            if (item.SortOrder > sort)
                                item.SortOrder += 1;
                            if (item.SysDepartmentSN == key)
                                item.SortOrder = sort + 1;
                        }
                    }
                    //重新排序
                    int i = 1;
                    var timeNow = DateTime.UtcNow.AddHours(8);
                    foreach (var item in Tree.AsEnumerable().OrderBy(x => x.SortOrder))
                    {
                        item.SortOrder = i;
                        i++;
                    }
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    Utility.Mail.Error(ex.ToString());
                    LogService.CreateLogAction(new LogAction()
                    {
                        Status = "0",
                        MessageResult = ex.ToString(),
                        ProcessIPAddress = "",
                        UserID = "",
                        WebSiteID = "",
                        WebPath = "",
                        ActionType = "1",
                        Action2 = "Edit",
                        SourceTable = "SysDepartment",
                        Action = "DeptReArrangeByChild",
                        Controller = "DepartmentManagementService",
                        SourceSN = 0,
                        CreatedDate = DateTime.UtcNow.AddHours(8)
                    });
                }
            }
        }

        public static void GetParentTitle(int? ParentID, ref List<SysDepartment> tiltes, int sort = 0)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    var DepartmentData = db.SysDepartment.OrderBy(x => x.SysDepartmentSN).Where(x => x.SysDepartmentSN == ParentID).FirstOrDefault();
                    if (DepartmentData != null)
                    {
                        tiltes.Add(new SysDepartment() { Description = DepartmentData.DepartmentName, SortOrder = sort });
                        if (!string.IsNullOrWhiteSpace(DepartmentData.ParentID.ToString()))
                        {
                            sort++;
                            GetParentTitle(DepartmentData.ParentID, ref tiltes, sort);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Utility.Mail.Error(ex.ToString());
                }
            }
        }
    }
}
