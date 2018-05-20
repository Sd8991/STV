using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace STVRogue.GameLogic
{

    [TestClass]
    public class MSTest_Pack
    {
        [TestMethod]
        public void MStest_create_pack()
        {
            Pack p = new Pack("pid", 3);
            Assert.IsTrue(p.id == "pid");
            Assert.IsTrue(p.members.Count == 3);
        }

        [TestMethod]
        public void MStest_move_when_able()
        {
            Dungeon d = new Dungeon(5);
            Pack p = new Pack("pid", 3);
            p.dungeon = d;
            p.location = d.zone[1][0];
            d.zone[1][0].packs.Add(p);
            p.move(d.zone[1][1]);
            Assert.IsTrue(p.location == d.zone[1][1]);
        }

        [TestMethod]
        public void MStest_move_with_insufficient_capacity()
        {
            Dungeon d = new Dungeon(5);
            Pack p = new Pack("pid", 3);
            Pack q = new Pack("qid", (d.M * (d.level(d.zone[1][1]) + 1)));
            p.dungeon = d;
            q.dungeon = d;    
            p.location = d.zone[1][0];
            q.location = d.zone[1][1];
            d.zone[1][0].packs.Add(p);
            d.zone[1][1].packs.Add(q);     
            p.move(d.zone[1][1]);
            Assert.IsTrue(p.location == d.zone[1][0]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void MStest_move_to_nonconnected_node()
        {
            Node N0 = new Node("N0");
            Node N1 = new Node("N1");
            Pack p = new Pack("pid", 3);
            p.location = N0;
            N0.packs.Add(p);
            p.move(N1);
        }


        [TestMethod]
        public void MStest_follow_shortest_path()
        {
            Dungeon d = new Dungeon(5);
            
            Pack p = new Pack("pid", 3);
            p.dungeon = d;
            p.location = d.zone[1][0];
            d.zone[1][0].packs.Add(p);
            p.moveTowards(d.zone[1][2]);      
            Assert.IsTrue(p.location == d.zone[1][2]);
        }

        [TestMethod]
        public void MStest_currenthp()
        {
            Pack p = new Pack("pid", 3);
            Assert.IsTrue(p.startingHP == p.CurrentHP());
        }

        [TestMethod]
        public void MStest_attack_player()
        {
            Player pl = new Player();
            Pack pa = new Pack("pid", 3);
            pa.Attack(pl);
            Assert.IsTrue(pl.HP == pl.HPbase - 3);
        }

        [TestMethod]
        public void MStest_kill_player()
        {
            Player pl = new Player();
            Pack pa = new Pack("pid", 200);
            pa.Attack(pl);
            Assert.IsTrue(pl.HP == 0);  //failure would leave pl.HP at -100
        }
    }
}
