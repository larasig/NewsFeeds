using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ditw.App.MediaSource.DbUtil;
using System.ServiceModel.Syndication;
using System.Xml;
using System.Diagnostics;
using System.Net;
using HtmlAgilityPack;

namespace Ditw.App.MediaSource.WebScraping
{
    public delegate void NewFeedAvailable(String tableName, IDataStore dataStore);

    public abstract class RssFeedWebScraper : IWebScraper
    {
        public String FeedUrl
        {
            get;
            private set;
        }

        public Int32 SourceId
        {
            get;
            private set;
        }

        public Encoding SourcePageEncoding
        {
            get;
            private set;
        }

        public RssFeedWebScraper(String feedUrl, Int32 sourceId, Encoding sourcePageEncoding = null)
        {
            FeedUrl = feedUrl;
            SourceId = sourceId;

            SourcePageEncoding = sourcePageEncoding == null ?
                Encoding.UTF8 : sourcePageEncoding;
        }

        protected abstract String GetContent(String rawHtml);

        private const Int32 RAW_ABSTRACT_LENGTH = 1024;
        private IDataStore BuildDataStore(
            Int32 srcId,
            //String id,
            DateTimeOffset date,
            String raw,
            String uri
        )
        {
            String text = GetContent(raw);
            IDataStore dataStore = new DataStoreBase();
#if true
            dataStore["srcId"] = srcId;
            dataStore["id"] = GetItemId(date, text);//id;
            dataStore["pubDate"] = date.ToUniversalTime().DateTime;
            dataStore["content"] = text;
            dataStore["raw"] = String.Format(
                "[{0}] {1}",
                uri,
                text.Substring(0, text.Length > RAW_ABSTRACT_LENGTH ? RAW_ABSTRACT_LENGTH : text.Length)
                );
#endif
            return dataStore;
        }

        public NewFeedAvailable OnNewFeedAvailable;

        private const Int32 ID_LENGTH = 32;
        private static String GetItemId(DateTimeOffset date, String content)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("[{0}][{1}]",
                date.ToString("yyyyMMdd"),
                content.Length);
            String result = builder.ToString();
            if (result.Length > ID_LENGTH)
            {
                return result.Substring(0, ID_LENGTH);
            }
            else
            {
                Int32 len3 = ID_LENGTH - result.Length;
                return result + content.Substring(0, len3);
            }
        }

        protected virtual Uri GetFeedLink(SyndicationItem feed)
        {
            return feed.Links[0].Uri;
        }

        public virtual void Start()
        {
            // ReadIdsFromSource

            XmlTextReader xmlReader = new XmlTextReader(FeedUrl);

            SyndicationFeed feed;
            try
            {
                feed = SyndicationFeed.Load(xmlReader);
            }
            catch
            {
                return;
            }

            //Int32 idIdx = 0;
            foreach (var f in feed.Items)
            {
#if false
                Trace.WriteLine("-----------------------------------------");
                Trace.WriteLine(f.Title.Text);
                Trace.WriteLine(f.PublishDate);
                Trace.WriteLine(f.Summary.Text);

                //Trace.WriteLine(f.Content.Type);
                Trace.WriteLine(f.Links.Count);
                Trace.WriteLine(f.Links[0].Uri);
#endif
                Uri uri = GetFeedLink(f);

                WebClient wc = new WebClient();
                wc.Encoding = SourcePageEncoding;
                String content = wc.DownloadString(uri);
                var ds = BuildDataStore(
                    SourceId,
                    //(++idIdx).ToString(),
                    f.PublishDate,
                    content,
                    uri.ToString());

                if (ds != null)
                {
                    OnNewFeedAvailable("news_eng", ds);
                    Trace.WriteLine(String.Format(
                        "Get feed from URL: {0}", uri)
                    );
                }
                else
                {
                    Trace.WriteLine("No data found!");
                }
            }
        }
    }
}
