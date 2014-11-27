using System;
using System.Threading.Tasks;
using Microsoft.FSharp.Control;
using Microsoft.FSharp.Core;
using System.Threading;
using LanguageExt.Prelude;

namespace LanguageExt
{
    internal class FSHelper
    {
        public static FSharpAsync<A> CreateAsync<A>(Func<Task<A>> f) =>
            FSharpAsync.FromContinuations<A>(
                FuncConvert.ToFSharpFunc<Tuple<FSharpFunc<A, Microsoft.FSharp.Core.Unit>, FSharpFunc<Exception, Microsoft.FSharp.Core.Unit>, FSharpFunc<OperationCanceledException, Microsoft.FSharp.Core.Unit>>>(
                    conts =>
                    {
                        f().ContinueWith(task =>
                        {
                            try { conts.Item1.Invoke(task.Result); }
                            catch (Exception e) { conts.Item2.Invoke(e); }
                        });
                    }));

        public static Tuple<Action,FSharpMailboxProcessor<SystemMessage>> StartSystemMailbox(IProcess self, ProcessId supervisor, Func<Unit, SystemMessage, Unit> actor, Func<Unit> setup)
        {
            bool active = true;
            Action quit = () => active = false;

            var body = FuncConvert.ToFSharpFunc<FSharpMailboxProcessor<SystemMessage>, FSharpAsync<Microsoft.FSharp.Core.Unit>>(
                mbox =>

                    CreateAsync<Microsoft.FSharp.Core.Unit>(async () =>
                    {
                        var state = setup();

                        while (active)
                        {
                            var msg = await FSharpAsync.StartAsTask(mbox.Receive(FSharpOption<int>.None), FSharpOption<TaskCreationOptions>.None, FSharpOption<CancellationToken>.None);

                            if (msg == null || !active)
                            {
                                active = false;
                            }
                            else
                            {
                                switch (msg.GetType().Name)
                                {
                                    case "SystemSuspendMessage":
                                        Console.WriteLine("SYS: Suspend - " + self.Id);
                                        self.Suspend();
                                        break;

                                    case "SystemShutdownMessage":
                                        Console.WriteLine("SYS: Shutdown - " + self.Id);
                                        self.Shutdown();
                                        active = false;
                                        break;

                                    case "SystemChildIsFaultedMessage":
                                        ((IProcessInternal)self).HandleFaultedChild((SystemChildIsFaultedMessage)msg);
                                        break;

                                    case "SystemRestartMessage":
                                        Console.WriteLine("SYS: Restart - " + self.Id);
                                        self.Restart();
                                        break;
                                }
                            }
                        }
                        return null;
                    })
            );

            var mailbox = FSharpMailboxProcessor<SystemMessage>.Start(body, FSharpOption<CancellationToken>.None);
            mailbox.Error += (object sender, Exception args) =>
                               Process.tell(supervisor,new SystemChildIsFaultedMessage(self.Name, self.Id, args));
            return tuple(quit,mailbox);
        }

        public static Tuple<Action,FSharpMailboxProcessor<UserControlMessage>> StartUserMailbox<S, T>(IProcess self, ProcessId supervisor, Func<S, T, S> actor, Func<S> setup)
        {
            bool active = true;
            Action quit = () => active = false;

            var body = FuncConvert.ToFSharpFunc<FSharpMailboxProcessor<UserControlMessage>, FSharpAsync<Microsoft.FSharp.Core.Unit>>(
                mbox =>

                    CreateAsync<Microsoft.FSharp.Core.Unit>(async () =>
                    {
                        ActorContext.SetContext(self, ActorContext.NoSender);
                        S state = setup();

                        while (active)
                        {
                            var msg = await FSharpAsync.StartAsTask(mbox.Receive(FSharpOption<int>.None), FSharpOption<TaskCreationOptions>.None, FSharpOption<CancellationToken>.None);
                            if (msg == null || !active)
                            {
                                active = false;
                            }
                            else
                            {
                                if (msg.MessageType == Message.Type.User)
                                {
                                    var umsg = (UserMessage)msg;

                                    ActorContext.SetContext(self, umsg.Sender);
                                    state = actor(state, (T)((UserMessage)msg).Content);
                                }
                                else if (msg.MessageType == Message.Type.UserControl)
                                {
                                    switch (msg.GetType().Name)
                                    {
                                        case "UserControlShutdownMessage":
                                            Console.WriteLine("UC: Shutdown - " + self.Id);
                                            self.Shutdown();
                                            active = false;
                                            break;

                                        case "UserControlKillMessage":
                                            Console.WriteLine("UC: KILL! - " + self.Id);
                                            throw new SystemKillActorException();
                                    }
                                }
                            }
                        }
                        return null;
                    })
            );

            var mailbox = FSharpMailboxProcessor<UserControlMessage>.Start(body, FSharpOption<CancellationToken>.None);
            mailbox.Error += (object sender, Exception args) => 
                Process.tell(supervisor, new SystemChildIsFaultedMessage(self.Name, self.Id, args));

            return tuple(quit, mailbox);
        }

    }
}
