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
        public SQLiteHelper db = new SQLiteHelper();
        public SQLiteCommand cmd = null;

        //查询是否绑定
        public Boolean SqlSeac(CommandContext ctx)
        {
            string userID = ctx.User.Id.ToString();
            string sql = $"select count(*) from DiscordUserInfo where DiscordUserId = '{userID}'";
            cmd = new SQLiteCommand(sql, db.Conn);
            try
            {
                db.OpenConn();
                //检查是否已经绑定
                int dr = Convert.ToInt32(cmd.ExecuteScalar());
                if (dr <= 0)
                {
                    //未绑定
                    ctx.RespondAsync("你还没有绑定过，请输入\"!bind\"来绑定bot!");
                    return false;
                }
                else
                {
                    //已经绑定
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            finally
            {
                db.CloseConn();
            }

        }

        //测试
        [Command("test")]
        public async Task testCommand(CommandContext ctx)
        {
            bool sq = SqlSeac(ctx);
            await ctx.RespondAsync(sq.ToString());

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
        public async Task MorningCommand(CommandContext ctx)
        {
            GoodMorningBot morningBot = new GoodMorningBot(ctx);
            await ctx.RespondAsync(morningBot.Ret);
        }

        [Command("晚安")]
        public async Task NightCommand(CommandContext ctx)
        {
            GoodNightBot nightBot = new GoodNightBot(ctx);
            await ctx.RespondAsync(nightBot.Ret);
        }

        [Command("awsl")]
        public async Task NmslCommand(CommandContext ctx, DiscordMember member)
        {
            await ctx.RespondAsync($" {member.Mention}! awsl!");
        }
    }
}