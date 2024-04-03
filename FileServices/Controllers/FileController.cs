using Microsoft.AspNetCore.Mvc;
using Nancy;
using Newtonsoft.Json;
using System;
using System.IO;
using Utility;
using static Utility.Files;

namespace FileServices.Controllers
{
    public class FileController : BaseController
    {
       
        /// <summary>
        /// 儲存
        /// </summary>
        /// <param name="saveFileModel"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Save([FromBody] SaveFileModel saveFileModel = null)
        {
            var item = JsonConvert.SerializeObject(saveFileModel);
            try
            {
                saveFileModel.localPath = AppSettingHelper.GetAppsetting("Upload");

                if (saveFileModel.bytes == null) return StatusResult(System.Net.HttpStatusCode.Unauthorized, item);
                var file = Files.Upload(saveFileModel);
               
                return StatusResult(System.Net.HttpStatusCode.OK, file);
            }
            catch (System.Exception ex)
            {
                return StatusResult(System.Net.HttpStatusCode.BadRequest, ex.Message);
            }
        }
        /// <summary>
        /// 抓取檔案
        /// </summary>
        /// <param name="saveFileModel"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Get([FromBody] SaveFileModel saveFileModel = null )
        {
            var msg = "";
            try
            {
                string localPath = AppSettingHelper.GetAppsetting("Upload").ToString().Replace("//Uploads","");
                string path = localPath + saveFileModel.path.Replace("\\", "/");
              
                msg = $@"file parh :{path}";
               
                if (GetFile(path, out FileResponse response))
                {
                    var frStr = System.Text.Json.JsonSerializer.Serialize(response);
                    return StatusResult(System.Net.HttpStatusCode.OK, frStr);
                }
                else 
                {
                    msg += $@" ,   not find :  {response.msg}";

                    return StatusResult(System.Net.HttpStatusCode.BadRequest, msg);

                    
                }
            }
            catch (Exception ex)
            {
                msg += $@" ,   error :  {ex.Message}";
                return StatusResult(System.Net.HttpStatusCode.BadRequest, msg);
            }
        }
    }
}
