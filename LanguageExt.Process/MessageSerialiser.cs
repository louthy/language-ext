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
                case Message.TagSpec.UserAsk: return new ActorRequest(DeserialiseMsgContent(msg), actorId, replyTo, msg.RequestId);
                case Message.TagSpec.User: return new UserMessage(DeserialiseMsgContent(msg), sender, replyTo);

                case Message.TagSpec.AddToStore: throw new Exception("Can't deserialise AddToStore messages");
                case Message.TagSpec.GetChildren: return JsonConvert.DeserializeObject<GetChildrenMessage>(msg.Content);
                case Message.TagSpec.Startup: return JsonConvert.DeserializeObject<StartupMessage>(msg.Content);
                case Message.TagSpec.ShutdownProcess: return JsonConvert.DeserializeObject<ShutdownProcessMessage>(msg.Content);
                case Message.TagSpec.ObservePub: return JsonConvert.DeserializeObject<ObservePubMessage>(msg.Content);
                case Message.TagSpec.ObserveState: return JsonConvert.DeserializeObject<ObserveStateMessage>(msg.Content);
                case Message.TagSpec.Reply: return JsonConvert.DeserializeObject<ReplyMessage>(msg.Content);
                case Message.TagSpec.Publish: return JsonConvert.DeserializeObject<PubMessage>(msg.Content);
                case Message.TagSpec.Tell: return JsonConvert.DeserializeObject<TellMessage>(msg.Content);

                case Message.TagSpec.Shutdown: return new UserControlShutdownMessage();

                case Message.TagSpec.ShutdownAll: return JsonConvert.DeserializeObject<ShutdownAllMessage>(msg.Content);
                case Message.TagSpec.ChildIsFaulted: return new SystemChildIsFaultedMessage(msg.Child, new Exception(msg.Exception));
                case Message.TagSpec.Restart: return new SystemRestartMessage();
                case Message.TagSpec.LinkChild: return new SystemLinkChildMessage(msg.Child);
                case Message.TagSpec.UnLinkChild: return new SystemUnLinkChildMessage(msg.Child);
            }

            throw new Exception("Unknown Message Type: " + msg.Type);
        }

        private static object DeserialiseMsgContent(RemoteMessageDTO msg)
        {
            object content = null;

            if (msg.Content == null)
            {
                throw new Exception("Message content is null from " + msg.Sender);
            }
            else
            {
                var contentType = Type.GetType(msg.ContentType);
                if (contentType == null)
                {
                    throw new Exception("Can't resolve type: " + msg.ContentType);
                }

                content = JsonConvert.DeserializeObject(msg.Content, contentType);
            }

            return content;
        }
    }
}
