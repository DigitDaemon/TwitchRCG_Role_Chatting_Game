using GameApplication.Abstracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameApplication.Classes
{
    class Fighter : Class
    {
        static new List<string> ActionPriority = new List<string>() { "Item","Skill","PhysAttack","Guard"};
        static new Skill baseSkill = null;
    }

    class Sorcerer : Class
    {
        static new List<string> ActionPriority = new List<string>() { "Item", "Skill", "MagAttack", "Guard" };
        static new Skill baseSkill = null;
    }

    class Barbarian : Class
    {
        static new List<string> ActionPriority = new List<string>() { "Item", "Skill", "PhysAttack", "Guard" };
        static new Skill baseSkill = null;
    }

    class Paladin : Class
    {
        static new List<string> ActionPriority = new List<string>() { "Item", "Skill", "PhysAttack", "Guard" };
        static new Skill baseSkill = null;
    }

    class Bard : Class
    {
        static new List<string> ActionPriority = new List<string>() { "Item", "Skill", "Heal", "MagAttack", "Guard" };
        static new Skill baseSkill = null;
    }

    class Rogue : Class
    {
        static new List<string> ActionPriority = new List<string>() { "Item", "Skill", "PhysAttack", "Guard" };
        static new Skill baseSkill = null;
    }

    class Wizard : Class
    {
        static new List<string> ActionPriority = new List<string>() { "Item", "Skill", "MagAttack", "Guard" };
        static new Skill baseSkill = null;
    }

    class Warlock : Class
    {
        static new List<string> ActionPriority = new List<string>() { "Item", "Skill", "MagAttack", "Guard" };
        static new Skill baseSkill = null;
    }

    class Cleric : Class
    {
        static new List<string> ActionPriority = new List<string>() { "Item", "Skill", "Heal", "PhysAttack", "Guard" };
        static new Skill baseSkill = null;
    }

    class Druid : Class
    {
        static new List<string> ActionPriority = new List<string>() { "Item", "Skill", "MagAttack", "Guard" };
        static new Skill baseSkill = null;
    }
}
