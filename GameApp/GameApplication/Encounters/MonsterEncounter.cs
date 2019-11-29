using Medallion.Collections;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameApplication
{

    class MonsterEncounter : Encounter
    {
        public List<Abstracts.Agent> monsters;
        public List<Abstracts.Agent> deadMonsters;
        PriorityQueue<Abstracts.Agent> turnList;

        public MonsterEncounter(List<Abstracts.Agent> playerList, List<Abstracts.Agent> monsters)
            : base(playerList)
        {
            deadMonsters = new List<Abstracts.Agent>();
            this.monsters = monsters;
            turnList = new PriorityQueue<Abstracts.Agent>(new speedComparer());
            
        }

        public override void checkCompletion(List<string> messages)
        {
            if(monsters.Count == 0 || playerList.Count == 0)
            {
                throw new Exceptions.GameOverException("The game is over", messages);
            }
        }

        public override string endEncounter()
        {
            var messages = "";

            if (playerList.Count > 0) {
                if(deadPlayers.Count == 0)
                {
                    messages = "The quest was successful and there were no casualties";
                }
                else
                {
                    var message = "The quest was successful but there were losses. ";
                    foreach(Abstracts.Agent player in deadPlayers)
                    {
                        message += player.getName() + " ";
                    }
                    message += "perished. However, ";
                    foreach (Abstracts.Agent player in playerList)
                    {
                        message += player.getName() + " ";
                    }
                    message += "managed to make it out alive after finishing off the monsters.";
                    messages = message;
                }
            }
            else
            {
                messages = "The quest has failed and all party members perished";
            }

            int exp = 0;
            foreach(Abstracts.Monster monster in deadMonsters)
            {
                exp += monster.getExp();
            }

            foreach(Abstracts.Agent player in playerList)
            {
                PlayerUpdater.updateCharacter(exp, player.getName());
            }
            foreach (Abstracts.Agent player in deadPlayers)
            {
                PlayerUpdater.updateCharacter(exp/2, player.getName());
            }

            return messages;
        }

        public override List<string> nextTurn()
        {
            if (turnList.Count == 0)
                fillTurnList();

            var messages = new List<string>();

            Abstracts.Agent currentAgent = turnList.Dequeue();
            KeyValuePair<string, string> action;

            if (currentAgent.getType().Equals("Player"))
            {
                action = currentAgent.getAction(getMonstersCondition(currentAgent.getName()), getPlayersCondition(currentAgent.getName()));
            }
            else
            {
                action = currentAgent.getAction(getPlayersCondition(currentAgent.getName()), getMonstersCondition(currentAgent.getName()));
            }

            if (action.Value.Equals("PhysAttack"))
            {
                Console.WriteLine(currentAgent.getName() + " is physically attacking " + action.Key);
                messages.Add(currentAgent.getName() + " is physically attacking " + action.Key);
                try
                {
                    if (currentAgent.getType().Equals("Player"))
                    {
                        var damage = currentAgent.physAttack();
                        Console.WriteLine(damage);
                        monsters.Find(x => x.getName().Equals(action.Key)).TakeDamage(damage, "Physical", null);
                    }
                    else
                    {
                        var damage = currentAgent.physAttack();
                        Console.WriteLine(damage);
                        playerList.Find(x => x.getName().Equals(action.Key)).TakeDamage(damage, "Physical", null);
                    }
                }
                catch (Exceptions.DeathException death)
                {
                    if (death.getObject().getType().Equals("Player"))
                    {
                        deadPlayers.Add(death.getObject());
                        playerList.RemoveAt(playerList.IndexOf(death.getObject()));
                        Console.WriteLine(death.Message);
                        messages.Add(death.Message);
                    }
                    else
                    {
                        deadMonsters.Add(death.getObject());
                        monsters.RemoveAt(monsters.IndexOf(death.getObject()));
                        Console.WriteLine(death.Message);
                        messages.Add(death.Message);
                    }
                    checkCompletion(messages);
                }
            }
            else if (action.Value.Equals("MagAttack"))
            {
                Console.WriteLine(currentAgent.getName() + " is attacking " + action.Key + "with magic");
                messages.Add(currentAgent.getName() + " is attacking " + action.Key + "with magic");

                try
                {
                    if (currentAgent.getType().Equals("Player"))
                        monsters.Find(x => x.getName().Equals(action.Key)).TakeDamage(currentAgent.physAttack(), "Magical", null);
                    else
                        playerList.Find(x => x.getName().Equals(action.Key)).TakeDamage(currentAgent.physAttack(), "Magical", null);
                }
                catch (Exceptions.DeathException death)
                {
                    if (death.getObject().getType().Equals("Player"))
                    {
                        deadPlayers.Add(death.getObject());
                        playerList.RemoveAt(playerList.IndexOf(death.getObject()));
                        Console.WriteLine(death.Message);
                        messages.Add(death.Message);
                    }
                    else
                    {
                        deadMonsters.Add(death.getObject());
                        monsters.RemoveAt(monsters.IndexOf(death.getObject()));
                        Console.WriteLine(death.Message);
                        messages.Add(death.Message);
                    }
                    checkCompletion(messages);
                }
            }
            else if (action.Value.Equals("Heal"))
            {
                Console.WriteLine(currentAgent.getName() + " is healing " + action.Key + ".");
                messages.Add(currentAgent.getName() + " is healing " + action.Key + ".");
                if (currentAgent.getType().Equals("Player"))
                {
                    playerList.Find(x => x.getName().Equals(action.Key)).getHealed(currentAgent.Heal(), null);
                }
                else
                {
                    monsters.Find(x => x.getName().Equals(action.Key)).getHealed(currentAgent.Heal(), null);
                }
            }
            else if (action.Value.Contains("Skill"))
            {
                var target = action.Key.Substring(0, action.Key.IndexOf(" "));
                var type = action.Key.Substring(action.Key.IndexOf(" "), action.Key.Length - target.Length).Trim();
                var skillname = action.Value.Substring(0, action.Value.IndexOf(" "));
                try
                {
                    if (type.Equals("Monster"))
                    {
                        messages.Add(currentAgent.useSkill(monsters.Find(x => x.getName().Equals(target)), skillname));
                    }
                    else
                    {
                        messages.Add(currentAgent.useSkill(playerList.Find(x => x.getName().Equals(target)), skillname));
                    }
                }
                catch (Exceptions.DeathException death)
                {
                    if (death.getObject().getType().Equals("Player"))
                    {
                        deadPlayers.Add(death.getObject());
                        playerList.RemoveAt(playerList.IndexOf(death.getObject()));
                        Console.WriteLine(death.Message);
                        messages.Add(death.Message);
                    }
                    else
                    {
                        deadMonsters.Add(death.getObject());
                        monsters.RemoveAt(monsters.IndexOf(death.getObject()));
                        Console.WriteLine(death.Message);
                        messages.Add(death.Message);
                    }
                    checkCompletion(messages);
                }
            }

                return messages;     
        }

        public void fillTurnList()
        {
            foreach (Abstracts.Agent player in playerList)
            {
                turnList.Enqueue(player);
            }
            foreach (Abstracts.Agent monster in monsters)
            {
                turnList.Enqueue(monster);
            }
        }

        public List<KeyValuePair<string,string>> getPlayersCondition(string source)
        {
            List<KeyValuePair<string, string>> conditionList = new List<KeyValuePair<string, string>>();
            foreach(Abstracts.Agent player in playerList)
            {
                if (!player.getName().Equals(source))
                {
                    var list = player.getStatus();
                    foreach(KeyValuePair<string, string> status in list)
                    {
                        conditionList.Add(status);
                    }
                }
            }

            return conditionList;
        }

        public List<KeyValuePair<string, string>> getMonstersCondition(string source)
        {
            List<KeyValuePair<string, string>> conditionList = new List<KeyValuePair<string, string>>();
            foreach (Abstracts.Agent monster in monsters)
            {
                if (!monster.getName().Equals(source))
                {
                    var list = monster.getStatus();
                    foreach (KeyValuePair<string, string> status in list)
                    {
                        conditionList.Add(status);
                    }
                }
            }

            return conditionList;
        }


    }
}
