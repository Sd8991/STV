using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace STVRogue.GameLogic
{
    [TestClass]
    class MSTest_Pack
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
            Node N0 = new Node("N0");
            Node N1 = new Node("N1");
            N0.connect(N1);
            Pack p = new Pack("pid", 3);
            p.location = N0;
            N0.packs.Add(p);
            //p.move(N1);          //not sure on how to create a set instance of Dungeon to test on, as Pack.move() requires one to exist
            Assert.IsTrue(p.location == N1);
        }

        [TestMethod]
        public void MStest_move_with_insufficient_capacity()
        {
            Node N0 = new Node("N0");
            Node N1 = new Node("N1");
            N0.connect(N1);
            Pack p = new Pack("pid", 3);
            //Pack q = new Pack("qid", (int) (dungeon.M * (dungeon.level(N1) + 1));     //Set Dungeon Issue
            p.location = N0;
            //q.location = N1;      //Set Dungeon Issue
            N0.packs.Add(p);
            //N1.packs.Add(q);      //Set Dungeon Issue
            //p.move(N1);           //Set Dungeon Issue
            Assert.IsTrue(p.location == N1);
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
            //p.move(N1);           //Set Dungeon Issue
        }


        [TestMethod]
        public void MStest_follow_shortest_path()
        {
            Node N0 = new Node("N0");
            Node N1 = new Node("N1");
            Node N2 = new Node("N2");
            Node N3 = new Node("N3");
            Node N4 = new Node("N4");
            N0.connect(N1);
            N0.connect(N3);
            N1.connect(N2);
            N2.connect(N4);
            N3.connect(N4);

            Pack p = new Pack("pid", 3);
            p.location = N0;
            N0.packs.Add(p);
            //p.moveTowards(N4);      //Set Dungeon Issue
            Assert.IsTrue(p.location == N3);
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
