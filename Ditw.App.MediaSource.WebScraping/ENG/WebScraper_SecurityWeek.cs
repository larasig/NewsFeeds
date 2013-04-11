using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;

namespace Ditw.App.MediaSource.WebScraping.ENG
{
    public class WebScraper_SecurityWeek : RssFeedWebScraper
    {
        public WebScraper_SecurityWeek(String feedUrl, Int32 sourceId, Encoding sourcePageEncoding = null)
            : base(feedUrl, sourceId, sourcePageEncoding)
        {
        }

        protected override string GetContent(string rawHtml)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.OptionFixNestedTags = true;
            htmlDoc.LoadHtml(rawHtml);
            const String contentElemId = "center";
            HtmlNode mainNode = htmlDoc.GetElementbyId(contentElemId);
            if (mainNode == null)
            {
                return null;
            }
            var node2 = mainNode.ChildNodes.Where(
                n => !String.IsNullOrEmpty(n.Id) && n.Id.StartsWith("node")
                );
            var node3 = node2.First();
            if (node3 == null)
            {
                return null;
            }
            var contentDiv = node3.ChildNodes.Where(
                node =>
                {
                    var t = node.Attributes["class"];
                    //if (t != null)
                    //{
                    //    Trace.WriteLine(t.Value);
                    //}
                    return t != null && t.Value.Contains("content");
                }
                );
            var contentNode = contentDiv.First();
            if (contentNode == null)
            {
                return null;
            }

            //Trace.WriteLine(contentNode.Elements("p").Count());
            //Trace.WriteLine("-----------------------------------------------------");
            StringBuilder text = new StringBuilder();
            foreach (var n in contentNode.Elements("p"))
            {
                //Trace.WriteLine(n.InnerText);
                text.AppendLine(n.InnerText.Trim());
            }

            return text.ToString();
        }
    }
}
