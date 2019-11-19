using System;
using System.Collections.Generic;
using System.Text;

namespace GameApplication.Abstracts
{
    interface Class
    {
        Skill getBaseSkill();

        List<string> getActionPriority();
    }
}
