/*
 * Created by SharpDevelop.
 * User: jiajiwu
 * Date: 12/24/2012
 * Time: 2:03 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Ditw.App.MediaSource.DbUtil
{
	/// <summary>
	/// Description of MySQLAgentNewsFeeds.
	/// </summary>
	public static class MySQLAgentNewsFeeds
	{
		public static void InsertNews(IDataStore dataStore, String connString = null)
		{
			InsertNews(
				(Int32)dataStore["srcId"],
				(String)dataStore["id"],
				(DateTime)dataStore["pubDate"],
				(String)dataStore["content"],
				(String)dataStore["raw"],
				connString
			);
		}
		
		public static void InsertNews(
			Int32 srcId,
			String id,
			DateTime date,
			String content,
			String raw,
			String connString = null)
		{
			MySQLAgent.OpenConnectionIfNotYet(connString);

			MySQLAgent.RunCommandsInOrder(
				connString,
				CreateInsertNewsCommand(srcId, id, date, content, raw)
			);
		}
		
		
		public static void InsertCapturedText(
			DbCommand[] cmds,
			String connString = null
		)
		{
			MySQLAgent.OpenConnectionIfNotYet(connString);

			MySQLAgent.RunCommandsInOrder(
				connString,
				cmds
			);
		}
		
		public static void ReadNews(
			DateTime? dt,
			Action<String> newsHandler,
			String connString = null,
			CommandBehavior commandBehavior = CommandBehavior.CloseConnection)
		{
			MySQLAgent.OpenConnectionIfNotYet(connString);

			DbCommand cmd = CreateReadCommand(dt);
			
			using (DbDataReader reader = cmd.ExecuteReader(commandBehavior))
			{
				while (reader.Read())
				{
					newsHandler((String)reader["content"]);
				}
			}
			
			if (CommandBehavior.CloseConnection == commandBehavior)
			{
				MySQLAgent.CloseConnection();
			}
		}
		
		public static void ReadCapturedText(
			Int32? filterId,
			Action<String> textHandler,
			String connString = null,
			CommandBehavior commandBehavior = CommandBehavior.CloseConnection)
		{
			MySQLAgent.OpenConnectionIfNotYet(connString);

			DbCommand cmd = CreateReadCapturedTextCommand(filterId);
			
			using (DbDataReader reader = cmd.ExecuteReader(commandBehavior))
			{
				while (reader.Read())
				{
					textHandler((String)reader["capturedText"]);
				}
			}
			
			if (CommandBehavior.CloseConnection == commandBehavior)
			{
				MySQLAgent.CloseConnection();
			}
		}

		public static void ReadCapturedText(
			Int32? filterId,
			Action<IDataStore> rowHandler,
			String connString = null,
			CommandBehavior commandBehavior = CommandBehavior.CloseConnection)
		{
			MySQLAgent.OpenConnectionIfNotYet(connString);

			DbCommand cmd = CreateReadCapturedTextCommand(filterId);
			
			using (DbDataReader reader = cmd.ExecuteReader(commandBehavior))
			{
				while (reader.Read())
				{
					IDataStore ds = new DataStoreBase();
					ds["filterId"] = reader["filterId"];
					ds["capturedText"] = reader["capturedText"];
					ds["status"] = reader["status"];
					ds["count"] = reader["count"];
					ds["contextText"] = reader["contextText"];
					rowHandler(ds);
				}
			}
			
			if (CommandBehavior.CloseConnection == commandBehavior)
			{
				MySQLAgent.CloseConnection();
			}
		}

		/// <summary>
		/// create table NewsSource (
		/// 		id INT,
		/// 		name VARCHAR(32),
		/// 		description VARCHAR(64)
		/// );
		/// </summary>
		/// <returns></returns>
		public static DbCommand CreateInsertNewsSourceCommand(
			Int32 id,
			String name,
			String description
			)
		{
			DbCommand cmd = MySQLAgent.CreateCommand();
			cmd.CommandText = String.Format(@"INSERT INTO NewsSource VALUES(
				{0},
				'{1}',
				'{2}');",
			    id,
			    name,
			    description
			);
			return cmd;
		}
		
		/// <summary>
		/// create table CapturedText (
		/// 	filterId INT,
		/// 	capturedText VARCHAR(128),
		/// 	status INT,
		/// 	contextText VARCHAR(256)
		/// 	);
		/// </summary>
		/// <returns></returns>
		public static DbCommand CreateInsertCapturedTextCommand(
			Int32 filterId,
			String capturedText,
			Int32 status,
			Int32 count,
			String contextText,
			String connString = null
			)
		{
			MySQLAgent.OpenConnectionIfNotYet(connString);
			
			const String insertTempl =
				@"INSERT INTO CapturedText(filterId, capturedText, status, count, contextText) VALUES(
	@filterId,
	@capturedText,
	@status,
	@count,
	@contextText);";
			
			DbCommand cmd = MySQLAgent.CreateCommand();
			cmd.CommandText = insertTempl;
			//DbParameter param;
			AddParameter(cmd, "filterId", filterId);
			AddParameter(cmd, "capturedText", capturedText);
			AddParameter(cmd, "status", status);
			AddParameter(cmd, "count", count);
			AddParameter(cmd, "contextText", contextText);
			
			return cmd;
		}
		
		public static DbCommand CreateInsertCapturedTextCommand(
			IDataStore dataStore,
			String connString = null			
		)
		{			
			return CreateInsertCapturedTextCommand(
				(Int32)dataStore["filterId"],
				(String)dataStore["capturedText"],
				(Int32)dataStore["status"],
				(Int32)dataStore["count"],
				(String)dataStore["contextText"],
				connString
			);
		}

		/// <summary>
		/// create table FilterInfo (
		/// 	filterId INT,
		/// 	name VARCHAR(32),
		/// 	description VARCHAR(64),
		/// 	status VARCHAR(16)
		/// 	);
		/// </summary>
		/// <returns></returns>
		public static DbCommand CreateInsertFilterCommand(
			Int32 filterId,
			String name,
			String description,
			String status
			)
		{
			const String insertTempl =
				@"INSERT INTO FilterInfo(filterId, name, description, status) VALUES(
	@filterId,
	@name,
	@description,
	@status);";
			
			DbCommand cmd = MySQLAgent.CreateCommand();
			cmd.CommandText = insertTempl;
			//DbParameter param;
			AddParameter(cmd, "filterId", filterId);
			AddParameter(cmd, "name", name);
			AddParameter(cmd, "description", description);
			AddParameter(cmd, "status", status);
			
			return cmd;
		}
		

		internal static void AddParameter(DbCommand cmd, String paramName, Object paramValue)
		{
			DbParameter param;
			param = cmd.CreateParameter();
			param.ParameterName = paramName;
			param.Value = paramValue;
			cmd.Parameters.Add(param);
		}
		
		/// <summary>
		/// create table News (
		/// 		srcId INT,
		/// 		id VARCHAR(32),
		/// 		pubDate DATE,
		/// 		content VARCHAR(19200),
		/// 		raw TEXT,
		/// 		content_idx INT
		/// );
		/// </summary>
		/// <returns></returns>
		public static DbCommand[] CreateInsertNewsCommand(
			Int32 srcId,
			String id,
			DateTime date,
			String content,
			String raw
		)
		{
			Int32 contentIndex = 0;
			List<DbCommand> cmdList = new List<DbCommand>();
			const String insertTempl = @"INSERT INTO News(srcId, id, pubDate, content, raw, content_idx) VALUES(
	@srcId,
	@id,
	@pubDate,
	@content,
	@raw,
	@content_idx);";
			
			DbCommand cmd = MySQLAgent.CreateCommand();
			//DbParameter param;
			AddParameter(cmd, "srcId", srcId);
			AddParameter(cmd, "id", id);
			AddParameter(cmd, "pubDate", date.ToString("yyyy-MM-dd"));
			AddParameter(cmd, "content",
				(content.Length <= NewsContentMaxLength ?
			    	content : content.Substring(0, NewsContentMaxLength))
			            );
			AddParameter(cmd, "raw",
			             (raw.Length > 1000 ? raw.Substring(0, 1000) : raw)
			            );
			AddParameter(cmd, "content_idx", contentIndex++);
			cmd.CommandText = insertTempl;
			#if false
			cmd.CommandText = String.Format(
				insertTempl,
			    srcId,
			    id,
			    date.ToString("yyyy-MM-dd"),
			    (content.Length <= NewsContentMaxLength ?
			    	content : content.Substring(0, NewsContentMaxLength)),
			    raw,
			    contentIndex++
			);
			#endif
			cmdList.Add(cmd);
			
			Int32 lenContentLeft = content.Length - NewsContentMaxLength;
			String contentLeft = content;
			while (lenContentLeft > 0)
			{
				cmd = MySQLAgent.CreateCommand();
				contentLeft = contentLeft.Substring(contentLeft.Length - lenContentLeft);

				AddParameter(cmd, "srcId", srcId);
				AddParameter(cmd, "id", id);
				AddParameter(cmd, "pubDate", date.ToString("yyyy-MM-dd"));
				AddParameter(cmd, "content",
					(lenContentLeft <= NewsContentMaxLength ?
				    	contentLeft : contentLeft.Substring(0, NewsContentMaxLength))
				            );
				AddParameter(cmd, "raw", String.Empty);
				AddParameter(cmd, "content_idx", contentIndex++);
				cmd.CommandText = insertTempl;
				cmdList.Add(cmd);
				
				lenContentLeft -= NewsContentMaxLength;
			}
			return cmdList.ToArray();
		}
		
		public static DbCommand CreateReadCapturedTextCommand(Int32? filterId, String dataField = null)
		{
			DbCommand cmd = MySQLAgent.CreateCommand();
			if (filterId != null)
			{
				cmd.CommandText = String.Format(
					@"SELECT {0} FROM capturedText WHERE filterId=@filterId",
				    String.IsNullOrEmpty(dataField) ? "*" : dataField
	                );
				AddParameter(cmd, "filterId", filterId);
				return cmd;
			}
			else
			{
				cmd.CommandText = String.Format(
					@"SELECT {0} FROM capturedText",
				    String.IsNullOrEmpty(dataField) ? "*" : dataField
	                );
				return cmd;
			}
		}
		
		public static DbCommand CreateReadCommand(DateTime? dt = null, String dataField = null)
		{
			//String paramName = "pubDate";
			DbCommand cmd = MySQLAgent.CreateCommand();
			if (dt != null)
			{
				cmd.CommandText = String.Format(
					@"SELECT {0} FROM news WHERE pubDate=@pubDate",
				    String.IsNullOrEmpty(dataField) ? "*" : dataField
	                );
				AddParameter(cmd, "pubDate", dt);
				return cmd;
			}
			else
			{
				cmd.CommandText = String.Format(
					@"SELECT {0} FROM news",
				    String.IsNullOrEmpty(dataField) ? "*" : dataField
	                );
				return cmd;
			}
		}
		
		private const Int32 NewsContentMaxLength = 19200;
		
	}
		
}
