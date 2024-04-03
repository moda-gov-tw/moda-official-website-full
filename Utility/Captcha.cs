using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace Utility
{
    /// <summary>圖形驗證</summary>
    public class Captcha
    {
        public bool ShowRandomLine = true;

        public Color Color = ColorTranslator.FromHtml("#185FFF");

        public Color BackColor = ColorTranslator.FromHtml("#EFEFEF");

        /// <summary>輸出字元長度</summary>
        public int StringLength = 5;

        /// <summary>輸出字元</summary>
        public string strLetters = "abcdefghijkmnpqrstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ23456789";

        /// <summary>產生驗證圖型</summary>
        /// <param name="CaptchaString">圖形文字</param>
        /// <param name="linecount">干擾線條</param>
        // <param name="_strLetters">輸出字元</param>
        /// <param name="_StringLength">碼數</param>
        /// <returns></returns>
        public Bitmap GetCaptcha(out string CaptchaString, int linecount = 10, string _strLetters = "", int _StringLength = 5)
        {
            Bitmap b = new Bitmap(150, 50);
            Graphics g = Graphics.FromImage(b);

            if (!string.IsNullOrEmpty(_strLetters))
                strLetters = _strLetters;

            StringLength = _StringLength;
            g.FillRectangle(new SolidBrush(BackColor), 0, 0, 150, 50);
            Font font = new Font(FontFamily.GenericMonospace, 40, FontStyle.Bold, GraphicsUnit.Pixel);
            Random r = new Random();

            StringBuilder s = new StringBuilder();
            //隨機產生
            for (int i = 0; i < StringLength; i++)
            {
                s.Append(strLetters.Substring(r.Next(0, strLetters.Length - 1), 1));
                g.DrawString(s[s.Length - 1].ToString(), font, new SolidBrush(Color), i * 30 - 5, r.Next(-5, 8));
            }

            if (ShowRandomLine)
            {
                //干擾線條
                Pen pen = new Pen(new SolidBrush(Color), 2);
                for (int i = 0; i < linecount; i++)
                {
                    g.DrawLine(pen, new Point(r.Next(0, 150), r.Next(0, 50)), new Point(r.Next(0, 150), r.Next(0, 50)));
                }
            }

            CaptchaString = s.ToString();
            return b;
        }
    }
}
