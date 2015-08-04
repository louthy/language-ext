using Microsoft.FSharp.Control;
using Microsoft.FSharp.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LanguageExt
{
    internal class ActorInboxCommon
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

        public static FSharpMailboxProcessor<TMsg> Mailbox<S,T, TMsg>(Option<ICluster> cluster, ProcessFlags flags, CancellationToken cancelToken, Action<TMsg> handler)
            where TMsg : Message
        {
            var body = FuncConvert.ToFSharpFunc<FSharpMailboxProcessor<TMsg>, FSharpAsync<Microsoft.FSharp.Core.Unit>>(
                mbox =>
                    CreateAsync<Microsoft.FSharp.Core.Unit>(async () =>
                    {
                        while (!cancelToken.IsCancellationRequested)
                        {
                            try
                            {
                                var msg = await FSharpAsync.StartAsTask(mbox.Receive(FSharpOption<int>.None), FSharpOption<TaskCreationOptions>.None, FSharpOption<CancellationToken>.Some(cancelToken));
                                if (msg != null && !cancelToken.IsCancellationRequested)
                                {
                                    handler(msg);
                                }
                            }
                            catch (TaskCanceledException)
                            {
                                // We're being shutdown, ignore.
                            }
                            catch (Exception e)
                            {
                                Process.logSysErr(e);
                            }
                        }
                        return null;
                    })
            );

            return FSharpMailboxProcessor<TMsg>.Start(body, FSharpOption<CancellationToken>.None);
        }

        public static void SystemMessageInbox<S,T>(Actor<S,T> actor, SystemMessage msg)
        {
            ActorContext.WithContext(actor, ProcessId.NoSender, msg, () =>
            {
                switch (msg.Tag)
                {
                    case Message.TagSpec.ChildIsFaulted:
                        // TODO: Add extra strategy behaviours here
                        var scifm = (SystemChildIsFaultedMessage)msg;
                        Process.tell(scifm.ChildId, SystemMessage.Restart);
                        Process.tell(ActorContext.Errors, scifm.Exception);
                        break;

                    case Message.TagSpec.Restart:
                        actor.Restart();
                        break;

                    case Message.TagSpec.LinkChild:
                        var slcm = (SystemLinkChildMessage)msg;
                        actor.LinkChild(slcm.ChildId);
                        break;

                    case Message.TagSpec.UnLinkChild:
                        var ulcm = (SystemUnLinkChildMessage)msg;
                        actor.UnlinkChild(ulcm.ChildId);
                        break;
                }
            });
        }

        public static void UserMessageInbox<S, T>(Actor<S, T> actor, UserControlMessage msg)
        {
            if (msg.Tag == Message.TagSpec.UserAsk)
            {
                var rmsg = (ActorRequest)msg;
                ActorContext.CurrentRequest = rmsg;
                ActorContext.WithContext(actor, rmsg.ReplyTo, msg, () => actor.ProcessAsk(rmsg));
            }
            else if (msg.Tag == Message.TagSpec.UserReply)
            {
                var rmsg = (ActorResponse)msg;
                ActorContext.WithContext(actor, rmsg.ReplyFrom, msg, () => actor.ProcessMessage(msg));
            }
            else if (msg.Tag == Message.TagSpec.User)
            {
                var umsg = (UserMessage)msg;
                ActorContext.WithContext(actor, umsg.Sender, msg, () => actor.ProcessMessage(umsg.Content));
            }
            else if (msg.MessageType == Message.Type.UserControl)
            {
                switch (msg.Tag)
                {
                    case Message.TagSpec.Shutdown:
                        Process.kill(actor.Id);
                        break;
                }
            }
        }
    }
}
