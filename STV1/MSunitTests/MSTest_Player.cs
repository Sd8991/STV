﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace STVRogue.GameLogic
{
    /* An example of a test class written using VisualStudio's own testing
     * framework. 
     * This one is to unit-test the class Player. The test is incomplete though, 
     * as it only contains two test cases. 
     */
    [TestClass]
    public class MSTest_Player
    {
        [TestMethod]
        public void MSTest_create_player()
        {
            Player P = new Player();
            Assert.IsTrue(P.id == "player");
            Assert.IsTrue(P.AttackRating == 5);
            //Maybe other values too?
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void MSTest_use_onEmptyBag()
        {
            Player P = new Player();
            P.use(new Item());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void MSTest_using_used_item()
        {
            Player P = new Player();
            P.HP = 50;
            Item x = new HealingPotion("pot1");
            P.bag.Add(x);
            x.used = true;
            P.use(x);
        }

        [TestMethod]
        public void MSTest_use_item_in_bag()
        {
            Player P = new Player();
            Item x = new HealingPotion("pot1");
            P.bag.Add(x);
            P.use(x);
            Assert.IsFalse(P.bag.Contains(x));
        }

        [TestMethod]
        public void MSTest_become_accelerated()
        {
            Player P = new Player();
            Pack pa = new Pack("pid", 1);
            Dungeon d = new Dungeon(5);
            P.dungeon = d;
            P.location = d.zone[1][0];
            pa.dungeon = d;
            pa.location = d.zone[1][0];
            d.zone[1][0].packs.Add(pa);
            Item x = new Crystal("cry1");
            P.bag.Add(x);
            P.use(x);
            Assert.IsFalse(P.bag.Contains(x));
            Assert.IsTrue(P.accelerated);
        }

        [TestMethod]
        public void MSTest_test_base_creature_attack_surviving_enemy()
        {
            Player P = new Player();
            Pack pack = new Pack("pack", 1);
            pack.members[0].HP = 7;
            int preHP = pack.members[0].HP;
            P.Attack(pack.members[0]);
            Assert.IsTrue(pack.members[0].HP < preHP);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void MSTest_attack_non_foe()
        {
            Player P = new Player();
            Creature M = new Creature();
            P.Attack(M);
        }

        [TestMethod]
        public void MSTest_remove_monster_from_pack_non_acc()
        {
            Player P = new Player();
            Pack pack = new Pack("pack", 2);
            pack.members[0].HP = 1;
            Monster M = pack.members[0];
            pack.startingHP = 1 + pack.members[1].HP;
            P.Attack(pack.members[0]);
            Assert.IsTrue(!pack.members.Contains(M));
        }

        [TestMethod]
        public void MSTest_check_dead_monsters()
        {
            Player P = new Player();
            Pack pack = new Pack("pack", 2);
            pack.members[0].HP = 1;
            pack.members[1].HP = 7;
            Monster M1 = pack.members[0];
            Monster M2 = pack.members[1];
            P.accelerated = true;
            P.Attack(pack.members[0]);
            Assert.IsTrue(pack.members.Contains(M2));
            Assert.IsTrue(M2.HP > 0);
            Assert.IsFalse(pack.members.Contains(M1));
            Assert.IsTrue(M1.HP == 0);
        }

        [TestMethod]
        public void MSTest_check_dead_monsters_no_deaths()
        {
            Player P = new Player();
            Pack pack = new Pack("pack", 2);
            pack.members[0].HP = 7;
            pack.members[1].HP = 7;
            Monster M1 = pack.members[0];
            Monster M2 = pack.members[1];
            P.accelerated = true;
            P.Attack(pack.members[0]);
            Assert.IsTrue(pack.members.Contains(M1));
            Assert.IsTrue(pack.members.Contains(M2));
        }

        [TestMethod]
        public void MSTest_check_dead_monsters_all_dead()
        {
            Player P = new Player();
			Node n = new Node();
            Pack pack = new Pack("pack", 2);
			P.location = n;
			pack.location = n;
            pack.members[0].HP = 1;
            pack.members[1].HP = 1;
            Monster M1 = pack.members[0];
            Monster M2 = pack.members[1];
            P.accelerated = true;
            P.Attack(pack.members[0]);
            Assert.IsTrue(pack.CurrentHP() == 0 && !n.packs.Contains(pack));
        }

		[TestMethod]
		public void MStest_Pickup_items()
		{
			Item item1 = new Item("item1");
			Item item2 = new Item("item2");
			Item item3 = new Item("item3");
			Node node = new Node();
			node.items.Add(item1);
			node.items.Add(item2);
			node.items.Add(item3);
			Player player = new Player();
			player.location = node;
			player.PickUpItems();

			Assert.IsTrue(player.bag.Contains(item1));
			Assert.IsFalse(node.items.Contains(item1));
			Assert.IsTrue(player.bag.Contains(item2));
			Assert.IsFalse(node.items.Contains(item2));
			Assert.IsTrue(player.bag.Contains(item3));
			Assert.IsFalse(node.items.Contains(item3));
		}
	}
}
