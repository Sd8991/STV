using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using STVRogue.Utils;

namespace STVRogue.GameLogic
{
    public class Creature
    {
        public String id;
        public String name;
        public int HP;
        public uint AttackRating = 1;
        public Node location;
        public Creature() { }
        virtual public void Attack(Creature foe)
        {
            foe.HP = (int)Math.Max(0, foe.HP - AttackRating);
            String killMsg = foe.HP == 0 ? ", KILLING it" : "";
            Logger.log("Creature " + id + " attacks " + foe.id + killMsg + ".");
        }
    }

    public class Monster : Creature
    {
        public Pack pack;

        /* Create a monster with a random HP */
        public Monster(String id, Pack pack)
        {
            this.pack = pack;
            this.id = id; name = "Orc";
            HP = 1 + RandomGenerator.rnd.Next(6);
        }
    }

    public class Player : Creature
    {
        public Dungeon dungeon;
        public int zone = 1;
        public int HPbase = 100;
        public Boolean accelerated = false;
        public bool inCombat = false;
        public uint KillPoint = 0;
        public List<Item> bag = new List<Item>();
        public Player()
        {
            id = "player";
            AttackRating = 5;
            HP = HPbase;
        }

        public virtual void GetNextCommand()
        {
            Pack targetPack = location.packs[RandomGenerator.rnd.Next(location.packs.Count)];
            Monster targetMon = (targetPack.members.Count > 1) ? targetPack.members[RandomGenerator.rnd.Next(1, targetPack.members.Count) - 1] : targetPack.members[0];
            Attack(targetMon);
            if (targetPack.members.Count == 0) location.packs.Remove(targetPack);

        }

        public void processZone(Node n) 
        {
            if (n is Bridge && (n as Bridge).GetToNodes.Contains(location))
            {
                n.toggleAlert(dungeon.zone[zone]);
                zone--;
            }
            if (location is Bridge && (location as Bridge).GetToNodes.Contains(n))
            {
                n.toggleAlert(dungeon.zone[zone]);
                zone++;
                lastZoneCheck();
            }
        }

        public void lastZoneCheck()
        {
            if (zone == dungeon.zone.Keys.Max())
                foreach (Node n in dungeon.zone[zone])
                    foreach (Pack p in n.packs)
                        p.rLastZone = true;
        }

        public void use(Item item)
        {
            if (!bag.Contains(item)) throw new ArgumentException();
            item.use(this);
            if(!item.rejected)bag.Remove(item);
        }

		public void PickUpItems()
		{
			foreach (Item item in location.items)
			{
				bag.Add(item);
			}
			location.items.RemoveAll((x)=>bag.Contains(x));
		}

        override public void Attack(Creature foe)
        {
            if (!(foe is Monster)) throw new ArgumentException();
            Monster foe_ = foe as Monster;
            if (!accelerated)
            {
                base.Attack(foe);
                if (foe_.HP == 0)
                {
                    foe_.pack.members.Remove(foe_);
                    KillPoint++;
                }
            }
            else
            {
                int packCount = foe_.pack.members.Count;
                List<Creature> deadFoes = new List<Creature>();
                foe_.pack.members.RemoveAll(target => target.HP <= 0);
                KillPoint += (uint) (packCount - foe_.pack.members.Count) ;
                foreach (Monster target in foe_.pack.members)
                {
                    base.Attack(target);
                    if (target.HP <= 0)
                        deadFoes.Add(target);
                }
                foreach (Monster deadFoe in deadFoes)
                {
                    foe_.pack.members.Remove(deadFoe);
                    KillPoint++;
                }
                /* Wrong implementation; can't remove while iterating:
                foreach (Monster target in foe_.pack.members)
                {
                    base.Attack(target);
                    if (target.HP == 0)
                    {
                        foe_.pack.members.Remove(foe_);
                        KillPoint++;
                    }
                }
                */
                accelerated = false;
            }
        }
    }

    public class TestPlayer : Player
    {
        List<Command> CommandQueue;
        //Test-version of player to run tests with that the normal player doesn't allow for
        public TestPlayer(List<Command> CommandQueue)
        {
            this.CommandQueue = CommandQueue;
        }

        public override void GetNextCommand()
        {
            CommandQueue[0].ExecuteCommand(this);
            CommandQueue.RemoveAt(0);
        }
    }
}
