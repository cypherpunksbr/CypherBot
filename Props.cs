using System;
using System.IO;

namespace CypherBot
{
    internal class Props
    {
        public static string telegramToken { get; set; } = "telegram_bot_token";
        public static Int64 postChannelChatId { get; } = -1001399462961;
        public static string PostChannelInviteLink { get; } = "https://t.me/CypherpunksBrasil";
        public static string OfftopicGroupInviteLink { get; } = "https://t.me/+WromIg-zQmg4MDAx";
        public static Int64 moderatorGroupChatId { get; } = -1001153829050;
        public static Int64 grupoPrincipalChatId { get; } = -1001206367167;
        public static string dataPostsPatch { get; } = Directory.GetCurrentDirectory() + @"/data/posts.json";
        public static string dataSorteioPatch { get; } = Directory.GetCurrentDirectory() + @"/data/sorteio.json";
        public static string dataMessagesToDeletePatch { get; } = Directory.GetCurrentDirectory() + @"/data/messagesToDelete.json";
        public static int postCutAvaliationsNumber { get; set; } = 5;
    }
}