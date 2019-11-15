using System;
using System.Collections.Generic;
using System.Text;

namespace GameApplication.Exceptions
{
    public class DeathException : Exception
    {
        public DeathException()
        {
        }

        public DeathException(string message)
            : base(message)
        {
        }

        public DeathException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
