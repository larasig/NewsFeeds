/*
 * Created by SharpDevelop.
 * User: jiajiwu
 * Date: 12/25/2012
 * Time: 5:53 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;

namespace Ditw.App.MediaSource.DbUtil
{
	/// <summary>
	/// Description of IDbData.
	/// </summary>
	public interface IDataStore
	{
		IEnumerable<String> Names
		{
			get;
		}
		
		Object this[String name]
		{
			get;
			set;
		}
		
		Boolean HasData(String name);
	}
}
