/*
 * Created by SharpDevelop.
 * User: jiajiwu
 * Date: 12/25/2012
 * Time: 5:57 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;

namespace Ditw.App.MediaSource.DbUtil
{
	/// <summary>
	/// Description of DbDataBase.
	/// </summary>
	public class DataStoreBase : IDataStore
	{
		public DataStoreBase()
		{
		}
		
		protected Dictionary<String, Object> _dataStore =
			new Dictionary<String, Object>();
		
		public IEnumerable<String> Names
		{
			get
			{
				return _dataStore.Keys;
			}
		}
		
		public Object this[String name]
		{
			get
			{
				return _dataStore[name];
			}
			set
			{
				_dataStore[name] = value;
			}
		}
		
		public bool HasData(String name)
		{
			return _dataStore.ContainsKey(name);
		}
	}
}
