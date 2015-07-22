using Microsoft.FSharp.Control;
using Microsoft.FSharp.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    internal interface IActorInbox : IDisposable
    {
        Unit Startup(IProcess pid, ProcessId supervisor);
        Unit Shutdown();
        Unit Tell(object message, ProcessId sender);
        Unit TellUserControl(UserControlMessage message);
        Unit TellSystem(SystemMessage message);
    }

    internal class ActorInbox<S,T> : IActorInbox
    {
        ProcessId pid;
        ProcessId supervisor;
        CancellationTokenSource tokenSource;
        FSharpMailboxProcessor<UserControlMessage> userInbox;
        FSharpMailboxProcessor<SystemMessage> sysInbox;
        Actor<S, T> actor;

        public Unit Startup(IProcess process, ProcessId supervisor)
        {

            if (Active)
            {
                Shutdown();
            }
            this.tokenSource = new CancellationTokenSource();
            this.actor = (Actor<S, T>)process;
            this.supervisor = supervisor;
            this.sysInbox = StartSystemMailbox(actor, supervisor, tokenSource.Token);
            this.userInbox = StartUserMailbox(actor, supervisor, tokenSource.Token);

            return unit;
        }

        public Unit Shutdown()
        {
            var ts = tokenSource;
            if (ts != null)
            {
                ts.Cancel();
                ts.Dispose();
                tokenSource = null;
                userInbox = null;
                sysInbox = null;
                pid = ProcessId.None;
                supervisor = ProcessId.None;
            }
            return unit;
        }

        public bool Active => 
            tokenSource != null;

        public Unit Tell(object message, ProcessId sender )
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            if (!typeof(T).IsAssignableFrom(message.GetType()))
            {
                Process.tell(ActorContext.DeadLetters, message);
                return unit;
            }

            sender = sender.IsValid
                ? sender
                : ActorContext.Self.IsValid
                    ? ActorContext.Self
                    : ActorContext.NoSender;

            userInbox.Post(new UserMessage(message, sender, sender));

            return unit;
        }

        public Unit TellUserControl(UserControlMessage message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            userInbox.Post(message);
            return unit;
        }

        public Unit TellSystem(SystemMessage message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            sysInbox.Post(message);
            return unit;
        }

        private FSharpAsync<A> CreateAsync<A>(Func<Task<A>> f) =>
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

        private FSharpMailboxProcessor<SystemMessage> StartSystemMailbox(Actor<S,T> actor, ProcessId supervisor, CancellationToken cancelToken)
        {
            var body = FuncConvert.ToFSharpFunc<FSharpMailboxProcessor<SystemMessage>, FSharpAsync<Microsoft.FSharp.Core.Unit>>(
                mbox =>

                    CreateAsync<Microsoft.FSharp.Core.Unit>(async () =>
                    {
                        while (!cancelToken.IsCancellationRequested)
                        {
                            var msg = await FSharpAsync.StartAsTask(mbox.Receive(FSharpOption<int>.None), FSharpOption<TaskCreationOptions>.None, FSharpOption<CancellationToken>.Some(cancelToken));

                            if (msg != null && !cancelToken.IsCancellationRequested)
                            {
                                ActorContext.WithContext(actor.Id, ProcessId.NoSender, () =>
                                {
                                    switch (msg.Tag)
                                    {
                                        case SystemMessageTag.Shutdown:
                                            actor.Shutdown();
                                            tokenSource?.Cancel();
                                            break;

                                        case SystemMessageTag.ChildIsFaulted:
                                            // TODO: Add extra strategy behaviours here
                                            var scifm = (SystemChildIsFaultedMessage)msg;
                                            Process.tell(scifm.ChildId, SystemMessage.Restart);
                                            Process.tell(ActorContext.Errors, scifm.Exception);
                                            break;

                                        case SystemMessageTag.Restart:
                                            actor.Restart();
                                            break;

                                        case SystemMessageTag.LinkChild:
                                            var slcm = (SystemLinkChildMessage)msg;
                                            ((IProcessInternal)actor).LinkChild(slcm.ChildId);
                                            break;

                                        case SystemMessageTag.UnLinkChild:
                                            ((IProcessInternal)actor).UnlinkChild(((SystemUnLinkChildMessage)msg).ChildId);
                                            break;
                                    }
                                });
                            }
                        }
                        return null;
                    })
            );

            return FSharpMailboxProcessor<SystemMessage>.Start(body, FSharpOption<CancellationToken>.None);
        }

        private FSharpMailboxProcessor<UserControlMessage> StartUserMailbox(Actor<S, T> actor, ProcessId supervisor, CancellationToken cancelToken)
        {
            var body = FuncConvert.ToFSharpFunc<FSharpMailboxProcessor<UserControlMessage>, FSharpAsync<Microsoft.FSharp.Core.Unit>>(
                mbox =>

                    CreateAsync<Microsoft.FSharp.Core.Unit>(async () =>
                    {
                        while (!cancelToken.IsCancellationRequested)
                        {
                            var msg = await FSharpAsync.StartAsTask(mbox.Receive(FSharpOption<int>.None), FSharpOption<TaskCreationOptions>.None, FSharpOption<CancellationToken>.Some(cancelToken));
                            if (msg != null && !tokenSource.IsCancellationRequested)
                            {
                                if (msg.MessageType == Message.Type.User)
                                {
                                    var umsg = (UserMessage)msg;
                                    ActorContext.WithContext(actor.Id, umsg.Sender, () => actor.ProcessMessage((T)((UserMessage)msg).Content));

                                }
                                else if (msg.MessageType == Message.Type.UserControl)
                                {
                                    switch (msg.Tag)
                                    {
                                        case UserControlMessageTag.Shutdown:
                                            ActorContext.WithContext(actor.Id, ProcessId.NoSender, () => actor.Shutdown() );
                                            break;
                                    }
                                }
                            }
                        }
                        actor.Dispose();
                        return null;
                    })
            );

            var mailbox = FSharpMailboxProcessor<UserControlMessage>.Start(body, FSharpOption<CancellationToken>.None);
            mailbox.Error += (object sender, Exception args) => Process.tell(supervisor, SystemMessage.ChildIsFaulted(actor.Id, args));
            return mailbox;
        }

        public void Dispose()
        {
            var ts = tokenSource;
            if (ts != null) ts.Dispose();
        }
    }
}
