using System;
using System.Collections.Generic;
using System.Text;

namespace GameApplication.Abstracts
{
    public abstract class Skill
    {
        internal object getCondition()
        {
            throw new NotImplementedException();
        }

        internal object getTarget()
        {
            throw new NotImplementedException();
        }

        internal string getSkill()
        {
            throw new NotImplementedException();
        }

        public KeyValuePair<string, string> getCondition(List<KeyValuePair<string, string>> enemyStatus, List<KeyValuePair<string, string>> allyStatus, List<KeyValuePair<string, string>> list)
        {
            return new KeyValuePair<string, string>("false","");
        }

        internal object getName()
        {
            throw new NotImplementedException();
        }

        internal void useSkill(Agent target, Agent agent)
        {
            throw new NotImplementedException();
        }
    }
}
