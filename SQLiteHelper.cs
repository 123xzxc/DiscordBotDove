using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Data;
using System.Threading.Tasks;

namespace DiscordBot
{
	public class SQLiteHelper
	{
        //连接字符串
        private const string STR = "Data Source=./DiscordBot.db;Version=3;";
		private SQLiteConnection conn;
		public SQLiteConnection Conn
        {
            get
            {
                    if (conn == null)
                    {
                        conn = new SQLiteConnection(STR);
                        return conn;
                    }
                    else
                    {
                        return conn;
                    }
            }
        }
        public void OpenConn()
        {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                if (conn.State == ConnectionState.Broken)
                {
                    conn.Close();
                    conn.Open();
                }
        }
        public void CloseConn()
        {
            if(conn.State == ConnectionState.Open || conn.State == ConnectionState.Broken)
            {
                conn.Close();
            }
        }
	}
}