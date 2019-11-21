using GameApplication.Abstracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameApplication.Races
{
    class Human : Race
    {
        protected int baseHeatlh = 69;

        protected int baseMind = 13;

        protected int baseStrength = 13;

        protected int baseSpeed = 7;

        public int getBaseHealth()
        {
            return baseHeatlh;
        }

        public int getBaseMind()
        {
            return baseMind;
        }

        public int getBaseSpeed()
        {
            return baseSpeed;
        }

        public int getBaseStrength()
        {
            return baseStrength;
        }
    }
}
