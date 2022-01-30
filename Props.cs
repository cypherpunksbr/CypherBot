using System;
using System.IO;

namespace CypherBot
{
    internal class Props
    {
        public static string telegramToken { get; set; } = "telegram_bot_token";
        public static Int64 postChannelChatId { get; set; } = -1001399462961;
        public static string PostChannelInviteLink { get; set; } = "https://t.me/CypherpunksBrasil";
        public static Int64 moderatorGroupChatId { get; set; } = -1001153829050;
        public static Int64 grupoPrincipalChatId { get; set; } = -1001206367167;
        public static string dataPostsPatch { get; set; } = Directory.GetCurrentDirectory() + @"/data/posts.json";
        public static string dataSorteioPatch { get; set; } = Directory.GetCurrentDirectory() + @"/data/sorteio.json";
        public static string dataMessagesToDeletePatch { get; set; } = Directory.GetCurrentDirectory() + @"/data/messagesToDelete.json";
        public static int postCutAvaliationsNumber { get; set; } = 5;
    }
}