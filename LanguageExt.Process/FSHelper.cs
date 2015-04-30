using System;
using System.Threading.Tasks;
using Microsoft.FSharp.Control;
using Microsoft.FSharp.Core;
using System.Threading;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    /// <summary>
    /// TODO: Tidy this up, the helper has stopped being a helper
    /// </summary>
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

        public static Tuple<Action,FSharpMailboxProcessor<SystemMessage>> StartSystemMailbox(IProcess self, ProcessId supervisor)
        {
            bool active = true;
            Action quit = () => active = false;

            var body = FuncConvert.ToFSharpFunc<FSharpMailboxProcessor<SystemMessage>, FSharpAsync<Microsoft.FSharp.Core.Unit>>(
                mbox =>

                    CreateAsync<Microsoft.FSharp.Core.Unit>(async () =>
                    {
                        while (active)
                        {
                            var msg = await FSharpAsync.StartAsTask(mbox.Receive(FSharpOption<int>.None), FSharpOption<TaskCreationOptions>.None, FSharpOption<CancellationToken>.None);

                            if (msg == null || !active)
                            {
                                active = false;
                            }
                            else
                            {
                                switch (msg.Tag)
                                {
                                    case SystemMessageTag.Shutdown:
                                        self.Shutdown();
                                        active = false;
                                        break;

                                    case SystemMessageTag.ChildIsFaulted:
                                        ((IProcessInternal)self).HandleFaultedChild((SystemChildIsFaultedMessage)msg);
                                        break;

                                    case SystemMessageTag.Restart:
                                        self.Restart();
                                        break;

                                    case SystemMessageTag.UnLinkChild:
                                        ((IProcessInternal)self).UnlinkChild(((SystemUnLinkChildMessage)msg).ChildId);
                                        break;
                                }
                            }
                        }
                        return null;
                    })
            );

            var mailbox = FSharpMailboxProcessor<SystemMessage>.Start(body, FSharpOption<CancellationToken>.None);
            mailbox.Error += (object sender, Exception args) =>
                               Process.tell(supervisor, SystemMessage.ChildIsFaulted(self.Id, args));
            return tuple(quit,mailbox);
        }

        public static Tuple<Action,FSharpMailboxProcessor<UserControlMessage>> StartUserMailbox<S, T>(IProcess self, ProcessId supervisor, Func<S, T, S> actor, Func<S> setup)
        {
            bool active = true;
            S state = default(S);
            Action quit = () => active = false;

            var body = FuncConvert.ToFSharpFunc<FSharpMailboxProcessor<UserControlMessage>, FSharpAsync<Microsoft.FSharp.Core.Unit>>(
                mbox =>

                    CreateAsync<Microsoft.FSharp.Core.Unit>(async () =>
                    {
                        ActorContext.SetContext(self, ActorContext.NoSender);
                        state = setup();

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
                                    switch (msg.Tag)
                                    {
                                        case UserControlMessageTag.Shutdown:
                                            self.Shutdown();
                                            active = false;
                                            break;
                                    }
                                }
                            }
                        }

                        (state as IDisposable)?.Dispose();

                        return null;
                    })
            );

            var mailbox = FSharpMailboxProcessor<UserControlMessage>.Start(body, FSharpOption<CancellationToken>.None);
            mailbox.Error += (object sender, Exception args) =>
            {
                Process.tell(supervisor, SystemMessage.ChildIsFaulted(self.Id, args));
                (state as IDisposable)?.Dispose();
            };

            return tuple(quit, mailbox);
        }
    }
}
