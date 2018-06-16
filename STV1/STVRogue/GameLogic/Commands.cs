﻿using System;
using STVRogue.GameLogic;

namespace STVRogue
{
	[Serializable]
    public class Command
    {
        public Command() { }

		public virtual void ExecuteCommand(Player p) { }
        override public string ToString() { return "no-action"; }
    }

	[Serializable]
	public class AttackCommand : Command
	{
		private string targetid;
		public AttackCommand(string targetid)
		{
			this.targetid = targetid;
		}

		public override void ExecuteCommand(Player p)
		{
			Creature target = null;
			foreach (Pack pack in p.location.packs)
			{
				Monster found = pack.members.Find(M => M.id == targetid);
				if (found != null) 
					target = found;
			}
			if (target == null)
				throw new Exception("target not found");
			p.Attack(target);
            if (!p.location.contested(p)) p.inCombat = false; //end combat by winning
		}
		public override string ToString()
		{
			return "attack ";// + target.name;
		}
	}

	[Serializable]
	public class MoveCommand : Command
	{
		private int neighborIndex;
		public MoveCommand(int neighborIndex)
		{
			this.neighborIndex = neighborIndex;
		}

        public override void ExecuteCommand(Player p)
        {
			Node node = p.location.neighbors[neighborIndex];
            p.processZone(node);
            p.location = node;
            p.PickUpItems();
            if (!p.location.contested(p)) p.inCombat = false; //end combat by fleeing
            if (p.location.contested(p))    //start combat
            {
                p.inCombat = true;
                if (!p.location.packs[0].rAlert) p.location.toggleAlert(p.dungeon.zone[p.zone]);
            }
        }
		public override string ToString()
		{
			return "move to ";// + node.id;
		}
	}

	[Serializable]
	public class ItemCommand : Command
	{
		private Item item;
		public ItemCommand(Item item)
		{
			this.item = item;
		}
		public override void ExecuteCommand(Player p)
		{
			p.use(item);
		}
		public override string ToString()
		{
			return "use " + item.GetType().Name + " " + item.id;
		}
	}
}
