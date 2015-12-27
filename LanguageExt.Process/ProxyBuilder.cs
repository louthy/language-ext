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

        public static void VoidInvoke0(ProcessId pid, string method)
        {
            Process.tell(pid, new ProxyMsg
            {
                Method = method,
                Args = new string[0],
                ArgTypes = new string[0],
                ReturnType = "System.Void"
            });
        }
        public static void VoidInvoke1(ProcessId pid, string method, object arg1)
        {
            Process.tell(pid, new ProxyMsg
            {
                Method = method,
                Args = new string[] { JsonConvert.SerializeObject(arg1) },
                ArgTypes = new string[] { arg1.GetType().GetTypeInfo().FullName },
                ReturnType = "System.Void"
            });
        }
        public static void VoidInvoke2(ProcessId pid, string method, object arg1, object arg2)
        {
            Process.tell(pid, new ProxyMsg
            {
                Method = method,
                Args = new string[] {
                    JsonConvert.SerializeObject(arg1),
                    JsonConvert.SerializeObject(arg2)
                },
                ArgTypes = new string[] {
                    arg1.GetType().GetTypeInfo().FullName,
                    arg2.GetType().GetTypeInfo().FullName
                },
                ReturnType = "System.Void"
            });
        }
        public static void VoidInvoke3(ProcessId pid, string method, object arg1, object arg2, object arg3)
        {
            Process.tell(pid, new ProxyMsg
            {
                Method = method,
                Args = new string[] {
                    JsonConvert.SerializeObject(arg1),
                    JsonConvert.SerializeObject(arg2),
                    JsonConvert.SerializeObject(arg3)
                },
                ArgTypes = new string[] {
                    arg1.GetType().GetTypeInfo().FullName,
                    arg2.GetType().GetTypeInfo().FullName,
                    arg3.GetType().GetTypeInfo().FullName
                },
                ReturnType = "System.Void"
            });
        }
        public static void VoidInvoke4(ProcessId pid, string method, object arg1, object arg2, object arg3, object arg4)
        {
            Process.tell(pid, new ProxyMsg
            {
                Method = method,
                Args = new string[] {
                    JsonConvert.SerializeObject(arg1),
                    JsonConvert.SerializeObject(arg2),
                    JsonConvert.SerializeObject(arg3),
                    JsonConvert.SerializeObject(arg4)
                },
                ArgTypes = new string[] {
                    arg1.GetType().GetTypeInfo().FullName,
                    arg2.GetType().GetTypeInfo().FullName,
                    arg3.GetType().GetTypeInfo().FullName,
                    arg4.GetType().GetTypeInfo().FullName
                },
                ReturnType = "System.Void"
            });
        }
        public static void VoidInvoke5(ProcessId pid, string method, object arg1, object arg2, object arg3, object arg4, object arg5)
        {
            Process.tell(pid, new ProxyMsg
            {
                Method = method,
                Args = new string[] {
                    JsonConvert.SerializeObject(arg1),
                    JsonConvert.SerializeObject(arg2),
                    JsonConvert.SerializeObject(arg3),
                    JsonConvert.SerializeObject(arg4),
                    JsonConvert.SerializeObject(arg5)
                },
                ArgTypes = new string[] {
                    arg1.GetType().GetTypeInfo().FullName,
                    arg2.GetType().GetTypeInfo().FullName,
                    arg3.GetType().GetTypeInfo().FullName,
                    arg4.GetType().GetTypeInfo().FullName,
                    arg5.GetType().GetTypeInfo().FullName
                },
                ReturnType = "System.Void"
            });
        }
        public static void VoidInvoke6(ProcessId pid, string method, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6)
        {
            Process.tell(pid, new ProxyMsg
            {
                Method = method,
                Args = new string[] {
                    JsonConvert.SerializeObject(arg1),
                    JsonConvert.SerializeObject(arg2),
                    JsonConvert.SerializeObject(arg3),
                    JsonConvert.SerializeObject(arg4),
                    JsonConvert.SerializeObject(arg5),
                    JsonConvert.SerializeObject(arg6)
                },
                ArgTypes = new string[] {
                    arg1.GetType().GetTypeInfo().FullName,
                    arg2.GetType().GetTypeInfo().FullName,
                    arg3.GetType().GetTypeInfo().FullName,
                    arg4.GetType().GetTypeInfo().FullName,
                    arg5.GetType().GetTypeInfo().FullName,
                    arg6.GetType().GetTypeInfo().FullName
                },
                ReturnType = "System.Void"
            });
        }
        public static void VoidInvoke7(ProcessId pid, string method, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7)
        {
            Process.tell(pid, new ProxyMsg
            {
                Method = method,
                Args = new string[] {
                    JsonConvert.SerializeObject(arg1),
                    JsonConvert.SerializeObject(arg2),
                    JsonConvert.SerializeObject(arg3),
                    JsonConvert.SerializeObject(arg4),
                    JsonConvert.SerializeObject(arg5),
                    JsonConvert.SerializeObject(arg6),
                    JsonConvert.SerializeObject(arg7)
                },
                ArgTypes = new string[] {
                    arg1.GetType().GetTypeInfo().FullName,
                    arg2.GetType().GetTypeInfo().FullName,
                    arg3.GetType().GetTypeInfo().FullName,
                    arg4.GetType().GetTypeInfo().FullName,
                    arg5.GetType().GetTypeInfo().FullName,
                    arg6.GetType().GetTypeInfo().FullName,
                    arg7.GetType().GetTypeInfo().FullName
                },
                ReturnType = "System.Void"
            });
        }
        public static void VoidInvoke8(ProcessId pid, string method, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8)
        {
            Process.tell(pid, new ProxyMsg
            {
                Method = method,
                Args = new string[] {
                    JsonConvert.SerializeObject(arg1),
                    JsonConvert.SerializeObject(arg2),
                    JsonConvert.SerializeObject(arg3),
                    JsonConvert.SerializeObject(arg4),
                    JsonConvert.SerializeObject(arg5),
                    JsonConvert.SerializeObject(arg6),
                    JsonConvert.SerializeObject(arg7),
                    JsonConvert.SerializeObject(arg8)
                },
                ArgTypes = new string[] {
                    arg1.GetType().GetTypeInfo().FullName,
                    arg2.GetType().GetTypeInfo().FullName,
                    arg3.GetType().GetTypeInfo().FullName,
                    arg4.GetType().GetTypeInfo().FullName,
                    arg5.GetType().GetTypeInfo().FullName,
                    arg6.GetType().GetTypeInfo().FullName,
                    arg7.GetType().GetTypeInfo().FullName,
                    arg8.GetType().GetTypeInfo().FullName
                },
                ReturnType = "System.Void"
            });
        }
        public static void VoidInvoke9(ProcessId pid, string method, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9)
        {
            Process.tell(pid, new ProxyMsg
            {
                Method = method,
                Args = new string[] {
                    JsonConvert.SerializeObject(arg1),
                    JsonConvert.SerializeObject(arg2),
                    JsonConvert.SerializeObject(arg3),
                    JsonConvert.SerializeObject(arg4),
                    JsonConvert.SerializeObject(arg5),
                    JsonConvert.SerializeObject(arg6),
                    JsonConvert.SerializeObject(arg7),
                    JsonConvert.SerializeObject(arg8),
                    JsonConvert.SerializeObject(arg9)
                },
                ArgTypes = new string[] {
                    arg1.GetType().GetTypeInfo().FullName,
                    arg2.GetType().GetTypeInfo().FullName,
                    arg3.GetType().GetTypeInfo().FullName,
                    arg4.GetType().GetTypeInfo().FullName,
                    arg5.GetType().GetTypeInfo().FullName,
                    arg6.GetType().GetTypeInfo().FullName,
                    arg7.GetType().GetTypeInfo().FullName,
                    arg8.GetType().GetTypeInfo().FullName,
                    arg9.GetType().GetTypeInfo().FullName
                },
                ReturnType = "System.Void"
            });
        }
        public static void VoidInvoke10(ProcessId pid, string method, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10)
        {
            Process.tell(pid, new ProxyMsg
            {
                Method = method,
                Args = new string[] {
                    JsonConvert.SerializeObject(arg1),
                    JsonConvert.SerializeObject(arg2),
                    JsonConvert.SerializeObject(arg3),
                    JsonConvert.SerializeObject(arg4),
                    JsonConvert.SerializeObject(arg5),
                    JsonConvert.SerializeObject(arg6),
                    JsonConvert.SerializeObject(arg7),
                    JsonConvert.SerializeObject(arg8),
                    JsonConvert.SerializeObject(arg9),
                    JsonConvert.SerializeObject(arg10)
                },
                ArgTypes = new string[] {
                    arg1.GetType().GetTypeInfo().FullName,
                    arg2.GetType().GetTypeInfo().FullName,
                    arg3.GetType().GetTypeInfo().FullName,
                    arg4.GetType().GetTypeInfo().FullName,
                    arg5.GetType().GetTypeInfo().FullName,
                    arg6.GetType().GetTypeInfo().FullName,
                    arg7.GetType().GetTypeInfo().FullName,
                    arg8.GetType().GetTypeInfo().FullName,
                    arg9.GetType().GetTypeInfo().FullName,
                    arg10.GetType().GetTypeInfo().FullName
                },
                ReturnType = "System.Void"
            });
        }
        public static void VoidInvoke11(ProcessId pid, string method, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11)
        {
            Process.tell(pid, new ProxyMsg
            {
                Method = method,
                Args = new string[] {
                    JsonConvert.SerializeObject(arg1),
                    JsonConvert.SerializeObject(arg2),
                    JsonConvert.SerializeObject(arg3),
                    JsonConvert.SerializeObject(arg4),
                    JsonConvert.SerializeObject(arg5),
                    JsonConvert.SerializeObject(arg6),
                    JsonConvert.SerializeObject(arg7),
                    JsonConvert.SerializeObject(arg8),
                    JsonConvert.SerializeObject(arg9),
                    JsonConvert.SerializeObject(arg10),
                    JsonConvert.SerializeObject(arg11)
                },
                ArgTypes = new string[] {
                    arg1.GetType().GetTypeInfo().FullName,
                    arg2.GetType().GetTypeInfo().FullName,
                    arg3.GetType().GetTypeInfo().FullName,
                    arg4.GetType().GetTypeInfo().FullName,
                    arg5.GetType().GetTypeInfo().FullName,
                    arg6.GetType().GetTypeInfo().FullName,
                    arg7.GetType().GetTypeInfo().FullName,
                    arg8.GetType().GetTypeInfo().FullName,
                    arg9.GetType().GetTypeInfo().FullName,
                    arg10.GetType().GetTypeInfo().FullName,
                    arg11.GetType().GetTypeInfo().FullName
                },
                ReturnType = "System.Void"
            });
        }
        public static void VoidInvoke12(ProcessId pid, string method, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12)
        {
            Process.tell(pid, new ProxyMsg
            {
                Method = method,
                Args = new string[] {
                    JsonConvert.SerializeObject(arg1),
                    JsonConvert.SerializeObject(arg2),
                    JsonConvert.SerializeObject(arg3),
                    JsonConvert.SerializeObject(arg4),
                    JsonConvert.SerializeObject(arg5),
                    JsonConvert.SerializeObject(arg6),
                    JsonConvert.SerializeObject(arg7),
                    JsonConvert.SerializeObject(arg8),
                    JsonConvert.SerializeObject(arg9),
                    JsonConvert.SerializeObject(arg10),
                    JsonConvert.SerializeObject(arg11),
                    JsonConvert.SerializeObject(arg12)
                },
                ArgTypes = new string[] {
                    arg1.GetType().GetTypeInfo().FullName,
                    arg2.GetType().GetTypeInfo().FullName,
                    arg3.GetType().GetTypeInfo().FullName,
                    arg4.GetType().GetTypeInfo().FullName,
                    arg5.GetType().GetTypeInfo().FullName,
                    arg6.GetType().GetTypeInfo().FullName,
                    arg7.GetType().GetTypeInfo().FullName,
                    arg8.GetType().GetTypeInfo().FullName,
                    arg9.GetType().GetTypeInfo().FullName,
                    arg10.GetType().GetTypeInfo().FullName,
                    arg11.GetType().GetTypeInfo().FullName,
                    arg12.GetType().GetTypeInfo().FullName
                },
                ReturnType = "System.Void"
            });
        }

        public static object Invoke0(ProcessId pid, string method, Type rettyp)
        {
            return ActorContext.Ask(pid, new ProxyMsg
            {
                Method = method,
                Args = new string[0],
                ArgTypes = new string[0],
                ReturnType = rettyp.GetTypeInfo().FullName
            });
        }
        public static object Invoke1(ProcessId pid, string method, Type rettyp, object arg1)
        {
            return ActorContext.Ask(pid, new ProxyMsg
            {
                Method = method,
                Args = new string[] {
                    JsonConvert.SerializeObject(arg1)
                },
                ArgTypes = new string[] {
                    arg1.GetType().GetTypeInfo().FullName
                },
                ReturnType = rettyp.GetTypeInfo().FullName
            });
        }
        public static object Invoke2(ProcessId pid, string method, Type rettyp, object arg1, object arg2)
        {
            return ActorContext.Ask(pid, new ProxyMsg
            {
                Method = method,
                Args = new string[] {
                    JsonConvert.SerializeObject(arg1),
                    JsonConvert.SerializeObject(arg2)
                },
                ArgTypes = new string[] {
                    arg1.GetType().GetTypeInfo().FullName,
                    arg2.GetType().GetTypeInfo().FullName
                },
                ReturnType = rettyp.GetTypeInfo().FullName
            });
        }
        public static object Invoke3(ProcessId pid, string method, Type rettyp, object arg1, object arg2, object arg3)
        {
            return ActorContext.Ask(pid, new ProxyMsg
            {
                Method = method,
                Args = new string[] {
                    JsonConvert.SerializeObject(arg1),
                    JsonConvert.SerializeObject(arg2),
                    JsonConvert.SerializeObject(arg3)
                },
                ArgTypes = new string[] {
                    arg1.GetType().GetTypeInfo().FullName,
                    arg2.GetType().GetTypeInfo().FullName,
                    arg3.GetType().GetTypeInfo().FullName
                },
                ReturnType = rettyp.GetTypeInfo().FullName
            });
        }
        public static object Invoke4(ProcessId pid, string method, Type rettyp, object arg1, object arg2, object arg3, object arg4)
        {
            return ActorContext.Ask(pid, new ProxyMsg
            {
                Method = method,
                Args = new string[] {
                    JsonConvert.SerializeObject(arg1),
                    JsonConvert.SerializeObject(arg2),
                    JsonConvert.SerializeObject(arg3),
                    JsonConvert.SerializeObject(arg4),
                },
                ArgTypes = new string[] {
                    arg1.GetType().GetTypeInfo().FullName,
                    arg2.GetType().GetTypeInfo().FullName,
                    arg3.GetType().GetTypeInfo().FullName,
                    arg4.GetType().GetTypeInfo().FullName
                },
                ReturnType = rettyp.GetTypeInfo().FullName
            });
        }
        public static object Invoke5(ProcessId pid, string method, Type rettyp, object arg1, object arg2, object arg3, object arg4, object arg5)
        {
            return ActorContext.Ask(pid, new ProxyMsg
            {
                Method = method,
                Args = new string[] {
                    JsonConvert.SerializeObject(arg1),
                    JsonConvert.SerializeObject(arg2),
                    JsonConvert.SerializeObject(arg3),
                    JsonConvert.SerializeObject(arg4),
                    JsonConvert.SerializeObject(arg5)
                },
                ArgTypes = new string[] {
                    arg1.GetType().GetTypeInfo().FullName,
                    arg2.GetType().GetTypeInfo().FullName,
                    arg3.GetType().GetTypeInfo().FullName,
                    arg4.GetType().GetTypeInfo().FullName,
                    arg5.GetType().GetTypeInfo().FullName
                },
                ReturnType = rettyp.GetTypeInfo().FullName
            });
        }
        public static object Invoke6(ProcessId pid, string method, Type rettyp, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6)
        {
            return ActorContext.Ask(pid, new ProxyMsg
            {
                Method = method,
                Args = new string[] {
                    JsonConvert.SerializeObject(arg1),
                    JsonConvert.SerializeObject(arg2),
                    JsonConvert.SerializeObject(arg3),
                    JsonConvert.SerializeObject(arg4),
                    JsonConvert.SerializeObject(arg5),
                    JsonConvert.SerializeObject(arg6)
                },
                ArgTypes = new string[] {
                    arg1.GetType().GetTypeInfo().FullName,
                    arg2.GetType().GetTypeInfo().FullName,
                    arg3.GetType().GetTypeInfo().FullName,
                    arg4.GetType().GetTypeInfo().FullName,
                    arg5.GetType().GetTypeInfo().FullName,
                    arg6.GetType().GetTypeInfo().FullName
                },
                ReturnType = rettyp.GetTypeInfo().FullName
            });
        }
        public static object Invoke7(ProcessId pid, string method, Type rettyp, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7)
        {
            return ActorContext.Ask(pid, new ProxyMsg
            {
                Method = method,
                Args = new string[] {
                    JsonConvert.SerializeObject(arg1),
                    JsonConvert.SerializeObject(arg2),
                    JsonConvert.SerializeObject(arg3),
                    JsonConvert.SerializeObject(arg4),
                    JsonConvert.SerializeObject(arg5),
                    JsonConvert.SerializeObject(arg6),
                    JsonConvert.SerializeObject(arg7)
                },
                ArgTypes = new string[] {
                    arg1.GetType().GetTypeInfo().FullName,
                    arg2.GetType().GetTypeInfo().FullName,
                    arg3.GetType().GetTypeInfo().FullName,
                    arg4.GetType().GetTypeInfo().FullName,
                    arg5.GetType().GetTypeInfo().FullName,
                    arg6.GetType().GetTypeInfo().FullName,
                    arg7.GetType().GetTypeInfo().FullName
                },
                ReturnType = rettyp.GetTypeInfo().FullName
            });
        }
        public static object Invoke8(ProcessId pid, string method, Type rettyp, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8)
        {
            return ActorContext.Ask(pid, new ProxyMsg
            {
                Method = method,
                Args = new string[] {
                    JsonConvert.SerializeObject(arg1),
                    JsonConvert.SerializeObject(arg2),
                    JsonConvert.SerializeObject(arg3),
                    JsonConvert.SerializeObject(arg4),
                    JsonConvert.SerializeObject(arg5),
                    JsonConvert.SerializeObject(arg6),
                    JsonConvert.SerializeObject(arg7),
                    JsonConvert.SerializeObject(arg8)
                },
                ArgTypes = new string[] {
                    arg1.GetType().GetTypeInfo().FullName,
                    arg2.GetType().GetTypeInfo().FullName,
                    arg3.GetType().GetTypeInfo().FullName,
                    arg4.GetType().GetTypeInfo().FullName,
                    arg5.GetType().GetTypeInfo().FullName,
                    arg6.GetType().GetTypeInfo().FullName,
                    arg7.GetType().GetTypeInfo().FullName,
                    arg8.GetType().GetTypeInfo().FullName
                },
                ReturnType = rettyp.GetTypeInfo().FullName
            });
        }
        public static object Invoke9(ProcessId pid, string method, Type rettyp, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9)
        {
            return ActorContext.Ask(pid, new ProxyMsg
            {
                Method = method,
                Args = new string[] {
                    JsonConvert.SerializeObject(arg1),
                    JsonConvert.SerializeObject(arg2),
                    JsonConvert.SerializeObject(arg3),
                    JsonConvert.SerializeObject(arg4),
                    JsonConvert.SerializeObject(arg5),
                    JsonConvert.SerializeObject(arg6),
                    JsonConvert.SerializeObject(arg7),
                    JsonConvert.SerializeObject(arg8),
                    JsonConvert.SerializeObject(arg9)
                },
                ArgTypes = new string[] {
                    arg1.GetType().GetTypeInfo().FullName,
                    arg2.GetType().GetTypeInfo().FullName,
                    arg3.GetType().GetTypeInfo().FullName,
                    arg4.GetType().GetTypeInfo().FullName,
                    arg5.GetType().GetTypeInfo().FullName,
                    arg6.GetType().GetTypeInfo().FullName,
                    arg7.GetType().GetTypeInfo().FullName,
                    arg8.GetType().GetTypeInfo().FullName,
                    arg9.GetType().GetTypeInfo().FullName
                },
                ReturnType = rettyp.GetTypeInfo().FullName
            });
        }
        public static object Invoke10(ProcessId pid, string method, Type rettyp, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10)
        {
            return ActorContext.Ask(pid, new ProxyMsg
            {
                Method = method,
                Args = new string[] {
                    JsonConvert.SerializeObject(arg1),
                    JsonConvert.SerializeObject(arg2),
                    JsonConvert.SerializeObject(arg3),
                    JsonConvert.SerializeObject(arg4),
                    JsonConvert.SerializeObject(arg5),
                    JsonConvert.SerializeObject(arg6),
                    JsonConvert.SerializeObject(arg7),
                    JsonConvert.SerializeObject(arg8),
                    JsonConvert.SerializeObject(arg9),
                    JsonConvert.SerializeObject(arg10)
                },
                ArgTypes = new string[] {
                    arg1.GetType().GetTypeInfo().FullName,
                    arg2.GetType().GetTypeInfo().FullName,
                    arg3.GetType().GetTypeInfo().FullName,
                    arg4.GetType().GetTypeInfo().FullName,
                    arg5.GetType().GetTypeInfo().FullName,
                    arg6.GetType().GetTypeInfo().FullName,
                    arg7.GetType().GetTypeInfo().FullName,
                    arg8.GetType().GetTypeInfo().FullName,
                    arg9.GetType().GetTypeInfo().FullName,
                    arg10.GetType().GetTypeInfo().FullName
                },
                ReturnType = rettyp.GetTypeInfo().FullName
            });
        }
        public static object Invoke11(ProcessId pid, string method, Type rettyp, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11)
        {
            return ActorContext.Ask(pid, new ProxyMsg
            {
                Method = method,
                Args = new string[] {
                    JsonConvert.SerializeObject(arg1),
                    JsonConvert.SerializeObject(arg2),
                    JsonConvert.SerializeObject(arg3),
                    JsonConvert.SerializeObject(arg4),
                    JsonConvert.SerializeObject(arg5),
                    JsonConvert.SerializeObject(arg6),
                    JsonConvert.SerializeObject(arg7),
                    JsonConvert.SerializeObject(arg8),
                    JsonConvert.SerializeObject(arg9),
                    JsonConvert.SerializeObject(arg10),
                    JsonConvert.SerializeObject(arg11)
                },
                ArgTypes = new string[] {
                    arg1.GetType().GetTypeInfo().FullName,
                    arg2.GetType().GetTypeInfo().FullName,
                    arg3.GetType().GetTypeInfo().FullName,
                    arg4.GetType().GetTypeInfo().FullName,
                    arg5.GetType().GetTypeInfo().FullName,
                    arg6.GetType().GetTypeInfo().FullName,
                    arg7.GetType().GetTypeInfo().FullName,
                    arg8.GetType().GetTypeInfo().FullName,
                    arg9.GetType().GetTypeInfo().FullName,
                    arg10.GetType().GetTypeInfo().FullName,
                    arg11.GetType().GetTypeInfo().FullName
                },
                ReturnType = rettyp.GetTypeInfo().FullName
            });
        }
        public static object Invoke12(ProcessId pid, string method, Type rettyp, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12)
        {
            return ActorContext.Ask(pid, new ProxyMsg
            {
                Method = method,
                Args = new string[] {
                    JsonConvert.SerializeObject(arg1),
                    JsonConvert.SerializeObject(arg2),
                    JsonConvert.SerializeObject(arg3),
                    JsonConvert.SerializeObject(arg4),
                    JsonConvert.SerializeObject(arg5),
                    JsonConvert.SerializeObject(arg6),
                    JsonConvert.SerializeObject(arg7),
                    JsonConvert.SerializeObject(arg8),
                    JsonConvert.SerializeObject(arg9),
                    JsonConvert.SerializeObject(arg10),
                    JsonConvert.SerializeObject(arg11),
                    JsonConvert.SerializeObject(arg12)
                },
                ArgTypes = new string[] {
                    arg1.GetType().GetTypeInfo().FullName,
                    arg2.GetType().GetTypeInfo().FullName,
                    arg3.GetType().GetTypeInfo().FullName,
                    arg4.GetType().GetTypeInfo().FullName,
                    arg5.GetType().GetTypeInfo().FullName,
                    arg6.GetType().GetTypeInfo().FullName,
                    arg7.GetType().GetTypeInfo().FullName,
                    arg8.GetType().GetTypeInfo().FullName,
                    arg9.GetType().GetTypeInfo().FullName,
                    arg10.GetType().GetTypeInfo().FullName,
                    arg11.GetType().GetTypeInfo().FullName,
                    arg12.GetType().GetTypeInfo().FullName
                },
                ReturnType = rettyp.GetTypeInfo().FullName
            });
        }
    }
}

#endif