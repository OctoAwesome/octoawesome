using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using OctoAwesome.Client.Components;
using Microsoft.Xna.Framework.Content;
using System.Linq;
using System.Text;

namespace OctoAwesome.Client
{
    class CrewMember
    {
        public enum Achievements
        {
            Patron,
            Streamer,
            HardwareDealer,
            Coder,
            Livegast,
            Tools,
            EmailJoker,
            Musik,
            Grafik
        };

        public string Username { get; set; }
        public string Alias { get; set; }

        public string Description { get; set; }

        public Dictionary<String, String> Urls { get; set; }

        public List<Achievements> AchievementList { get; set; }

        public Texture2D Picture { get; set; }

        public CrewMember(String username)
        {
            Username = username;
        }

        public CrewMember(String username, Texture2D picture)
        {
            Username = username;
            Picture = picture;
        }



        public static List<CrewMember> getCrew(ContentManager content)
        {
            List<CrewMember> crew = new List<CrewMember>();

            CrewMember Andy = new CrewMember("Andy");
            Andy.Description = "Andreas hat mich in den vergangenen 200 Folgen sicher mit den meisten Emails versorgt zum Thema Octo-Theorie. Egal ob es um den Cache oder die Serialisierung ging: Von ihm habe ich nicht nur \"du hast da nen Tippfehler gemacht\"-Mails bekommen, sondern sehr umfangreiche Verbesserungsvorschläge, teilweise mit Code-Fragmenten, erklärenden Zeichnungen und sonstigen Anregungen.";
            Andy.Alias = "Andreas";
            Andy.Urls = new Dictionary<string, string> { {"Test","www.google.at"} };
            Andy.AchievementList = new List<Achievements> { Achievements.EmailJoker };
            crew.Add(Andy);

            CrewMember Anteru = new CrewMember("Anteru");
            Anteru.Description = "Matthäus, der Mann fürs grafisch Grobe. Wir haben uns auf dem Game Camp kennen gelernt und haben festgestellt, dass er hier noch einiges beitragen kann. Als AMD Engineer hilft er im echten Leben den wirklich großen Projekten zur besseren Grafik-Performance und hat in diesem Zusammenhang schon etliche tolle Tipps platziert.";
            Anteru.Alias = "Matthäus";
            Anteru.Urls = new Dictionary<string, string> { { "Blog", "www.google.at" } };
            Anteru.AchievementList = new List<Achievements> { Achievements.EmailJoker, Achievements.Livegast, Achievements.Streamer};
            crew.Add(Anteru);

            CrewMember bobstriker = new CrewMember("bobstriker");
            bobstriker.Description = "Das ist der Typ, der meistens vor der Kamera sitzt und versucht das Projekt zusammen zu halten. Ich denke das läuft ganz gut. Als gelernter Programmierer versucht er wohl zu allem ein bisschen was zu wissen und zeigt in OctoAwesome seine Herangehensweise an Software-Probleme.";
            bobstriker.Alias = "Tom";
            bobstriker.Urls = new Dictionary<string, string> { { "Test", "www.google.at" } };
            bobstriker.AchievementList = new List<Achievements> { Achievements.Streamer, Achievements.Livegast };
            crew.Add(bobstriker);

            return crew;

        }
    }
}

    
