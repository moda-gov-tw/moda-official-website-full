using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Model
{


    public class RssNodeModel
    {
        public string Title { get; set; }
        public string Link { get; set; }
        public string Description { get; set; }
        public DateTime? PubDate { get; set; }
        public string Copyright { get; set; }
        public string Language { get; set; } = "zh-tw";
        public string Ttl { get; set; }


        public List<RssItemModel> Items { get; set; }
    }
    public class RssItemModel
    {
        public string Title { get; set; }
        public string Link { get; set; }
        public string Description { get; set; }
        public DateTime PubDate { get; set; }
        public string Author { get; set; } = "2.16.886.101.20003.20082";
        //item.Title = n.BasicNews.Title;
        //item.Description = n.BasicNews.ContentText ?? "";
        //item.Link = new Uri(n.DynamicURL);
        ////Item GMT time set
        //item.PubDate = n.BasicNews.StartDate.Value.AddHours(-8);
        //item.Author = "2.16.886.101.20003.20082";

    }



}
