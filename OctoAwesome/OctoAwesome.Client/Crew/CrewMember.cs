using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using OctoAwesome.Client.Components;
using System;

namespace OctoAwesome.Client.Crew
{
    /// <summary>
    /// Describes an OctoAwesome crew member.
    /// </summary>
    public class CrewMember
    {
        /// <summary>
        /// Enumeration of crew member achievements.
        /// </summary>
        public enum Achievements
        {
            /// <summary>
            /// Crew member is a patron.
            /// </summary>
            Patron,
            /// <summary>
            /// Crew member is a streamer.
            /// </summary>
            Streamer,
            /// <summary>
            /// Crew member is a hardware dealer.
            /// </summary>
            HardwareDealer,
            /// <summary>
            /// Crew member is a coder.
            /// </summary>
            Coder,
            /// <summary>
            /// Crew member is a life guest.
            /// </summary>
            Livegast,
            /// <summary>
            /// Crew member developed tools.
            /// </summary>
            Tools,
            /// <summary>
            /// Crew member is an email joker.
            /// </summary>
            EmailJoker,
            /// <summary>
            /// Crew member is a contributor.
            /// </summary>
            Contributor,
            /// <summary>
            /// Crew member made music/sound.
            /// </summary>
            Musik,
            /// <summary>
            /// Crew member made graphic.
            /// </summary>
            Grafik,
            /// <summary>
            /// Crew member is a wiki admin.
            /// </summary>
            WikiAdmin,
            /// <summary>
            /// Crew member has databases activated.
            /// </summary>
            DatenbankFreigeschaltet
        };

        /// <summary>
        /// Gets or sets the username of the crew member.
        /// </summary>
        public string Username { get; set; }


        /// <summary>
        /// Gets or sets the alias of the crew member.
        /// </summary>
        public string Alias { get; set; }


        /// <summary>
        /// Gets or sets the description for the crew member.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the username of the crew member.
        /// </summary>
        public Achievements[] AchievementList { get; set; }

        /// <summary>
        /// Gets or sets the filename for the picture of the crew member.
        /// </summary>
        /// <remarks><c>null</c> falls back to a default picture.</remarks>
        public string? PictureFilename { get; set; }

        /// <summary>
        /// Gets or sets a list of additional links of the crew member.
        /// </summary>
        public Link[] Links { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CrewMember"/> class.
        /// </summary>
        public CrewMember()
        {
            Username = string.Empty;
            Alias = string.Empty;
            Description = string.Empty;
            PictureFilename = null;
            Links = Array.Empty<Link>();
            AchievementList = Array.Empty<Achievements>();
        }

        internal static List<CrewMember> GetCrew(ScreenComponent manager)
        {
            using (Stream stream = manager.Game.Assets.LoadStream(typeof(CrewMember), "Crew.crew", "xml"))
            {
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(List<CrewMember>));
                    var res = serializer.Deserialize(stream);
                    if (res is List<CrewMember> crewMembers)
                    {
                        return crewMembers;
                    }
                }
                catch (Exception)
                { }

                return new List<CrewMember>();
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return Username;
        }

        /// <summary>
        /// Link class for having titled urls.
        /// </summary>
        public class Link
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Link"/> class.
            /// </summary>
            public Link()
            {
                Title = string.Empty;
                Url = string.Empty;
            }

            /// <summary>
            /// Gets or sets the title of the link.
            /// </summary>
            [XmlAttribute]
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the url of the link.
            /// </summary>
            [XmlAttribute]
            public string Url { get; set; }
        }
    }
}


