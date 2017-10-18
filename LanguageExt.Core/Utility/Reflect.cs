using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    class Reflect
    {
        public static IEnumerable<FieldInfo> GetPublicInstanceFields<A>(params Type[] excludeAttrs)
        {
            var excludeAttrsSet = toSet(excludeAttrs.Map(a => a.Name));
            var publicFields = typeof(A)
                .GetTypeInfo()
                .GetAllFields()
#if !COREFX13
                .OrderBy(f => f.MetadataToken)
#endif
                .Where(f =>
                {
                    if (!f.IsPublic || f.IsStatic) return false;
                    if (toSet(f.CustomAttributes.Map(a => a.AttributeType.Name)).Intersect(excludeAttrsSet).Any()) return false;
                    return true;
                });

            var publicPropNames = typeof(A)
                                    .GetTypeInfo()
                                    .GetAllProperties()
#if !COREFX13
                                    .OrderBy(p => p.MetadataToken)
#endif
                                    .Where(p => p.CanRead && p.GetMethod.IsPublic && !p.GetAccessors(true)[0].IsStatic)
                                    .Where(p => !toSet(p.CustomAttributes.Map(a => a.AttributeType.Name)).Intersect(excludeAttrsSet).Any())
                                    .ToArray();

            var backingFields = typeof(A)
                                    .GetTypeInfo()
                                    .GetAllFields()
#if !COREFX13
                                    .OrderBy(p => p.MetadataToken)
#endif
                                    .Where(f => f.IsPrivate &&
                                                publicPropNames.Exists(p => f.Name.StartsWith($"<{p.Name}>")))
                                    .ToArray();

            return Enumerable.Concat(publicFields, backingFields);
        }

        public static Option<MethodInfo> GetPublicStaticMethod(Type type, string name, Type argA) =>
            type.GetTypeInfo()
                .DeclaredMethods
                .Where(x =>
                {
                    if (!x.IsStatic) return false;
                    if (x.Name != name) return false;
                    var ps = x.GetParameters();
                    if (ps.Length != 1) return false;
                    if (ps[0].ParameterType != argA) return false;
                    return true;
                })
                .FirstOrDefault();

        public static Option<MethodInfo> GetPublicStaticMethod<TYPE>(string name, Type argA) =>
            typeof(TYPE)
                .GetTypeInfo()
                .DeclaredMethods
                .Where(x =>
                {
                    if (!x.IsStatic) return false;
                    if (x.Name != name) return false;
                    var ps = x.GetParameters();
                    if (ps.Length != 1) return false;
                    if (ps[0].ParameterType != argA) return false;
                    return true;
                })
                .FirstOrDefault();

        public static Option<MethodInfo> GetPublicStaticMethod<TYPE, A>(string name) =>
            typeof(TYPE)
                .GetTypeInfo()
                .DeclaredMethods
                .Where(x =>
                {
                    if (!x.IsStatic) return false;
                    if (x.Name != name) return false;
                    var ps = x.GetParameters();
                    if (ps.Length != 1) return false;
                    if (ps[0].ParameterType != typeof(A)) return false;
                    return true;
                })
                .FirstOrDefault();

        public static Option<MethodInfo> GetPublicStaticMethod<TYPE, A, B>(string name) =>
            typeof(TYPE)
                .GetTypeInfo()
                .DeclaredMethods
                .Where(x =>
                {
                    if (!x.IsStatic) return false;
                    if (x.Name != name) return false;
                    var ps = x.GetParameters();
                    if (ps.Length != 2) return false;
                    if (ps[0].ParameterType != typeof(A)) return false;
                    if (ps[1].ParameterType != typeof(B)) return false;
                    return true;
                })
                .FirstOrDefault();

        public static Option<MethodInfo> GetPublicStaticMethod<TYPE>(string name, Type argA, Type argB) =>
            typeof(TYPE)
                .GetTypeInfo()
                .DeclaredMethods
                .Where(x =>
                {
                    if (!x.IsStatic) return false;
                    if (x.Name != name) return false;
                    var ps = x.GetParameters();
                    if (ps.Length != 2) return false;
                    if (ps[0].ParameterType != argA) return false;
                    if (ps[1].ParameterType != argB) return false;
                    return true;
                })
                .FirstOrDefault();

        public static Option<MethodInfo> GetPublicInstanceMethod<TYPE>(string name) =>
            typeof(TYPE)
                .GetTypeInfo()
                .GetAllMethods()
                .Where(x =>
                {
                    if (x.IsStatic) return false;
                    if (x.Name != name) return false;
                    if (x.GetParameters().Length != 0) return false;
                    return true;
                })
                .FirstOrDefault();

        public static Option<MethodInfo> GetPublicInstanceMethod<TYPE, A>(string name) =>
            typeof(TYPE)
                .GetTypeInfo()
                .GetAllMethods()
                .Where(x =>
                {
                    if (x.IsStatic) return false;
                    if (x.Name != name) return false;
                    var ps = x.GetParameters();
                    if (ps.Length != 1) return false;
                    if (ps[0].ParameterType != typeof(A)) return false;
                    return true;
                })
                .FirstOrDefault();


        public static Option<MethodInfo> GetPublicInstanceMethod<TYPE, A, B>(string name) =>
            typeof(TYPE)
                .GetTypeInfo()
                .GetAllMethods()
                .Where(x =>
                {
                    if (x.IsStatic) return false;
                    if (x.Name != name) return false;
                    var ps = x.GetParameters();
                    if (ps.Length != 2) return false;
                    if (ps[0].ParameterType != typeof(A)) return false;
                    if (ps[1].ParameterType != typeof(B)) return false;
                    return true;
                })
                .FirstOrDefault();

        public static Option<MethodInfo> GetPublicInstanceMethod(Type type, string name) =>
            type.GetTypeInfo()
                .GetAllMethods()
                .Where(x =>
                {
                    if (x.IsStatic) return false;
                    if (x.Name != name) return false;
                    if (x.GetParameters().Length != 0) return false;
                    return true;
                })
                .FirstOrDefault();

        public static Option<MethodInfo> GetPublicInstanceMethod<TYPE>(string name, Type arg1, Type arg2) =>
            typeof(TYPE)
                .GetTypeInfo()
                .GetAllMethods()
                .Where(x =>
                {
                    if (x.IsStatic) return false;
                    if (x.Name != name) return false;
                    var ps = x.GetParameters();
                    if (ps.Length != 2) return false;
                    if (ps[0].ParameterType != arg1) return false;
                    if (ps[1].ParameterType != arg2) return false;
                    return true;
                })
                .FirstOrDefault();

        public static Option<ConstructorInfo> GetConstructor<TYPE>() =>
            typeof(TYPE)
                .GetTypeInfo()
                .DeclaredConstructors
                .Where(x =>
                {
                    if (x.IsStatic) return false;
                    if (x.GetParameters().Length != 0) return false;
                    return true;
                })
                .FirstOrDefault();

        public static Option<ConstructorInfo> GetConstructor<TYPE, A>() =>
            typeof(TYPE)
                .GetTypeInfo()
                .DeclaredConstructors
                .Where(x =>
                {
                    if (x.IsStatic) return false;
                    var ps = x.GetParameters();
                    if (ps.Length != 1) return false;
                    if (ps[0].ParameterType != typeof(A)) return false;
                    return true;
                })
                .FirstOrDefault();

        public static Option<ConstructorInfo> GetConstructor<TYPE, A, B>() =>
            typeof(TYPE)
                .GetTypeInfo()
                .DeclaredConstructors
                .Where(x =>
                {
                    if (x.IsStatic) return false;
                    var ps = x.GetParameters();
                    if (ps.Length != 2) return false;
                    if (ps[0].ParameterType != typeof(A)) return false;
                    if (ps[1].ParameterType != typeof(B)) return false;
                    return true;
                })
                .FirstOrDefault();

        public static Option<ConstructorInfo> GetConstructor<TYPE, A, B, C>() =>
            typeof(TYPE)
                .GetTypeInfo()
                .DeclaredConstructors
                .Where(x =>
                {
                    if (x.IsStatic) return false;
                    var ps = x.GetParameters();
                    if (ps.Length != 3) return false;
                    if (ps[0].ParameterType != typeof(A)) return false;
                    if (ps[1].ParameterType != typeof(B)) return false;
                    if (ps[2].ParameterType != typeof(C)) return false;
                    return true;
                })
                .FirstOrDefault();

        public static Option<ConstructorInfo> GetConstructor<TYPE, A, B, C, D>() =>
            typeof(TYPE)
                .GetTypeInfo()
                .DeclaredConstructors
                .Where(x =>
                {
                    if (x.IsStatic) return false;
                    var ps = x.GetParameters();
                    if (ps.Length != 4) return false;
                    if (ps[0].ParameterType != typeof(A)) return false;
                    if (ps[1].ParameterType != typeof(B)) return false;
                    if (ps[2].ParameterType != typeof(C)) return false;
                    if (ps[3].ParameterType != typeof(D)) return false;
                    return true;
                })
                .FirstOrDefault();

    }
}
