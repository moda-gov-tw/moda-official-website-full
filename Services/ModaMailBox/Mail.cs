using DBModel;
//using Grpc.Core;
using Services.Models.ModaMailBox;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using Utility;
using Utility.MailBox;
using static Utility.Files;

namespace Services.ModaMailBox
{
    public class MailBoxSendMail
    {
        public static string MailBoxUrl = "";

        public string actionKey;

        /// <summary>
        /// 信箱驗證信
        /// </summary>
        /// <param name="data"></param>
        public static bool verifyemailSend(CaseApplyValidate data, out string errorMsg)
        {
            errorMsg = "";
            try
            {
                var Url = $@"{MailBoxUrl}/writemail?token={data.Token}";
                var filePath = $@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}/wwwroot/mail/MailLayout.html";
                var MailBoxPage = MailBoxService.GetCaseApplyPage("VerifyMail");
                var mailLayout = File.ReadAllText(filePath);
                var mailTemplate = mailLayout.Replace("[MailArea]", MailBoxPage.PageContent);
                mailTemplate = mailTemplate
                   .Replace("[writemail]", Url)
                   .Replace("[EffectiveDate]", data.EffectiveDate.ToString("yyyy-MM-dd HH:mm:ss"));

                MailInfoModel mailInfo = new MailInfoModel()
                {
                    Type = "MailBox",
                    ToMail = data.Email.Trim(),
                    Subject = MailBoxPage.PageTitle,
                    Body = mailTemplate,
                };

                if (Mail.Send(mailInfo, out Exception exception))
                {
                    return true;
                }
                else
                {
                    errorMsg = exception.Message;
                    return false;
                }
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 案件確認信
        /// </summary>
        /// <param name="data"></param>
        /// <param name="casesModel"></param>
        /// <param name="type">0-確認信 1-已確認(已取消確認信流程)</param>
        public static bool writemailSend(CaseApply data, CasesModel casesModel, SysCategory sysCategory, out string errorMsg, int type = 0, List<SaveFileModel> saveFiles = null, string token = "")
        {
            errorMsg = "";
            try
            {
                var Url = $@"{MailBoxUrl}/confirmation?token={token}";

                var MailBoxPage = MailBoxService.GetCaseApplyPage("CaseInfoMail");

                var mailname = MailBoxPage.PageTitle;
                var filePath = $@"{Directory.GetCurrentDirectory()}/wwwroot/mail/MailLayout.html";
                var mailTemplate = System.IO.File.ReadAllText(filePath).Replace("[MailArea]", MailBoxPage.PageContent);

                //附件
                var filearea = "";
                var fileshow = "display:none;";
                if (saveFiles?.Count() > 0)
                {
                    fileshow = "";
                    filearea += string.Join("<br/>", saveFiles.Select(x => x.FileName).ToArray());
                }

                mailTemplate = mailTemplate
                    .Replace("[href]", Url)
                    .Replace("[CaseNo]", data.CaseNo)
                    .Replace("[CasePwd]", data.CasePwd)
                    .Replace("[CreateDate]", data.CreateDate.ToString("yyyy-MM-dd HH:mm"))
                    .Replace("[ApplyUser]", data.ApplyUser)
                    .Replace("[ContactEmail]", data.ContactEmail)
                    .Replace("[Tel]", string.IsNullOrWhiteSpace(data.Tel) ? "" : $@"{data.TelAreacode}-{data.Tel}" + (!string.IsNullOrEmpty(data.TelExtension) ? $" 分機 {data.TelExtension}" : ""))
                    .Replace("[Mobile]", data.Mobile)
                    .Replace("[WebName]", casesModel.WebSiteName)
                    .Replace("[SysCategory]", sysCategory?.Value ?? "")
                    .Replace("[CaseApplyClass]", casesModel.CaseName)
                    .Replace("[Subject]", data.Subject)
                    .Replace("[EffectiveDate]", data.EffectiveDate?.ToString("yyyy-MM-dd HH:mm:ss") ?? "")
                    .Replace("[FileShow]", fileshow)
                    .Replace("[FileArea]", filearea)
                    .Replace("[CaseContent]", data.CaseContent.Replace("\n", "<br>"));
                Exception outex = new Exception();
                MailInfoModel mailInfo = new MailInfoModel()
                {
                    Type = "MailBox",
                    ToMail = data.ContactEmail.Trim(),
                    Subject = mailname,
                    Body = mailTemplate,
                };


                if (Utility.Mail.Send(mailInfo, out Exception exception))
                {
                    return true;
                }
                else
                {
                    errorMsg = exception.Message;
                    return false;
                }
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 案件指派通知信
        /// </summary>
        /// <param name="data"></param>
        /// <param name="casesModel"></param>
        /// <param name="mailto"></param>
        public void writeAssignCase(CaseApply data, CasesModel casesModel, SysCategory sysCategory, List<CaseApplyClassTo> mailto)
        {
            try
            {
                if (mailto == null) return;
                var mailList = string.Join(";", mailto);

                var filePath = $@"{Directory.GetCurrentDirectory()}/wwwroot/mail/A-9-2.html";
                var mailTemplate = System.IO.File.ReadAllText(filePath);
                mailTemplate = mailTemplate
                    .Replace("[CaseNo]", data.CaseNo)
                    .Replace("[CasePwd]", data.CasePwd)
                    .Replace("[ReplyCaseNo]", data.ReplyCaseNo)
                    .Replace("[CreateDate]", data.CreateDate.ToString("yyyy-MM-dd HH:mm"))
                    .Replace("[ApplyUser]", data.ApplyUser)
                    .Replace("[WebName]", casesModel.WebSiteName)
                    .Replace("[SysCategory]", sysCategory?.Value ?? "")
                    .Replace("[CaseApplyClass]", casesModel.CaseName)
                    .Replace("[Subject]", data.Subject)
                    .Replace("[CaseContent]", data.CaseContent.Replace("\n", "<br>"));
                //Exception outex = new Exception();
                //Utility.Mail.Send(mailList.Trim(), $@"數位發展部民意信箱─{"民意信箱案件受理通知信"}", mailTemplate, true, ref outex);
                MailInfoModel mailInfo = new MailInfoModel()
                {
                    Type = "MailBox",
                    ToMail = data.ContactEmail.Trim(),
                    Subject = $@"數位發展部民意信箱─{"民意信箱案件受理通知信"}",
                    Body = mailTemplate,

                };
                Utility.Mail.Send(mailInfo, out Exception exception);
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// 案件回覆處理信
        /// </summary>
        /// <param name="data">案件資料</param>
        /// <param name="casesModel">分類</param>
        /// <param name="saveFiles">民眾上傳檔案</param>
        /// <param name="mailFiles">回覆夾檔</param>
        public static bool SendReplyMail(CaseApply data, CasesModel casesModel, SysCategory sysCategory, bool NeedSurvey, out string errorMsg, List<SaveFileModel> saveFiles = null, List<Utility.MailFile> mailFiles = null, string presetReply = "")
        {
            try
            {
                var MailBoxPage = MailBoxService.GetCaseApplyPage("ReplyMail");
                var filePath = $@"{Directory.GetCurrentDirectory()}/wwwroot/mail/MailLayout.html";
                var mailTemplate = System.IO.File.ReadAllText(filePath).Replace("[MailArea]", MailBoxPage.PageContent);
                //附件
                var filearea = "";
                var fileshow = "display:none;";
                if (saveFiles?.Count() > 0)
                {
                    fileshow = "";
                    filearea += string.Join("<br/>", saveFiles.Select(x => x.FileName).ToArray());
                }
                var suveryArea = "";
                if (data.Status != EnumTpye.GetEnumNumberToSting(EnumCassApplyStatus.step16) && NeedSurvey)
                {
                    suveryArea = MailBoxService.GetCaseApplyPage("SurveyArea").PageContent;
                    var url = $@"{MailBoxUrl}/survey";
                    suveryArea = suveryArea.Replace("[surveyurl]", url);
                }
                var FileNames = "";
                if (mailFiles?.Count() > 0)
                {
                    FileNames = string.Join("<br/>", mailFiles.Select(x => x.FileName).ToArray());
                }
                var ReplyContent = "";
                var ReplyDate = "";

                var replySource = EnumTpye.GetEnum<EnumReplySource>(data.ReplySource);
                switch (replySource)
                {
                    case EnumReplySource.Mgr:
                        ReplyContent = data.ReplySource2?.Replace("\n", "<br>");
                        ReplyDate = data.ReplySource2Date?.ToString("yyyy-MM-dd HH:mm:ss");
                        break;
                    case EnumReplySource.Speed:
                        ReplyContent = data.ReplyContent.Replace("\n", "<br>");
                        ReplyDate = data.ReplyDate?.ToString("yyyy-MM-dd HH:mm:ss");
                        break;
                }

                if (string.IsNullOrWhiteSpace(ReplyContent))
                {
                    ReplyContent = presetReply;
                }

                mailTemplate = mailTemplate
                    .Replace("[CaseNo]", data.CaseNo)
                    .Replace("[CasePwd]", data.CasePwd)
                    .Replace("[CreateDate]", data.CreateDate.ToString("yyyy-MM-dd HH:mm"))
                    .Replace("[AcceptDate]", data.AcceptDate?.ToString("yyyy-MM-dd HH:mm") ?? "")
                    .Replace("[ApplyUser]", data.ApplyUser)
                    .Replace("[ContactEmail]", data.ContactEmail)
                    .Replace("[Tel]", string.IsNullOrWhiteSpace(data.Tel) ? "" : $@"{data.TelAreacode}-{data.Tel}" + (!string.IsNullOrEmpty(data.TelExtension) ? $" 分機 {data.TelExtension}" : ""))
                    .Replace("[Mobile]", data.Mobile)
                    .Replace("[WebName]", casesModel.WebSiteName)
                    .Replace("[SysCategory]", sysCategory?.Value ?? "")
                    .Replace("[CaseApplyClass]", casesModel.CaseName)
                    .Replace("[Subject]", data.Subject)
                    .Replace("[EffectiveDate]", data.EffectiveDate?.ToString("yyyy-MM-dd HH:mm:ss") ?? "")
                    .Replace("[FileShow]", fileshow)
                    .Replace("[FileArea]", filearea)
                    .Replace("[NeedSurvey]", suveryArea)
                    .Replace("[CaseContent]", data.CaseContent.Replace("\n", "<br>"))
                    .Replace("[ReplyContent]", ReplyContent)
                    .Replace("[ReplyFile]", mailFiles?.Count() > 0 ? FileNames : "無")
                    .Replace("[ReplyDate]", ReplyDate);
                MailInfoModel mailInfo = new MailInfoModel()
                {
                    Type = "MailBox",
                    ToMail = data.ContactEmail.Trim(),
                    Subject = MailBoxPage.PageTitle,
                    Body = mailTemplate,
                    Files = mailFiles
                };

                if (Utility.Mail.Send(mailInfo, out Exception exception))
                {
                    errorMsg = "";
                    return true;
                }
                else
                {
                    errorMsg = exception.Message;
                    return false;
                }
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 特殊信-直接寄給官方
        /// </summary>
        /// <param name="data"></param>
        /// <param name="casesModel"></param>
        /// <param name="sysCategory"></param>
        /// <param name="saveFiles"></param>
        /// <returns></returns>
        public static bool SendToSpecialOffcialMail(CaseApply data, CasesModel casesModel, SysCategory sysCategory, List<Utility.MailFile> saveFiles, out string errorMsg, string To)
        {
            try
            {
                var MailBoxPage = MailBoxService.GetCaseApplyPage("SpecialOffcialMail");
                var filePath = $@"{Directory.GetCurrentDirectory()}/wwwroot/mail/MailLayout.html";
                var mailTemplate = System.IO.File.ReadAllText(filePath).Replace("[MailArea]", MailBoxPage.PageContent);
                //附件
                var filearea = "";
                var fileshow = "display:none;";
                if (saveFiles?.Count() > 0)
                {
                    fileshow = "";
                    filearea += string.Join("<br/>", saveFiles.Select(x => x.FileName).ToArray());
                }

                var surveyarea = "";

                var FileNames = "";
                if (saveFiles?.Count() > 0)
                {
                    FileNames = string.Join("<br/>", saveFiles.Select(x => x.FileName).ToArray());
                }
                var ReplyContent = "";
                ReplyContent = string.IsNullOrWhiteSpace(data.ReplyContent) ? data.ReplySource2?.Replace("\n", "<br>") : data.ReplyContent.Replace("\n", "<br>");
                mailTemplate = mailTemplate
                    .Replace("[CaseNo]", data.CaseNo)
                    .Replace("[CasePwd]", data.CasePwd)
                    .Replace("[CreateDate]", data.CreateDate.ToString("yyyy-MM-dd HH:mm"))
                    .Replace("[AcceptDate]", data.AcceptDate?.ToString("yyyy-MM-dd HH:mm") ?? "")
                    .Replace("[ApplyUser]", data.ApplyUser)
                    .Replace("[ContactEmail]", data.ContactEmail)
                    .Replace("[Tel]", string.IsNullOrWhiteSpace(data.Tel) ? "" : $@"{data.TelAreacode}-{data.Tel}" + (!string.IsNullOrEmpty(data.TelExtension) ? $" 分機 {data.TelExtension}" : ""))
                    .Replace("[Mobile]", data.Mobile)
                    .Replace("[WebName]", casesModel.WebSiteName)
                    .Replace("[SysCategory]", sysCategory?.Value ?? "")
                    .Replace("[CaseApplyClass]", casesModel.CaseName)
                    .Replace("[Subject]", data.Subject)
                    .Replace("[EffectiveDate]", data.EffectiveDate?.ToString("yyyy-MM-dd HH:mm:ss") ?? "")
                    .Replace("[FileShow]", fileshow)
                    .Replace("[FileArea]", filearea)
                    .Replace("[NeedSurvey]", surveyarea)
                    .Replace("[CaseContent]", data.CaseContent.Replace("\n", "<br>"));

                MailInfoModel mailInfo = new MailInfoModel()
                {
                    Type = "MailBox",
                    ToMail = To,
                    Subject = MailBoxPage.PageTitle.Replace("[ClassName]", casesModel.CaseName),
                    Body = mailTemplate,
                    Files = saveFiles
                };

                if (Utility.Mail.Send(mailInfo, out Exception exception))
                {
                    errorMsg = "";
                    return true;
                }
                else
                {
                    errorMsg = exception.Message;
                    return false;
                }
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                return false;
            }
        }

        public static bool SendConsoleErrorMail(List<string> CaseError, List<string> FileError, string mailTo, out string errorMsg)
        {
            try
            {
                var MailBoxPage = MailBoxService.GetCaseApplyPage("ConsoleErrorMail");
                var filePath = $@"{Directory.GetCurrentDirectory()}/wwwroot/mail/MailLayout.html";
                var mailTemplate = System.IO.File.ReadAllText(filePath).Replace("[MailArea]", MailBoxPage.PageContent);

                var strCaseError = string.Join(";", CaseError);
                var strFileError = string.Join(";", FileError);

                mailTemplate = mailTemplate
                    .Replace("[CaseError]", strCaseError)
                    .Replace("[FileError]", strFileError);

                MailInfoModel mailInfo = new MailInfoModel()
                {
                    Type = "MailBox",
                    ToMail = mailTo,
                    Subject = MailBoxPage.PageTitle,
                    Body = mailTemplate,
                };

                if (Utility.Mail.Send(mailInfo, out Exception exception))
                {
                    errorMsg = "";
                    return true;
                }
                else
                {
                    errorMsg = exception.Message;
                    return false;
                }
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                return false;
            }
        }
    }
}
