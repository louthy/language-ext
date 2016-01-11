using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using System.Threading.Tasks;

#if !COREFX

namespace LanguageExt
{
    /// <summary>
    /// Builds a proxy from an interface.  It proxies a Process so that methods 
    /// on the state type can be invoked directly.  
    /// </summary>
    class ProxyBuilder
    {
        static AssemblyBuilder assemblyBuilder;
        static ModuleBuilder module;
        static readonly IDictionary<Type, Func<object>> cache = new Dictionary<Type, Func<object>>();

        static ProxyBuilder()
        {
            var name = "ProcessProxies";
            var assemblyName = new AssemblyName(name);
            assemblyBuilder = Thread.GetDomain().DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
            module = assemblyBuilder.DefineDynamicModule(name, name + ".dll");
        }

        public static T Build<T>(ProcessId processId)
        {
            ProcessAssert.HasStateTypeOf<T>(processId);

            var interf = typeof(T);
            if(!interf.IsInterface)
            {
                throw new ArgumentException("Process.proxy<T>(...) can only build a proxy from an interface type.");
            }

            if (!cache.ContainsKey(interf))
            {
                var generatedTypeName = $"{typeof(T).FullName}Proxy";

                // Define type
                var typeBuilder = module.DefineType(generatedTypeName, TypeAttributes.Public | TypeAttributes.Class);
                typeBuilder.AddInterfaceImplementation(typeof(T));

                // pid field
                var pidField = typeBuilder.DefineField("pid", typeof(ProcessId), FieldAttributes.Public);

                // Ctor
                var ctor = typeBuilder.DefineConstructor(
                    MethodAttributes.Public,
                    CallingConventions.Standard,
                    new Type[0]
                    );

                var il = ctor.GetILGenerator();
                il.Emit(OpCodes.Ret);

                // Methods
                var methods = interf.GetMethods();
                foreach (var method in methods)
                {
                    CreateMethod(method, typeBuilder, pidField);
                }

                cache.Add(interf, CreateDynamicConstructor(typeBuilder));
            }
            var res = (T)cache[interf]();

            res.GetType().GetField("pid").SetValue(res, processId);

            return res;
        }

        static Func<object> CreateDynamicConstructor(TypeBuilder typeBuilder)
        {
            var type = typeBuilder.CreateType();
            var dynamic = new DynamicMethod("CreateInstance",
                                            typeof(object),
                                            new Type[0],
                                            type);
            var il = dynamic.GetILGenerator();
            il.Emit(OpCodes.Newobj, type.GetConstructor(new Type[0]));
            il.Emit(OpCodes.Ret);
            return (Func<object>)dynamic.CreateDelegate(typeof(Func<object>));
        }

        static void CreateMethod(MethodInfo method, TypeBuilder typeBuilder, FieldBuilder pidField)
        {
            var attributes = method.Attributes;
            attributes &= ~MethodAttributes.Abstract;
            attributes |= MethodAttributes.Final;
            var parms = method.GetParameters();

            var methodBuilder = typeBuilder.DefineMethod(
                method.Name,
                attributes,
                CallingConventions.Standard,
                method.ReturnType,
                parms.Select(p => p.ParameterType).ToArray());

            var il = methodBuilder.GetILGenerator();

            var proxyMethod = typeof(ProxyMsg).GetTypeInfo().GetMethod(
                    (method.ReturnType.Name == "Void" ? "VoidInvoke" : "Invoke") + parms.Length
                );

            if (method.ReturnType.Name != "Void")
            {
                // Create a local slot for the return type
                il.DeclareLocal(method.ReturnType);
            }

            il.Emit(OpCodes.Ldarg_0);                   // this
            il.Emit(OpCodes.Ldfld, pidField);           // this.pid
            il.Emit(OpCodes.Ldstr, method.Name);        // method name
            if (method.ReturnType.Name != "Void")
            {
                // returnType.GetType()
                il.Emit(OpCodes.Ldtoken, method.ReturnType);    
                il.EmitCall(OpCodes.Call, typeof(System.Type).GetMethod("GetTypeFromHandle"), new[] { typeof(RuntimeTypeHandle) });
            }

            int i = 1;
            foreach (var p in parms)
            {
                // Stack the parameters
                il.Emit(OpCodes.Ldarg, i);
                if (p.ParameterType.IsValueType)
                {
                    il.Emit(OpCodes.Box, p.ParameterType);
                }
                i++;
            }

            // Call
            il.Emit(OpCodes.Call, proxyMethod);

            // Return
            if (method.ReturnType.Name == "Void")
            {
                il.Emit(OpCodes.Ret);
            }
            else
            {
                if (method.ReturnType.IsValueType)
                {
                    il.Emit(OpCodes.Unbox_Any, method.ReturnType);
                }
                else
                {
                    il.Emit(OpCodes.Castclass, method.ReturnType);
                }

                il.Emit(OpCodes.Stloc_0);
                var label = il.DefineLabel();
                il.Emit(OpCodes.Br_S, label);
                il.MarkLabel(label);
                il.Emit(OpCodes.Ldloc_0);
                il.Emit(OpCodes.Ret);
            }
        }
    }
    public class ProxyMsg
    {
        public string Method;
        public string[] Args;
        public string[] ArgTypes;
        public string ReturnType;

