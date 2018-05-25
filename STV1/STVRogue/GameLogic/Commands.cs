﻿using System;
using STVRogue.GameLogic;

namespace STVRogue
{
    public class Command
    {
        public Command() { }

		public virtual void ExecuteCommand(Player p) { }
        override public string ToString() { return "no-action"; }
    }

	public class AttackCommand : Command
	{
		private Creature target;
		public AttackCommand(Creature target)
		{
			this.target = target;
		}

		public override void ExecuteCommand(Player p)
		{
			p.Attack(target);
		}
		public override string ToString()
		{
			return "attack " + target.name;
		}
	}
	public class MoveCommand : Command
	{
		private Node node;
		public MoveCommand(Node node)
		{
			this.node = node;
		}

		public override void ExecuteCommand(Player p)
		{
			p.location = node;
		}
		public override string ToString()
		{
			return "move to " + node.id;
		}
	}
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
