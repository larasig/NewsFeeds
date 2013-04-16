using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;

namespace Ditw.App.MediaSource.WebScraping.ENG
{
    public class WebScraper_ZDNetSec : RssFeedWebScraper
    {
        public WebScraper_ZDNetSec(String feedUrl, Int32 sourceId, Encoding sourcePageEncoding = null)
            : base(feedUrl, sourceId, sourcePageEncoding)
        {
        }

        protected override string GetContent(string rawHtml)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.OptionFixNestedTags = true;
            htmlDoc.LoadHtml(rawHtml);
            const String contentElemId = "siu-container";
            HtmlNode mainNode = htmlDoc.GetElementbyId(contentElemId);
            if (mainNode == null)
            {
                return null;
            }
            StringBuilder text = new StringBuilder();
            HtmlNode contentNode = mainNode.GetOffspringNodeWithLongestContent("p", 2);
            foreach (var n in contentNode.ChildNodes.Where(cn => cn.Name == "p"))
            {
                //Trace.WriteLine(n.InnerText);
                text.AppendLine(n.InnerText.Trim());
            }

            return text.ToString();
        }
    }
}
