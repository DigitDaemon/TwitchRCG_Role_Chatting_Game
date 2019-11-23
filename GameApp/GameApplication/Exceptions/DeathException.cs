using System;
using System.Collections.Generic;
using System.Text;

namespace GameApplication.Exceptions
{
    public class DeathException : Exception
    {
        Abstracts.Agent source;

        public DeathException(Abstracts.Agent source)
        {
            this.source = source;
        }

        public DeathException(string message, Abstracts.Agent source)
            : base(message)
        {
            this.source = source;
        }

        public DeathException(string message, Exception inner, Abstracts.Agent source)
            : base(message, inner)
        {
            this.source = source;
        }

        public Abstracts.Agent getObject()
        {
            return source;
        }
    }
}
