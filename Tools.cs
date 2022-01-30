using System.IO;
using System.Text;
using Telegram.Bot.Types;

namespace CypherBot
{
    internal class Tools
    {
        public static string GetFullName(User messageFrom)
        {
            string fullname = null;

            try
            {
                if (messageFrom.FirstName != null) { fullname += messageFrom.FirstName; }
            }
            catch { }
            try
            {
                if (messageFrom.LastName != null) { fullname += " " + messageFrom.LastName; }
            }
            catch { }

            return fullname;
        }

        public static string GetFullNameForMarkdownUse(User messageFrom)
        {
            string fullname = null;

            try
            {
                if (messageFrom.FirstName != null) { fullname += messageFrom.FirstName; }
            }
            catch { }
            try
            {
                if (messageFrom.LastName != null) { fullname += " " + messageFrom.LastName; }
            }
            catch { }

            return fullname.Replace("_", @"\_").Replace("*", @"\*").Replace("[", @"\[").Replace("]", @"\]").Replace("(", @"\(").Replace(")", @"\)").Replace("~", @"\~").Replace("`", @"\`").Replace(">", @"\>").Replace("#", @"\#").Replace("+", @"\+").Replace("-", @"\-").Replace("=", @"\=").Replace("|", @"\|").Replace("{", @"\{").Replace("}", @"\}").Replace(".", @"\.").Replace("!", @"\!");
        }

        public static string StringForkMarkdownV2Use(string text = null)
        {
            if (text == null) { return null; }

            return text.Replace("_", @"\_").Replace("*", @"\*").Replace("[", @"\[").Replace("]", @"\]").Replace("(", @"\(").Replace(")", @"\)").Replace("~", @"\~").Replace("`", @"\`").Replace(">", @"\>").Replace("#", @"\#").Replace("+", @"\+").Replace("-", @"\-").Replace("=", @"\=").Replace("|", @"\|").Replace("{", @"\{").Replace("}", @"\}").Replace(".", @"\.").Replace("!", @"\!");
        }

        public static string GetChannelPostDescription(Message message)
        {
            StringBuilder description = new StringBuilder();

            description.AppendLine("Postada por [" + Tools.GetFullNameForMarkdownUse(message.ReplyToMessage.From) + "](tg://user?id=" + message.ReplyToMessage.From.Id + ")");
            //if (message.ReplyToMessage.IsForwarded)
            //{
            //    Console.WriteLine("IsForwarded");
            //    description.AppendLine("Encaminhada de [" + Tools.StringForkMarkdownUse(message.ReplyToMessage.ForwardSenderName) + "](tg://user?id=" + message.ReplyToMessage.ForwardFrom.Id + ")");
            //}
            description.AppendLine("Sugerida por [" + Tools.GetFullNameForMarkdownUse(message.From) + "](tg://user?id=" + message.From.Id + ')');

            description.AppendLine();
            description.AppendLine("Via @CypherpunksBrasil");

            return description.ToString();
        }

        public static Telegram.Bot.Types.InputFiles.InputOnlineFile GetparticipantesList()
        {
            StringBuilder participantesListString = new StringBuilder();
            int counter = 0;

            participantesListString.AppendLine("Num.\tUserID\tData da participação (UTC)\tNome");

            foreach (var user in Data.SorteioParticipantes.participantes)
            {
                if (user.Value.IsValid)
                {
                    counter++;
                    participantesListString.AppendLine(counter.ToString().PadLeft(4, '0') + "\t" + user.Key + "\t" + user.Value.DateTime.ToShortDateString() + ' ' + user.Value.DateTime.ToShortTimeString() + "\t" + user.Value.Name);
                }
            }

            return new Telegram.Bot.Types.InputFiles.InputOnlineFile(new MemoryStream(Encoding.UTF8.GetBytes(participantesListString.ToString())), "participantes.csv");
        }
    }
}