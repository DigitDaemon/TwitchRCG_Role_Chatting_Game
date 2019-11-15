using GameApplication.Agent_Dependencies;
using GameApplication.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameApplication
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
        private List<Modifier> modsList { get; set; }
        private List<Skill> skills { get; }

        public Agent(int baseHealth, int strength, int mind, int concentration, int mastery, List<Skill> skills)
        {
            this.baseHealth = baseHealth;
            currentHealth = baseHealth;
            this.baseStrength = strength;
            this.baseMastery = mastery;
            this.baseMind = mind;
            this.baseConcentration = concentration;
            this.skills = skills;
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

            foreach(Modifier mod in effects)
            {
                modsList.Add(mod);
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
                    throw new DeathException(name + " has died.");
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
            var attackBase = currentStrength - (int)((float)currentStrength * (1f / (2f - (float)currentMastery)));
            var attackRange = 2 * (int)((float)currentStrength * (1f / (2f - (float)currentMastery)));
            var damage = attackBase + rng.Next(attackRange);
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
            var attackBase = currentMind - (int)((float)currentMind * (1f / (2f - (float)currentConcentration)));
            var attackRange = 2 * (int)((float)currentMind * (1f / (2f - (float)currentConcentration)));
            var damage = attackBase + rng.Next(attackRange);
            return damage;
        }

        public int getCurrentHealth()
        {
            return currentHealth;
        }

        public List<string> getStatus()
        {
            var statusList = new List<string>();
            if (currentHealth > (float)(baseHealth * 66))
            {
                statusList.Add("Healthy");
            }
            else if (currentHealth < (float)(baseHealth * 33))
            {
                statusList.Add("Critical");
            }
            else
            {
                statusList.Add("Hurt");
            }

            var debuffs = 0;
            var dots = 0;
            foreach(Modifier mod in modsList)
            {
                if (mod.type.Equals("debuff"))
                    debuffs++;
                if (mod.type.Equals("Dot"))
                    dots++;
            }

            if(debuffs > 2)
            {
                statusList.Add("Heavily debuffed");
            }else if (debuffs > 0)
            {
                statusList.Add("Debuffed");
            }

            if (dots > 2)
            {
                statusList.Add("Heavily Dotted");
            }
            else if (dots > 0)
            {
                statusList.Add("Dotted");
            }


            return statusList;

        }
    }
}
