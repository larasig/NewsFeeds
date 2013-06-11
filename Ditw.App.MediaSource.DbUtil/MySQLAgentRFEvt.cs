using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;

namespace Ditw.App.MediaSource.DbUtil
{
    public static class MySQLAgentRFEvt
    {
        public static void InsertEvt(String newsTableName, IDataStore dataStore, String connString = null)
        {
            InsertEvt(
                newsTableName,
                (Int32)dataStore["id"],
                (String)dataStore["rawJson"],
                connString
            );
        }

        public static void InsertEvt(
            String newsTableName,
            Int32 id,
            String rawJson,
            String connString = null)
        {
            MySQLAgent.OpenConnectionIfNotYet(connString);

            MySQLAgent.RunCommandsInOrder(
                connString,
                CreateInsertRFEvtCommand(newsTableName, id, rawJson)
            );
        }

        public static DbCommand CreateInsertRFEvtCommand(
            String newsTableName,
            Int32 id,
            String rawJson
        )
        {
            //Int32 contentIndex = 0;
            //List<DbCommand> cmdList = new List<DbCommand>();
            String insertTempl = "INSERT INTO " + newsTableName + @"(evt_id, raw_json) VALUES(
	@id,
	@rawJson);";

            DbCommand cmd = MySQLAgent.CreateCommand();
            //DbParameter param;
            MySQLAgent.AddParameter(cmd, "id", id);
            MySQLAgent.AddParameter(cmd, "rawJson", rawJson);
            cmd.CommandText = insertTempl;

            return cmd;
            //cmdList.Add(cmd);
        }
    }
}
