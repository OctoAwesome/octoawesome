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
            Grafik,
            WikiAdmin
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



        public static List<CrewMember> getCrew(ScreenComponent manager)
        {
            List<CrewMember> crew = new List<CrewMember>();

            CrewMember Andy = new CrewMember("Andy");
            Andy.Description = "Andreas hat mich in den vergangenen 200 Folgen sicher mit den meisten Emails versorgt zum Thema Octo-Theorie. Egal ob es um den Cache oder die Serialisierung ging: Von ihm habe ich nicht nur \"du hast da nen Tippfehler gemacht\"-Mails bekommen, sondern sehr umfangreiche Verbesserungsvorschläge, teilweise mit Code-Fragmenten, erklärenden Zeichnungen und sonstigen Anregungen.";
            Andy.Alias = "Andreas";
            Andy.Urls = new Dictionary<string, string> { { "Test", "www.google.at" } };
            Andy.AchievementList = new List<Achievements> { Achievements.EmailJoker };
            crew.Add(Andy);

            CrewMember Anteru = new CrewMember("Anteru");
            Anteru.Description = "Matthäus, der Mann fürs grafisch Grobe. Wir haben uns auf dem Game Camp kennen gelernt und haben festgestellt, dass er hier noch einiges beitragen kann. Als AMD Engineer hilft er im echten Leben den wirklich großen Projekten zur besseren Grafik-Performance und hat in diesem Zusammenhang schon etliche tolle Tipps platziert.";
            Anteru.Alias = "Matthäus";
            Anteru.Urls = new Dictionary<string, string> { { "Blog", "www.google.at" } };
            Anteru.AchievementList = new List<Achievements> { Achievements.EmailJoker, Achievements.Livegast, Achievements.Streamer };
            crew.Add(Anteru);

            CrewMember bobstriker = new CrewMember("bobstriker");
            bobstriker.Description = "Das ist der Typ, der meistens vor der Kamera sitzt und versucht das Projekt zusammen zu halten. Ich denke das läuft ganz gut. Als gelernter Programmierer versucht er wohl zu allem ein bisschen was zu wissen und zeigt in OctoAwesome seine Herangehensweise an Software-Probleme.";
            bobstriker.Alias = "Tom";
            bobstriker.Picture = manager.Content.LoadTexture2DFromFile("./Assets/OctoAwesome.Client/Crew/bobstriker.png", manager.GraphicsDevice);
            bobstriker.Urls = new Dictionary<string, string> { { "Test", "www.google.at" } };
            bobstriker.AchievementList = new List<Achievements> { Achievements.Streamer, Achievements.Livegast };
            crew.Add(bobstriker);

            CrewMember Christian = new CrewMember("Christian");
            Christian.Description = "Christian kenne ich schon seit meiner Zeit bei Microsoft. Er ist zwar erster Software Entwickler, hat seinen Spieltrieb aber wohl nie verloren und hat sogar an einem recht erfolgreichen Bomberman-Remake mitgearbeitet. Von ihm stammt übrigens das großartige \"Ich bin kein Tutorial\"-Shirt ;)";
            Christian.Alias = "Christian";
            Christian.AchievementList = new List<Achievements> { Achievements.Patron, Achievements.HardwareDealer };
            crew.Add(Christian);

            CrewMember Dennis = new CrewMember("Dennis");
            Dennis.Description = "Nach Folge 250 hat mir Dennis (aka. BlackOrca) doch prompt eine kleine Wer-bin-ich Nachricht geschickt die mich sehr gefreut hat. Viele Jahre verfolgt er bereits Projekte wie MoonTaxi, AntMe! und nun eben OctoAwesome. Ihm steht IoT auf der Flagge - speziell das Verbinden von Hard- und Software im Heimbereich.";
            Dennis.Alias = "Dennis";
            Dennis.AchievementList = new List<Achievements> { Achievements.Patron };
            crew.Add(Dennis);

            CrewMember jvbsl = new CrewMember("jvbsl");
            jvbsl.Description = "Julian (aka jvbsl) studiert die gute alte Informatik hier direkt ums Eck in Garching. Wir haben uns so wirklich bewusst - zumindest für mich - am Game Camp in München getroffen uns seither ein paar coole Sachen zusammen gemacht. Angefangen natürlich bei unserem gemeinsamen Livestream zu MoonTaxi. Außerdem arbeitet er gerade an einem Remake des Jump'n'Run Klassikers Jazz Jackrabbit 2 (zu sehen auch im Livestream). Zusammen mit Patrick ist er für den bisherigen Map-Generator in OctoAwesome verantwortlich. Außerdem geht der OctoBot auf sein Konto ;)";
            jvbsl.Alias = "Julian";
            jvbsl.AchievementList = new List<Achievements> { Achievements.Livegast, Achievements.Streamer, Achievements.Tools };
            crew.Add(jvbsl);

            CrewMember juergen = new CrewMember("Juergen");
            juergen.Description = "Jürgen ist ein stiller aber treuer Zuschauer, der aber in die Presche springt, wenn es drauf ankommt. Dank Jürgen wurde Internet in Leipzig möglich.";
            juergen.Alias = "Juergen";
            juergen.AchievementList = new List<Achievements> { Achievements.Patron };
            crew.Add(juergen);

            CrewMember Kapu = new CrewMember("Kapu");
            Kapu.Description = "Kapu (aka Philipp) habe ich in meiner vorherigen Arbeit im Gründerzentrum kennen gelernt. Er studiert an der MD.H in München und hat sich spontan bereit erklärt den Titel-Song für Octo zu machen. Das Ergebnis hört ihr im Abspann jeder einzelnen Folge.";
            Kapu.Alias = "Philipp";
            Kapu.AchievementList = new List<Achievements> { Achievements.Musik };
            crew.Add(Kapu);

            CrewMember Lassi = new CrewMember("Lassi")
            {
                Description = "Lassi war der erste Stargast im Octo-Universum und hat recht früh im Spiel dafür gesorgt, dass die Physik die richtigen Wege geht. Das ganze Trägheits- und Beschleunigungsthema war noch nie so meins. Wie wir alle wissen, hat er zuletzt seinen Elektrotechniker abgeschlossen, studiert aber tapfer weiter. Ich kann nur hoffen, dass es ein Master mit Schwerpunkt Animations-Physik ist.",
                Alias = "CSharpLassi",
                AchievementList = new List<Achievements> { Achievements.Livegast }
            };
            crew.Add(Lassi);

            CrewMember macx = new CrewMember("macx");
            macx.Description = "macx, eigentlich Maik, kenne ich von diversen Technik-Veranstaltungen im Raum Köln. Von ihm kamen zahlreiche helfende Mails und auch das großzügige Angebot mit uns das Thema Networking auf Basis von WCF anzugehen. Ich werde darauf zurück greifen.";
            macx.Alias = "Maik";
            macx.AchievementList = new List<Achievements> { Achievements.EmailJoker };
            crew.Add(macx);

            CrewMember manuelhu = new CrewMember("manuelhu");
            manuelhu.Description = "Von Manuel habe ich anfangs reichlich Feedback per YouTube-Kommentaren bekommen. Später dann per Email, bis er letztendlich durch die Livestreams zur rettenden Fee im Wiki wurde. Ohne ihn hätten wir hier keinen Episoden-Guide und auch mein Octo-Timer stammt aus seinem Compiler.";
            manuelhu.Alias = "Manuel";
            manuelhu.AchievementList = new List<Achievements> { Achievements.EmailJoker, Achievements.Tools, Achievements.WikiAdmin };
            crew.Add(manuelhu);

            CrewMember MarkusK = new CrewMember("MarkusK");
            MarkusK.Description = "Danke für immer wieder äußerst hilfreiche Mails und natürlich auch die finanzielle Unterstützung.";
            MarkusK.Alias = "Markus";
            MarkusK.AchievementList = new List<Achievements> { Achievements.EmailJoker, Achievements.Patron };
            crew.Add(MarkusK);

            CrewMember Mrsfuckingsunshine = new CrewMember("Mrsfuckingsunshine");
            Mrsfuckingsunshine.Description = "Frau Sonnenschein, auch unter Aline bekannt, ist die gute Seele im Stream. Sie bringt mir nicht nur immer wieder guten warmen Tee sondern ist auch mein grafisches Gewissen und meine Inspiration, ebenso der Saubermann im Chat.";
            Mrsfuckingsunshine.Alias = "Aline";
            Mrsfuckingsunshine.AchievementList = new List<Achievements> { Achievements.Livegast, Achievements.Grafik };
            crew.Add(Mrsfuckingsunshine);

            CrewMember patteki = new CrewMember("patteki");
            patteki.Description = "Patrick ist quasi immer verstrickt, wenn es mit Map-Generierung zu tun hat und ist für unseren ambitionierten Masterplan eines irrsinnigen Klima-Modells auf Octo-Planeten verantwortlich. Neben Octo studiert er Mechatronik und streamt gelegentlich auch seine eigenen Sachen. Zumindest wenn er nicht gerade mitten in einem AntMe! Wettkampf steckt.";
            patteki.Alias = "Patrick";
            patteki.AchievementList = new List<Achievements> { Achievements.Livegast, Achievements.Streamer };
            crew.Add(patteki);

            CrewMember Paul = new CrewMember("Paul");
            Paul.Description = "Paul kenne ich noch aus den guten alten Microsoft Student Partner Zeiten. Er ist Software Entwickler im Raum Berlin und hat uns einzig und alleine ins Chaos gestürzt. Ihn hatten wir im Stream zur Performance- und Speicher-Optimierung. Mit großem Erfolg hat er Octo umgekrempelt.";
            Paul.Alias = "Paul";
            Paul.AchievementList = new List<Achievements> { Achievements.Livegast };
            crew.Add(Paul);

            CrewMember Ralf = new CrewMember("Ralf");
            Ralf.Description = "alf teilt sich mit Andy zusammen die Spitze der mit Abstand umfangreichsten und ausgeklügelten Feedback-Mails. Da kommt nicht nur \"Das und das ist falsch\" sondern meist über Seiten hinweg Lösungsansätze und Referenz-Links. Vielen, vielen Dank dafür!";
            Ralf.Alias = "Ralf";
            Ralf.AchievementList = new List<Achievements> { Achievements.EmailJoker };
            crew.Add(Ralf);

            CrewMember Redwork = new CrewMember("Redwork");
            Redwork.Description = "Von Redwork weiß ich leider auch nicht besonders viel. Außer vielleicht, dass ich tolle Feedback Mails bekommen habe die mich schon über einige hundert Folgen begleiten und mir immer wieder tolle Hinweise geben.";
            Redwork.Alias = "Redwork";
            Redwork.AchievementList = new List<Achievements> { Achievements.EmailJoker };
            crew.Add(Redwork);

            CrewMember SomaFM = new CrewMember("SomaFM");
            SomaFM.Description = "Ich werde ständig gefragt welchen Radio-Sender ich im \"Ladescreen\" verwende. Es handelt sich dabei um \"Secret Agent\" von SomaFM, einem wirklich coolen Streamer. Schaut gerne rein, lasst eine Donation da,...";
            SomaFM.Alias = "Radio";
            SomaFM.AchievementList = new List<Achievements> { Achievements.Tools };
            crew.Add(SomaFM);

            CrewMember susch19 = new CrewMember("susch19");
            susch19.Description = "Susch war von Beginn an Befürworter das Stream-Formats und ist seither immer sehr hilfreich dabei meine Copy-Paste Probleme aufzudecken und an vielen Stellen gutes Feedback zu schicken.";
            susch19.Alias = "Susch19";
            susch19.AchievementList = new List<Achievements> { Achievements.EmailJoker };
            crew.Add(susch19);

            CrewMember Ulrich = new CrewMember("Ulrich");
            Ulrich.Description = "Von Ulrich weiß ich leider auch eigentlich überhaupt nichts :( Aber er unterstützt mich bei Patreon und hilft damit ungemein weiter";
            Ulrich.Alias = "Ulrich";
            Ulrich.AchievementList = new List<Achievements> { Achievements.Patron };
            crew.Add(Ulrich);

            CrewMember vengarioth = new CrewMember("Vengarioth");
            vengarioth.Description = "vengarioth heißt eigentlich Andy und arbeitet als Spieleentwickler hier in München. Nicht nur, dass er weiß wie ein Compiler funktioniert, er weiß auch wie man ein Zeichentablett benutzt. Perfekt also um ihn mal in den Stream zu holen um über Grafik und Style zu sprechen.";
            vengarioth.Alias = "Andy";
            vengarioth.AchievementList = new List<Achievements> { Achievements.Livegast, Achievements.Patron };

            //crew.Sort();




            return crew;

        }
    }
}

    
