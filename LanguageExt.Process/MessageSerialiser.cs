using LanguageExt.UnitsOfMeasure;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    internal static class MessageSerialiser
    {
        public static Message DeserialiseMsg(RemoteMessageDTO msg, ProcessId actorId)
        {
            var sender = String.IsNullOrEmpty(msg.Sender) ? ProcessId.NoSender : new ProcessId(msg.Sender);
            var replyTo = String.IsNullOrEmpty(msg.ReplyTo) ? ProcessId.NoSender : new ProcessId(msg.ReplyTo);

            switch ((Message.TagSpec)msg.Tag)
            {
                case Message.TagSpec.UserReply:
                    var content = DeserialiseMsgContent(msg);
                    return new ActorResponse(content, content.GetType().AssemblyQualifiedName, actorId, sender, msg.RequestId, msg.Exception == "RESPERR");

                case Message.TagSpec.UserAsk:           return new ActorRequest(DeserialiseMsgContent(msg), actorId, replyTo, msg.RequestId);
                case Message.TagSpec.User:              return new UserMessage(DeserialiseMsgContent(msg), sender, replyTo);
                case Message.TagSpec.UserTerminated:    return (TerminatedMessage)DeserialiseMsgContent(msg);

                case Message.TagSpec.GetChildren:       return UserControlMessage.GetChildren;
                case Message.TagSpec.StartupProcess:    return SystemMessage.StartupProcess;
                case Message.TagSpec.ShutdownProcess:   return (ShutdownProcessMessage)DeserialiseMsgContent(msg);

                case Message.TagSpec.Restart:           return SystemMessage.Restart;

                case Message.TagSpec.DispatchWatch:    return (SystemDispatchWatchMessage)DeserialiseMsgContent(msg);
                case Message.TagSpec.DispatchUnWatch:  return (SystemDispatchUnWatchMessage)DeserialiseMsgContent(msg);
                case Message.TagSpec.Watch:            return (SystemAddWatcherMessage)DeserialiseMsgContent(msg);
                case Message.TagSpec.UnWatch:          return (SystemRemoveWatcherMessage)DeserialiseMsgContent(msg);
            }

            throw new Exception($"Unknown Message Type: {msg.Type}");
        }

        private static object DeserialiseMsgContent(RemoteMessageDTO msg)
        {
            object content = null;

            if (msg.Content == null)
            {
                throw new Exception($"Message content is null from {msg.Sender}");
            }
            else
            {
                var contentType = Type.GetType(msg.ContentType);
                if (contentType == null)
                {
                    throw new Exception($"Can't resolve type: {msg.ContentType}");
                }

                content = JsonConvert.DeserializeObject(msg.Content, contentType);
            }

            return content;
        }
    }
}
