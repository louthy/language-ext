using System;

namespace LanguageExt
{
    public class DeadlockException : Exception 
    {
        public DeadlockException() : base("Deadlock occured during atomic update") { }
    }
}
