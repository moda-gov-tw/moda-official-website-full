using System;
using System.Collections.Generic;
using System.Text;

namespace Utility
{
    public  class DefaultPager
    {
        /// <summary>
        /// 總筆數
        /// </summary>
        public int TotalCount { get; set; }
        /// <summary>
        /// 顯示幾筆資料
        /// </summary>

        public int DisplayCount { get; set; }

        /// <summary>
        /// 目前的頁碼
        /// </summary>
        public int p { get; set; } = 1;

        public int PageIndex { get; set; }

        private int _PageButtonCount = 5;

        public int PageButtonCount
        {
            get
            {
                return _PageButtonCount;
            }
            set
            {
                _PageButtonCount = value;
            }
        }


        /// <summary>
        /// 總頁碼
        /// </summary>
        public int PageCount
        {
            get
            {
                return (this.TotalCount / this.DisplayCount) + ((this.TotalCount % this.DisplayCount) > 0 ? 1 : 0);
            }
        }

        public DefaultPager()
        {
        }

        public DefaultPager(int displaycount, int pageindex)
        {
            DisplayCount = displaycount;
            PageIndex = pageindex;
        }
        /// <summary>
        /// 前台顯示語系
        /// </summary>
        public string Lang { get; set; }

        /// <summary>
        /// 區分單頁面多組分頁
        /// </summary>
        public string key { get; set; } = "a";
    }
}
