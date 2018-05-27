using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace STVRogue.GameLogic
{
    [TestClass]
    public class MSTest_Items
    {
        [TestMethod]
        public void MStest_create_hppotion()
        {
            Item potion = new HealingPotion("potID");
            //Assert.IsInstanceOfType(potion.GetType(), HealingPotion);
            Assert.IsNotNull(potion);
            Assert.IsTrue(potion.id == "potID");
        }

        [TestMethod]
        public void MStest_healing()
        {
            Player P = new Player();
            HealingPotion x = new HealingPotion("pot1");
            P.HP = 50;
            P.bag.Add(x);
            P.use(x);
            Assert.IsTrue(P.HP == 50 + x.HPvalue);      //Can't call x.HPvalue
        }

        [TestMethod]
        public void MStest_excessive_healing()
        {
            Player P = new Player();
            Item x = new HealingPotion("pot1");
            P.HP = P.HPbase - 1;
            P.bag.Add(x);
            P.use(x);
            Assert.IsTrue(P.HP == P.HPbase);
        }

        [TestMethod]
        public void MStest_create_crystal()
        {
            Item crystal = new Crystal("crystID");
            //Assert.IsInstanceOfType(crystal.GetType(), typeof(Crystal));
            Assert.IsNotNull(crystal);
            Assert.IsTrue(crystal.id == "crystID");
        }

        [TestMethod]
        public void MStest_invalid_crystal_use()
        {
            Dungeon d = new Dungeon(5);
            Player p = new Player();
            p.location = d.zone[1][0];
            Crystal crystal = new Crystal("crystID");
            p.bag.Add(crystal);
            p.use(crystal);
            Assert.IsTrue(p.bag.Count == 1);
        }
    }
}
