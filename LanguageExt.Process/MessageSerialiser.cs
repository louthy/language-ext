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
        public static RemoteMessageDTO SerialiseMsg(Message msg, ProcessId sender)
        {
            return new RemoteMessageDTO()
            {
                Type = (int)msg.MessageType,
                Tag = (int)msg.Tag,
                Child = null,
                Exception = null,
                RequestId = ActorContext.CurrentRequest == null ? -1 : ActorContext.CurrentRequest.RequestId,
                Sender = sender.ToString(),
                ReplyTo = sender.ToString(),
                Content = msg == null
                    ? null
                    : JsonConvert.SerializeObject(msg, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All })
            };
        }

        public static Message DeserialiseMsg(RemoteMessageDTO msg, ProcessId actorId)
        {
            var obj = JsonConvert.DeserializeObject(msg.Content);

            switch ((Message.Type)msg.Type)
            {
                case Message.Type.System:       return DeserialiseSystemMsg(msg);
                case Message.Type.User:         return DeserialiseUserMsg(msg, actorId);
                case Message.Type.UserControl:  return DeserialiseUserControlMsg(msg);
                case Message.Type.ActorSystem:  return DeserialiseActorSystemMsg(msg);
            }
            throw new Exception("Unknown Message Type: " + msg.Type);
        }

        public static UserControlMessage DeserialiseUserMsg(RemoteMessageDTO msg, ProcessId actorId)
        {
            var sender = String.IsNullOrEmpty(msg.Sender) ? ProcessId.NoSender : new ProcessId(msg.Sender);
            var replyTo = String.IsNullOrEmpty(msg.ReplyTo) ? ProcessId.NoSender : new ProcessId(msg.ReplyTo);

            var contentType = Type.GetType(msg.ContentType);
            if (contentType == null)
            {
                throw new Exception("Can't resolve type: " + msg.ContentType);
            }

            var content = JsonConvert.DeserializeObject(msg.Content, contentType);

            switch ((Message.TagSpec)msg.Tag)
            {
                case Message.TagSpec.UserAsk: return new ActorRequest(content, actorId, replyTo, msg.RequestId);
                case Message.TagSpec.User:    return new UserMessage(content, sender, replyTo);
            }

            throw new Exception("Unknown UserControlMessage Tag: " + msg.Tag);
        }

        public static ActorSystemMessage DeserialiseActorSystemMsg(RemoteMessageDTO msg)
        {
            switch ((Message.TagSpec)msg.Tag)
            {
                case Message.TagSpec.AddToStore:      throw new Exception("Can't deserialise AddToStore messages");
                case Message.TagSpec.GetChildren:     return JsonConvert.DeserializeObject<GetChildrenMessage>(msg.Content);
                case Message.TagSpec.Startup:         return JsonConvert.DeserializeObject<StartupMessage>(msg.Content);
                case Message.TagSpec.ShutdownProcess: return JsonConvert.DeserializeObject<ShutdownProcessMessage>(msg.Content);
                case Message.TagSpec.ObservePub:      return JsonConvert.DeserializeObject<ObservePubMessage>(msg.Content);
                case Message.TagSpec.ObserveState:    return JsonConvert.DeserializeObject<ObserveStateMessage>(msg.Content);
                case Message.TagSpec.Reply:           return JsonConvert.DeserializeObject<ReplyMessage>(msg.Content);
                case Message.TagSpec.Publish:         return JsonConvert.DeserializeObject<PubMessage>(msg.Content);
                case Message.TagSpec.Tell:            return JsonConvert.DeserializeObject<TellMessage>(msg.Content);
                case Message.TagSpec.ShutdownAll:     return JsonConvert.DeserializeObject<ShutdownAllMessage>(msg.Content);
            }

            throw new Exception("Unknown UserControlMessage Tag: " + msg.Tag);
        }

        public static UserControlMessage DeserialiseUserControlMsg(RemoteMessageDTO msg)
        {
            switch ((Message.TagSpec)msg.Tag)
            {
                case Message.TagSpec.Shutdown:
                    new UserControlShutdownMessage();
                    break;
            }
            throw new Exception("Unknown ActorSystemMessage Tag: " + msg.Tag);
        }

        public static SystemMessage DeserialiseSystemMsg(RemoteMessageDTO msg)
        {
            switch ((Message.TagSpec)msg.Tag)
            {
                case Message.TagSpec.ChildIsFaulted: return new SystemChildIsFaultedMessage(msg.Child, new Exception(msg.Exception));
                case Message.TagSpec.Restart:        return new SystemRestartMessage();
                case Message.TagSpec.LinkChild:      return new SystemLinkChildMessage(msg.Child);
                case Message.TagSpec.UnLinkChild:    return new SystemUnLinkChildMessage(msg.Child);
            }

            throw new Exception("Unknown SystemMessage Tag: " + msg.Tag);
        }
    }
}
