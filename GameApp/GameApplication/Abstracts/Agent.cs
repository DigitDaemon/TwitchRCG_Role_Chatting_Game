using GameApplication.Agent_Dependencies;
using GameApplication.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameApplication.Abstracts
{
    public abstract class Agent
    {
        static protected Random rng = new Random();
        protected string name { get; }
        protected int baseHealth { get; }
        protected int currentHealth { get; set; }
        protected int baseStrength { get; }
        protected int baseMind { get; }
        protected int baseConcentration { get; }
        protected int baseMastery { get; }
        protected int baseArmorValue { get; }
        protected int baseWarding { get; }
        protected int speed { get; }
        private List<Modifier> modsList { get; set; }
        protected List<Skill> skills { get; }
        protected Item item;


        public Agent(string name, int baseHealth, int strength, int mind, int concentration, int mastery, List<Skill> skills, int speed)
        {
            this.name = name;
            this.baseHealth = baseHealth;
            currentHealth = baseHealth;
            this.baseStrength = strength;
            this.baseMastery = mastery;
            this.baseMind = mind;
            this.baseConcentration = concentration;
            this.skills = skills;
            this.speed = speed;
            modsList = new List<Modifier>();
        }

        public Agent(string name, int baseHealth, int strength, int mind, int concentration, int mastery, List<Skill> skills, int speed, Item item)
        {
            this.name = name;
            this.baseHealth = baseHealth;
            currentHealth = baseHealth;
            this.baseStrength = strength;
            this.baseMastery = mastery;
            this.baseMind = mind;
            this.baseConcentration = concentration;
            this.skills = skills;
            this.speed = speed;
            this.item = item;
            modsList = new List<Modifier>();
        }

        public string getName()
        {
            return name;
        }

        public void TakeDamage(int amount, string type, List<Modifier> effects)
        {
            var currentArmour = baseArmorValue;
            var currentWarding = baseWarding;
            foreach (Modifier mod in modsList)
            {
                if (mod.defend())
                {
                    if (mod.target.Equals("Armour"))
                        currentArmour += mod.value;
                    else if (mod.target.Equals("Warding"))
                        currentWarding += mod.value;
                    else if (mod.target.Equals("Health"))
                        currentHealth += mod.value;
                }
            }
            if (type.Equals("Physical"))
            {
                currentHealth -= amount * (int)((100 - currentArmour) / 100f);
            }
            else if (type.Equals("Magical"))
            {
                currentHealth -= amount * (int)((100 - currentWarding) / 100f);
            }

            if (effects != null)
            {
                foreach (Modifier mod in effects)
                {
                    modsList.Add(mod);
                }
            }

            checkDeath();
        }

        public void checkDeath()
        {
            if (currentHealth <= 0)
            {
                foreach(Modifier mod in modsList)
                {
                    if (mod.onDeath())
                    {
                        if (mod.target.Equals("Health"))
                            currentHealth += mod.value;
                    }
                    
                }
                if (currentHealth <= 0)
                    throw new DeathException(name + " has died.", this);
            }
        }

        public void upkeep()
        {
            foreach (Modifier mod in modsList)
            {
                if (mod.turnStart())
                {
                    if (mod.target.Equals("Health"))
                    {
                        currentHealth += mod.value;
                    }
                }

            }

            checkDeath();
        }

        public void endstep()
        {
            foreach (Modifier mod in modsList)
            {
                if (mod.turnEnd())
                {
                    if (mod.target.Equals("Health"))
                    {
                        currentHealth += mod.value;
                    }
                }
                mod.duration--;
                if (mod.duration >= 0)
                {
                    modsList.RemoveAt(modsList.IndexOf(mod));
                }
            }

            checkDeath();
        }

        public int physAttack()
        {
            var currentStrength = baseStrength;
            var currentMastery = baseMastery;
            foreach(Modifier mod in modsList)
            {
                if (mod.attack())
                {
                    if (mod.target.Equals("Strength"))
                    {
                        currentStrength += mod.value;
                    }
                    else if (mod.target.Equals("Mastery"))
                    {
                        currentMastery += mod.value;
                    }
                }
            }
            var attackBase = currentStrength - (int)((float)currentStrength * (1f / (2f + (float)currentMastery)));
            Console.WriteLine("attackBase" + attackBase);
            var attackRange = 2 * (int)((float)currentStrength * (1f / (2f + (float)currentMastery)));
            Console.WriteLine("attackRange" + attackRange);
            var damage = attackBase + rng.Next(attackRange);
            Console.WriteLine("damage" + damage);
            return damage;
        }

        public int magicAttack()
        {
            var currentMind = baseMind;
            var currentConcentration = baseConcentration;
            foreach (Modifier mod in modsList)
            {
                if (mod.attack())
                {
                    if (mod.target.Equals("Strength"))
                    {
                        currentMind += mod.value;
                    }
                    else if (mod.target.Equals("Mastery"))
                    {
                        currentConcentration += mod.value;
                    }
                }
            }
            var attackBase = currentMind - (int)((float)currentMind * (1f / (2f + (float)currentConcentration)));
            var attackRange = 2 * (int)((float)currentMind * (1f / (2f + (float)currentConcentration)));
            var damage = attackBase + rng.Next(attackRange);
            return damage;
        }

        public int getCurrentHealth()
        {
            return currentHealth;
        }

        public List<KeyValuePair<string, string>> getStatus()
        {
            var statusList = new List<KeyValuePair<string,string>>();
            if (currentHealth > (float)(baseHealth * 66))
            {
                statusList.Add(new KeyValuePair<string,string>(name,"Healthy"));
            }
            else if (currentHealth < (float)(baseHealth * 33))
            {
                statusList.Add(new KeyValuePair<string, string>(name, "Critical"));
            }
            else
            {
                statusList.Add(new KeyValuePair<string, string>(name, "Hurt"));
            }

            var debuffs = 0;
            var dots = 0;
            if (modsList.Count > 0)
            {
                foreach (Modifier mod in modsList)
                {
                    if (mod.type.Equals("debuff"))
                        debuffs++;
                    if (mod.type.Equals("Dot"))
                        dots++;
                }
            }

            if(debuffs > 2)
            {
                statusList.Add(new KeyValuePair<string, string>(name, "Heavily Debuffed"));
            }else if (debuffs > 0)
            {
                statusList.Add(new KeyValuePair<string, string>(name, "Debuffed"));
            }

            if (dots > 2)
            {
                statusList.Add(new KeyValuePair<string, string>(name, "Heavily Dotted"));
            }
            else if (dots > 0)
            {
                statusList.Add(new KeyValuePair<string, string>(name, "Dotted"));
            }


            return statusList;

        }

        public KeyValuePair<string, string> getAction(List<KeyValuePair<string, string>> enemyStatus, List<KeyValuePair<string, string>> allyStatus, List<string> actionPriority)
        {
            foreach (string action in actionPriority)
            {
                if (action.Equals("Item"))
                {
                    if (item != null)
                    {
                        foreach (KeyValuePair<string, string> status in this.getStatus())
                        {
                            if (item.getCondition().Equals(status.Value))
                            {
                                return new KeyValuePair<string, string>(name, "UsedItem");
                            }

                        }
                    }
                }
                else if (action.Equals("Skill"))
                {
                    if (skills.Count != 0)
                    {
                        foreach (Skill skill in skills)
                        {
                            if (skill.getTarget().Equals("Enemy"))
                            {
                                foreach (KeyValuePair<string, string> status in enemyStatus)
                                {
                                    if (status.Value.Equals(skill.getCondition()))
                                        return new KeyValuePair<string, string>(status.Key, skill.getSkill());
                                }
                            }
                            if (skill.getTarget().Equals("Ally"))
                            {
                                foreach (KeyValuePair<string, string> status in allyStatus)
                                {
                                    if (status.Value.Equals(skill.getCondition()))
                                        return new KeyValuePair<string, string>(status.Key, skill.getSkill());
                                }
                            }
                            if (skill.getTarget().Equals("Self"))
                            {
                                foreach (KeyValuePair<string, string> status in getStatus())
                                {
                                    if (status.Value.Equals(skill.getCondition()))
                                        return new KeyValuePair<string, string>(status.Key, skill.getSkill());
                                }
                            }
                        }
                    }
                }
                else if (action.Equals("Heal"))
                {
                    foreach (KeyValuePair<string, string> status in allyStatus)
                    {
                        if (status.Value.Equals("Critical"))
                            return new KeyValuePair<string, string>(status.Key, "Heal");
                    }
                    foreach (KeyValuePair<string, string> status in getStatus())
                    {
                        if (status.Value.Equals("Critical"))
                            return new KeyValuePair<string, string>(status.Key, "Heal");
                    }
                    foreach (KeyValuePair<string, string> status in allyStatus)
                    {
                        if (status.Value.Equals("Hurt"))
                            return new KeyValuePair<string, string>(status.Key, "Heal");
                    }
                    foreach (KeyValuePair<string, string> status in getStatus())
                    {
                        if (status.Value.Equals("Hurt"))
                            return new KeyValuePair<string, string>(status.Key, "Heal");
                    }
                }
                else if (action.Equals("PhysAttack"))
                {
                    foreach (KeyValuePair<string, string> status in enemyStatus)
                    {
                        if (status.Value.Equals("Hurt") || status.Value.Equals("Critical"))
                            return new KeyValuePair<string, string>(status.Key, "PhysAttack");
                    }
                    foreach (KeyValuePair<string, string> status in enemyStatus)
                    {
                        if (status.Value.Equals("Healthy"))
                            return new KeyValuePair<string, string>(status.Key, "PhysAttack");
                    }
                }
                else if (action.Equals("MagAttack"))
                {
                    foreach (KeyValuePair<string, string> status in enemyStatus)
                    {
                        if (status.Value.Equals("Hurt") || status.Value.Equals("Critical"))
                            return new KeyValuePair<string, string>(status.Key, "MagAttack");
                    }
                    foreach (KeyValuePair<string, string> status in enemyStatus)
                    {
                        if (status.Value.Equals("Healthy"))
                            return new KeyValuePair<string, string>(status.Key, "MagAttack");
                    }
                }
                else if (action.Equals("Guard"))
                {
                    return new KeyValuePair<string, string>(name, "Guard");
                }
            }
            return new KeyValuePair<string, string>(name, getDefaultAction());
        }

        public void modify(Modifier mod)
        {
            modsList.Add(mod);
        }

        public abstract KeyValuePair<string, string> getAction(List<KeyValuePair<string, string>> enemyStatus, List<KeyValuePair<string, string>> allyStatus);

        public abstract void defaultAction();

        public abstract string getDefaultAction();

        public int getSpeed()
        {
            return speed;
        }

        public abstract string getType();
       
    }
}
