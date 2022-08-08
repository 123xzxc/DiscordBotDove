using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using System.Data.SQLite;
using System.Collections.Generic;

namespace DiscordBot
{
    internal class Program
    {
        static SQLiteHelper db = new SQLiteHelper();
        static SQLiteCommand cmd;
        static void Main(string[] args)
        {
            GoodClearBot goodClearBot = new GoodClearBot();
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
                if (e.Message.Content.ToLower() == "random")
                {
                    await e.Message.RespondAsync("数字为:" + ran.Next(100));
                    
                };
                if (e.Message.Content.ToLower() == "list")
                {
                    var options = new List<DiscordSelectComponentOption>()
                    {
                    new DiscordSelectComponentOption(
                        "标签1",
                        "label_no_desc"),

                    new DiscordSelectComponentOption(
                        "带备注的标签",
                        "label_with_desc",
                        "这是备注!"),

                    new DiscordSelectComponentOption(
                        "带备注和表情的标签",
                        "label_with_desc_emoji",
                        "这也是备注!",
                        emoji: new DiscordComponentEmoji(854260064906117121)),

                    new DiscordSelectComponentOption(
                        "带备注，表情的默认选中的标签",
                        "label_with_desc_emoji_default",
                        "我是默认选中的!",
                        isDefault: true,
                        new DiscordComponentEmoji(854260064906117121))
                    };

                    // Make the dropdown
                    var dropdown = new DiscordSelectComponent("dropdown", null, options, false, 1, 2);

                    var builder = new DiscordMessageBuilder()
                    .WithContent("我会在标签前发出来")
                    .AddComponents(dropdown);

                    await builder.SendAsync(e.Message.Channel);
                };
            };

            //开启连接
            commands.RegisterCommands<Command>();
            await discord.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}
