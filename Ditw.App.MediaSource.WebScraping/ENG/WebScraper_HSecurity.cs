using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;

namespace Ditw.App.MediaSource.WebScraping.ENG
{
    public class WebScraper_HSecurity : RssFeedWebScraper
    {
        public WebScraper_HSecurity(String feedUrl, Int32 sourceId, Encoding sourcePageEncoding = null)
            : base(feedUrl, sourceId, sourcePageEncoding)
        {
        }

        protected override string GetContent(string rawHtml)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.OptionFixNestedTags = true;
            htmlDoc.LoadHtml(rawHtml);
            const String contentElemId = "item";
            HtmlNode mainNode = htmlDoc.GetElementbyId(contentElemId);
            if (mainNode == null)
            {
                return null;
            }
            StringBuilder text = new StringBuilder();
            String prefix = "<!-- RSPEAK_START -->";
            foreach (var n in mainNode.GetOffspringNodesOfType("p", 2))
            {
                Int32 idx = n.InnerText.IndexOf(prefix);
                if (idx > 0)
                {
                    text.AppendLine(n.InnerText.Substring(idx + prefix.Length).Trim());
                }
                else
                {
                    text.AppendLine(n.InnerText.Trim());
                }
            }

            return text.ToString();
        }
    }
}
