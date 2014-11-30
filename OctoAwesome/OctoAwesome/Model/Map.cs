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
        public int Columns { get; set; }

        public int Rows { get; set; }

        public CellType[] Cells { get; set; }

        public Map()
        {
        }

        public CellType GetCell(int x, int y)
        {
            return Cells[(y * Columns) + x];
        }

        public void SetCell(int x, int y, CellType cellType)
        {
            Cells[(y * Columns) + x] = cellType;
        }

        #region Generators

        public static Map Generate(int width, int height, CellType defaultType)
        {
            if (width < 1 || width > 200)
                throw new ArgumentException("width");

            if (height < 1 || height > 200)
                throw new ArgumentException("height");

            Map map = new Map();
            map.Columns = width;
            map.Rows = height;
            map.Cells = new CellType[width * height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    map.SetCell(x, y, defaultType);
                }
            }

            return map;
        }

        #endregion

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
