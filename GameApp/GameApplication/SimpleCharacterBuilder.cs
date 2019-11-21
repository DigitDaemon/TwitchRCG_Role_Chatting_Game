using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.IO;
using System.Text;

namespace GameApplication
{

    static class SimpleCharacterBuilder
    {
        //strings: ODBC configuration
        //
        //path - the path to the folder containing sensitive information for this project on my computer
        //dbConnString - the text contained inside the Connection.txt file with information for connecting to the database
        private static string path = Path.Combine(Environment.CurrentDirectory.Replace(@"bin\Debug\netcoreapp2.1", ""), @"Data\");
        static private string dbConnString = File.ReadAllText(Path.Combine(path, "Connection.txt"));//do not push this!!!

        //objs: ODBS objects
        //
        //data - the data returned from a query
        //dbAdapter - the object that handles retrival of data from the database
        //dbCommand - the command builder for <dbAdapter>
        //connDB - the connection to the database
        static DataSet data;
        static OdbcDataAdapter dbAdapter = new OdbcDataAdapter();
        static OdbcCommandBuilder dbCommand = new OdbcCommandBuilder(dbAdapter);
        static OdbcConnection connDB = new OdbcConnection(dbConnString);

        static public Agents.Player buildCharacter(string uname)
        {
            try
            {
                connDB.Open();
            }
            catch (OdbcException e)
            {
                Console.WriteLine(e.Message + "\n\n" + e.StackTrace);
            }

            dbAdapter.SelectCommand = new OdbcCommand("SELECT * FROM users WHERE twitch_name='" + uname + "';", connDB);
            dbAdapter.Fill(data);

            if (data.Tables[0].Rows.Count == 0)
                throw new GameApplication.Exceptions.NoSuchPlayerException("There is no " + uname + " in the database.");

            Abstracts.Class spec = new Classes.Fighter();
            Abstracts.Race race = new Races.Human();

            int baseHealth = race.getBaseHealth() + (int)((1f / 3f) * float.Parse(data.Tables[0].Rows[0]["temper"].ToString())) + (int)((1f / 4f) * float.Parse(data.Tables[0].Rows[0]["cheer"].ToString()));
            int strength = race.getBaseStrength() + (int)((1f / 4f) * float.Parse(data.Tables[0].Rows[0]["temper"].ToString()));
            int mind = race.getBaseMind() + (int)((1f / 3f) * float.Parse(data.Tables[0].Rows[0]["curiosity"].ToString()));
            int mastery = (int)(1f / 2f) * int.Parse(data.Tables[0].Rows[0]["empathy"].ToString());
            int concentration = (int)(1f / 2f) * int.Parse(data.Tables[0].Rows[0]["charisma"].ToString());
            var skills = new List<Abstracts.Skill>();
            int speed = race.getBaseSpeed() + (int)(1f / 2f) * int.Parse(data.Tables[0].Rows[0]["cheer"].ToString());
            Agents.Player player = new Agents.Player(baseHealth, strength, mind, concentration, mastery, skills, speed, spec, race, uname);
            connDB.Close();

            return player;
        }
    }
}
