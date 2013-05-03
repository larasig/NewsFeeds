/*
 * Created by SharpDevelop.
 * User: jiajiwu
 * Date: 12/20/2012
 * Time: 12:12 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;

using Ditw.App.MediaSource.DbUtil;
using MySql.Data.MySqlClient;

namespace Ditw.App.MediaSource.DbTest
{
	class Program
	{
//		const String MySQLConnectionString =
//			"Server=localhost;Database=NewsFeeds;Uid=root;Pwd=Passw0rd!;";
		public static void Main(string[] args)
		{
			//MySQLAgent.InitializeConnection();
			MySQLAgentNewsFeeds.InsertNews(
                "news",
				1, "1", DateTime.Now, "中文11 chinese", "zhong we'n 原始");
			
			Console.Write("Press any key to continue . . . ");
			Console.ReadKey(true);
		}
		
		static void ReadRecord(DbConnection conn)
		{
			DbCommand cmd = conn.CreateCommand();
			cmd.CommandText = "select * from news;";
			using (DbDataReader reader = cmd.ExecuteReader())
			{
				while(reader.Read())
				{
					Trace.WriteLine(reader["content"]);
					Trace.WriteLine(reader["raw"]);
				}
			}
		}
		
		static void InsertRecord(DbConnection conn)
		{
			const String InsertTemplate = @"INSERT INTO News values (
1,
1,
'2012-12-20',
'中文',
'中文1');";
			DbCommand cmd = conn.CreateCommand();
			cmd.CommandText = InsertTemplate;
			cmd.ExecuteNonQuery();
		}
	}
}