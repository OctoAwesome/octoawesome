using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome
{
    public class PlayerInventory : Inventory
    {
        Player Player;

        public PlayerInventory(Player player, int sizeX, int sizeY) : base(sizeX, sizeY)
        {
            Player = player;
        }
    }
}
