using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;

namespace Ditw.App.MediaSource.WebScraping.ENG
{
    public class WebScraper_TheRegister : RssFeedWebScraper
    {
        public WebScraper_TheRegister(String feedUrl, Int32 sourceId, Encoding sourcePageEncoding = null)
            : base(feedUrl, sourceId, sourcePageEncoding)
        {
        }


        protected override string GetContent(string rawHtml)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.OptionFixNestedTags = true;
            htmlDoc.LoadHtml(rawHtml);
            const String contentElemId = "body";
            HtmlNode mainNode = htmlDoc.GetElementbyId(contentElemId);
            if (mainNode == null)
            {
                return null;
            }
            StringBuilder text = new StringBuilder();
            foreach (var n in mainNode.GetOffspringNodesOfType("p", 2))
            {
                //Trace.WriteLine(n.InnerText);
                text.AppendLine(n.InnerText.Trim());
            }

            return text.ToString();
        }
    }
}
