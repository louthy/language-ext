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
        static RemoteMessageDTO FixupPathsSystemPrefix(RemoteMessageDTO dto, SystemName system)
        {
            if (dto == null) return null;

            // Fix up the paths so we know what system they belong to.
            dto.ReplyTo = String.IsNullOrEmpty(dto.ReplyTo) || dto.ReplyTo.StartsWith("//") ? dto.ReplyTo : $"//{system}{dto.ReplyTo}";
            dto.Sender = String.IsNullOrEmpty(dto.Sender) || dto.Sender.StartsWith("//") ? dto.Sender : $"//{system}{dto.Sender}";
            dto.To = String.IsNullOrEmpty(dto.To) || dto.To.StartsWith("//") ? dto.To : $"//{system}{dto.To}";
            return dto;
        }

        public static Message DeserialiseMsg(RemoteMessageDTO msg, ProcessId actorId)
        {
            var sys = actorId.System;
            msg = FixupPathsSystemPrefix(msg, sys);

            var sender = String.IsNullOrEmpty(msg.Sender) ? ProcessId.NoSender : new ProcessId(msg.Sender);
            var replyTo = String.IsNullOrEmpty(msg.ReplyTo) ? ProcessId.NoSender : new ProcessId(msg.ReplyTo);

            switch ((Message.TagSpec)msg.Tag)
            {
                case Message.TagSpec.UserReply:
                    var content = DeserialiseMsgContent(msg);
                    return new ActorResponse(content, content.GetType().AssemblyQualifiedName, actorId, sender, msg.RequestId, msg.Exception == "RESPERR");

                case Message.TagSpec.UserAsk:           return new ActorRequest(DeserialiseMsgContent(msg), actorId, replyTo.SetSystem(sys), msg.RequestId);
                case Message.TagSpec.User:              return new UserMessage(DeserialiseMsgContent(msg), sender.SetSystem(sys), replyTo.SetSystem(sys));
                case Message.TagSpec.UserTerminated:    return ((TerminatedMessage)DeserialiseMsgContent(msg)).SetSystem(sys);

                case Message.TagSpec.GetChildren:       return UserControlMessage.GetChildren;
                case Message.TagSpec.StartupProcess:    return SystemMessage.StartupProcess;
                case Message.TagSpec.ShutdownProcess:   return (ShutdownProcessMessage)DeserialiseMsgContent(msg);

                case Message.TagSpec.Restart:           return SystemMessage.Restart;
                case Message.TagSpec.Pause:             return SystemMessage.Pause;
                case Message.TagSpec.Unpause:           return SystemMessage.Unpause;

                case Message.TagSpec.DispatchWatch:    return (SystemDispatchWatchMessage)DeserialiseMsgContent(msg);
                case Message.TagSpec.DispatchUnWatch:  return (SystemDispatchUnWatchMessage)DeserialiseMsgContent(msg);
                case Message.TagSpec.Watch:            return (SystemAddWatcherMessage)DeserialiseMsgContent(msg);
                case Message.TagSpec.UnWatch:          return (SystemRemoveWatcherMessage)DeserialiseMsgContent(msg);
            }

            throw new Exception($"Unknown Message Tag: {msg.Tag}");
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

                content = Deserialise.Object(msg.Content, contentType);
            }

            return content;
        }
    }
}
