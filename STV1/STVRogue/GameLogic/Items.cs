﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using STVRogue.Utils;

namespace STVRogue.GameLogic
{

	[Serializable]
	public class Item
    {
        public String id;
        public Boolean used = false;
        public bool rejected = false;
        public Item() { }
        public Item(String id) { this.id = id; }

        virtual public void use(Player player)
        {
            if (used)
            {
                Logger.log("" + player.id + " is trying to use an expired item: "
                              + this.GetType().Name + " " + id
                              + ". Rejected.");
                throw new ArgumentException();
            }
            Logger.log("" + player.id + " uses " + this.GetType().Name + " " + id);
            used = true;
        }
    }

	[Serializable]
	public class HealingPotion : Item
    {
        public uint HPvalue;

        /* Create a healing potion with random HP-value */
        public HealingPotion(String id)
            : base(id)
        {
            HPvalue = (uint)RandomGenerator.rnd.Next(10) + 1;
        }

        override public void use(Player player)
        {
            base.use(player);
            player.HP = (int)Math.Min(player.HPbase, player.HP + HPvalue);
        }
    }

	[Serializable]
	public class Crystal : Item
    {
        public Crystal(String id) : base(id) { }
        override public void use(Player player)
        {
            rejected = false;
            if (player.location.contested(player) || player.location is Bridge)
            {
                base.use(player);
                if (player.location.contested(player)) player.accelerated = true;
                if (player.location is Bridge) player.dungeon.disconnect(player.location as Bridge);
            }
            else
            {
                Logger.log("Player not in combat or on bridge. Rejected.");
                rejected = true;
            }
            
        }
    }
}
