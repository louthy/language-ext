using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static class ProcessAssert
    {
        public static void CanAccept<T>(ProcessId pid, string message = null)
        {
            if (!ActorContext.GetDispatcher(pid).CanAccept<T>())
            {
                failwith<Unit>(message == null ? $"Process ({pid}) can't accept messages of type {typeof(T).FullName} " : message);
            }
        }

        public static void HasStateTypeOf<T>(ProcessId pid, string message = null)
        {
            if (!ActorContext.GetDispatcher(pid).HasStateTypeOf<T>())
            {
                failwith<Unit>(message == null ? $"Process ({pid}) doesn't have the expected state-type of {typeof(T).FullName} " : message);
            }
        }
    }
}
