using System;
using System.IO;

namespace CypherBot
{
    internal class Props
    {
        public static string telegramToken { get; } = "1410185496:AAEm7sjfJnIn_GUOhRc1p1m5nZfQOUgat_8";
        public static Int64 postChannelChatId { get; } = -1001399462961;
        public static Int64 postChannelPeneiraChatId { get; } = -1001417096675;
        public static string PostChannelInviteLink { get; } = "https://t.me/CypherpunksBrasil";
        public static string OfftopicGroupInviteLink { get; } = "https://t.me/+GMSf4a3JhZdlYmEx";
        public static string GrupoPrincipalInviteLink { get; } = "https://t.me/+Ti-3VBjgzwxkZmIx";
        public static Int64 moderatorGroupChatId { get; } = -1001153829050;
        public static Int64 grupoPrincipalChatId { get; } = -1001206367167;
        public static string dataPostsPatch { get; } = Directory.GetCurrentDirectory() + @"/data/posts.json";
        public static string dataSorteioPatch { get; } = Directory.GetCurrentDirectory() + @"/data/sorteio.json";
        public static string dataMessagesToDeletePatch { get; } = Directory.GetCurrentDirectory() + @"/data/messagesToDelete.json";
        public static int postCutAvaliationsNumber { get; set; } = 5;
    }
}