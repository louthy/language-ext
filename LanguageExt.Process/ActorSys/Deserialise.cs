using Newtonsoft.Json;
using System;
using System.Reflection;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    /// <summary>
    /// Helper function for invoking the generic JsonConvert.DeserializeObject function
    /// instead of the variant that takes a Type argument.  This forces the type to be
    /// cast away from JObject and gives the caller the best chance of getting a useful
    /// value.
    /// </summary>
    internal static class Deserialise
    {
        static readonly Func<Type, MethodInfo> DeserialiseFunc =
           memo<Type, MethodInfo>(type =>
                typeof(JsonConvert).GetTypeInfo()
                                   .GetDeclaredMethods("DeserializeObject")
                                   .Filter(m => m.IsGenericMethod)
                                   .Filter(m => m.GetParameters().Length == 1)
                                   .Head()
                                   .MakeGenericMethod(type));

        public static object Object(string value, Type type) =>
            DeserialiseFunc(type).Invoke(null, new[] { value });
    }
}
