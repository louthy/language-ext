using System;
using static LanguageExt.Process;

namespace LanguageExt
{
    internal static class ActorSystem
    {
        public static ActorSystemState Inbox(ActorSystemState state, object msg)
        {
            try
            {
                if (msg is ActorSystemMessage)
                {
                    logInfo(msg);

                    var rmsg = msg as ActorSystemMessage;
                    switch (rmsg.Tag)
                    {
                        case ActorSystemMessageTag.Startup:
                            state = state.Startup();
                            break;

                        case ActorSystemMessageTag.AddToStore: 
                            state = AddToStore(state, rmsg as AddToStoreMessage);
                            break;

                        case ActorSystemMessageTag.RemoveFromStore:
                            state = RemoveFromStore(state, rmsg as RemoveFromStoreMessage);
                            break;

                        case ActorSystemMessageTag.Tell:
                        case ActorSystemMessageTag.TellSystem:
                        case ActorSystemMessageTag.TellUserControl:
                            Tell(state, rmsg as TellMessage);
                            break;

                        case ActorSystemMessageTag.ShutdownProcess:
                            ShutdownProcess(state, rmsg as ShutdownProcessMessage);
                            break;

                        case ActorSystemMessageTag.ShutdownAll:
                            ShutdownAll(state);
                            break;

                        case ActorSystemMessageTag.GetChildren:
                            GetChildren(state, rmsg as GetChildrenMessage);
                            break;

                        case ActorSystemMessageTag.Register:
                            state = Register(state, rmsg as RegisterMessage);
                            break;

                        case ActorSystemMessageTag.Deregister:
                            Deregister(state, rmsg as DeregisterMessage);
                            break;

                        case ActorSystemMessageTag.Publish:
                            Publish(state, rmsg as PubMessage);
                            break;

                        case ActorSystemMessageTag.Reply:
                            Reply(state, rmsg as ReplyMessage);
                            break;

                        case ActorSystemMessageTag.ObservePub:
                            ObservePub(state, rmsg as ObservePubMessage);
                            break;

                        case ActorSystemMessageTag.ObserveState:
                            ObserveState(state, rmsg as ObserveStateMessage);
                            break;
                    }
                }
                else if (msg is SystemMessage)
                {
                    state.RootInbox.TellSystem(msg as SystemMessage);
                }
                else if (msg is UserControlMessage)
                {
                    state.RootInbox.TellUserControl(msg as UserControlMessage);
                }
                else
                {
                    publish(msg);
                }
            }
            catch (Exception e)
            {
                logSysErr(e);
            }
            return state;
        }

        private static void Reply(ActorSystemState state, ReplyMessage msg) =>
            state.Reply(msg.Message, msg.Requestid, msg.Sender);

        private static Unit GetChildren(ActorSystemState state, GetChildrenMessage msg) =>
            state.GetChildren(msg.ProcessId);

        private static ActorSystemState ShutdownProcess(ActorSystemState state, ShutdownProcessMessage msg) =>
            state = state.ShutdownProcess(msg.ProcessId);

        private static Unit ObservePub(ActorSystemState state, ObservePubMessage msg) =>
            state.ObservePub(msg.ProcessId, msg.MsgType);

        private static Unit ObserveState(ActorSystemState state, ObserveStateMessage msg) =>
            state.ObserveState(msg.ProcessId);

        private static Unit Publish(ActorSystemState state, PubMessage msg) =>
            state.Publish(msg.ProcessId, msg.Message);

        private static ActorSystemState AddToStore(ActorSystemState state, AddToStoreMessage msg) =>
            state.AddOrUpdateStoreAndStartup(msg.Process, msg.Inbox, msg.Flags);

        private static ActorSystemState RemoveFromStore(ActorSystemState state, RemoveFromStoreMessage msg) =>
            state.RemoveFromStore(msg.ProcessId);

        private static ActorSystemState Tell(ActorSystemState state, TellMessage msg)
        {
            state.Tell(msg.ProcessId, msg.Sender, msg.Tag, msg.Message);
            return state;
        }

        private static Unit ShutdownAll(ActorSystemState state) =>
            state.Shutdown();

        private static ActorSystemState Register(ActorSystemState state, RegisterMessage msg) =>
            state.FindInStore(msg.ProcessId,
                Some: actorItem =>
                {
                    var proxy = new ActorProxy(
                                    state.Cluster,
                                    state.Registered,
                                    msg.Name,
                                    ActorProxyTemplate.Registered,
                                    () => new ActorProxyConfig(msg.ProcessId),
                                    ProcessFlags.Default
                                );

                    var inbox = new ActorInbox<ActorProxyConfig, object>();

                    return state.FindInStore(
                        state.Registered,
                        Some: _  => state.AddOrUpdateStoreAndStartup(proxy, inbox, actorItem.Flags),
                        None: () => state
                    );
                },
                None: () => state
            );

        private static ActorSystemState Deregister(ActorSystemState state, DeregisterMessage msg) =>
            state.ShutdownProcess(state.Registered.MakeChildId(msg.Name));
    }
}
