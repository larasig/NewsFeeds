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
using Ditw.App.MediaSource.WebScraping.ZHS;
using HtmlAgilityPack;

namespace Ditw.App.MediaSource.ConsoleTest
{
	class Program
	{
		public static void Main(string[] args)
		{			
			WebScraperQQ wsQQ = new WebScraperQQ(DateTime.Today.AddYears(-1).AddDays(26), 0, 1500);
			
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
			wsQQ.OnNewFeedAvailable += new NewFeedAvailable(wsQQ_OnNewFeedAvailable);
			wsQQ.Start();
			#endif
			
			Console.Write("Press any key to continue . . . ");
			Console.ReadKey(true);
		}

		static void wsQQ_OnNewFeedAvailable(IDataStore dataStore)
		{
			MySQLAgentNewsFeeds.InsertNews(dataStore);
		}
	}
}