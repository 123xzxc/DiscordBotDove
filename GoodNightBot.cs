using System;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Data.SQLite;

namespace DiscordBot
{
    internal class GoodNightBot : Command
    {
        private string ret = null;
        public string Ret
        {
            get { return ret; }
        }
        public GoodNightBot(CommandContext ctx)
        {
            if (!SqlSeac(ctx))
            {
                return;
            }
            //数据库
            //要执行的命令
            string sql = $"select count(*) from GoodNightInfo where UserId = {ctx.User.Id}";
            cmd = new SQLiteCommand(sql, db.Conn);
            try
            {
                //连接数据库
                db.OpenConn();
                //执行查询
                int es = Convert.ToInt32(cmd.ExecuteScalar());
                //如果没晚安过
                if (es == 0)
                {
                    //修改命令
                    cmd.CommandText = $"select LastGoodMorningDate from DiscordUserInfo where DiscordUserId = {ctx.User.Id}";
                    //最后早安时间
                    DateTime dtNight = Convert.ToDateTime(cmd.ExecuteScalar());
                    //如果最后早安时间在4小时前
                    DateTime dt = DateTime.Now;
                    dt = dt.AddHours(-4);
                    if (!(dtNight > dt))
                    {
                        //晚安
                        //提交晚安
                        cmd.CommandText = $"insert into GoodNightInfo(UserId,Date) values({ctx.User.Id},'{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')";
                        int i = cmd.ExecuteNonQuery();
                        if (!(i > 0))
                        {
                            Console.WriteLine(ctx.User.Id + "晚安出错");
                            return;
                        }
                        cmd.CommandText = $"update DiscordUserInfo set LastGoodNightDate = '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}' where DiscordUserId = {ctx.User.Id}";
                        i = cmd.ExecuteNonQuery();
                        if (!(i > 0))
                        {
                            Console.WriteLine(ctx.User.Id + "晚安出错");
                            return;
                        }
                        //查询性别
                        cmd.CommandText = $"select d.GenderAliases from GoodNightInfo g join DiscordUserInfo d on g.UserId = d.DiscordUserId where UserId = {ctx.User.Id}";
                        string s = Convert.ToString(cmd.ExecuteScalar());
                        if (string.IsNullOrEmpty(s))
                        {
                            s = "群友";
                        }
                        //查询排名
                        cmd.CommandText = $"select id from GoodNightInfo where UserId = {ctx.User.Id}";
                        int b = Convert.ToInt32(cmd.ExecuteScalar());
                        //发送消息
                        ret = $"晚安! {ctx.Member.Mention} 您是今天第{b}个睡觉的{s}";
                        return;

                    }
                    else
                    {
                        ret = $"刚起床就要不要再睡啦w!";
                        return;
                    }
                }
                else
                {
                    ret = $"你已经晚安过了w!";
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
