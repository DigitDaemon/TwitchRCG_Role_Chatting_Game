using System;
using System.Collections.Generic;
using System.Text;

namespace GameApplication.Exceptions
{
    class GameOverException : Exception
    {
        public GameOverException()
        {
        }

        public GameOverException(string message)
            : base(message)
        {
        }

        public GameOverException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
