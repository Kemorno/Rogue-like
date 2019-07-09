using System;
using System.Collections.Generic;
using UnityEngine;

namespace AutoChess
{
    namespace Resources
    {
        public class Player
        {
            public Hero[] Inventory = new Hero[9];
            public List<Hero> onField = new List<Hero>();

            public void CheckForUpgrades()
            {
                List<Hero> FullList = new List<Hero>(onField);
                Dictionary<int, List<Hero>> Grouped = new Dictionary<int, List<Hero>>();
                FullList.AddRange(Inventory);

                foreach(Hero h in FullList)
                {
                    if (!Grouped.ContainsKey(h.ID))
                        Grouped.Add(h.ID, new List<Hero>(h));
                    else
                        Grouped[h.ID].Add(h);
                }
                foreach(List<Hero> h in Grouped.Values)
                {
                    
                }
            }
        }

        public class Unit
        {
            public HexaCoord Coord;
            public int Health
            {
                get
                {
                    return Health;
                }
                set
                {
                    if (value < 0)
                        Health = 0;
                    else if (value > MaxHealth)
                        Health = MaxHealth;
                    else
                        Health = value;
                }
            }
            public int MaxHealth = 100;
            public int Mana
            {
                get
                {
                    return Mana;
                }
                set
                {
                    if (value < 0)
                        Health = 0;
                    else if (value > MaxMana)
                        Health = MaxMana;
                    else
                        Health = value;
                }
            }
            public int MaxMana = 100;
            public int AttackSpeed = 1;
            public bool canAttack = true;
            public Item[] items = new Item[3];
        }

        public class Hero : Unit
        {
            public int ID;
            public int Level;
        }

        public class Monster : Unit
        {
            float DropChance
            {
                get
                {
                    return DropChance;
                }
                set
                {
                    DropChance = Mathf.Clamp(value, 0, 100);
                }
            }
            
            public IEnumerator AttackSpeedDelay()
            {
                if (AttackSpeed == 0)
                    yield return null;

                canAttack = false;
                yield return new WaitForSeconds(1f / AttackSpeed);
                canAttack = true;
            }

            public object Kill()
            {
                System.Random rng = new System.Random(DateTime.Now.GetHashCode());

                if (rng.Next(0, 100) <= DropChance)
                    return new Item();
                else
                    return null;
            }
        }

        public class HexaCoord
        {
            public int x = 0;
            public int y = 0;

            public HexaCoord()
            {
            }
            public HexaCoord(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }
        public class Item
        {
            string Name;
            int AttackSpeed
            {
                get
                {
                    return AttackSpeed;
                }
                set
                {
                    AttackSpeed = Mathf.Clamp(value, 0, 1000);
                }
            }
            int AttackDamage
            {
                get
                {
                    return AttackDamage;
                }
                set
                {
                    AttackDamage = Mathf.Clamp(value, 0, 1000);
                }
            }
            int SpellDamage
            {
                get
                {
                    return SpellDamage;
                }
                set
                {
                    SpellDamage = Mathf.Clamp(value, 0, 1000);
                }
            }
            int Defense
            {
                get
                {
                    return Defense;
                }
                set
                {
                    Defense = Mathf.Clamp(value, 0, 1000);
                }
            }
            int MagicDefense
            {
                get
                {
                    return MagicDefense;
                }
                set
                {
                    MagicDefense = Mathf.Clamp(value, 0, 1000);
                }
            }
            int Health
            {
                get
                {
                    return Health;
                }
                set
                {
                    Health = Mathf.Clamp(value, 0, 1000);
                }
            }
        }
    }
}
