using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using System.Data.SQLite;

namespace DiscordBot
{
    internal class Program
    {
        static SQLiteHelper db = new SQLiteHelper();
        static SQLiteCommand cmd;
        static void Main(string[] args)
        {
            try
            {
                MainAsync().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }   

        static async Task MainAsync()
        {
            //随机数
            Random ran = new Random();
            //Discord Bot 连接Token
            var discord = new DiscordClient(new DiscordConfiguration()
            {
                Token = DiscordToken.Token,
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.AllUnprivileged
            });;
            var commands = discord.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefixes = new[] { "!" }
            });
            discord.MessageCreated += async (sender, e) =>
            {
                //Ping
                if(e.Message.Content.ToLower() == "ping")
                {
                    await e.Message.RespondAsync("Pong!");
                };
                //显示时间
                if (e.Message.Content.ToLower() == "time")
                {
                    await e.Message.RespondAsync("当前时间为" + System.DateTime.Now.ToString("F"));
                };
                //随机数
                if (e.Message.Content.ToLower() == "randomr")
                {
                    await e.Message.RespondAsync("数字为:" + ran.Next(100));
                };
                if (e.Message.Content.ToLower() == "早安")
                {
                    string sql = "";
                    cmd = new SQLiteCommand(sql, db.Conn);
                    try
                    {
                        db.OpenConn();
                    }
                    catch (Exception ex)
                    {
                        await e.Message.RespondAsync(ex.Message);
                        return;
                    }
                    finally
                    {
                        db.CloseConn();
                    }
                    await e.Message.RespondAsync("数字为:" + ran.Next(100));
                };
            };

            //开启连接
            commands.RegisterCommands<Command>();
            await discord.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}
