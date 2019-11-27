using GameApplication.Abstracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameApplication.Races
{
    class Human : Race
    {
        static new int baseHeatlh = 69;
        static new int baseMind = 13;
        static new int baseStrength = 13;
        static new int baseSpirit = 13;
        static new int baseSpeed = 7;
    }

    class Elf : Race
    {
        static new int baseHeatlh = 60;
        static new int baseMind = 19;
        static new int baseStrength = 7;
        static new int baseSpirit = 13;
        static new int baseSpeed = 12;
    }

    class HalfElf : Race
    {
        static new int baseHeatlh = 63;
        static new int baseMind = 15;
        static new int baseStrength = 11;
        static new int baseSpirit = 13;
        static new int baseSpeed = 10;
    }

    class Dwarf : Race
    {
        static new int baseHeatlh = 80;
        static new int baseMind = 5;
        static new int baseStrength = 20;
        static new int baseSpirit = 15;
        static new int baseSpeed = 4;
    }

    class Tiefling : Race
    {
        static new int baseHeatlh = 50;
        static new int baseMind = 18;
        static new int baseStrength = 10;
        static new int baseSpirit = 6;
        static new int baseSpeed = 15;
    }

    class Orc : Race
    {
        static new int baseHeatlh = 85;
        static new int baseMind = 6;
        static new int baseStrength = 22;
        static new int baseSpirit = 6;
        static new int baseSpeed = 9;
    }

    class Gnome : Race
    {
        static new int baseHeatlh = 50;
        static new int baseMind = 16;
        static new int baseStrength = 8;
        static new int baseSpirit = 16;
        static new int baseSpeed = 10;
    }

    class Halfling : Race
    {
        static new int baseHeatlh = 45;
        static new int baseMind = 13;
        static new int baseStrength = 8;
        static new int baseSpirit = 19;
        static new int baseSpeed = 12;
    }
}
