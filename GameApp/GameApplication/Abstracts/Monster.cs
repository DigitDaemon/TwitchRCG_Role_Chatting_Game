﻿using System;
using System.Collections.Generic;
using System.Text;

namespace GameApplication.Abstracts
{
    public abstract class Monster : Agent
    {
        protected int CR = 3;
        protected int Exp = 35;

        public Monster(string name, int baseHealth, int strength, int mind, int concentration, int mastery, List<Skill> skills, int speed, int CR, int Exp)
            :base(name, baseHealth, strength, mind, concentration, mastery, skills, speed)
        {
            this.CR = CR;
            this.Exp = Exp;
        }

        public override string getType()
        {
            return "Monster";
        }

        public int getCR()
        {
            return CR;
        }

        public int getExp()
        {
            return Exp;
        }
    }
}
