using System;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Data.SQLite;

namespace DiscordBot
{
    internal class GoodMorningBot : Command
    {
        private string ret = null;
        public string Ret
        {
            get { return ret; }
        }
        public GoodMorningBot(CommandContext ctx)
        {
            if (!SqlSeac(ctx))
            {
                return;
            }
            //数据库
            //要执行的命令
            string sql = $"select count(*) from GoodMorningInfo where UserId = {ctx.User.Id}";
            cmd = new SQLiteCommand(sql, db.Conn);
            try
            {
                //连接数据库
                db.OpenConn();
                //执行查询
                int es = Convert.ToInt32(cmd.ExecuteScalar());
                //如果没早安过
                if (es == 0)
                {
                    //修改命令
                    cmd.CommandText = $"select LastGoodNightDate from DiscordUserInfo where DiscordUserId = {ctx.User.Id}";
                    //最后晚安时间
                    DateTime dtNight = Convert.ToDateTime(cmd.ExecuteScalar());
                    //如果最后晚安时间在4小时前
                    DateTime dt = DateTime.Now;
                    dt = dt.AddHours(-4);
                    if (!(dtNight > dt))
                    {
                        //早安
                        //今日是否已经说过早安
                        bool isMorningOK = false;
                        //如果没有说过早安
                        if (!isMorningOK)
                        {
                            //提交早安
                            cmd.CommandText = $"insert into GoodMorningInfo(UserId,Date) values({ctx.User.Id},'{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')";
                            int i = cmd.ExecuteNonQuery();
                            if (!(i > 0))
                            {
                                Console.WriteLine(ctx.User.Id + "早安出错");
                                return;
                            }
                            cmd.CommandText = $"update DiscordUserInfo set LastGoodMorningDate = '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}' where DiscordUserId = {ctx.User.Id}";
                            i = cmd.ExecuteNonQuery();
                            if (!(i > 0))
                            {
                                Console.WriteLine(ctx.User.Id + "早安出错");
                                return;
                            }
                            //查询性别
                            cmd.CommandText = $"select d.GenderAliases from GoodMorningInfo g join DiscordUserInfo d on g.UserId = d.DiscordUserId where UserId = {ctx.User.Id}";
                            string s = Convert.ToString(cmd.ExecuteScalar());
                            if (string.IsNullOrEmpty(s))
                            {
                                s = "群友";
                            }
                            //查询排名
                            cmd.CommandText = $"select id from GoodMorningInfo where UserId = {ctx.User.Id}";
                            int b = Convert.ToInt32(cmd.ExecuteScalar());
                            //发送消息
                            ret = $"早上好 {ctx.Member.Mention} ，您是今天第{b}个起床的{s}";
                            return;
                        }
                        //如果说过早安
                        else
                        {
                            ret = $"您已经说过早安了";
                        }

                    }
                    else
                    {
                        ret = $"你才睡了多久啊，快去睡觉!";
                        return;
                    }
                }
                else
                {
                    ret = $"你已经早安过了w!";
                    return;
                }
            }
            catch (Exception ex)
            {

                ret = ex.Message;
            }
            finally
            {
                db.CloseConn();
            }
        }

    }
}
