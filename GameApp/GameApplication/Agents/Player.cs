using GameApplication.Abstracts;
using GameApplication.Agent_Dependencies;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameApplication.Agents
{
    public class Player : Agent
    {
        protected Class spec { get; }
        protected Race race { get; } 
        protected Item item { get; }

        public Player(int baseHealth, int strength, int mind, int concentration, int mastery, List<Skill> skills, int speed, Class spec, Race race, string playerName)
           : base(playerName, baseHealth, strength, mind, concentration, mastery, skills, speed)
        {
            this.spec = spec;
            this.race = race;
        }

        public override KeyValuePair<string,string> getAction(List<KeyValuePair<string, string>> enemyStatus, List<KeyValuePair<string, string>> allyStatus)
        {
            foreach (string action in spec.getActionPriority())
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
                                        return new KeyValuePair<string, string>(status.Key, "UsedItem");
                                }
                            }
                            if (skill.getTarget().Equals("Ally"))
                            {
                                foreach (KeyValuePair<string, string> status in allyStatus)
                                {
                                    if (status.Value.Equals(skill.getCondition()))
                                        return new KeyValuePair<string, string>(status.Key, "UsedItem");
                                }
                            }
                            if (skill.getTarget().Equals("Self"))
                            {
                                foreach (KeyValuePair<string, string> status in getStatus())
                                {
                                    if (status.Value.Equals(skill.getCondition()))
                                        return new KeyValuePair<string, string>(status.Key, "UsedItem");
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
                        if (status.Value.Equals("Hurt"))
                            return new KeyValuePair<string, string>(status.Key, "PhysAttack");
                    }
                    foreach (KeyValuePair<string, string> status in enemyStatus)
                    {
                        if (status.Value.Equals("Critical"))
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
                        if (status.Value.Equals("Hurt"))
                            return new KeyValuePair<string, string>(status.Key, "PhysAttack");
                    }
                    foreach (KeyValuePair<string, string> status in enemyStatus)
                    {
                        if (status.Value.Equals("Critical"))
                            return new KeyValuePair<string, string>(status.Key, "PhysAttack");
                    }
                    foreach (KeyValuePair<string, string> status in enemyStatus)
                    {
                        if (status.Value.Equals("Healthy"))
                            return new KeyValuePair<string, string>(status.Key, "PhysAttack");
                    }
                }
                else if (action.Equals("Guard"))
                {
                    return new KeyValuePair<string, string>(name, "Guard");
                }  
            }
            return new KeyValuePair<string, string>(name, "Guard");
        }

        public void guard()
        {
                modify(new Modifier("Buff", "Armour", 2, 2));
                modify(new Modifier("Buff", "Warding", 2, 2));
            
        }
    }
}
