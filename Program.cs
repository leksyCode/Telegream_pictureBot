using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;

namespace BotV2
{
    class Program
    {
        private static string userQuery;
        private static ITelegramBotClient botClient;
        private static string currentUrl;

        static void Main(string[] args)
        {
            botClient = new TelegramBotClient("632174257:AAEAbDwxgrd-EcsjWCW6E36zkPeH7GiiHao");

            var me = botClient.GetMeAsync().Result;

            botClient.OnMessage += Bot_OnMessage;
            botClient.StartReceiving();
            Thread.Sleep(int.MaxValue);
        }


        static async void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            Random rand = new Random(DateTime.Now.Millisecond);

            if (e.Message.Text != null)
            {
                userQuery = e.Message.Text;
           
                string html = GetHtmlCode();
                List<string> urls = GetUrls(html);
                currentUrl = urls[rand.Next(0, urls.Count - 1)];

                try
                {
                    await botClient.SendPhotoAsync(
           chatId: e.Message.Chat,
           caption: $"<b>{userQuery}</b>",
           photo: currentUrl,
           parseMode: ParseMode.Html);
                }
                catch (Exception)
                {
                    await botClient.SendTextMessageAsync(
           chatId: e.Message.Chat,
           text: "OMG, thi's unbelivable i can't find a picture! \n " +
           "Only text & emoji are correct. Don't remember check your connection :D");

                }
            }
        }

        private static string GetHtmlCode()
        {
            string url = "https://www.google.com/search?q=" + userQuery + "&tbm=isch";
            string data = "";

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Accept = "text/html, application/xhtml+xml, */*";
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko";

            var response = (HttpWebResponse)request.GetResponse();

            using (Stream dataStream = response.GetResponseStream())
            {
                if (dataStream == null)
                    return "";
                using (var sr = new StreamReader(dataStream))
                {
                    data = sr.ReadToEnd();
                }
            }
            return data;
        }

        private static List<string> GetUrls(string html)
        {
            var urls = new List<string>();

            int ndx = html.IndexOf("\"ou\"", StringComparison.Ordinal);

            while (ndx >= 0)
            {
                ndx = html.IndexOf("\"", ndx + 4, StringComparison.Ordinal);
                ndx++;
                int ndx2 = html.IndexOf("\"", ndx, StringComparison.Ordinal);
                string url = html.Substring(ndx, ndx2 - ndx);
                urls.Add(url);
                ndx = html.IndexOf("\"ou\"", ndx2, StringComparison.Ordinal);
            }
            return urls;
        }
    }
}
