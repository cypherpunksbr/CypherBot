using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using Telegram.Bot;

namespace CypherBot.Data
{
    internal class MessagesToDelete
    {
        public static List<MessageForDelete> messagesToDelete = new List<MessageForDelete>();

        public static System.Timers.Timer timerMessageToDeletesData = new System.Timers.Timer(1000);

        public static void TimerMessageToDeletesData_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            foreach (MessageForDelete messageForDelete in messagesToDelete.ToArray())
            {
                if (messageForDelete.MessageExpire <= DateTime.UtcNow)
                {
                    CypherBot.botClient.DeleteMessageAsync(messageForDelete.ChatId, messageForDelete.MessageId);
                    messagesToDelete.Remove(messageForDelete);
                }
            };

            if (tempSerailizedData != JsonConvert.SerializeObject(MessagesToDelete.messagesToDelete, Formatting.Indented))
            {
                dontWriteFile = true;
                tempSerailizedData = JsonConvert.SerializeObject(MessagesToDelete.messagesToDelete, Formatting.Indented);
                if (!Directory.Exists(Directory.GetCurrentDirectory() + @"/data/")) { Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"/data/"); }
                File.WriteAllText(Props.dataMessagesToDeletePatch, tempSerailizedData);
                dontWriteFile = false;
            }
        }

        public static void LoadMessageToDeletesData()
        {
            dontWriteFile = true;
            if (File.Exists(Props.dataMessagesToDeletePatch))
            {
                messagesToDelete = JsonConvert.DeserializeObject<List<MessageForDelete>>(File.ReadAllText(Props.dataMessagesToDeletePatch));
            }
            dontWriteFile = false;
        }

        private static bool dontWriteFile;
        private static string tempSerailizedData;
    }

    public class MessageForDelete
    {
        public DateTime MessageExpire { get; set; }
        public int MessageId { get; set; }
        public Int64 ChatId { get; set; }
    }
}