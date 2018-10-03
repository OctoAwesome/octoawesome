using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.GameServer.Commands
{
    public enum OfficialCommands : ushort
    {
        Whoami = 11,
        GetUniverse = 12,
        GetPlanet = 13,
        LoadColumn = 14,
        SaveColumn = 15
    }
}
