/*
 * Created by SharpDevelop.
 * User: jiajiwu
 * Date: 12/20/2012
 * Time: 1:00 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;

using MySql.Data.MySqlClient;

namespace Ditw.App.MediaSource.DbUtil
{
	/// <summary>
	/// Description of MyClass.
	/// </summary>
	public static class MySQLAgent
	{
		private static DbConnection _conn;
		
		private const String _defaultConnectionString =
			"Server=localhost;Database=NewsFeeds;Uid=root;Pwd=Passw0rd!;";
		
		public static void InitializeConnection(String conn)
		{
			CloseConnection();
			_conn = new MySqlConnection(String.IsNullOrEmpty(conn) ?
			                            _defaultConnectionString : conn);
			_conn.Open();
		}
		
		public static void CloseConnection()
		{
			if (_conn != null)
			{
				try
				{
					_conn.Close();
				}
				catch (Exception ex)
				{
					Trace.WriteLine(ex.GetType());
					Trace.WriteLine(ex.Message);
					// swallowed!
				}
				finally
				{
					_conn = null;
				}
			}
		}
		
		public static DbCommand CreateCommand()
		{
			if (_conn == null)
			{
				throw new NullReferenceException("Connection!");
			}
			
			return _conn.CreateCommand();
		}
		
		public static void RunCommandsInOrder(
			String connString,
			params DbCommand[] commands)
		{
			OpenConnectionIfNotYet(connString);
			
			foreach(DbCommand cmd in commands)
			{
				cmd.ExecuteNonQuery();
			}
			
			CloseConnection();
		}
		
		internal static void OpenConnectionIfNotYet(String connString)
		{
			if (_conn == null)
			{
				InitializeConnection(connString);
			}
		}
		

	}
}