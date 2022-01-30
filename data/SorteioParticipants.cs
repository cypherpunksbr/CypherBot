using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Timers;

namespace CypherBot.Data // fazendo na classe errada
{
    internal class SorteioParticipantes
    {
        public static Dictionary<long, Participante> participantes = new Dictionary<long, Participante>();

        public static System.Timers.Timer timerParticipantesData = new System.Timers.Timer(5000);

        public static void SaveParticipantesData(bool now = false)
        {
            if (now) { WriteParticipantesData(); } else { timerParticipantesData.Elapsed -= timerParticipantesData_Elapsed; timerParticipantesData.Elapsed += timerParticipantesData_Elapsed; timerParticipantesData.AutoReset = false; timerParticipantesData.Stop(); timerParticipantesData.Start(); }
        }

        private static void timerParticipantesData_Elapsed(object sender, ElapsedEventArgs e)
        {
            WriteParticipantesData();
        }

        private static void WriteParticipantesData()
        {
            while (dontWriteFile) { }

            if (tempSerailizedData != JsonConvert.SerializeObject(SorteioParticipantes.participantes, Formatting.Indented))
            {
                dontWriteFile = true;
                tempSerailizedData = JsonConvert.SerializeObject(SorteioParticipantes.participantes, Formatting.Indented);
                if (!Directory.Exists(Directory.GetCurrentDirectory() + @"/data/")) { Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"/data/"); }
                File.WriteAllText(Props.dataSorteioPatch, tempSerailizedData);
                dontWriteFile = false;
            }
        }

        public static void LoadParticipantesData()
        {
            dontWriteFile = true;
            if (File.Exists(Props.dataSorteioPatch))
            {
                participantes = JsonConvert.DeserializeObject<Dictionary<long, Participante>>(File.ReadAllText(Props.dataSorteioPatch));
            }
            dontWriteFile = false;
        }

        private static bool dontWriteFile;
        private static string tempSerailizedData;
    }

    public class Participante
    {
        private string p_Name = null;
        private bool p_IsValid = false;
        private DateTime p_DateTime = DateTime.UtcNow;

        public string Name { get { return p_Name; } set { p_Name = value; SorteioParticipantes.SaveParticipantesData(); } }
        public bool IsValid { get { return p_IsValid; } set { p_IsValid = value; SorteioParticipantes.SaveParticipantesData(); } }
        public DateTime DateTime { get { return p_DateTime; } set { p_DateTime = value; SorteioParticipantes.SaveParticipantesData(); } }
    }
}