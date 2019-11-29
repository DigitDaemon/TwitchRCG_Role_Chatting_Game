using GameApplication.Abstracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameApplication.Classes
{
    class Fighter : Class
    {
        static new string name = "Fighter";
        static new List<string> ActionPriority = new List<string>() { "Item","Skill","PhysAttack","Guard"};
        static new Skill baseSkill = new Skills.PowerAttack();
    }

    class Sorcerer : Class
    {
        static new string name = "Sorcerer";
        static new List<string> ActionPriority = new List<string>() { "Item", "Skill", "MagAttack", "Guard" };
        static new Skill baseSkill = new Skills.Fireball();
    }

    class Barbarian : Class
    {
        static new string name = "Barbarian";
        static new List<string> ActionPriority = new List<string>() { "Item", "Skill", "PhysAttack", "Guard" };
        static new Skill baseSkill = new Skills.AttackRecklessly();
    }

    class Paladin : Class
    {
        static new string name = "Paladin";
        static new List<string> ActionPriority = new List<string>() { "Item", "Skill", "PhysAttack", "Guard" };
        static new Skill baseSkill = new Skills.LightOfSalvation();
    }

    class Bard : Class
    {
        static new string name = "Bard";
        static new List<string> ActionPriority = new List<string>() { "Item", "Skill", "Heal", "MagAttack", "Guard" };
        static new Skill baseSkill = null;
    }

    class Rogue : Class
    {
        static new string name = "Rogue";
        static new List<string> ActionPriority = new List<string>() { "Item", "Skill", "PhysAttack", "Guard" };
        static new Skill baseSkill = null;
    }

    class Wizard : Class
    {
        static new string name = "Wizard";
        static new List<string> ActionPriority = new List<string>() { "Item", "Skill", "MagAttack", "Guard" };
        static new Skill baseSkill = null;
    }

    class Warlock : Class
    {
        static new string name = "Warlock";
        static new List<string> ActionPriority = new List<string>() { "Item", "Skill", "MagAttack", "Guard" };
        static new Skill baseSkill = null;
    }

    class Cleric : Class
    {
        static new string name = "Cleric";
        static new List<string> ActionPriority = new List<string>() { "Item", "Skill", "Heal", "PhysAttack", "Guard" };
        static new Skill baseSkill = null;
    }

    class Druid : Class
    {
        static new string name = "Druid";
        static new List<string> ActionPriority = new List<string>() { "Item", "Skill", "MagAttack", "Guard" };
        static new Skill baseSkill = null;
    }
}
