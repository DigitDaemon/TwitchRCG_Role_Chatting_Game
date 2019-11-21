using GameApplication.Agent_Dependencies;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameApplication.Agents
{
    public class DireWolf : Abstracts.Agent
    {
        const int baseSpeed = 12;
        const int CR = 3;
        
        public DireWolf(int UnId, int speed)
            : base("DireWolf"+UnId,120,30,0,0,1,new List<Abstracts.Skill>(),speed)
        {
        }

        static int getBaseSpeed()
        {
            return baseSpeed;
        }
        static int getCR()
        {
            return CR;
        }

        public override KeyValuePair<string, string> getAction(List<KeyValuePair<string, string>> enemyStatus, List<KeyValuePair<string, string>> allyStatus)
        {
            foreach (KeyValuePair<string, string> status in enemyStatus)
            {
                if (status.Value.Equals("Critical"))
                    return new KeyValuePair<string, string>(status.Key, "PhysAttack");
            }
            foreach (KeyValuePair<string, string> status in enemyStatus)
            {
                if (status.Value.Equals("Hurt"))
                    return new KeyValuePair<string, string>(status.Key, "PhysAttack");
            }
            foreach (KeyValuePair<string, string> status in enemyStatus)
            {
                if (status.Value.Equals("Healthy"))
                    return new KeyValuePair<string, string>(status.Key, "PhysAttack");
            }
            return new KeyValuePair<string, string>(name, "Wait");
        }
    }
}
