using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;
using static LanguageExt.Prelude;
using LanguageExt.Trans;
using Newtonsoft.Json;

namespace LanguageExt
{
    public static class TypeHelper
    {
        public static Either<string, bool> HasStateTypeOf(Type stateType, Type[] stateTypeInterfaces)
        {
            if (stateType == null) throw new ArgumentNullException(nameof(stateType));

            var stateTypeInfo = stateType.GetTypeInfo();

            var res = stateTypeInterfaces == null || stateTypeInterfaces.Length == 0
                ? Left<string, bool>("No public interfaces")
                : Right<string, bool>(stateTypeInterfaces.Filter(notnull).Map(t => t.GetTypeInfo()).Fold(false, (value, type) =>
                    value
                        ? true
                        : type.IsAssignableFrom(stateTypeInfo)));

            if(res.IsRight && res.Lift())
            {
                return res;
            }
            else
            {
                return Left<string, bool>("State-type (" + stateType.FullName + ") doesn't match inbox declared interfaces: " + String.Join(", ", stateTypeInterfaces.Map(x => x.FullName)));
            }
        }

        public static Either<string, bool> HasStateTypeOf(Type stateType, string[] stateTypeInterfaces) =>
            stateTypeInterfaces == null
                ? Left<string, bool>("State-type invalid (meta-data type is null)")
                : HasStateTypeOf(stateType, stateTypeInterfaces.Map(x => x.GetType()).ToArray());


        public static Either<string, bool> IsMessageValidForProcess(Type messageType, Type[] inboxDeclaredType)
        {
            if (messageType == null)
            {
                return Left<string, bool>("Message invalid (null)");
            }
            var messageTypeInfo = messageType.GetTypeInfo();

            if (typeof(TerminatedMessage).GetTypeInfo().IsAssignableFrom(messageTypeInfo) ||
                typeof(UserControlMessage).GetTypeInfo().IsAssignableFrom(messageTypeInfo) ||
                typeof(SystemMessage).GetTypeInfo().IsAssignableFrom(messageTypeInfo) ||
                typeof(RelayMsg).GetTypeInfo().IsAssignableFrom(messageTypeInfo))
            {
                return Right<string, bool>(true);
            }

            return inboxDeclaredType == null || inboxDeclaredType.Length == 0
                ? Left<string, bool>("No declared types")
                : Right<string, bool>(inboxDeclaredType.Filter(notnull).Map(t => t.GetTypeInfo()).Fold(false, (value, type) =>
                    value
                        ? true
                        : type.IsAssignableFrom(messageTypeInfo)));
        }

        public static Either<string, object> IsMessageValidForProcess(object message, Type[] inboxDeclaredType)
        {
            if (message == null)
            {
                return Left<string, object>("Message invalid (null)");
            }
            var result = IsMessageValidForProcess(message.GetType(), inboxDeclaredType);

            if( result.IsRight && result.Lift() )
            {
                return result.Map(_ => message);
            }
            else
            {
                if (message is string)
                {
                    foreach (var type in inboxDeclaredType)
                    {
                        var value = Deserialise.Object((string)message, type);
                        if( value != null)
                        {
                            return Right<string,object>(value);
                        }
                    }
                    return Left<string, object>("Message-type (" + message.GetType().FullName + ") is string, but couldn't convert it to any inbox supported type: " + String.Join(", ", inboxDeclaredType.Map(x => x.FullName)));
                }
                else
                {
                    return Left<string, object>("Message-type ("+message.GetType().FullName+") doesn't match inbox declared type: " + String.Join(", ", inboxDeclaredType.Map(x => x.FullName)));
                }
            }
        }

        public static Either<string, object> IsMessageValidForProcess(object message, string[] inboxDeclaredType) =>
            inboxDeclaredType == null
                ? Left<string, object>("Message-type invalid (meta-data type is null)")
                : IsMessageValidForProcess(message, inboxDeclaredType.Map(Type.GetType).ToArray());
    }
}
