using System;
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

	[Serializable]
	public class MoveCommand : Command
	{
		private Node node;
		public MoveCommand(Node node)
		{
			this.node = node;
		}

		public override void ExecuteCommand(Player p)
		{
            p.processZone(node);
			p.location = node;
			p.PickUpItems();
		}
		public override string ToString()
		{
			return "move to " + node.id;
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
