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
            var res = ActorContext.GetDispatcher(pid).CanAccept<T>();

            res.IfLeft(err =>
            {
                failwith<Unit>($"{err} for {pid}");
            });
        }

        public static void HasStateTypeOf<T>(ProcessId pid, string message = null)
        {
            var res = ActorContext.GetDispatcher(pid).HasStateTypeOf<T>();

            res.IfLeft(err =>
            {
                failwith<Unit>($"{err} for {pid}");
            });
        }
    }
}
