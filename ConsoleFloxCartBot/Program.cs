using System.Globalization;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;


decimal current = 0;
decimal total = 0;
decimal rate = 0;
decimal Comission = 0;
decimal SuggestCurrency = 0;
Int64 SuggestDay = 0;
List<string> list = new List<string>();


using var cts = new CancellationTokenSource();
var bot = new TelegramBotClient("7218401198:AAGRcEI1y09uhV-CtwIsoeh4r9JLKLuVm5k", cancellationToken: cts.Token);
var me = await bot.GetMeAsync();
Console.WriteLine($"@{me.Username} is running... Press Enter to terminate(before on message)");

bot.OnMessage += OnMessage;

Console.WriteLine($"@{me.Username} is running... Press Enter to terminate");
////var replyMarkup = new ReplyKeyboardMarkup(true)
////    .AddButton("Help me")
////    .AddNewRow("Call me ☎️", "Write me ✉️");

////var sent = await bot.SendTextMessageAsync(me.Id, "Choose a response", replyMarkup: replyMarkup);
Console.ReadLine();
cts.Cancel(); // stop the bot

// method that handle messages received by the bot:
async Task OnMessage(Message msg, UpdateType type)
{
    ////var replyMarkup = new ReplyKeyboardMarkup()
    ////.AddButton("مقدار موجودی الان خود را وارد کنید");    

    //// var sent = await bot.SendTextMessageAsync(msg.Chat, "Choose a response", replyMarkup: replyMarkup);
    try
    {


        if (msg.Text == "/start")
        {
            current = 0;
            total = 0;
            rate = 0;
            Comission = 0;
            SuggestCurrency = 0;
            SuggestDay = 0;

            list.Clear();

            ////var replyMarkup = new ReplyKeyboardMarkup().AddButton("مقدار موجودی الان خود را وارد کنید");
            ////var sent = await bot.SendTextMessageAsync(msg.Chat, "Choose a response", replyMarkup: replyMarkup);
            await bot.SendTextMessageAsync(msg.Chat, "به ربات محاسبه درآمد فلوکارت خوش آمدید");
            await bot.SendTextMessageAsync(msg.Chat, "مقدار موجودی الان خود را وارد کنید و علامت $ در انتها قرار دهید");
        }
        if (msg.Text.Contains('$'))
        {
            current = decimal.Parse(msg.Text.Remove(msg.Text.IndexOf('$'), 1), CultureInfo.InvariantCulture.NumberFormat);
            await bot.SendTextMessageAsync(msg.Chat, "درصد VIP خود را وارد کنید و علامت % را درانتها قرار دهید");
            //await bot.SendTextMessageAsync(msg.Chat, "مقدار وارد شده = " + current);
        }

        if (msg.Text.Contains('%'))
        {
            rate = decimal.Parse(msg.Text.Remove(msg.Text.IndexOf('%'), 1), CultureInfo.InvariantCulture.NumberFormat);
            rate = rate / 100;
            await bot.SendTextMessageAsync(msg.Chat, "مقدار کمیسیون روزانه خود از تیم را وارد کنید و علامت & را در انتها وارد کنید");
            //await bot.SendTextMessageAsync(msg.Chat, "مقدار وارد شده = " + current);
        }

        if (msg.Text.Contains('&'))

        {
            Comission = decimal.Parse(msg.Text.Remove(msg.Text.IndexOf('&'), 1), CultureInfo.InvariantCulture.NumberFormat);
            await bot.SendTextMessageAsync(msg.Chat, "درآمد مورد انتظار خود را وارد کنید(به واحد تتر) و در انتها علامت # را وارد کنید");
            //await bot.SendTextMessageAsync(msg.Chat, "مقدار وارد شده = " + current);
        }
        if (msg.Text.Contains('#'))
        {

            list.Clear();

            SuggestCurrency = decimal.Parse(msg.Text.Remove(msg.Text.IndexOf('#'), 1), CultureInfo.InvariantCulture.NumberFormat);

            int day = 0;
            total = current;
            //richTextBox1.Text = "";

            if (total <= 0 || rate <= 0 || SuggestCurrency <= 0)
                return;
            
            while (total < SuggestCurrency)
            {
                for (int i = 1; i <= 11; i++)
                    total = total + (total * rate);
                ++day;
                total = total + Comission;
                list.Add(Environment.NewLine + " بعد از " + day.ToString() +" روز "  + " " +   total.ToString("0.00") + " " );
                //await bot.SendTextMessageAsync(msg.Chat, Environment.NewLine + day.ToString() + " روز : " + total.ToString("0.00"));
                //richTextBox1.AppendText(Environment.NewLine + day.ToString() + " روز : " + total.ToString("0.00"));
            }
            await bot.SendTextMessageAsync(msg.Chat, Environment.NewLine + day.ToString() + " روز نیاز است ");
            await bot.SendTextMessageAsync(msg.Chat, "درصورت نیاز به مشاهده درآمد روزهای محاسبه شده علامت + را وارد نمایید");
        }
        if (msg.Text == "+")
        {
            if (list.Count > 60)
            {
                await bot.SendTextMessageAsync(msg.Chat, "تعداد روز بیشتر از 60 است و فرآیند طولانی می شود لذا جزییات نمایش داده نمی شود.");
                await bot.SendTextMessageAsync(msg.Chat, "احتمالا درآمد مورد انتظار خود را زیادتر از حد معمول وارد کرده اید");
                return;
            }
            await bot.SendTextMessageAsync(msg.Chat, "به تعداد روزهای نمایش داده شده در پیام قبلی باید رکورد نمایش داده شود. لذا کمی صبور باشد");
            foreach (var item in list)
                await bot.SendTextMessageAsync(msg.Chat, item);
        }
        } catch (Exception ex) 
    { 
        //do nothing
    }


        // await bot.SendTextMessageAsync(msg.Chat, "موجودی مورد انتظار خود را وارد کنید و در انتها علامت # را وارد کنید");
        //await bot.SendTextMessageAsync(msg.Chat, "مقدار وارد شده = " + current);
    
    //if (msg.Text is null) return;	// we only handle Text messages here
    //Console.WriteLine($"Received {type} '{msg.Text}' in {msg.Chat}");
    // let's echo back received text in the chat
   // await bot.SendTextMessageAsync(msg.Chat, $"{msg.From} said: {msg.Text}");
}
