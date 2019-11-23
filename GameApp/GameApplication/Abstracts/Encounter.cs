using System;
using System.Collections.Generic;
using System.Text;

namespace GameApplication
{
    public abstract class Encounter
    {
        protected List<Abstracts.Agent> playerList { get; }
        protected List<Abstracts.Agent> deadPlayers { get; }

        public Encounter(List<Abstracts.Agent> playerList)
        {
            this.playerList = playerList;
            this.deadPlayers = new List<Abstracts.Agent>();
        }

        public abstract List<string> nextTurn();

        public abstract void checkCompletion(List<string> messages);

        public abstract string endEncounter();
    }
}
