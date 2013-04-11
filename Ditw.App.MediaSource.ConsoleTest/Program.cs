/*
 * Created by SharpDevelop.
 * User: jiajiwu
 * Date: 12/19/2012
 * Time: 11:04 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Threading;
using Ditw.App.MediaSource.DbUtil;
using Ditw.App.MediaSource.WebScraping;
using Ditw.App.MediaSource.WebScraping.ENG;
using Ditw.App.MediaSource.WebScraping.ZHS;
using HtmlAgilityPack;
using System.Text;
using System.Collections.Generic;

namespace Ditw.App.MediaSource.ConsoleTest
{
	class Program
	{
		public static void Main(string[] args)
		{
            //FetchFeed_QQ();
            //FetchFeed_SecurityWeek();
            //FetchFeed_TrendMicro();
            //FetchFeed_VirusOrg();
            //FetchFeed_Avg();
            FetchFeed_HSecurity();

			Console.Write("Press any key to continue . . . ");
			Console.ReadKey(true);
		}

        private static HashSet<String> IdSet = new HashSet<String>();
        static void GetIds(Int32 sourceId)
        {
            foreach (var id in MySQLAgentNewsFeeds.ReadIdsFromSource(sourceId))
            {
                IdSet.Add(id);
            }
        }

        static void FetchFeed_SecurityWeek(
            )
        {
            WebScraper_SecurityWeek securityWeek = new WebScraper_SecurityWeek(
                @"http://feeds.feedburner.com/securityweek",
                MediaSourceID.ENG_SECURITYWEEK,
                Encoding.UTF8);
            FetchRssFeed(securityWeek);
        }

        static void FetchFeed_HSecurity(
            )
        {
            WebScraper_HSecurity hSec = new WebScraper_HSecurity(
                @"http://rss.feedsportal.com/c/32569/f/491736/index.rss",
                MediaSourceID.ENG_HSECURITY,
                Encoding.UTF8);
            FetchRssFeed(hSec);
        }

        static void FetchFeed_Avg(
            )
        {
            WebScraper_Avg avg = new WebScraper_Avg(
                @"http://feeds.avg.com/avgblogs-news-and-threats?format=xml",
                MediaSourceID.ENG_AVG,
                Encoding.UTF8);
            FetchRssFeed(avg);
        }

        static void FetchFeed_VirusOrg(
            )
        {
            WebScraper_VirusOrg virusOrg = new WebScraper_VirusOrg(
                @"http://www.virus.org/blog/feed.rss",
                MediaSourceID.ENG_VIRUSORG,
                Encoding.UTF8);
            FetchRssFeed(virusOrg);
        }

        static void FetchFeed_TrendMicro(
            )
        {
            WebScraper_TrandMicro trendMicro = new WebScraper_TrandMicro(
                @"http://feeds.trendmicro.com/Anti-MalwareBlog?format=xml",
                MediaSourceID.ENG_TRENDMICRO,
                Encoding.UTF8);
            FetchRssFeed(trendMicro);
        }

        static void FetchRssFeed(RssFeedWebScraper feedScraper)
        {
            GetIds(feedScraper.SourceId);

            feedScraper.OnNewFeedAvailable = InsertNewsCheckDuplicate_OnNewFeedAvailable;

            feedScraper.Start();
        }

        static void FetchFeed_QQ()
        {
            WebScraperQQ wsQQ = new WebScraperQQ(DateTime.Today.AddYears(-1).AddDays(32), 0, 1500);

#if false
			foreach(String s in wsQQ.RawFeeds)
			{
				//Console.WriteLine(s);
				HtmlDocument htmlDoc = new HtmlDocument();
				htmlDoc.OptionFixNestedTags = true;
				htmlDoc.LoadHtml(s);
				HtmlNode mainNode = htmlDoc.GetElementbyId("Cnt-Main-Article-QQ");
				
				if (mainNode != null)
				{
					MySQLAgentNewsFeeds.InsertNews(
						1, "1", DateTime.Now,
						mainNode.InnerText,
						mainNode.InnerHtml
					);
	
					Thread.Sleep(15000);
				}
				
				//Console.WriteLine(mainNode.InnerText);
			}
#else
            wsQQ.OnNewFeedAvailable += new NewFeedAvailable(InsertNews_OnNewFeedAvailable);
            wsQQ.Start();
#endif
        }

        static void InsertNews_OnNewFeedAvailable(IDataStore dataStore)
        {
            MySQLAgentNewsFeeds.InsertNews(dataStore);
        }

        static void InsertNewsCheckDuplicate_OnNewFeedAvailable(IDataStore dataStore)
        {
            String id = (String)dataStore["id"];
            if (!IdSet.Contains(id))
            {
                MySQLAgentNewsFeeds.InsertNews(dataStore);
            }
            else
            {
                Console.WriteLine("Duplidate: {0}", id);
            }
        }
    }
}