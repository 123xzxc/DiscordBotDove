using System;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Data.SQLite;

namespace DiscordBot
{
    public class Command : BaseCommandModule
    {
        //数据库
        SQLiteHelper db = new SQLiteHelper();
        SQLiteCommand cmd = null;

        //测试
        [Command("test")]
        public async Task testCommand(CommandContext ctx)
        {
            string userID = ctx.User.Id.ToString();
            await ctx.RespondAsync("您的ID为"+userID);
        }

        //绑定DiscordBotDB
        [Command("bind")]
        public async Task bindCommand(CommandContext ctx)
        {
            string userID = ctx.User.Id.ToString();
            string sql = $"select count(*) from DiscordUserInfo where DiscordUserId = '{userID}'";
            cmd = new SQLiteCommand(sql, db.Conn);
            try
            {
                db.OpenConn();
                //检查是否已经绑定
                int dr = Convert.ToInt32(cmd.ExecuteScalar());
                if(dr <= 0)
                {
                    cmd.CommandText = $"insert into DiscordUserInfo(DiscordUserId) values({userID})"; 
                    int enq = Convert.ToInt32(cmd.ExecuteNonQuery());
                    if(enq > 0)
                    {
                        await ctx.RespondAsync("绑定成功!");
                        return;
                    }
                    else
                    {
                        await ctx.RespondAsync("绑定失败!");
                    }
                }
                else
                {
                    await ctx.RespondAsync("您已经绑定过了");
                }
            }
            catch (Exception ex)
            {
                await ctx.RespondAsync(ex.Message);
                return;
            }
            finally
            {
                db.CloseConn();
            }
        }


        //早安
        [Command("早安")]
        public async Task CmbCommand(CommandContext ctx)
        {
            //起床排名
            int num = 0;
            //性别
            int gar = 0;
            //晚安时间
            int nightTime = 0;
            //今日是否已经说过早安
            bool isMorningOK = false;
            //如果没有说过早安
            if (!isMorningOK)
            {
                //如果晚安时间小于4小时
                if(nightTime < 4)
                {
                    await ctx.RespondAsync("才睡了多久就早安啊——");
                    return;
                }
                await ctx.RespondAsync($"早上好，您是今天第{num}个起床的{gar}");
            }
            //如果说过早安
            else
            {
                await ctx.RespondAsync($"您已经说过早安了");
            }

        }

        [Command("awsl")]
        public async Task NmslCommand(CommandContext ctx, DiscordMember member)
        {
            await ctx.RespondAsync($" {member.Mention}! awsl!");
        }
    }
}