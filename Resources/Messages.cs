using System;
using Telegram.Bot.Types.ReplyMarkups;

namespace CypherBot.Resources
{
    internal class Messages
    {
        public static string offtopic_request = @"Acesse nosso grupo offtopic pelo link 🔗 [\#cypherpunks \[OFF\]](" + Props.OfftopicGroupInviteLink + ") 🔗";
        public static string sorteio_group_request;

        public static InlineKeyboardMarkup replyMarkupChannelRating(Int64 chatID = 0, int messageID = 0)
        {
            string likeText = "👍";
            if (Data.ChannelPosts.posts != null && Data.ChannelPosts.posts.ContainsKey(chatID + ":" + messageID) && (Data.ChannelPosts.posts[chatID + ":" + messageID].PostLikes.Count > 0 || Data.ChannelPosts.posts[chatID + ":" + messageID].PostDislikes.Count > 0)) { likeText += " " + Data.ChannelPosts.posts[chatID + ":" + messageID].PostLikes.Count; }

            string dislikeText = "🚫";
            if (Data.ChannelPosts.posts != null && Data.ChannelPosts.posts.ContainsKey(chatID + ":" + messageID) && (Data.ChannelPosts.posts[chatID + ":" + messageID].PostLikes.Count > 0 || Data.ChannelPosts.posts[chatID + ":" + messageID].PostDislikes.Count > 0)) { dislikeText += " " + Data.ChannelPosts.posts[chatID + ":" + messageID].PostDislikes.Count; }

            return new InlineKeyboardMarkup(new[]
            {
                InlineKeyboardButton.WithCallbackData(likeText, "ChannelPostLike"),
                InlineKeyboardButton.WithCallbackData(dislikeText, "ChannelPostDislike"),
                InlineKeyboardButton.WithUrl("💬 comentários", Props.GrupoPrincipalInviteLink)
            });
        }
    }
}