        public static void VoidInvoke(ProcessId pid, string method, params object[] args)
        {
            if (args.Filter(a => a == null).Any())
            {
                throw new ArgumentNullException();
            }

            Process.tell(pid, new ProxyMsg
            {
                Method = method,
                Args = args.Map(JsonConvert.SerializeObject).ToArray(),
                ArgTypes = args.Map(a=> a.GetType().GetTypeInfo().AssemblyQualifiedName).ToArray(),
                ReturnType = "System.Void"
            });
        }
        public static void VoidInvoke0(ProcessId pid, string method) =>
            VoidInvoke(pid, method);

        public static void VoidInvoke1(ProcessId pid, string method, object arg1) =>
            VoidInvoke(pid, method, arg1);

        public static void VoidInvoke2(ProcessId pid, string method, object arg1, object arg2) =>
            VoidInvoke(pid, method, arg1, arg2);

        public static void VoidInvoke3(ProcessId pid, string method, object arg1, object arg2, object arg3) =>
            VoidInvoke(pid, method, arg1, arg2, arg3);

        public static void VoidInvoke4(ProcessId pid, string method, object arg1, object arg2, object arg3, object arg4) =>
            VoidInvoke(pid, method, arg1, arg2, arg3, arg4);

        public static void VoidInvoke5(ProcessId pid, string method, object arg1, object arg2, object arg3, object arg4, object arg5) =>
            VoidInvoke(pid, method, arg1, arg2, arg3, arg4, arg5);

        public static void VoidInvoke6(ProcessId pid, string method, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6) =>
            VoidInvoke(pid, method, arg1, arg2, arg3, arg4, arg5, arg6);

        public static void VoidInvoke7(ProcessId pid, string method, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7) =>
            VoidInvoke(pid, method, arg1, arg2, arg3, arg4, arg5, arg6, arg7);

        public static void VoidInvoke8(ProcessId pid, string method, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8) =>
            VoidInvoke(pid, method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);

        public static void VoidInvoke9(ProcessId pid, string method, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9) =>
            VoidInvoke(pid, method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);

        public static void VoidInvoke10(ProcessId pid, string method, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10) =>
            VoidInvoke(pid, method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);

        public static void VoidInvoke11(ProcessId pid, string method, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11) =>
            VoidInvoke(pid, method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);

        public static void VoidInvoke12(ProcessId pid, string method, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12) =>
            VoidInvoke(pid, method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);

        public static object Invoke(ProcessId pid, string method, Type rettyp, params object[] args)
        {
            if (args.Filter(a => a == null).Any())
            {
                throw new ArgumentNullException();
            }

            return ActorContext.Ask(pid, new ProxyMsg
            {
                Method = method,
                Args = args.Map(JsonConvert.SerializeObject).ToArray(),
                ArgTypes = args.Map(a => a.GetType().GetTypeInfo().AssemblyQualifiedName).ToArray(),
                ReturnType = rettyp.GetTypeInfo().AssemblyQualifiedName
            });
        }

        public static object Invoke0(ProcessId pid, string method, Type rettyp) =>
            Invoke(pid, method, rettyp);

        public static object Invoke1(ProcessId pid, string method, Type rettyp, object arg1) =>
            Invoke(pid, method, rettyp, arg1);

        public static object Invoke2(ProcessId pid, string method, Type rettyp, object arg1, object arg2) =>
            Invoke(pid, method, rettyp, arg1, arg2);

        public static object Invoke3(ProcessId pid, string method, Type rettyp, object arg1, object arg2, object arg3) =>
            Invoke(pid, method, rettyp, arg1, arg2, arg3);

        public static object Invoke4(ProcessId pid, string method, Type rettyp, object arg1, object arg2, object arg3, object arg4) =>
            Invoke(pid, method, rettyp, arg1, arg2, arg3, arg4);

        public static object Invoke5(ProcessId pid, string method, Type rettyp, object arg1, object arg2, object arg3, object arg4, object arg5) =>
            Invoke(pid, method, rettyp, arg1, arg2, arg3, arg4, arg5);

        public static object Invoke6(ProcessId pid, string method, Type rettyp, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6) =>
            Invoke(pid, method, rettyp, arg1, arg2, arg3, arg4, arg5, arg6);

        public static object Invoke7(ProcessId pid, string method, Type rettyp, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7) =>
            Invoke(pid, method, rettyp, arg1, arg2, arg3, arg4, arg5, arg6, arg7);

        public static object Invoke8(ProcessId pid, string method, Type rettyp, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8) =>
            Invoke(pid, method, rettyp, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);

        public static object Invoke9(ProcessId pid, string method, Type rettyp, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9) =>
            Invoke(pid, method, rettyp, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);

        public static object Invoke10(ProcessId pid, string method, Type rettyp, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10) =>
            Invoke(pid, method, rettyp, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);

        public static object Invoke11(ProcessId pid, string method, Type rettyp, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11) =>
            Invoke(pid, method, rettyp, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);

        public static object Invoke12(ProcessId pid, string method, Type rettyp, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12) =>
            Invoke(pid, method, rettyp, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
    }
}

#endif