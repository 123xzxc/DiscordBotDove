using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Manganese.Text;
using Mirai.Net.Data.Messages;
using Mirai.Net.Data.Messages.Concretes;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Sessions;
using Mirai.Net.Utils.Scaffolds;

namespace maiBotPlus
{
    public class Program
    {
        static String ssMan = null;
        static int ss = 0;
        static DateTime ssDate = DateTime.Now;
        static String zjMan = null;
        static int zj = 0;
        static DateTime zjDate = DateTime.Now;

        public static async Task Main()
        {
            Console.WriteLine("启动成功");
            setTaskAtFixedTime();
            SQLiteHelper db = new SQLiteHelper();
            Random r = new Random();
            var bot = new MiraiBot
            {
                Address = "127.0.0.1:6666",
                QQ = "",
                VerifyKey = ""
            };
            await bot.LaunchAsync();
            //基础回显
            bot.MessageReceived
                .OfType<GroupMessageReceiver>()
                .Subscribe(x =>
                {
                    Console.WriteLine($"收到了来自群{x.GroupId}由{x.Sender.Id}发送的消息:{x.MessageChain.GetPlainMessage()}");
                });
            bot.MessageReceived
                .OfType<FriendMessageReceiver>()
                .Subscribe(x =>
                {
                    Console.WriteLine($"收到了来自好友{x.FriendId}发送的消息:{x.MessageChain.GetPlainMessage()}");
                });
            bot.MessageReceived
                .OfType<StrangerMessageReceiver>()
                .Subscribe(x =>
                {
                    Console.WriteLine($"收到了来自陌生人{x.StrangerId}发送的消息:{x.MessageChain.GetPlainMessage()}");
                });
            //测试消息发送+随机数
            bot.MessageReceived
                .OfType<GroupMessageReceiver>()
                .Subscribe(async receiver =>
                {
                    if (receiver.MessageChain.GetPlainMessage().Equals("我可爱吗"))
                    {
                        if (r.Next(2) == 1)
                        {
                            await receiver.SendMessageAsync("超可爱的!");
                        }
                        else
                        {
                            await receiver.SendMessageAsync("一点也不可爱!");
                        }
                    }
                });
            //读取机厅人数
            bot.MessageReceived
                .OfType<GroupMessageReceiver>()
                .Subscribe(async receiver =>
                {
                    if (receiver.MessageChain.GetPlainMessage().Trim().StartsWith("ss几"))
                    {
                        if (!ssTime())
                        {
                            await receiver.SendMessageAsync("三盛现在还没开门哦~");
                            return;
                        }
                        string s = null;
                        switch (r.Next(5))
                        {
                            case 0:
                                s = "你绝赞粉了~";
                                break;
                            case 1:
                                s = "花花花屏一整轮~";
                                break;
                            case 2:
                                s = "100.7500%";
                                break;
                            case 3:
                                s = "潘小鬼来也";
                                break;
                            case 4:
                                s = "我觉得你今天可以收歌~";
                                break;
                        }
                        if (ss == 0)
                        {
                            await receiver.SendMessageAsync("三盛现在没人哦");
                        }
                        else
                        {
                            await receiver.SendMessageAsync($"三盛现在有{ss}人哦~ \n最后更新时间为{ssDate}\n更新人为{ssMan}\n"+s);
                        }
                    }
                });

            //添加机厅人数
            bot.MessageReceived
                .OfType<GroupMessageReceiver>()
                .Subscribe(async receiver =>
                {
                    if (receiver.MessageChain.GetPlainMessage().Trim().StartsWith("ss+"))
                    {
                        if (!ssTime())
                        {
                            await receiver.SendMessageAsync("三盛明明没开门,你是怎么进去的?");
                            return;
                        }
                        int i = 0;
                        try
                        {
                            i = Convert.ToInt32(receiver.MessageChain.GetPlainMessage().Remove(0, 3));
                        }
                        catch (Exception ex)
                        {
                            await receiver.SendMessageAsync(ex.Message);
                            return;
                        }
                        if (i >= 0 && ss + i < 80)
                        {
                            ss += i;
                            ssDate = DateTime.Now;
                            ssMan = receiver.Sender.Name;
                            await receiver.SendMessageAsync($"添加成功!三盛现在有{ss}人");
                        }
                        else
                        {
                            await receiver.SendMessageAsync($"请不要乱玩bot(怒)");
                        }
                    }
                });
            //修改机厅人数
            bot.MessageReceived
                .OfType<GroupMessageReceiver>()
                .Subscribe(async receiver =>
                {
                    if (receiver.MessageChain.GetPlainMessage().Trim().StartsWith("ss="))
                    {
                        if (!ssTime())
                        {
                            await receiver.SendMessageAsync("三盛现在没开门,你怎么知道里面有多少人?");
                            return;
                        }
                        int i = 0;
                        try
                        {
                            i = Convert.ToInt32(receiver.MessageChain.GetPlainMessage().Remove(0, 3));
                        }
                        catch (Exception ex)
                        {
                            await receiver.SendMessageAsync(ex.Message);
                            return;
                        }
                        if (i >= 0 && ss + i < 100)
                        {
                            ss = i;
                            ssDate = DateTime.Now;
                            ssMan = receiver.Sender.Name;
                            if (i == 0)
                            {
                                await receiver.SendMessageAsync($"修改成功!三盛现在没人了");
                            }
                            else
                            {
                                await receiver.SendMessageAsync($"修改成功!三盛现在有{ss}人");
                            }
                        }
                        else
                        {
                            await receiver.SendMessageAsync($"请不要乱玩bot(怒)");
                        }
                    }
                });
            //减少机厅人数
            bot.MessageReceived
                .OfType<GroupMessageReceiver>()
                .Subscribe(async receiver =>
                {
                    if (receiver.MessageChain.GetPlainMessage().Trim().StartsWith("ss-"))
                    {
                        if (!ssTime())
                        {
                            await receiver.SendMessageAsync("三盛还没开门,哪里来的人?");
                            return;
                        }
                        int i = 0;
                        try
                        {
                            i = Convert.ToInt32(receiver.MessageChain.GetPlainMessage().Remove(0, 3));
                        }
                        catch (Exception ex)
                        {
                            await receiver.SendMessageAsync(ex.Message);
                            return;
                        }
                        if (i > 0 && ss - i >= 0)
                        {
                            ss -= i;
                            ssDate = DateTime.Now;
                            ssMan = receiver.Sender.Name;
                            await receiver.SendMessageAsync($"减少成功!三盛现在有{ss}人");
                        }
                        else
                        {
                            await receiver.SendMessageAsync($"请不要乱玩bot(怒)");
                        }
                    }
                });
            //读取机厅人数 zj
            bot.MessageReceived
                .OfType<GroupMessageReceiver>()
                .Subscribe(async receiver =>
                {
                    if (receiver.MessageChain.GetPlainMessage().Trim().StartsWith("zj几"))
                    {
                        if (!ssTime())
                        {
                            await receiver.SendMessageAsync("中集现在还没开门哦~");
                            return;
                        }
                        string s = null;
                        switch (r.Next(5))
                        {
                            case 0:
                                s = "你绝赞粉了~";
                                break;
                            case 1:
                                s = "花花花屏一整轮~";
                                break;
                            case 2:
                                s = "100.7500%";
                                break;
                            case 3:
                                s = "潘小鬼来也";
                                break;
                            case 4:
                                s = "我觉得你今天可以收歌~";
                                break;
                        }
                        if (zj == 0)
                        {
                            await receiver.SendMessageAsync("中集现在没人哦");
                        }
                        else
                        {
                            await receiver.SendMessageAsync($"中集现在有{zj}人哦~ \n最后更新时间为{zjDate}\n更新人为{zjMan}\n" + s);
                        }
                    }
                });

            //添加机厅人数 zj
            bot.MessageReceived
                .OfType<GroupMessageReceiver>()
                .Subscribe(async receiver =>
                {
                    if (receiver.MessageChain.GetPlainMessage().Trim().StartsWith("zj+"))
                    {
                        if (!ssTime())
                        {
                            await receiver.SendMessageAsync("中集明明没开门,你是怎么进去的?");
                            return;
                        }
                        int i = 0;
                        try
                        {
                            i = Convert.ToInt32(receiver.MessageChain.GetPlainMessage().Remove(0, 3));
                        }
                        catch (Exception ex)
                        {
                            await receiver.SendMessageAsync(ex.Message);
                            return;
                        }
                        if (i >= 0 && zj + i < 80)
                        {
                            zj += i;
                            zjDate = DateTime.Now;
                            zjMan = receiver.Sender.Name;
                            await receiver.SendMessageAsync($"添加成功!中集现在有{zj}人");
                        }
                        else
                        {
                            await receiver.SendMessageAsync($"请不要乱玩bot(怒)");
                        }
                    }
                });
            //修改机厅人数 zj
            bot.MessageReceived
                .OfType<GroupMessageReceiver>()
                .Subscribe(async receiver =>
                {
                    if (receiver.MessageChain.GetPlainMessage().Trim().StartsWith("zj="))
                    {
                        if (!ssTime())
                        {
                            await receiver.SendMessageAsync("中集现在没开门,你怎么知道里面有多少人?");
                            return;
                        }
                        int i = 0;
                        try
                        {
                            i = Convert.ToInt32(receiver.MessageChain.GetPlainMessage().Remove(0, 3));
                        }
                        catch (Exception ex)
                        {
                            await receiver.SendMessageAsync(ex.Message);
                            return;
                        }
                        if (i >= 0 && zj + i < 100)
                        {
                            zj = i;
                            zjDate = DateTime.Now;
                            zjMan = receiver.Sender.Name;
                            if (i == 0)
                            {
                                await receiver.SendMessageAsync($"修改成功!中集现在没人了");
                            }
                            else
                            {
                                await receiver.SendMessageAsync($"修改成功!中集现在有{zj}人");
                            }
                        }
                        else
                        {
                            await receiver.SendMessageAsync($"请不要乱玩bot(怒)");
                        }
                    }
                });
            //减少机厅人数 zj
            bot.MessageReceived
                .OfType<GroupMessageReceiver>()
                .Subscribe(async receiver =>
                {
                    if (receiver.MessageChain.GetPlainMessage().Trim().StartsWith("zj-"))
                    {
                        if (!ssTime())
                        {
                            await receiver.SendMessageAsync("中集还没开门,哪里来的人?");
                            return;
                        }
                        int i = 0;
                        try
                        {
                            i = Convert.ToInt32(receiver.MessageChain.GetPlainMessage().Remove(0, 3));
                        }
                        catch (Exception ex)
                        {
                            await receiver.SendMessageAsync(ex.Message);
                            return;
                        }
                        if (i > 0 && zj - i >= 0)
                        {
                            zj -= i;
                            zjDate = DateTime.Now;
                            zjMan = receiver.Sender.Name;
                            await receiver.SendMessageAsync($"减少成功!中集现在有{zj}人");
                        }
                        else
                        {
                            await receiver.SendMessageAsync($"请不要乱玩bot(怒)");
                        }
                    }
                });
            //星
            bot.MessageReceived
                .OfType<GroupMessageReceiver>()
                .Subscribe(async receiver =>
                {
                    if (receiver.MessageChain.GetPlainMessage().Trim().Equals("星将进度") || receiver.MessageChain.GetPlainMessage().Trim().Equals("星舞舞进度") || receiver.MessageChain.GetPlainMessage().Trim().Equals("星级进度"))
                    {
                            await receiver.SendMessageAsync("星星星,天天就知道星,这么喜欢星你怎么自己不变成星星飞到$BGA的家里");
                    }
                });

            //宙
            bot.MessageReceived
                .OfType<GroupMessageReceiver>()
                .Subscribe(async receiver =>
                {
                    if (receiver.MessageChain.GetPlainMessage().Trim().Equals("宙将进度") || receiver.MessageChain.GetPlainMessage().Trim().Equals("宙舞舞进度") || receiver.MessageChain.GetPlainMessage().Trim().Equals("宙级进度"))
                    {
                        await receiver.SendMessageAsync("再宙就让你和&BGA不存在的东西一起送到宇宙和舞萌DX2022跳舞");
                    }
                });

            //输入STOP退出与手动添加数据
            String s;
            do
            {
                s = Console.ReadLine();
                if (!s.Equals("stop"))
                {
                    try
                    {
                        ss = Convert.ToInt32(s);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    ssDate = DateTime.Now;
                    ssMan = "柊韵(控制台)";
                }
            } while (!s.Equals("stop"));
        }
        private static void setTaskAtFixedTime()
        {
            DateTime now = DateTime.Now;
            DateTime oneOClock = DateTime.Today.AddHours(3.0); //凌晨3：00
            if (now > oneOClock)
            {
                oneOClock = oneOClock.AddDays(3.0);
            }
            int msUntilFour = (int)((oneOClock - now).TotalMilliseconds);

            var t = new System.Threading.Timer(doAt3AM);
            t.Change(msUntilFour, Timeout.Infinite);
        }
        private static void doAt3AM(object state)
        {
            //执行功能...
            ss = 0;
            ssMan = "3点自动重置";
            ssDate = DateTime.Now;
            //再次设定
            setTaskAtFixedTime();

        }
        private static bool ssTime()
        {
            if (DateTime.Now.Hour < 10 || DateTime.Now.Hour > 23)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
