using System;
using System.Collections.Generic;
using System.Text;

namespace DatabaseApplication
{
    class UserProfile
    {
        public string twitch_name { get; set; }

        public string discord_name { get; set; }

        public Int16 temper { get; set; }

        public Int16 charisma { get; set; }

        public Int16 cheer { get; set; }

        public Int16 empathy { get; set; }

        public Int16 curiosity { get; set; }

        public Int64 experience { get; set; }

        public string _class { get; set; }

        public string race { get; set; }

        public bool growing { get; set; }

        public string armor { get; set; }

        public string weapon { get; set; }

        public string Item { get; set; }

        public string skill { get; set; }

        public string Level { get; set; }
    }
}
