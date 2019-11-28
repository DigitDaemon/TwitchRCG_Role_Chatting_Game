using System;
using System.Collections.Generic;
using System.Text;
using GameApplication.Abstracts;

namespace GameApplication.Skills
{
    class AttackRecklessly : Skill
    {
        static string name = "AttackRecklessly";

        public KeyValuePair<string, string> getCondition(List<KeyValuePair<string, string>> enemyStatus, List<KeyValuePair<string, string>> allyStatus, List<KeyValuePair<string, string>> selfStatus)
        {
            return GetCondition( enemyStatus, allyStatus, selfStatus);
        }

        static public KeyValuePair<string, string> GetCondition(List<KeyValuePair<string, string>> enemyStatus, List<KeyValuePair<string, string>> allyStatus, List<KeyValuePair<string, string>> selfStatus)
        {
            if(selfStatus.Exists(x => x.Value.Equals("Hurt")))
            {
                if(enemyStatus.Exists(x => x.Value.Equals("Taunting") && (x.Value.Equals("Critical") || x.Value.Equals("Hurt"))))
                    return new KeyValuePair<string, string>(enemyStatus.Find(x => x.Value.Equals("Taunting") && (x.Value.Equals("Critical") || x.Value.Equals("Hurt"))).Key + " Monster", GetName() + " Skill");
                else if (enemyStatus.Exists(x => x.Value.Equals("Taunting")))
                    return new KeyValuePair<string, string>("false", "");
                else
                    foreach (KeyValuePair<string,string> enemyCon in enemyStatus)
                {
                    if (enemyCon.Value.Equals("Critical") || enemyCon.Value.Equals("Hurt"))
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
            target.TakeDamage(agent.physAttack(), "Physical", null);
            target.TakeDamage(agent.physAttack(), "Physical", null);

            agent.modify(new Agent_Dependencies.Modifier("Debuff", "status", 2, "Vulnerable"));
            agent.modify(new Agent_Dependencies.Modifier("Debuff", "defend", "Armour", -2, 2));
            agent.modify(new Agent_Dependencies.Modifier("Debuff", "defend", "Warding", -2, 2));
        }
    }
}
