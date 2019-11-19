using GameApplication.Abstracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameApplication.Classes
{
    class Fighter : Class
    {
        static List<string> ActionPriority = new List<string>() { "Item","Skill","PhysAttack","Guard"};


        public List<string> getActionPriority()
        {
            return ActionPriority;
        }

        public Skill getBaseSkill()
        {
            return null;
        }
    }
}
