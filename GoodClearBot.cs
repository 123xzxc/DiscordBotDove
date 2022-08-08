using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Data.SQLite;
namespace DiscordBot
{
    internal class GoodClearBot
    {
        SQLiteHelper db = new SQLiteHelper();
        public GoodClearBot()
        {
            Console.WriteLine("早安自动清理已启动");
            setTaskAtFixedTime();
        }
        private void setTaskAtFixedTime()
        {
            DateTime now = DateTime.Now;
            DateTime oneOClock = DateTime.Today;
            if (now > oneOClock)
            {
                oneOClock = oneOClock.AddDays(1.0);
            }
            int msUntilFour = (int)((oneOClock - now).TotalMilliseconds);

            var t = new System.Threading.Timer(doAt0AM);
            t.Change(msUntilFour, Timeout.Infinite);
        }
        private void doAt0AM(object state)
        {
            string sql = @"delete from GoodMorningInfo;
                           delete from GoodNightInfo;
                           update sqlite_sequence SET seq = 0 where name ='GoodMorningInfo';
                           update sqlite_sequence SET seq = 0 where name ='GoodNightInfo'";
            SQLiteCommand cmd = new SQLiteCommand(sql,db.Conn);
            try
            {
                db.OpenConn();
                int enq =cmd.ExecuteNonQuery();
                if(enq > 0)
                {
                    Console.WriteLine("重置成功");
                }
                else
                {
                    Console.WriteLine("重置失败");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                db.CloseConn();
            }
            setTaskAtFixedTime();
        }
    }
}
