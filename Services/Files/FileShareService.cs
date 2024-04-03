using DBModel;
using Microsoft.AspNetCore.Http;
using Services.Authorization;
using Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Utility.Files;

namespace Services.Files
{
    public class FileShareService
    {
        public static bool Save(string path, List<CommonFileModel> Files, sysUserModel userData)
        {
            try
            {
                using (var db = new MODAContext())
                {
                    foreach (var item in Files)
                    {
                        var dbfile = db.WEBFile.FirstOrDefault(x => x.WEBFileID == item.webFileID);
                        if (dbfile != null) 
                        {
                            dbfile.IsEnable = "1";
                            db.SaveChanges();
                        }
                    }

                    var deletedfile = db.WEBFile.AsEnumerable().Where(x => x.FilePath.Replace(x.FileName, "").Equals(path + "/") 
                                        && x.IsEnable == "1" && !Files.Any(y => y.webFileID.Equals(x.WEBFileID))).ToList();

                    foreach (var item in deletedfile)
                    {
                        var dbfile = db.WEBFile.FirstOrDefault(x => x.WEBFileID == item.WEBFileID);
                        if (dbfile != null)
                        {
                            item.IsEnable = "0";
                            db.SaveChanges();
                        }
                    }

                }
                return true;
            }
            catch (Exception ex)
            {
                Utility.Mail.Error(ex.ToString());
                LogService.CreateLogAction(new LogAction()
                {
                    Status = "0",
                    MessageResult = ex.ToString(),
                    ProcessIPAddress = userData.sysUser.ProcessIPAddress,
                    UserID = userData.sysUser.UserID,
                    WebSiteID = userData.WebSiteID,
                    WebPath = "",
                    ActionType = "1",
                    Action2 = "Update",
                    SourceTable = "WEBFile",
                    Action = "Save",
                    Controller = "FileShareService",
                    SourceSN = null,
                    CreatedDate = DateTime.UtcNow.AddHours(8)
                });
                return false;
            }
        }

        public static List<CommonFileModel> GetFileShareByPath(string currentNode, sysUserModel userData)
        {
            try
            {
                using (var db = new MODAContext())
                {
                    var dbfile = db.WEBFile.Where(x => x.FilePath.Replace(x.FileName,"").Equals(currentNode + "/") && x.IsEnable == "1");
                    if (dbfile.Count() > 0)
                    {
                        var sessionFile = dbfile.Select(x => new CommonFileModel()
                        {
                            fileExt = x.FileType,
                            fileNewName = x.FileName,
                            fileOriginName = x.OriginalFileName,
                            filePath = x.FilePath,
                            fileTitle = x.FileTitle,
                            fileSize = x.FileSize.Value,
                            webFileID = x.WEBFileID,
                            IsEnable = x.IsEnable
                        }).ToList();

                        return sessionFile;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                Utility.Mail.Error(ex.ToString());
                LogService.CreateLogAction(new LogAction()
                {
                    Status = "0",
                    MessageResult = ex.ToString(),
                    ProcessIPAddress = userData.sysUser.ProcessIPAddress,
                    UserID = userData.sysUser.UserID,
                    WebSiteID = userData.WebSiteID,
                    WebPath = "",
                    ActionType = "1",
                    Action2 = "Update",
                    SourceTable = "WEBFile",
                    Action = "Save",
                    Controller = "FileShareService",
                    SourceSN = null,
                    CreatedDate = DateTime.UtcNow.AddHours(8)
                });
                return null;
            }
        }

        public static List<CommonFileModel> GetFileShareByParentPath(string currentNode, sysUserModel userData)
        {
            try
            {
                using (var db = new MODAContext())
                {
                    var dbfile = db.WEBFile.Where(x => x.FilePath.StartsWith(currentNode) && x.IsEnable == "1");
                    if (dbfile.Count() > 0)
                    {
                        var sessionFile = dbfile.Select(x => new CommonFileModel()
                        {
                            fileExt = x.FileType,
                            fileNewName = x.FileName,
                            fileOriginName = x.OriginalFileName,
                            filePath = x.FilePath,
                            fileTitle = x.FileTitle,
                            fileSize = x.FileSize.Value,
                            webFileID = x.WEBFileID,
                            IsEnable = x.IsEnable
                        }).ToList();

                        return sessionFile;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                Utility.Mail.Error(ex.ToString());
                LogService.CreateLogAction(new LogAction()
                {
                    Status = "0",
                    MessageResult = ex.ToString(),
                    ProcessIPAddress = userData.sysUser.ProcessIPAddress,
                    UserID = userData.sysUser.UserID,
                    WebSiteID = userData.WebSiteID,
                    WebPath = "",
                    ActionType = "1",
                    Action2 = "Update",
                    SourceTable = "WEBFile",
                    Action = "Save",
                    Controller = "FileShareService",
                    SourceSN = null,
                    CreatedDate = DateTime.UtcNow.AddHours(8)
                });
                return null;
            }
        }

        public static bool CheckFileShareFileName(string path ,List<IFormFile> files) 
        {
            using (var db = new MODAContext())
            {
                try
                {
                    var filepath = ("/Uploads/" + path).Replace(@"\", @"/").Replace(@"//", @"/") + "/";

                    var deletedfile = db.WEBFile.AsEnumerable().Where(x => files.Any(y => (filepath + y.FileName).Equals(x.FilePath))
                        && x.IsEnable == "1").ToList();

                    return deletedfile.Count() > 0;
                }
                catch (Exception ex) { Utility.Mail.Error(ex.ToString()); return false; }
            }
        }

        public static void DeleteFile(string fileid, sysUserModel userData ,out int sn)
        {
            sn = 0;
            try
            {
                using (var db = new MODAContext())
                {
                    var dbfile = db.WEBFile.Where(x => x.WEBFileID == fileid && x.IsEnable == "1").FirstOrDefault();
                    if (dbfile != null)
                    {
                        sn = dbfile.WEBFileSN;
                        dbfile.IsEnable = "0";
                        dbfile.ProcessDate = DateTime.UtcNow.AddHours(8);
                        dbfile.ProcessIPAddress = userData.sysUser.ProcessIPAddress;
                        dbfile.ProcessUserID = userData.sysUser.UserID;
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
                    ProcessIPAddress = userData.sysUser.ProcessIPAddress,
                    UserID = userData.sysUser.UserID,
                    WebSiteID = userData.WebSiteID,
                    WebPath = "",
                    ActionType = "1",
                    Action2 = "Update",
                    SourceTable = "WEBFile",
                    Action = "Save",
                    Controller = "FileShareService",
                    SourceSN = null,
                    CreatedDate = DateTime.UtcNow.AddHours(8)
                });
            }
        }

        public static WEBFile GetFile(string fileid)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    return db.WEBFile.FirstOrDefault(x => x.WEBFileID == fileid);

                }
                catch (Exception ex) { Utility.Mail.Error(ex.ToString()); return null; }
            }
        }

    }
}
