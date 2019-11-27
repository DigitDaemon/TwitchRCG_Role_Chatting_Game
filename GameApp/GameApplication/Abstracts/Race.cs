using System;
using System.Collections.Generic;
using System.Text;

namespace GameApplication.Abstracts
{
    public abstract class Race
    {
        static protected int baseHeatlh = 69;

        static protected int baseMind = 13;

        static protected int baseStrength = 13;

        static protected int baseSpirit = 13;

        static protected int baseSpeed = 7;

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

        public int getBaseSpirit()
        {
            return baseSpirit;
        }
    }}
