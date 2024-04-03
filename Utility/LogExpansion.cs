using System;
using System.Collections.Generic;
using System.IO;

namespace Utility
{
    public class LogExpansion
    {
        /// <summary>
        /// 共用 log
        /// </summary>
        /// <param name="logFolder">路徑</param>
        /// <param name="text">訊息</param>
        public static void Write(string logFolder, string text)
        {
            try
            {
                var txt = text;
                logFolder = logFolder.Replace("..", "");
                if (!Directory.Exists(logFolder))
                {
                    Directory.CreateDirectory(logFolder);
                }
                File.AppendAllText($"{logFolder}/{DateTime.UtcNow.AddHours(8).ToString("yyyyMMdd")}.txt", $"{txt}" + Environment.NewLine);
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logFolder"></param>
        /// <param name="text"></param>
        /// <param name="start"></param>
        public static void Write(string logFolder, string text , string start  )
        {
            try
            {
                var txt = text;
                logFolder = logFolder.Replace("..", "");
                if (!Directory.Exists(logFolder))
                {
                    Directory.CreateDirectory(logFolder);
                }
                File.AppendAllText($"{logFolder}/{DateTime.UtcNow.AddHours(8).ToString("yyyyMMdd")}.txt", $"{DateTime.UtcNow.AddHours(8).ToString("yyyy-MM-dd HH:mm:ss")}，執行：{start.ToLower()}，{ txt} {Environment.NewLine}");
            }
            catch (Exception)
            {

            }
        }
        public static void Write(string logFolder, List<string> text)
        {
            try
            {
               


                logFolder = logFolder.Replace("..", "");
                if (!Directory.Exists(logFolder))
                {
                    Directory.CreateDirectory(logFolder);
                }
                File.AppendAllLines(
                    $"{logFolder}/{DateTime.UtcNow.AddHours(8).ToString("yyyyMMdd")}.txt",
                    text
                    );
              //  File.AppendAllText($"{logFolder}/{DateTime.UtcNow.AddHours(8).ToString("yyyyMMdd")}.txt", $"{DateTime.UtcNow.AddHours(8).ToString("yyyy-MM-dd HH:mm:ss")}，執行：{start.ToLower()}，{txt} {Environment.NewLine}");
            }
            catch (Exception)
            {

            }
        }


    }
}
