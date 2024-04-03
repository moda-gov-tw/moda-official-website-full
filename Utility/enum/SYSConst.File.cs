using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utility.SYSConst
{
    /// <summary>
    /// 變數檔
    /// </summary>

    public class File
    {
        #region enum
        /// <summary>
        /// 
        /// </summary>
        public enum AllowType
        {
            All = 1,
            OnlyImag = 2
        }

        #endregion

        public static string GetFileType(AllowType item = AllowType.All)
        {
            string rtn = "";
            if (item == AllowType.All)
                rtn = "jpg,jpeg,png,gif,bmp,txt,doc,docx,ppt,pptx,xls,xlsx,pdf,rar,zip,mp3,odt,odp,ods,csv,svg,tif,mp4";
            else if (item == AllowType.OnlyImag)
                rtn = "jpg,jpeg,png,gif,bmp,svg,tif";
            return rtn;
        }
        /// <summary>
        /// 利用fileType 找尋對應的 FileMIME
        /// </summary>
        /// <param name="filetype"></param>
        /// <returns></returns>
        //public static string FileMIMEbyfileType(string filetype)
        //{
        //    switch (filetype.ToLower().Trim())
        //    {
        //        case ".aac": return "audio/aac"; 
        //        case ".abw": return "application/x-abiword";
        //        case ".arc": return "application/x-freearc"; 
        //        case ".avi": return "video/x-msvideo"; 
        //        case ".azw": return "application/vnd.amazon.ebook"; 
        //        case ".bin": return "application/octet-stream"; 
        //        case ".bmp": return "image/bmp"; 
        //        case ".bz": return "application/x-bzip"; 
        //        case ".bz2": return "application/x-bzip2"; 
        //        case ".csh": return "application/x-csh"; 
        //        case ".css": return "text/css"; 
        //        case ".csv": return "text/csv"; 
        //        case ".doc": return "application/msword"; 
        //        case ".docx": return "application/vnd.openxmlformats-officedocument.wordprocessingml.document"; 
        //        case ".eot": return "application/vnd.ms-fontobject"; 
        //        case ".epub": return "application/epub+zip";
        //        case ".gif": return "image/gif"; 
        //        case ".jpeg": return "image/jpeg"; 
        //        case ".png": return "image/png"; 
        //        case ".jpg": return "image/jpeg";
        //        case ".odp": return "application/vnd.oasis.opendocument.presentation";
        //        case ".ods": return "application/vnd.oasis.opendocument.spreadsheet";
        //        case ".odt": return "application/vnd.oasis.opendocument.text";
        //        case ".xls": return "application/vnd.ms-excel";
        //        case ".xlsx": return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //        case ".oga": return "audio/ogg"; 
        //        case ".ogv": return "video/ogg";
        //        case ".pdf": return "application/pdf"; 
        //        case ".otf": return "font/otf"; 
        //        case ".ppt": return "application/vnd.ms-powerpoint"; 
        //        case ".pptx": return "application/vnd.openxmlformats-officedocument.presentationml.presentation"; 
        //        case ".zip": return "application/x-zip-compressed";
        //        case ".json": return "application/json";
        //        case ".xml": return "text/xml";
        //        default: return "";
        //    }
        //}
    }
}
