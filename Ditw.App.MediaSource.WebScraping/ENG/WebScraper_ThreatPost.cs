using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;

namespace Ditw.App.MediaSource.WebScraping.ENG
{
    public class WebScraper_ThreatPost: RssFeedWebScraper
    {
        public WebScraper_ThreatPost(String feedUrl, Int32 sourceId, Encoding sourcePageEncoding = null)
            : base(feedUrl, sourceId, sourcePageEncoding)
        {
        }

        protected override string GetContent(string rawHtml)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.OptionFixNestedTags = true;
            htmlDoc.LoadHtml(rawHtml);
            const String contentElemId = "content-inner";
            HtmlNode mainNode = htmlDoc.GetElementbyId(contentElemId);
            if (mainNode == null)
            {
                return null;
            }
            var tnodes = mainNode.GetOffspringNodesWithIdStartsWith("node", 2);
            if (tnodes == null || tnodes.Count() == 0)
                return null;
            HtmlNode node2 = tnodes.First();
            StringBuilder text = new StringBuilder();
            HtmlNode contentNode = node2.GetOffspringNodeWithLongestContent("p", 2);
            foreach (var n in contentNode.ChildNodes.Where(cn => cn.Name == "p"))
            {
                //Trace.WriteLine(n.InnerText);
                text.AppendLine(n.InnerText.Trim());
            }

            return text.ToString();
        }

    }
}
