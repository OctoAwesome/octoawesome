using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using MonoGameUi;

namespace OctoAwesome.Client
{
    public class CrewMember
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
            Contributor,
            Musik,
            Grafik,
            WikiAdmin
        };

        public string Username { get; set; }
        public string Alias { get; set; }

        public string Description { get; set; }

        public List<Achievements> AchievementList { get; set; }

        public string PictureFilename { get; set; }

        public CrewMember() { }

        public static List<CrewMember> getCrew(BaseScreenComponent manager)
        {
            using (Stream stream = File.Open("./Assets/crew.xml", FileMode.Open, FileAccess.Read))
            {
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(List<CrewMember>));
                    return (List<CrewMember>)serializer.Deserialize(stream);
                }
                catch { }

                return new List<CrewMember>();                
            }
        }
    }
}

    
