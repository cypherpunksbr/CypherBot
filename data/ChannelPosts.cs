using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Timers;

namespace CypherBot.Data
{
    public class ChannelPosts // esse é a classe dos usuários
    {
        public static Dictionary<string, Post> posts = new Dictionary<string, Post>();

        public static System.Timers.Timer timerSaveData = new System.Timers.Timer(5000);

        public static void SavePostsData(bool now = false)
        {
            if (now) { WritePostsData(); } else { timerSaveData.Elapsed -= timerSaveData_Elapsed; timerSaveData.Elapsed += timerSaveData_Elapsed; timerSaveData.AutoReset = false; timerSaveData.Stop(); timerSaveData.Start(); }
        }

        private static void timerSaveData_Elapsed(object sender, ElapsedEventArgs e)
        {
            WritePostsData();
        }

        private static void WritePostsData() // salva dados
        {
            while (dontWriteFile) { }

            if (tempSerailizedData != JsonConvert.SerializeObject(ChannelPosts.posts, Formatting.Indented))
            {
                dontWriteFile = true;
                tempSerailizedData = JsonConvert.SerializeObject(ChannelPosts.posts, Formatting.Indented);
                if (!Directory.Exists(Directory.GetCurrentDirectory() + @"/data/")) { Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"/data/"); }
                File.WriteAllText(Props.dataPostsPatch, tempSerailizedData);
                dontWriteFile = false;
            }
        }

        public static void LoadPostsData() // restaura dados para a variáv
        {
            dontWriteFile = true;
            if (File.Exists(Props.dataPostsPatch))
            {
                posts = JsonConvert.DeserializeObject<Dictionary<string, Post>>(File.ReadAllText(Props.dataPostsPatch));
            }
            dontWriteFile = false;
        }

        private static bool dontWriteFile;
        private static string tempSerailizedData;
    }

    public class Post // classe dos post do canal
    {
        private Int64 p_forwardedFrom = 0;
        private Int64 p_sentBy = 0;
        private Int64 p_recomendedBy = 0;
        private Int64 p_authorizedBy = 0;
        private List<Int64> p_PostLikes = new List<Int64>();
        private List<Int64> p_PostDislikes = new List<Int64>();
        private bool p_banned = false;

        public Int64 forwardedFrom { get { return p_forwardedFrom; } set { p_forwardedFrom = value; ChannelPosts.SavePostsData(); } }
        public Int64 sentBy { get { return p_sentBy; } set { p_sentBy = value; ChannelPosts.SavePostsData(); } }
        public Int64 recomendedBy { get { return p_recomendedBy; } set { p_recomendedBy = value; ChannelPosts.SavePostsData(); } }
        public Int64 authorizedBy { get { return p_authorizedBy; } set { p_authorizedBy = value; ChannelPosts.SavePostsData(); } }
        public List<Int64> PostLikes { get { return p_PostLikes; } set { p_PostLikes = value; ChannelPosts.SavePostsData(); } }
        public List<Int64> PostDislikes { get { return p_PostDislikes; } set { p_PostDislikes = value; ChannelPosts.SavePostsData(); } }
        public bool banned { get { return p_banned; } set { p_banned = value; ChannelPosts.SavePostsData(); } }
    }
}