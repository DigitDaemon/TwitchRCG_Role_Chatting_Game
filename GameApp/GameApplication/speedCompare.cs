using System;
using System.Collections.Generic;
using System.Text;
using GameApplication.Abstracts;

namespace GameApplication
{
    public class speedComparer : IComparer<Abstracts.Agent>
    {
        public int Compare(Agent x, Agent y)
        {
            if (x.getSpeed() >= y.getSpeed())
                return 1;
            else
                return -1;
        }
    }
}
