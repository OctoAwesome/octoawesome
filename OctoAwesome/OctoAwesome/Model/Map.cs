using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OctoAwesome.Model
{
    [Serializable]
    public sealed class Map
    {
        public const int CELLSIZE = 100;

        public CellType[,] Cells { get; private set; }

        public Map(int width, int height)
        {
            Cells = new CellType[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    //if (x > 5 && x < 15 || y > 5 && y < 15)
                    //{
                    //    Cells[x, y] = CellType.Sand;
                    //}
                    //else
                    //{
                        Cells[x, y] = CellType.Gras;
                    //}
                }
            }
        }

        #region Loader

        public static Map Load(string filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Map));
            using (Stream stream = File.OpenRead(filename))
            {
                return (Map)serializer.Deserialize(stream);
            }
        }

        public static void Save(string filename, Map map)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Map));
            using (Stream stream = File.OpenWrite(filename))
            {
                serializer.Serialize(stream, map);
            }
        }

        #endregion
    }

    public enum CellType
    {
        Gras,
        Sand,
        Water
    }
}
