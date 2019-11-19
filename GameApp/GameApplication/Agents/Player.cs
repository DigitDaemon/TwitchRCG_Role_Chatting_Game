using GameApplication.Abstracts;
using GameApplication.Agent_Dependencies;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameApplication
{
    public class Player : Agent
    {
        Class spec;
        Race race;


        public Player(int baseHealth, int strength, int mind, int concentration, int mastery, List<Skill> skills)
           : base(baseHealth, strength, mind, concentration, mastery, skills)
        {
        }

    }
}
