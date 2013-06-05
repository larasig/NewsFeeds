using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using System.ServiceModel.Syndication;

namespace Ditw.App.MediaSource.WebScraping.ENG
{
    public class WebScraper_HackersPost : RssFeedWebScraper
    {
        public WebScraper_HackersPost(String feedUrl, Int32 sourceId, Encoding sourcePageEncoding = null)
            : base(feedUrl, sourceId, sourcePageEncoding)
        {
        }

        protected override Uri GetFeedLink(SyndicationItem feed)
        {
            return feed.Links
                .Where(l => l.MediaType == "text/html")
                .Last()
                .Uri;
        }


        protected override string GetContent(string rawHtml)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.OptionFixNestedTags = true;
            htmlDoc.LoadHtml(rawHtml);
            const String titleElemId = "titlebar";
            HtmlNode siblingNode = htmlDoc.GetElementbyId(titleElemId);
            if (siblingNode == null)
            {
                return null;
            }

            HtmlNode contentNode = siblingNode.FindSiblingOfClass("post-body");
            if (contentNode == null)
            {
                return null;
            }
            //contentNode = contentNode.ChildNodes[0];

            String r = contentNode.InnerText.Trim();
            return r;
        }
    }
}
