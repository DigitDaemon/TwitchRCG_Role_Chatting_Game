using GameApplication.Abstracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameApplication.Builders
{
    class EncounterBuilder
    {
        static Random rand = new Random();
        public static Encounter BuildEncounter(List<Agent> players)
        {
            var count = players.Count;
            var wolves = count / 2;
            if (wolves <= 0)
                wolves = 1;
            var monsters = new List<Abstracts.Agent>();
            for (int i = 0; i < wolves; i++)
            {

                monsters.Add(new Agents.DireWolf(i, rand.Next(6) - 3));
            }

            return new MonsterEncounter(players, monsters);


        }
    }
}
