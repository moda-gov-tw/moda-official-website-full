using SkiaSharp;
using System.Drawing;
using System.Text.RegularExpressions;

namespace WebSite.WebSiteUtility
{
    public class OpenGragh
    {
        public static string getImageType(string fileType)
        {
            switch (fileType.ToLower())
            {
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                case ".png":
                    return "image/png";
                default:
                    return "";
            }
        }

        public static bool getImageSize(string filePath , out int Height, out int Width)
        {
            try
            {
                var path = AppSettingHelper.GetAppsetting("Upload") + filePath;

                if (System.IO.File.Exists(path))
                {
                    var vs = filePath.Split('.');
                    string[] imagetype = { "bmp", "gif", "jpeg", "jpg", "png", "tiff" };
                    if (Array.IndexOf(imagetype, vs[vs.Length - 1].ToLower()) > -1)
                    {

                        SKImage image = SKImage.FromEncodedData(path);
                        Width = image.Width;
                        Height = image.Height;
                        return true;
                    }
                    else if (vs[vs.Length - 1].ToLower() == "svg")
                    {
                        Width = 900;
                        Height = 900;
                        return true;
                    }
                }
            }
            catch
            {

            }

            Width = 1200;
            Height = 628;
            return false;
        }

        public static string getFormattedDescription(string desc, int strlen = 78)
        {
            if (desc != null)
            {
                desc = Regex.Replace(desc.Replace("\n", ""), "<[^>]*(>|$)", string.Empty);
                return desc.Length <= strlen ? desc : (desc.Substring(0, strlen) + "...");
            }
            else
            {
                return "";
            }
        }
    }
}
