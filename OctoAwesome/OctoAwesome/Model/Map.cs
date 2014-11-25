using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Model
{
    internal sealed class Map
    {
        public const int CELLSIZE = 100;

        public CellType[,] Cells { get; private set; }

        public Map()
        {
            Cells = new CellType[20, 20];

            for (int x = 0; x < 20; x++)
            {
                for (int y = 0; y < 20; y++)
                {
                    if (x > 5 && x < 15 || y > 5 && y < 15)
                    {
                        Cells[x, y] = CellType.Sand;
                    }
                    else
                    {
                        Cells[x, y] = CellType.Gras;
                    }
                }
            }
        }
    }

    public enum CellType
    {
        Gras,
        Sand,
        Water
    }
}
