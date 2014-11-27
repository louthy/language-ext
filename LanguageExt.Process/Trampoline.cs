using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt.Prelude;

namespace LanguageExt
{
    public delegate Unit Cont<T>(T value);
    public delegate Unit ECont(Exception value);
    public delegate Unit CCont(OperationCanceledException value);

    public delegate Unit SendOrPostCallback(Cont<Unit> f);
    public delegate Unit WaitCallback(Cont<Unit> f);


    public class Trampoline
    {
        [ThreadStatic]
        static bool thisThreadHasTrampoline;
        static bool ThisThreadHasTrampoline => thisThreadHasTrampoline;
        Option<Cont<Unit>> cont = None;
        int bindCount;
        static int bindLimitBeforeHijack = 300;

        public Unit ExecuteAction(Cont<Unit> firstAction)
        {
            var action = firstAction;

            bool thisIsTopTrampoline = false;
            if (thisThreadHasTrampoline)
            {
                thisIsTopTrampoline = false;
            }
            else
            {
                Trampoline.thisThreadHasTrampoline = thisIsTopTrampoline = true;
            }

            try
            {
                while (true)
                {
                    action(unit);

                    if (cont.IsNone)
                    {
                        break;
                    }

                    match(cont,
                          Some: newAction =>
                          {
                              action = newAction;
                              cont = null;
                          },
                          None: () => { }
                    );
                }
            }
            finally
            {
                if (thisIsTopTrampoline)
                {
                    Trampoline.thisThreadHasTrampoline = false;
                }
            }
            return unit;
        }

        // returns true if time to jump on trampoline
        public bool IncrementBindCount()
        {
            bindCount = bindCount + 1;
            return bindCount >= bindLimitBeforeHijack;
        }

        public Unit Set(Cont<Unit> action) =>
            match(cont,
                None: () =>
                {
                    bindCount = 0;
                    cont = Some(action);
                },
                Some: _ => failwith("Internal error: attempting to install continuation twice")
            );
    }

    public class TrampolineHolder
    {
        Trampoline trampoline;

        public Unit Protect(Cont<Unit> firstAction)
        {
            trampoline = new Trampoline();
            return trampoline.ExecuteAction(firstAction);
        }

        public SendOrPostCallback SendOrPostCallback =>
            new SendOrPostCallback( (Cont<Unit> f) => Protect(f) );

        public WaitCallback WaitCallbackForQueueWorkItemWithTrampoline =>
            new WaitCallback((Cont<Unit> f) => Protect(f));
    }
}
