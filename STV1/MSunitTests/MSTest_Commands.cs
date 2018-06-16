using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using STVRogue.Utils;

namespace STVRogue.GameLogic
{
	[TestClass]
	public class MSTest_Commands
	{
		[TestMethod]
		public void MSTest_AttackCommand()
		{
			Pack pack = new Pack("test", 1);
			Creature victim = pack.members[0];
			victim.HP = 8;
			int innitalHP = victim.HP;
			Command attCom = new AttackCommand(victim.id);
			Assert.IsTrue(attCom.ToString() == "attack " + victim.name);
			Player player = new Player();
			attCom.ExecuteCommand(player);
			Assert.IsTrue(victim.HP == Math.Max(0, innitalHP - 5));

			while(victim.HP > 0)
			{
				attCom.ExecuteCommand(player);
			}
			Assert.IsFalse(pack.members.Contains(victim));
		}
		[TestMethod]
		public void MSTest_MoveCommand()
		{
			Node n1 = new Node("n1");
			Node n2 = new Node("n2");
			Item item = new Item("test");
			n2.items.Add(item);
			n1.connect(n2);
			Command moveCom = new MoveCommand(0);
			//Assert.IsTrue(moveCom.ToString() == "move to " + n2.id);
			Player player = new Player();
			player.location = n1;
			moveCom.ExecuteCommand(player);
			Assert.IsTrue(player.location == n2);
			Assert.IsTrue(player.bag.Contains(item));
			Assert.IsFalse(n2.items.Contains(item));
		}
		[TestMethod]
		public void MSTest_ItemCommand()
		{
			Item item = new HealingPotion("hpPotion");
			Command itemCom = new ItemCommand(item);
			uint healingpower = (item as HealingPotion).HPvalue;
			Assert.IsTrue(itemCom.ToString() == "use " + item.GetType().Name + " " + item.id);
			Player player = new Player();
			player.bag.Add(item);
			player.HP -= 20;
			int pHP = player.HP;
			itemCom.ExecuteCommand(player);
			Assert.IsTrue(player.HP == pHP + healingpower);
		}
	}
}
