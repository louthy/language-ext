using System;

namespace LanguageExt
{
    internal interface ILocalActorInbox : IDisposable
    {
        Unit Ask(object message, ProcessId sender);
        Unit Tell(object message, ProcessId sender);
        Unit TellUserControl(UserControlMessage message);
        Unit TellSystem(SystemMessage message);
        object ValidateMessageType(object message, ProcessId sender);
        Either<string, bool> CanAcceptMessageType<TMsg>();
        Either<string, bool> HasStateTypeOf<TState>();
        int Count { get; }
    }
}
