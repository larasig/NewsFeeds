using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;

namespace Ditw.App.MediaSource.WebScraping.ENG
{
    public class WebScraper_TrandMicro : RssFeedWebScraper
    {
        public WebScraper_TrandMicro(String feedUrl, Int32 sourceId, Encoding sourcePageEncoding = null)
            : base(feedUrl, sourceId, sourcePageEncoding)
        {
        }

        protected override string GetContent(string rawHtml)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.OptionFixNestedTags = true;
            htmlDoc.LoadHtml(rawHtml);
            const String contentElemId = "mainContent2010";
            HtmlNode mainNode = htmlDoc.GetElementbyId(contentElemId);
            if (mainNode == null)
            {
                return null;
            }
            //-----------
            var nodes = mainNode.GetOffspringNodesWithIdStartsWith("post", 2);
            if (nodes == null || nodes.Count() == 0)
            {
                return null;
            }
            var node3 = nodes.First();
            if (node3 == null)
            {
                return null;
            }
#if true
            StringBuilder text = new StringBuilder();
            var t = node3.GetOffspringNodesOfType("p", 3);
            foreach (var n in t)
            {
                String txt = n.InnerText.Trim();
                //Trace.WriteLine(n.InnerText);
                if (txt.StartsWith("This entry was posted"))
                {
                    continue;
                }
                text.AppendLine(txt);
            }
            return text.ToString();
#else
            
            return node3.InnerText;
#endif
        }
    }
}
