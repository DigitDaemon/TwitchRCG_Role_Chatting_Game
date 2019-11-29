using System;
using System.Collections.Generic;
using System.Text;

namespace GameApplication.Abstracts
{
    public abstract class Class
    {
        static protected List<string> ActionPriority;
        static protected Skill baseSkill;
        static protected string name;

        public string getName()
        {
            return name;
        }

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
