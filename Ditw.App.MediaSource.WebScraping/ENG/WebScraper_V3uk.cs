using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;

namespace Ditw.App.MediaSource.WebScraping.ENG
{
    public class WebScraper_V3uk : RssFeedWebScraper
    {
        public WebScraper_V3uk(String feedUrl, Int32 sourceId, Encoding sourcePageEncoding = null)
            : base(feedUrl, sourceId, sourcePageEncoding)
        {
        }

        protected override string GetContent(string rawHtml)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.OptionFixNestedTags = true;
            htmlDoc.LoadHtml(rawHtml);
            const String contentElemId = "container_left";
            HtmlNode mainNode = htmlDoc.GetElementbyId(contentElemId);
            if (mainNode == null)
            {
                return null;
            }

            StringBuilder text = new StringBuilder();
            var ps = mainNode.GetOffspringNodesOfType("p", 8);
            foreach (var n in ps)
            {
                //Trace.WriteLine(n.InnerText);
                if (n.Attributes.Contains("class") &&
                    n.Attributes["class"].Value.Equals("comment_img"))
                {
                    continue;
                }
                text.AppendLine(n.InnerText.Trim());
            }

            return text.ToString();

        }
    }
}
