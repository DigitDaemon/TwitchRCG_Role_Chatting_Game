using GameApplication.Abstracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameApplication.Skills
{
    class Fireball : Skill
    {
        static string name = "Fireball";

        public KeyValuePair<string, string> getCondition(List<KeyValuePair<string, string>> enemyStatus, List<KeyValuePair<string, string>> allyStatus, List<KeyValuePair<string, string>> selfStatus)
        {
            return GetCondition(enemyStatus, allyStatus, selfStatus);
        }

        static public KeyValuePair<string, string> GetCondition(List<KeyValuePair<string, string>> enemyStatus, List<KeyValuePair<string, string>> allyStatus, List<KeyValuePair<string, string>> selfStatus)
        {
            if (!selfStatus.Exists(x => x.Value.Equals("Tapped")))
            {
                if (enemyStatus.Exists(x => x.Value.Equals("Taunting")))
                    return new KeyValuePair<string, string>(enemyStatus.Find(x => x.Value.Equals("Taunting")).Key + " Monster", GetName() + " Skill");
                foreach (KeyValuePair<string, string> enemyCon in enemyStatus)
                {
                        if (enemyCon.Value.Equals("Critical") || enemyCon.Value.Equals("Vulnerable"))
                            return new KeyValuePair<string, string>(enemyCon.Key + " Monster", GetName() + " Skill");
                }
                foreach (KeyValuePair<string, string> enemyCon in enemyStatus)
                {
                    if (enemyCon.Value.Equals("Hurt"))
                        return new KeyValuePair<string, string>(enemyCon.Key + " Monster", GetName() + " Skill");
                }
                foreach (KeyValuePair<string, string> enemyCon in enemyStatus)
                {
                    if (enemyCon.Value.Equals("Healthy"))
                        return new KeyValuePair<string, string>(enemyCon.Key + " Monster", GetName() + " Skill");
                }
            }

            return new KeyValuePair<string, string>("false", "");
        }

        public string getName()
        {
            return GetName();
        }

        static public string GetName()
        {
            return name;
        }

        public string getSkill()
        {
            throw new NotImplementedException();
        }

        public void useSkill(Agent target, Agent agent)
        {
            UseSkill(target, agent);
        }

        static public void UseSkill(Agent target, Agent agent)
        {
            target.TakeDamage(agent.physAttack(), "Magical", null);
            target.modify(new Agent_Dependencies.Modifier("Dot", "start", "Health", -10, 3));
            agent.modify(new Agent_Dependencies.Modifier("Debuff", "status", 2, "Tapped"));
        }
    }
}
