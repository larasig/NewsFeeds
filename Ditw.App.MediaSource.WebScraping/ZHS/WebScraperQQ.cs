/*
 * Created by SharpDevelop.
 * User: jiajiwu
 * Date: 12/19/2012
 * Time: 11:38 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading;

using Ditw.App.MediaSource.DbUtil;
using HtmlAgilityPack;

namespace Ditw.App.MediaSource.WebScraping.ZHS
{	
	/// <summary>
	/// Description of WebScraperQQ.
	/// </summary>
	public class WebScraperQQ : IWebScraper
	{
		internal struct Status
		{
			internal Int32 IdxBegin;
			internal Int32 IdxEnd;
			
			internal List<Int32> ToVisit;
			internal Boolean AllVisited
			{
				get
				{
					return ToVisit.Count == 0;
				}
			}
			
			internal Int32 Normalize(Int32 idx)
			{
				return idx % IdxEnd + IdxBegin;
			}
			
			internal Int32 PopFirst()
			{
				Int32 rt = ToVisit[0];
				ToVisit.RemoveAt(0);
				return rt;
			}
		};
		
		private Status _status;
		
		private DateTime _date;
		/// <summary>
		/// QQ urls are in the format "http://news.qq.com/a/20121218/000502.htm";
		/// </summary>
		internal const String _urlPrefix = @"http://news.qq.com/a/";
		private String _urlTemplate;
		//private HashSet<Int32> _visitedUrls = new HashSet<Int32>();
		
		public WebScraperQQ(
			DateTime dt,
			Int32 idxBegin,
			Int32 idxEnd
		)
		{
			_date = dt;
			_urlTemplate = _urlPrefix + _date.ToString("yyyyMMdd") + "/{0:D6}.htm";
			
			InitializeStatus(idxBegin, idxEnd);
		}
		
		private void InitializeStatus(Int32 idxBegin, Int32 idxEnd)
		{
			_status = new Status()
			{
				IdxBegin = idxBegin,
				IdxEnd = idxEnd,
				ToVisit = new List<Int32>(idxEnd - idxBegin)
			};
			
			List<Int32> tmpList = new List<Int32>(_status.ToVisit.Count);
			for (Int32 i = idxBegin; i < idxEnd; i++)
			{
				tmpList.Add(i);
			}
			
			Int32 seed = (Int32)(DateTime.Now.Millisecond);
			Random r = new Random(seed);
			while(tmpList.Count > 0)
			{
				Int32 idx = r.Next() % tmpList.Count;
				_status.ToVisit.Add(tmpList[idx]);
				tmpList.RemoveAt(idx);
			}
		}
		
		public NewFeedAvailable OnNewFeedAvailable;
		
		private IDataStore BuildDataStore(
			Int32 srcId,
			String id,
			DateTime date,
			String raw
		)
		{
			#region find content node
			HtmlDocument htmlDoc = new HtmlDocument();
			htmlDoc.OptionFixNestedTags = true;
			htmlDoc.LoadHtml(raw);
			const String contentElemId = "Cnt-Main-Article-QQ";
			HtmlNode mainNode = htmlDoc.GetElementbyId(contentElemId);
			
			if (mainNode == null)
			{
				return null;
			}
			#endregion
			
			IDataStore dataStore = new DataStoreBase();
			
			dataStore["srcId"] = srcId;
			dataStore["id"] = id;
			dataStore["pubDate"] = date;
			dataStore["content"] = mainNode.InnerText;
			dataStore["raw"] = mainNode.InnerHtml;
			
			return dataStore;
		}
		
		public void Start()
		{
			Int32 timeToSleep = 0;
			while (!_status.AllVisited)
			{
				Int32 i = _status.PopFirst();
				
				Thread.Sleep(timeToSleep);

				String url = String.Format(_urlTemplate, i);
				//_status.VisitedSet.Add(i);
				Trace.WriteLine(String.Format("# {0}: Trying {1}", i, url));
				
				using(WebClient wc = new WebClient())
				{
					wc.Encoding = Encoding.GetEncoding("gb2312");
					String raw;
					try
					{
						raw = wc.DownloadString(url);
					}
					catch (Exception ex)
					{
						Trace.WriteLine(ex.GetType());
						Trace.WriteLine(ex.Message);
						timeToSleep = 12500; // 12.5 sec if 404
						continue;
					}
					
					timeToSleep = 10000; // 10 sec if get anything
					if (OnNewFeedAvailable != null)
					{
						IDataStore ds = BuildDataStore(
								MediaSourceID.ZHS_QQ,
								i.ToString(),
								_date,
								raw);
						if (ds != null)
						{
							OnNewFeedAvailable("news", ds);
							Trace.WriteLine(String.Format(
								"Get feed from URL: {0}", url)
							);
						}
						else
						{
							Trace.WriteLine("No data found!");
						}
					}
					//yield return raw;
					//wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(wc_DownloadStringCompleted);
					//wc.DownloadStringAsync(new Uri(url));
					//yield return raw;
				}
			}
		}

		void wc_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
		{
			throw new NotImplementedException();
		}
		
		#if false
		private IEnumerable<String> Urls
		{
			get
			{
				_maxIndex = 1000;
				for(Int32 i = 0; i < _maxIndex; i++)
				{
					yield return String.Format(_urlTemplate, i);
				}
			}
		}
		#endif
	}
}
