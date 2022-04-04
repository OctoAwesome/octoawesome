﻿using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using OctoAwesome.Client.Components;
using System;

namespace OctoAwesome.Client.Crew
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

            WikiAdmin,

            DatenbankFreigeschaltet
        };
        public string Username { get; set; }

        public string Alias { get; set; }

        public string Description { get; set; }
        public Achievements[] AchievementList { get; set; }

        public string? PictureFilename { get; set; }
        public Link[] Links { get; set; }
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
        public override string ToString()
        {
            return Username;
        }
        public class Link
        {

            public Link()
            {
                Title = string.Empty;
                Url = string.Empty;
            }
            [XmlAttribute]
            public string Title { get; set; }
            [XmlAttribute]
            public string Url { get; set; }
        }
    }
}


