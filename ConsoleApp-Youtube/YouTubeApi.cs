using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Services.Youtube.YoutubeService;
using static Utility.YouTube.GetApi;

namespace ConsoleApp
{
    public class YouTubeApi
    {
        public static void GetData(string logFile, string DemoDNS , ref List<string> logtxt)
        {
            logtxt.Add("開始執行");
            Services.Youtube.YoutubeService.GetPlayListApiData(DemoDNS, out string msg, out string error , out List<OutYoutubeMgs> outYoutubeMgsList);
            if (string.IsNullOrWhiteSpace(error))
            {
                foreach (var youtube in outYoutubeMgsList?.Distinct().ToList())
                {
                    logtxt.Add($@"播放清單:{youtube.Title}");
                    foreach (var item in youtube.InestTitles?.Distinct().ToList()) {
                        logtxt.Add($@"新增標題:{item}");
                    }
                    foreach (var item in youtube.DelelteTitles?.Select(x => x).Distinct())
                    {
                        logtxt.Add($@"刪除標題:{item}");
                    }
                }
            }
            else
            {
                logtxt.Add($@"錯誤資訊：{error}");
            }
            logtxt.Add($@"執行結束");
        }
    }
}
