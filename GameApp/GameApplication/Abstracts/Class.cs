using System;
using System.Collections.Generic;
using System.Text;

namespace GameApplication.Abstracts
{
    public abstract class Class
    {
        static protected List<string> ActionPriority;
        static protected Skill baseSkill;

        public List<string> getActionPriority()
        {
            return ActionPriority;
        }

        public Skill getBaseSkill()
        {
            return baseSkill;
        }
    }
}
