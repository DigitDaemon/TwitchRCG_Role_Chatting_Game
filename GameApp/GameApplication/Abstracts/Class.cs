using System;
using System.Collections.Generic;
using System.Text;

namespace GameApplication.Abstracts
{
    public interface Class
    {
        Skill getBaseSkill();

        List<string> getActionPriority();
    }
}
