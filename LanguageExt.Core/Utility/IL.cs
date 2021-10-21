using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using System.Text;
using LanguageExt.TypeClasses;
using LanguageExt.ClassInstances;
using static LanguageExt.Prelude;
using static LanguageExt.Reflect;

namespace LanguageExt
{
    public static class IL
    {
        /// <summary>
        /// Emits the IL to instantiate a type of R with a single argument to 
        /// the constructor
        /// </summary>
        public static Func<R> Ctor<R>()
        {
            var ctorInfo = GetConstructor<R>()
                               .IfNone(() => throw new ArgumentException($"Constructor not found for type {typeof(R).FullName}"));

            var ctorParams = ctorInfo.GetParameters();

            var dynamic = new DynamicMethod("CreateInstance",
                                            ctorInfo.DeclaringType,
                                            new Type[0],
                                            typeof(R).Module,
                                            true);

            var il = dynamic.GetILGenerator();
            il.Emit(OpCodes.Newobj, ctorInfo);
            il.Emit(OpCodes.Ret);

            return (Func<R>)dynamic.CreateDelegate(typeof(Func<R>));
        }

        /// <summary>
        /// Emits the IL to instantiate a type of R with a single argument to 
        /// the constructor
        /// </summary>
        public static Func<A, R> Ctor<A, R>()
        {
            var ctorInfo = GetConstructor<R, A>()
               .IfNone(() => throw new ArgumentException($"Constructor not found for type {typeof(R).FullName}"));

            var ctorParams = ctorInfo.GetParameters();

            if (ILCapability.Available)
            {
                var dynamic = new DynamicMethod("CreateInstance",
                                                ctorInfo.DeclaringType,
                                                ctorParams.Select(p => p.ParameterType).ToArray(),
                                                typeof(R).Module,
                                                true);

                var il = dynamic.GetILGenerator();
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Newobj, ctorInfo);
                il.Emit(OpCodes.Ret);

                return (Func<A, R>)dynamic.CreateDelegate(typeof(Func<A, R>));
            }
            else
            {
                var arg0 = Expression.Parameter(typeof(A), "arg0");
                var expr = Expression.New(ctorInfo, arg0);
                var lambda = Expression.Lambda<Func<A, R>>(expr, arg0);
                return lambda.Compile();   
            }
        }

        /// <summary>
        /// Emits the IL to instantiate a type of R with two arguments to 
        /// the constructor
        /// </summary>
        public static Func<A, B, R> Ctor<A, B, R>()
        {
            var ctorInfo = GetConstructor<R, A, B>()
               .IfNone(() => throw new ArgumentException($"Constructor not found for type {typeof(R).FullName}"));

            var ctorParams = ctorInfo.GetParameters();

            if (ILCapability.Available)
            {
                var dynamic = new DynamicMethod("CreateInstance",
                                                ctorInfo.DeclaringType,
                                                ctorParams.Select(p => p.ParameterType).ToArray(),
                                                typeof(R).Module,
                                                true);

                var il = dynamic.GetILGenerator();
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Newobj, ctorInfo);
                il.Emit(OpCodes.Ret);

                return (Func<A, B, R>)dynamic.CreateDelegate(typeof(Func<A, B, R>));
            }
            else
            {
                var arg0   = Expression.Parameter(typeof(A), "arg0");
                var arg1   = Expression.Parameter(typeof(B), "arg1");
                var expr   = Expression.New(ctorInfo, arg0, arg1);
                var lambda = Expression.Lambda<Func<A, B, R>>(expr, arg0, arg1);
                return lambda.Compile();   
            }
        }

        /// <summary>
        /// Emits the IL to instantiate a type of R with three arguments to 
        /// the constructor
        /// </summary>
        public static Func<A, B, C, R> Ctor<A, B, C, R>()
        {
            var ctorInfo = GetConstructor<R, A, B, C>()
                               .IfNone(() => throw new ArgumentException($"Constructor not found for type {typeof(R).FullName}"));

            var ctorParams = ctorInfo.GetParameters();

            if (ILCapability.Available)
            {
                var dynamic = new DynamicMethod("CreateInstance",
                                                ctorInfo.DeclaringType,
                                                ctorParams.Select(p => p.ParameterType).ToArray(),
                                                typeof(R).Module,
                                                true);

                var il = dynamic.GetILGenerator();
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Ldarg_2);
                il.Emit(OpCodes.Newobj, ctorInfo);
                il.Emit(OpCodes.Ret);

                return (Func<A, B, C, R>)dynamic.CreateDelegate(typeof(Func<A, B, C, R>));
            }
            else
            {
                var arg0   = Expression.Parameter(typeof(A), "arg0");
                var arg1   = Expression.Parameter(typeof(B), "arg1");
                var arg2   = Expression.Parameter(typeof(C), "arg2");
                var expr   = Expression.New(ctorInfo, arg0, arg1, arg2);
                var lambda = Expression.Lambda<Func<A, B, C, R>>(expr, arg0, arg1, arg2);
                return lambda.Compile();   
            }
        }

        /// <summary>
        /// Emits the IL to instantiate a type of R with four arguments to 
        /// the constructor
        /// </summary>
        public static Func<A, B, C, D, R> Ctor<A, B, C, D, R>()
        {
            var ctorInfo = GetConstructor<R, A, B, C, D>()
                               .IfNone(() => throw new ArgumentException($"Constructor not found for type {typeof(R).FullName}"));

            if (ctorInfo == null) throw new ArgumentException($"Constructor not found for type {typeof(R).FullName}");

            if (ILCapability.Available)
            {
                var ctorParams = ctorInfo.GetParameters();

                var dynamic = new DynamicMethod("CreateInstance",
                                                ctorInfo.DeclaringType,
                                                ctorParams.Select(p => p.ParameterType).ToArray(),
                                                typeof(R).Module,
                                                true);

                var il = dynamic.GetILGenerator();
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Ldarg_2);
                il.Emit(OpCodes.Ldarg_3);
                il.Emit(OpCodes.Newobj, ctorInfo);
                il.Emit(OpCodes.Ret);

                return (Func<A, B, C, D, R>)dynamic.CreateDelegate(typeof(Func<A, B, C, D, R>));
            }
            else
            {
                var arg0   = Expression.Parameter(typeof(A), "arg0");
                var arg1   = Expression.Parameter(typeof(B), "arg1");
                var arg2   = Expression.Parameter(typeof(C), "arg2");
                var arg3   = Expression.Parameter(typeof(D), "arg3");
                var expr   = Expression.New(ctorInfo, arg0, arg1, arg2, arg3);
                var lambda = Expression.Lambda<Func<A, B, C, D, R>>(expr, arg0, arg1, arg2, arg3);
                return lambda.Compile();   
            }
        }

        /// <summary>
        /// Emits the IL to invoke a static method
        /// </summary>
        public static Option<Func<object, R>> Func1<TYPE, R>(Type arg1, Func<MethodInfo, bool> methodPred = null)
        {
            methodPred ??= (_ => true);

            var methodInfo = typeof(TYPE)
                .GetTypeInfo()
                .GetAllMethods(true)
                .Where(x =>
                {
                    if (!x.IsStatic) return false;
                    var ps = x.GetParameters();
                    if (ps.Length != 1) return false;
                    if (ps[0].ParameterType != arg1) return false;
                    return methodPred(x);
                })
                .FirstOrDefault();

            if (methodInfo == null) return None;

            var methodParams = methodInfo.GetParameters();

            if (ILCapability.Available)
            {
                var dynamic = new DynamicMethod("CreateInstance",
                                                typeof(R),
                                                methodParams.Select(p => typeof(object)).ToArray(),
                                                typeof(R).Module,
                                                true);

                var il = dynamic.GetILGenerator();
                il.DeclareLocal(typeof(R));
                il.Emit(OpCodes.Ldarg_0);
                if (arg1.GetTypeInfo().IsValueType)
                {
                    il.Emit(OpCodes.Unbox_Any, arg1);
                }
                else
                {
                    il.Emit(OpCodes.Castclass, arg1);
                }

                il.Emit(OpCodes.Call, methodInfo);
                il.Emit(OpCodes.Ret);

                return (Func<object, R>)dynamic.CreateDelegate(typeof(Func<object, R>));
            }
            else
            {
                var larg1  = Expression.Parameter(typeof(object), "arg1");
                var expr   = Expression.Call(methodInfo, Expression.Convert(larg1, arg1));
                var lambda = Expression.Lambda<Func<object, R>>(expr, larg1);
                return lambda.Compile();   
            }
        }

        /// <summary>
        /// Emits the IL to invoke a static method with one argument
        /// </summary>
        public static Option<Func<A, R>> Func1<TYPE, A, R>(Func<MethodInfo, bool> methodPred = null)
        {
            methodPred = methodPred ?? (_ => true);

            var methodInfo = typeof(TYPE)
                .GetTypeInfo()
                .GetAllMethods(true)
                .Where(x =>
                {
                    if (!x.IsStatic) return false;
                    var ps = x.GetParameters();
                    if (ps.Length != 1) return false;
                    if (ps[0].ParameterType != typeof(A)) return false;
                    return methodPred(x);
                })
                .FirstOrDefault();

            if (methodInfo == null) return None;

            if (ILCapability.Available)
            {
                var methodParams = methodInfo.GetParameters();

                var dynamic = new DynamicMethod("CreateInstance",
                                                typeof(R),
                                                methodParams.Select(p => p.ParameterType).ToArray(),
                                                typeof(R).Module,
                                                true);

                var il = dynamic.GetILGenerator();
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Call, methodInfo);
                il.Emit(OpCodes.Ret);

                return (Func<A, R>)dynamic.CreateDelegate(typeof(Func<A, R>));
            }
            else
            {
                var larg0  = Expression.Parameter(typeof(A), "arg0");
                var expr   = Expression.Call(methodInfo, larg0);
                var lambda = Expression.Lambda<Func<A, R>>(expr, larg0);
                return lambda.Compile();
            }
        }

        /// <summary>
        /// Emits the IL to invoke a static method with two arguments
        /// </summary>
        public static Option<Func<A, B, R>> Func2<TYPE, A, B, R>(Func<MethodInfo, bool> methodPred = null)
        {
            methodPred = methodPred ?? (_ => true);

            var methodInfo = typeof(TYPE)
                .GetTypeInfo()
                .GetAllMethods(true)
                .Where(x =>
                {
                    if (!x.IsStatic) return false;
                    var ps = x.GetParameters();
                    if (ps.Length != 2) return false;
                    if (ps[0].ParameterType != typeof(A)) return false;
                    if (ps[1].ParameterType != typeof(B)) return false;
                    return methodPred(x);
                })
                .FirstOrDefault();

            if (methodInfo == null) return None;

            if (ILCapability.Available)
            {
                var methodParams = methodInfo.GetParameters();

                var dynamic = new DynamicMethod("CreateInstance",
                                                typeof(R),
                                                methodParams.Select(p => p.ParameterType).ToArray(),
                                                typeof(R).Module,
                                                true);

                var il = dynamic.GetILGenerator();
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Call, methodInfo);
                il.Emit(OpCodes.Ret);

                return (Func<A, B, R>)dynamic.CreateDelegate(typeof(Func<A, B, R>));
            }
            else
            {
                var larg0  = Expression.Parameter(typeof(A), "arg0");
                var larg1  = Expression.Parameter(typeof(B), "arg1");
                var expr   = Expression.Call(methodInfo, larg0, larg1);
                var lambda = Expression.Lambda<Func<A, B, R>>(expr, larg0, larg1);
                return lambda.Compile();
            }
        }

        /// <summary>
        /// Emits the IL to invoke a static method with three arguments
        /// </summary>
        public static Option<Func<A, B, C, R>> Func3<TYPE, A, B, C, R>(Func<MethodInfo, bool> methodPred = null)
        {
            methodPred = methodPred ?? (_ => true);

            var methodInfo = typeof(TYPE)
                .GetTypeInfo()
                .GetAllMethods(true)
                .Where(x =>
                {
                    if (!x.IsStatic) return false;
                    var ps = x.GetParameters();
                    if (ps.Length != 3) return false;
                    if (ps[0].ParameterType != typeof(A)) return false;
                    if (ps[1].ParameterType != typeof(B)) return false;
                    if (ps[2].ParameterType != typeof(C)) return false;
                    return methodPred(x);
                })
                .FirstOrDefault();

            if (methodInfo == null) return None;

            if(ILCapability.Available)
            {
                var methodParams = methodInfo.GetParameters();

                var dynamic = new DynamicMethod("CreateInstance",
                                                typeof(R),
                                                methodParams.Select(p => p.ParameterType).ToArray(),
                                                typeof(R).Module,
                                                true);

                var il = dynamic.GetILGenerator();
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Ldarg_2);
                il.Emit(OpCodes.Call, methodInfo);
                il.Emit(OpCodes.Ret);

                return (Func<A, B, C, R>)dynamic.CreateDelegate(typeof(Func<A, B, C, R>));
            }
            else
            {
                var larg0  = Expression.Parameter(typeof(A), "arg0");
                var larg1  = Expression.Parameter(typeof(B), "arg1");
                var larg2  = Expression.Parameter(typeof(C), "arg2");
                var expr   = Expression.Call(methodInfo, larg0, larg1, larg2);
                var lambda = Expression.Lambda<Func<A, B, C, R>>(expr, larg0, larg1, larg2);
                return lambda.Compile();
            }
        }

        /// <summary>
        /// Emits the IL to invoke a static method with four arguments
        /// </summary>
        public static Option<Func<A, B, C, D, R>> Func4<TYPE, A, B, C, D, R>(Func<MethodInfo, bool> methodPred = null)
        {
            methodPred = methodPred ?? (_ => true);

            var methodInfo = typeof(TYPE)
                .GetTypeInfo()
                .GetAllMethods(true)
                .Where(x =>
                {
                    if (!x.IsStatic) return false;
                    var ps = x.GetParameters();
                    if (ps.Length != 4) return false;
                    if (ps[0].ParameterType != typeof(A)) return false;
                    if (ps[1].ParameterType != typeof(B)) return false;
                    if (ps[2].ParameterType != typeof(C)) return false;
                    if (ps[3].ParameterType != typeof(D)) return false;
                    return methodPred(x);
                })
                .FirstOrDefault();

            if (methodInfo == null) return None;

            if (ILCapability.Available)
            {
                var methodParams = methodInfo.GetParameters();

                var dynamic = new DynamicMethod("CreateInstance",
                                                typeof(R),
                                                methodParams.Select(p => p.ParameterType).ToArray(),
                                                typeof(R).Module,
                                                true);

                var il = dynamic.GetILGenerator();
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Ldarg_2);
                il.Emit(OpCodes.Ldarg_3);
                il.Emit(OpCodes.Call, methodInfo);
                il.Emit(OpCodes.Ret);

                return (Func<A, B, C, D, R>)dynamic.CreateDelegate(typeof(Func<A, B, C, D, R>));
            }
            else
            {
                var larg0  = Expression.Parameter(typeof(A), "arg0");
                var larg1  = Expression.Parameter(typeof(B), "arg1");
                var larg2  = Expression.Parameter(typeof(C), "arg2");
                var larg3  = Expression.Parameter(typeof(D), "arg3");
                var expr   = Expression.Call(methodInfo, larg0, larg1, larg2, larg3);
                var lambda = Expression.Lambda<Func<A, B, C, D, R>>(expr, larg0, larg1, larg2, larg3);
                return lambda.Compile();
            }
        }

        /// <summary>
        /// Builds a function to provide a hash-code for a record type.  the hash-code is built from
        /// the hash-codes of all the *fields* that make up the type.  
        /// </summary>
        /// <remarks>You should cache the result of this method to reduce the work of building the IL 
        /// each time.  Better still use the `RecordType<A>` type to provide a cached version of these
        /// results.
        /// </remarks>
        public static Func<A, int> GetHashCode<A>(bool includeBase)
        {
            if (Class<Hashable<A>>.Default != null)
            {
                return Class<Hashable<A>>.Default.GetHashCode;
            }
            else if (Class<Eq<A>>.Default != null)
            {
                return Class<Eq<A>>.Default.GetHashCode;
            }
            else if (Class<Ord<A>>.Default != null)
            {
                return Class<Ord<A>>.Default.GetHashCode;
            }

            var fields = GetPublicInstanceFields<A>(
                includeBase,
#pragma warning disable CS0618
                typeof(OptOutOfHashCodeAttribute),
#pragma warning restore CS0618
                typeof(NonHashAttribute), 
                typeof(NonStructuralAttribute), 
                typeof(NonRecordAttribute));

            var self = Expression.Parameter(typeof(A));

            // Use 32-bit FNV hash parameters as signed values since .net GetHashCode returns a signed 32-bit integer.
            var fnvOffsetBasis = Expression.Constant(-2128831035);
            var fnvPrime = Expression.Constant(16777619);
            var zero = Expression.Constant(0);

            var Null = Expression.Constant(null, typeof(A));
            var refEq = Expression.ReferenceEqual(self, Null);

            (MemberExpression MemExpr, MethodInfo HashMethod) fieldHash(FieldInfo f)
            {
                var kinds = new[] { typeof(Hashable<>), typeof(Eq<>), typeof(Ord<>) };

                foreach(var kind in kinds)
                {
                    var typ = typeof(Class<>)
                        .MakeGenericType(kind.MakeGenericType(f.FieldType))
                        .GetTypeInfo();

                    var fld = typ.DeclaredFields.Where(m => m.Name == "Default").Single();

                    var fres = fld.GetValue(null);

                    if(fres != null)
                    {
                        var method = kind
                            .GetTypeInfo()
                            .MakeGenericType(f.FieldType)
                            .GetTypeInfo()
                            .GetAllMethods(true)
                            .Where(m => m.Name == "GetHashCode")
                            .Where(m => m.GetParameters().Map(p => p.ParameterType).ToSeq() == Seq1(f.FieldType))
                            .Head();

                        return (Expression.Field(
                            null,
                            typ.DeclaredFields
                               .Where(m => m.Name == "Default")
                               .Single()), method);
                    }
                }

                return (null, null);
            }

            IEnumerable<Expression> Fields()
            {
                foreach (var field in fields)
                {
                    var target = fieldHash(field);

                    if (target.MemExpr == null)
                    {
                        var propOrField = Expression.PropertyOrField(self, PrettyFieldName(field));

                        var method = field.FieldType.GetTypeInfo()
                                                    .GetAllMethods(true)
                                                    .Where(m => m.Name == "GetHashCode")
                                                    .Where(m => default(EqArray<EqDefault<Type>, Type>).Equals(
                                                                   m.GetParameters().Map(p => p.ParameterType).ToArray(),
                                                                   new Type[0]))
                                                    .HeadOrNone();

                        if (method.IsSome)
                        {
                            var call = field.FieldType.GetTypeInfo().IsValueType
                                           ? Expression.Call(propOrField, method.Value)
                                           : Expression.Condition(
                                                 Expression.ReferenceEqual(propOrField, Expression.Constant(null, field.FieldType)),
                                                 zero,
                                                 Expression.Call(propOrField, method.Value)) as Expression;
                            yield return call;
                        }
                    }
                    else
                    {
                         yield return Expression.Call(
                                  target.MemExpr,
                                  target.HashMethod,
                                  Expression.PropertyOrField(self, field.Name)
                                  );
                    }
                }
            }

            // Implement FNV 1a hashing algorithm - [Fowler–Noll–Vo hash function](https://en.wikipedia.org/wiki/Fowler%E2%80%93Noll%E2%80%93Vo_hash_function#FNV-1a_hash)
            var expr = Fields().Fold(fnvOffsetBasis as Expression, (state, field) =>
                Expression.Multiply(
                    fnvPrime,
                    Expression.ExclusiveOr(
                        state,
                        field)));

            var lambda = Expression.Lambda<Func<A, int>>(
                typeof(A).GetTypeInfo().IsValueType
                    ? expr
                    : Expression.Condition(refEq, Expression.Constant(0), expr)
                , self);

            return lambda.Compile();
        }

        /// <summary>
        /// Provides a function that compares two record type arguments (one of type A and one of 
        /// object) for structural equality, this first makes sure that the `Object` argument is of 
        /// type A and then compares the *fields* from each argument for equality and returns true if 
        /// all are equal.  
        /// </summary>
        /// <remarks>You should cache the result of this method to reduce the work of building the IL 
        /// each time.  Better still use the `RecordType<A>` type to provide a cached version of these
        /// results.
        /// </remarks>
        public static Func<A, object, bool> Equals<A>(bool includeBase)
        {
            if (Class<Eq<A>>.Default != null)
            {
                return new Func<A, object, bool>((a, obj) => obj is A b && Class<Eq<A>>.Default.Equals(a, b));
            }
            else if (Class<Ord<A>>.Default != null)
            {
                return new Func<A, object, bool>((a, obj) => obj is A b && Class<Ord<A>>.Default.Equals(a, b));
            }

            var fields = GetPublicInstanceFields<A>(
                includeBase,
#pragma warning disable CS0618
                typeof(OptOutOfEqAttribute),
#pragma warning restore CS0618
                typeof(NonEqAttribute),
                typeof(NonStructuralAttribute),
                typeof(NonRecordAttribute));

            var self = Expression.Parameter(typeof(A), "self");
            var other = Expression.Parameter(typeof(object), "other");
            var otherCast = Expression.Convert(other, typeof(A));
            var True = Expression.Constant(true);

            var NullA = Expression.Constant(null, typeof(A));
            var NullObj = Expression.Constant(null, typeof(object));
            var refEq = Expression.ReferenceEqual(self, other);
            var notNullX = Expression.ReferenceNotEqual(self, NullA);
            var notNullY = Expression.ReferenceNotEqual(other, NullObj);

            var typeA = Expression.TypeEqual(self, typeof(A));
            var typeB = Expression.TypeEqual(other, typeof(A));
            var typesEqual = Expression.Equal(typeA, typeB);

            var fieldEq = fun((FieldInfo f) =>
            {
                var typ = typeof(Class<>)
                    .MakeGenericType(typeof(Eq<>).MakeGenericType(f.FieldType))
                    .GetTypeInfo();

                var fld = typ.DeclaredFields.Where(m => m.Name == "Default").Single();

                return fld.GetValue(null);
            });

            var expr = Expression.AndAlso(
                typesEqual,
                fields.Fold(True as Expression, (state, field) =>
                    Expression.AndAlso(
                        state,
                        fieldEq(field) == null
                            ? Expression.Call(
                                  Expression.Property(null,
                                      typeof(EqualityComparer<>)
                                          .MakeGenericType(field.FieldType)
                                          .GetTypeInfo()
                                          .DeclaredProperties.Where(m => m.Name == "Default")
                                          .Single()),
                                
                                  typeof(IEqualityComparer<>)
                                      .GetTypeInfo()
                                      .MakeGenericType(field.FieldType)
                                      .GetTypeInfo()
                                      .GetAllMethods(true)
                                      .Where(m => m.Name == "Equals")
                                      .Where(m => m.GetParameters().Map(p => p.ParameterType).ToSeq() == Seq(field.FieldType, field.FieldType))
                                      .Head(),
                                  Expression.PropertyOrField(self, field.Name),
                                  Expression.PropertyOrField(otherCast, field.Name)
                                  )
                            : Expression.Call(
                                  Expression.Field(null,
                                      typeof(Class<>)
                                          .MakeGenericType(typeof(Eq<>).MakeGenericType(field.FieldType))
                                          .GetTypeInfo()
                                          .DeclaredFields.Where(m => m.Name == "Default")
                                          .Single()),

                                  typeof(Eq<>)
                                      .GetTypeInfo()
                                      .MakeGenericType(field.FieldType)
                                      .GetTypeInfo()
                                      .GetAllMethods(true)
                                      .Where(m => m.Name == "Equals")
                                      .Where(m => m.GetParameters().Map(p => p.ParameterType).ToSeq() == Seq(field.FieldType, field.FieldType))
                                      .Head(),
                                  Expression.PropertyOrField(self, field.Name),
                                  Expression.PropertyOrField(otherCast, field.Name)
                                  )
                        ) 
                    ));

            var orExpr = Expression.OrElse(refEq, Expression.AndAlso(notNullX, Expression.AndAlso(notNullY, expr)));

            var lambda = Expression.Lambda<Func<A, object, bool>>(
                typeof(A).GetTypeInfo().IsValueType
                    ? expr
                    : orExpr, self, other);

            return lambda.Compile();
        }

        /// <summary>
        /// Provides a function that compares two record type arguments for structural equality, this 
        /// first compares the *fields* from each argument for equality and returns true if all are 
        /// equal.  
        /// </summary>
        /// <remarks>You should cache the result of this method to reduce the work of building the IL 
        /// each time.  Better still use the `RecordType<A>` type to provide a cached version of these
        /// results.
        /// </remarks>
        public static Func<A, A, bool> EqualsTyped<A>(bool includeBase)
        {
            if (Class<Eq<A>>.Default != null)
            {
                return Class<Eq<A>>.Default.Equals;
            }

            var fields = GetPublicInstanceFields<A>(
                includeBase,
#pragma warning disable CS0618
                typeof(OptOutOfEqAttribute),
#pragma warning restore CS0618
                typeof(NonEqAttribute),
                typeof(NonStructuralAttribute),
                typeof(NonRecordAttribute)
                );

            var self = Expression.Parameter(typeof(A), "self");
            var other = Expression.Parameter(typeof(A), "other");
            var True = Expression.Constant(true);
            var Null = Expression.Constant(null, typeof(A));
            var refEq = Expression.ReferenceEqual(self, other);
            var notNullX = Expression.ReferenceNotEqual(self, Null);
            var notNullY = Expression.ReferenceNotEqual(other, Null);
            var typeA = Expression.TypeEqual(self, typeof(A));
            var typeB = Expression.TypeEqual(other, typeof(A));
            var typesEqual = Expression.Equal(typeA, typeB);

            var fieldEq = fun((FieldInfo f)=>
            {
                var typ = typeof(Class<>)
                    .MakeGenericType(typeof(Eq<>).MakeGenericType(f.FieldType))
                    .GetTypeInfo();

                var fld = typ.DeclaredFields.Where(m => m.Name == "Default").Single();
                return fld.GetValue(null);
            });

            var expr = Expression.AndAlso(
                typesEqual,
                fields.Fold(True as Expression, (state, field) =>
                    Expression.AndAlso(
                        state,
                        fieldEq(field) == null
                            ? Expression.Call(
                                  Expression.Property(null, 
                                      typeof(EqualityComparer<>)
                                          .MakeGenericType(field.FieldType)
                                          .GetTypeInfo()
                                          .DeclaredProperties.Where(m => m.Name == "Default")
                                          .Single()),
                             
                                  typeof(IEqualityComparer<>)
                                      .GetTypeInfo()
                                      .MakeGenericType(field.FieldType)
                                      .GetTypeInfo()
                                      .GetAllMethods(true)
                                      .Where(m => m.Name == "Equals")
                                      .Where(m => m.GetParameters().Map(p => p.ParameterType).ToSeq() == Seq(field.FieldType, field.FieldType))
                                      .Head(),
                                  Expression.PropertyOrField(self, field.Name),
                                  Expression.PropertyOrField(other, field.Name))
                             : Expression.Call(
                                      Expression.Field(null,
                                          typeof(Class<>)
                                              .MakeGenericType(typeof(Eq<>).MakeGenericType(field.FieldType))
                                              .GetTypeInfo()
                                              .DeclaredFields.Where(m => m.Name == "Default")
                                              .Single()),
                                      typeof(Eq<>)
                                          .GetTypeInfo()
                                          .MakeGenericType(field.FieldType)
                                          .GetTypeInfo()
                                          .GetAllMethods(true)
                                          .Where(m => m.Name == "Equals")
                                          .Where(m => m.GetParameters().Map(p => p.ParameterType).ToSeq() == Seq(field.FieldType, field.FieldType))
                                          .Head(),
                                      Expression.PropertyOrField(self, field.Name),
                                      Expression.PropertyOrField(other, field.Name)
                                  ))));

            var orExpr = Expression.OrElse(refEq,  Expression.AndAlso(notNullX, Expression.AndAlso(notNullY, expr)));

            var lambda = Expression.Lambda<Func<A, A, bool>>(
                typeof(A).GetTypeInfo().IsValueType
                    ? expr
                    : orExpr, self, other);

            return lambda.Compile();
        }

        /// <summary>
        /// Provides a function that compares two record type arguments for structural equality, this 
        /// compares the *fields* from each argument for equality and returns 0 if all are equal, -1 
        /// if X is less than Y, and 1 if X is greater than Y.
        /// </summary>
        /// <remarks>You should cache the result of this method to reduce the work of building the IL 
        /// each time.  Better still use the `RecordType<A>` type to provide a cached version of these
        /// results.
        /// </remarks>
        public static Func<A, A, int> Compare<A>(bool includeBase)
        {
            if (Class<Ord<A>>.Default != null)
            {
                return Class<Ord<A>>.Default.Compare;
            }

            var fields = GetPublicInstanceFields<A>(
                includeBase,
#pragma warning disable CS0618
                typeof(OptOutOfOrdAttribute),
#pragma warning restore CS0618
                typeof(NonOrdAttribute),
                typeof(NonStructuralAttribute),
                typeof(NonRecordAttribute)
                );

            var self = Expression.Parameter(typeof(A), "self");
            var other = Expression.Parameter(typeof(A), "other");
            var Zero = Expression.Constant(0);
            var Minus1 = Expression.Constant(-1);
            var Plus1 = Expression.Constant(1);
            var Null = Expression.Constant(null, typeof(A));
            var refEq = Expression.ReferenceEqual(self, other);
            var xIsNull = Expression.ReferenceEqual(self, Null);
            var yIsNull = Expression.ReferenceEqual(other, Null);
            var typeA = Expression.TypeEqual(self, typeof(A));
            var typeB = Expression.TypeEqual(other, typeof(A));
            var typesNotEqual = Expression.NotEqual(typeA, typeB);
            var returnTarget = Expression.Label(typeof(int));
            var ord = Expression.Variable(typeof(int), "ord");

            var fieldOrd = fun((FieldInfo f) =>
            {
                var typ = typeof(Class<>)
                    .MakeGenericType(typeof(Ord<>).MakeGenericType(f.FieldType))
                    .GetTypeInfo();

                var fld = typ.DeclaredFields.Where(m => m.Name == "Default").Single();
                return fld.GetValue(null);
            });

            IEnumerable<Expression[]> Fields()
            {
                foreach (var f in fields)
                {
                    var comparer =
                        Expression.Assign(ord,
                            fieldOrd(f) == null
                                    ? Expression.Call(
                                        Expression.Property(null,
                                            typeof(Comparer<>)
                                                .MakeGenericType(f.FieldType)
                                                .GetTypeInfo()
                                                .DeclaredProperties.Where(m => m.Name == "Default")
                                                .Single()),
                                        typeof(IComparer<>)
                                            .GetTypeInfo()
                                            .MakeGenericType(f.FieldType)
                                            .GetTypeInfo()
                                            .GetAllMethods(true)
                                            .Where(m => m.Name == "Compare")
                                            .Where(m => m.GetParameters().Map(p => p.ParameterType).ToSeq() == Seq(f.FieldType, f.FieldType))
                                            .Head(),
                                        Expression.PropertyOrField(self, f.Name),
                                        Expression.PropertyOrField(other, f.Name))
                                    : Expression.Call(
                                            Expression.Field(null,
                                                typeof(Class<>)
                                                    .MakeGenericType(typeof(Ord<>).MakeGenericType(f.FieldType))
                                                    .GetTypeInfo()
                                                    .DeclaredFields.Where(m => m.Name == "Default")
                                                    .Single()),
                                            typeof(Ord<>)
                                                .GetTypeInfo()
                                                .MakeGenericType(f.FieldType)
                                                .GetTypeInfo()
                                                .GetAllMethods(true)
                                                .Where(m => m.Name == "Compare")
                                                .Where(m => m.GetParameters().Map(p => p.ParameterType).ToSeq() == Seq(f.FieldType, f.FieldType))
                                                .Head(),
                                            Expression.PropertyOrField(self, f.Name),
                                            Expression.PropertyOrField(other, f.Name)
                                        ));

                    if (f.FieldType.IsValueType)
                    {
                        yield return new[] { comparer as Expression };
                    }
                    else
                    { 
                        var fnull = Expression.Constant(null, f.FieldType);

                        yield return new[] {
                            Expression.IfThen(
                                Expression.And(
                                    Expression.ReferenceEqual(Expression.PropertyOrField(self, f.Name), fnull),
                                    Expression.IsFalse(Expression.ReferenceEqual(Expression.PropertyOrField(other, f.Name), fnull))),
                                Expression.Return(returnTarget, Minus1)) as Expression,

                            Expression.IfThen(
                                Expression.And(
                                    Expression.ReferenceEqual(Expression.PropertyOrField(other, f.Name), fnull),
                                    Expression.IsFalse(Expression.ReferenceEqual(Expression.PropertyOrField(self, f.Name), fnull))),
                                Expression.Return(returnTarget, Plus1)) as Expression,

                            Expression.IfThenElse(
                                Expression.ReferenceEqual(
                                    Expression.PropertyOrField(self, f.Name),
                                    Expression.PropertyOrField(other, f.Name)),
                                Expression.Assign(ord, Zero),
                                comparer) as Expression
                            };
                    }

                    yield return new[] {
                        // Fields are not equal
                        Expression.IfThen(
                            Expression.NotEqual(ord, Zero),
                            Expression.Return(returnTarget, ord, typeof(int))) as Expression
                        };
                }
            }

            var block =  Expression.Block(
                new [] { ord },
                new[] {
                    Expression.IfThen(refEq, Expression.Return(returnTarget, Zero)),
                    Expression.IfThen(xIsNull, Expression.Return(returnTarget, Minus1)),
                    Expression.IfThen(yIsNull, Expression.Return(returnTarget, Plus1)),
                    Expression.IfThen(typesNotEqual, Expression.Return(returnTarget, Minus1))
                }
                .Append( Fields().Bind(identity))
                .Append( new [] { Expression.Label(returnTarget, Zero) as Expression }));

            var lambda = Expression.Lambda<Func<A, A, int>>(block, self, other);

            return lambda.Compile();
        }

        static Func<A, string> ToStringExpr<A>(bool includeBase)
        {
            var fields = GetPublicInstanceFields<A>(
                includeBase,
#pragma warning disable CS0618
                typeof(OptOutOfToStringAttribute),
#pragma warning restore CS0618
                typeof(NonShowAttribute),
                typeof(NonRecordAttribute)
                ).ToArray();
            
            var stringBuilder = GetConstructor<StringBuilder>().IfNone(() => throw new ArgumentException($"Constructor not found for StringBuilder"));
            var appendChar    = GetPublicInstanceMethod<StringBuilder, char>("Append", true).IfNone(() => throw new ArgumentException($"Append method found for StringBuilder"));
            var appendString  = GetPublicInstanceMethod<StringBuilder, string>("Append", true).IfNone(() => throw new ArgumentException($"Append method found for StringBuilder"));
            var toString      = GetPublicInstanceMethod<StringBuilder>("ToString", false).IfNone(() => throw new ArgumentException($"ToString method found for StringBuilder"));
            var name          = typeof(A).Name;
            var self          = Expression.Parameter(typeof(A), "self");
            var nullA         = Expression.Constant(null, typeof(A));
            var nullStr       = Expression.Constant(null, typeof(string));
            var sb            = Expression.Variable(typeof(StringBuilder), "sb");
            var tmpStr        = Expression.Variable(typeof(string), "tmpStr");
            var result        = Expression.Variable(typeof(string), "result");
            var returnTarget  = Expression.Label(typeof(string));
            
            if (name.IndexOf('`') != -1) name = name.Split('`').Head();

            Expression fieldExpr(FieldInfo field)
            {
                var convertToString = (GetPublicStaticMethod(typeof(Convert), "ToString", field.FieldType) ||
                                       GetPublicStaticMethod(typeof(Convert), "ToString", typeof(object)))
                                      .IfNone(() => throw new Exception());

                return Expression.Block(
                           Expression.Assign(tmpStr, convertToString.GetParameters()[0].ParameterType == typeof(object)
                                                        ? Expression.Call(convertToString, Expression.Convert(Expression.Field(self, field), typeof(object)))
                                                        : Expression.Call(convertToString, Expression.Field(self, field))),
                           Expression.IfThenElse(
                               Expression.ReferenceEqual(tmpStr, nullStr),
                               Expression.Call(sb, appendString, Expression.Constant("null")),
                               Expression.Call(sb, appendString, tmpStr)));
            }

            var inner = Expression.Block(
                fields.Select(fieldExpr).Intersperse(Expression.Call(sb, appendString, Expression.Constant(", "))));
            
            var outer = Expression.Block(
                Expression.Assign(sb, Expression.New(stringBuilder)),
                Expression.Call(sb, appendString, Expression.Constant(name)),
                Expression.Call(sb, appendChar, Expression.Constant('(')),
                inner,
                Expression.Call(sb, appendChar, Expression.Constant(')')),
                Expression.Assign(result, Expression.Call(sb, toString)));
            
            var expr = Expression.IfThenElse(
                Expression.ReferenceEqual(self, nullA),
                Expression.Assign(result, Expression.Constant("(null)")),
                outer);
                                  
            var block = Expression.Block(
                new [] { tmpStr, sb, result },
                expr,
                Expression.Return(returnTarget, result),
                Expression.Label(returnTarget, result));
            
            var lambda = Expression.Lambda<Func<A, string>>(block, self);

            return lambda.Compile();
        }

        public static Func<A, string> ToString<A>(bool includeBase)
        {
            if (!ILCapability.Available)
            {
                return ToStringExpr<A>(includeBase);
            }

            var isValueType = typeof(A).GetTypeInfo().IsValueType;
            var dynamic = new DynamicMethod("FieldsToString", 
                                            typeof(string), 
                                            new[] { typeof(A) },                                            
                                            typeof(A).Module,
                                            true);
            var fields = GetPublicInstanceFields<A>(
                includeBase,
#pragma warning disable CS0618
                typeof(OptOutOfToStringAttribute),
#pragma warning restore CS0618
                typeof(NonShowAttribute),
                typeof(NonRecordAttribute)
                ).ToArray();
            var stringBuilder = GetConstructor<StringBuilder>().IfNone(() => throw new ArgumentException($"Constructor not found for StringBuilder"));
            var appendChar = GetPublicInstanceMethod<StringBuilder, char>("Append", true).IfNone(() => throw new ArgumentException($"Append method found for StringBuilder"));
            var appendString = GetPublicInstanceMethod<StringBuilder, string>("Append", true).IfNone(() => throw new ArgumentException($"Append method found for StringBuilder"));
            var toString = GetPublicInstanceMethod<Object>("ToString", true).IfNone(() => throw new ArgumentException($"ToString method found for Object"));
            var name = typeof(A).Name;
            if (name.IndexOf('`') != -1) name = name.Split('`').Head();

            var il = dynamic.GetILGenerator();
            il.DeclareLocal(typeof(StringBuilder));
            var notNull = il.DefineLabel();

            if (!isValueType)
            {
                // Check reference == null
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Brtrue_S, notNull);

                // Is null so return "(null)"
                il.Emit(OpCodes.Ldstr, "(null)");
                il.Emit(OpCodes.Ret);

                il.MarkLabel(notNull);
            }

            // var sb = new StringBuilder()
            il.Emit(OpCodes.Newobj, stringBuilder);
            il.Emit(OpCodes.Stloc_0);

            // sb.Append('(')
            il.Emit(OpCodes.Ldloc_0);
            if (fields.Length == 0)
            {
                il.Emit(OpCodes.Ldstr, $"{name}");
            }
            else
            {
                il.Emit(OpCodes.Ldstr, $"{name}(");
            }
            il.Emit(OpCodes.Callvirt, appendString);
            il.Emit(OpCodes.Pop);

            bool first = true;
            foreach (var field in fields)
            {
                var skipAppend = il.DefineLabel();

                if (!first)
                {
                    // sb.Append(", ")
                    il.Emit(OpCodes.Ldloc_0);
                    il.Emit(OpCodes.Ldstr, ", ");
                    il.Emit(OpCodes.Callvirt, appendString);
                    il.Emit(OpCodes.Pop);
                }

                if (!field.FieldType.GetTypeInfo().IsValueType)
                {
                    var fieldNotNull = il.DefineLabel();

                    // If(this.field == null)
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(isValueType ? OpCodes.Ldflda : OpCodes.Ldfld, field);
                    il.Emit(OpCodes.Brtrue_S, fieldNotNull);

                    // sb.Append("null")
                    il.Emit(OpCodes.Ldloc_0);
                    il.Emit(OpCodes.Ldstr, "null");
                    il.Emit(OpCodes.Callvirt, appendString);
                    il.Emit(OpCodes.Pop);

                    // continue
                    il.Emit(OpCodes.Br_S, skipAppend);
                    il.MarkLabel(fieldNotNull);
                }

                il.Emit(OpCodes.Ldloc_0);  // sb
                il.Emit(OpCodes.Ldarg_0);  // this
                il.Emit(isValueType ? OpCodes.Ldflda : OpCodes.Ldfld, field);

                var convertToString = (GetPublicStaticMethod(typeof(Convert), "ToString", field.FieldType) ||
                                       GetPublicStaticMethod(typeof(Convert), "ToString", typeof(object)))
                                      .IfNone(() => throw new Exception());

                if (field.FieldType.GetTypeInfo().IsValueType && convertToString.GetParameters().Head().ParameterType == typeof(object))
                {
                    il.Emit(OpCodes.Box, field.FieldType);
                }

                il.Emit(convertToString.IsVirtual ? OpCodes.Callvirt : OpCodes.Call, convertToString);

                il.Emit(OpCodes.Callvirt, appendString);
                il.Emit(OpCodes.Pop);
                il.MarkLabel(skipAppend);

                first = false;
            }

            if (fields.Length > 0)
            {
                // Append(')')
                il.Emit(OpCodes.Ldloc_0);
                il.Emit(OpCodes.Ldc_I4_S, ')');
                il.Emit(OpCodes.Callvirt, appendChar);
                il.Emit(OpCodes.Pop);
            }

            // return sb.ToString()
            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Callvirt, toString);
            il.Emit(OpCodes.Ret);

            return (Func<A, string>)dynamic.CreateDelegate(typeof(Func<A, string>));
        }

        static Action<A, SerializationInfo> GetObjectDataExpr<A>(bool includeBase)
        {
            var isValueType   = typeof(A).GetTypeInfo().IsValueType;
            var fields        = GetPublicInstanceFields<A>(
                                       includeBase,
                                       #pragma warning disable CS0618
                                       typeof(OptOutOfSerializationAttribute),
                                       #pragma warning restore CS0618
                                       typeof(NonSerializedAttribute),
                                       typeof(NonRecordAttribute)
                                   );
            var argNullExcept = GetConstructor<ArgumentNullException, string>().IfNone(() => throw new Exception());
            var self          = Expression.Parameter(typeof(A), "self");
            var info          = Expression.Parameter(typeof(SerializationInfo), "info");

            var returnTarget = Expression.Label();

            Expression WriteField(FieldInfo field)
            {
                var name = PrettyFieldName(field);
                var addValue = (GetPublicInstanceMethod<SerializationInfo>("AddValue", typeof(string), field.FieldType, true) ||
                                GetPublicInstanceMethod<SerializationInfo>("AddValue", typeof(string), typeof(object), true))
                               .IfNone(() => throw new Exception());

                return addValue.GetParameters()[1].ParameterType == typeof(object)
                           ? Expression.Call(info, addValue, Expression.Constant(name), Expression.Convert(Expression.Field(self, field), typeof(object)))
                           : Expression.Call(info, addValue, Expression.Constant(name), Expression.Field(self, field));
            }
            
            var block = Expression.Block(
                Expression.IfThen(
                    Expression.ReferenceEqual(info, Expression.Constant(null, typeof(SerializationInfo))),
                    Expression.Throw(Expression.New(argNullExcept, Expression.Constant("info")))),
                Expression.Block(fields.Select(WriteField)),
                Expression.Return(returnTarget),
                Expression.Label(returnTarget));
            
            var lambda = Expression.Lambda<Action<A, SerializationInfo>>(block, self, info);

            return lambda.Compile();                
        }

        public static Action<A, SerializationInfo> GetObjectData<A>(bool includeBase)
        {
            if (!ILCapability.Available)
            {
                return GetObjectDataExpr<A>(includeBase);
            }
            
            var isValueType = typeof(A).GetTypeInfo().IsValueType;
            var dynamic = new DynamicMethod(
                "GetObjectData", 
                null, 
                new[] { typeof(A), typeof(SerializationInfo) }, 
                typeof(A).Module,
                true);
            var fields = GetPublicInstanceFields<A>(
                includeBase,
#pragma warning disable CS0618
                typeof(OptOutOfSerializationAttribute),
#pragma warning restore CS0618
                typeof(NonSerializedAttribute),
                typeof(NonRecordAttribute)
                );
            var argNullExcept =  GetConstructor<ArgumentNullException, string>().IfNone(() => throw new Exception());
            var il = dynamic.GetILGenerator();

            var infoIsNotNull = il.DefineLabel();

            // if(info == null)
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Brtrue_S, infoIsNotNull);

            /// throw new ArgumentNullException("info");
            il.Emit(OpCodes.Ldstr, "info");
            il.Emit(OpCodes.Newobj, argNullExcept);
            il.Emit(OpCodes.Throw);

            il.MarkLabel(infoIsNotNull);

            foreach (var field in fields)
            {
                var name = PrettyFieldName(field);

                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Ldstr, name);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(isValueType ? OpCodes.Ldflda : OpCodes.Ldfld, field);

                var addValue = (GetPublicInstanceMethod<SerializationInfo>("AddValue", typeof(string), field.FieldType, true) ||
                                GetPublicInstanceMethod<SerializationInfo>("AddValue", typeof(string), typeof(object), true))
                               .IfNone(() => throw new Exception());
                if (field.FieldType.GetTypeInfo().IsValueType && addValue.GetParameters()[1].ParameterType == typeof(object))
                {
                    il.Emit(OpCodes.Box, field.FieldType);
                }

                il.Emit(OpCodes.Callvirt, addValue);
            }
            il.Emit(OpCodes.Ret);

            return (Action<A, SerializationInfo>)dynamic.CreateDelegate(typeof(Action<A, SerializationInfo>));
        }
        
        static Action<A, SerializationInfo> SetObjectDataExpr<A>(bool includeBase)
        {
            // Expression doesn't support setting of fields that are readonly or init only.
            // So we fall back to reflection for this.  Not ideal.
            
            var fields = GetPublicInstanceFields<A>(includeBase,
                #pragma warning disable CS0618
                typeof(OptOutOfSerializationAttribute),
                #pragma warning restore CS0618
                typeof(NonSerializedAttribute),
                typeof(NonRecordAttribute)
                );
            
            return (A self, SerializationInfo info) =>
            {
                var selfType = typeof(A);
                foreach (var field in fields)
                {
                    var name = PrettyFieldName(field);
                    field.SetValue(self, info.GetValue(name, field.FieldType));
                }
            };
        }        

        public static Action<A, SerializationInfo> SetObjectData<A>(bool includeBase)
        {
            if (!ILCapability.Available)
            {
                return SetObjectDataExpr<A>(includeBase);
            }
            
            var dynamic = new DynamicMethod("SetObjectData",
                                            null,
                                            new[] {typeof(A), typeof(SerializationInfo)},
                                            typeof(A).Module,
                                            true);
            var fields = GetPublicInstanceFields<A>(includeBase,
#pragma warning disable CS0618
                typeof(OptOutOfSerializationAttribute),
#pragma warning restore CS0618
                typeof(NonSerializedAttribute),
                typeof(NonRecordAttribute)
                );
            var getTypeFromHandle = GetPublicStaticMethod<Type, RuntimeTypeHandle>("GetTypeFromHandle").IfNone(() => throw new Exception());
            var getValue = GetPublicInstanceMethod<SerializationInfo, string, Type>("GetValue", true).IfNone(() => throw new Exception());
            var argNullExcept = GetConstructor<ArgumentNullException, string>().IfNone(() => throw new Exception());
            var il = dynamic.GetILGenerator();

            var infoIsNotNull = il.DefineLabel();

            // if(info == null)
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Brtrue_S, infoIsNotNull);

            /// throw new ArgumentNullException("info");
            il.Emit(OpCodes.Ldstr, "info");
            il.Emit(OpCodes.Newobj, argNullExcept);
            il.Emit(OpCodes.Throw);

            il.MarkLabel(infoIsNotNull);

            foreach (var field in fields)
            {
                var name = PrettyFieldName(field);

                il.Emit(OpCodes.Ldarg_0);                   // this
                il.Emit(OpCodes.Ldarg_1);                   // info
                il.Emit(OpCodes.Ldstr, name);               // field-name
                il.Emit(OpCodes.Ldtoken, field.FieldType);  // typeof(FieldType)
                il.Emit(OpCodes.Call, getTypeFromHandle);   // Type.GetTypeFromHandle(typeof(FieldType))
                il.Emit(OpCodes.Callvirt, getValue);        // info.GetValue("field-name", FieldType)
                if (field.FieldType.GetTypeInfo().IsValueType)
                {
                    il.Emit(OpCodes.Unbox_Any, field.FieldType);
                }
                else
                {
                    il.Emit(OpCodes.Castclass, field.FieldType);
                }
                il.Emit(OpCodes.Stfld, field);
            }
            il.Emit(OpCodes.Ret);

            return (Action<A, SerializationInfo>)dynamic.CreateDelegate(typeof(Action<A, SerializationInfo>));
        }
        
        public static Func<A, B> GetPropertyOrField<A, B>(string name) =>
            GetProperty<A, B>(name) ?? GetField<A, B>(name);

        public static Func<A, B> GetProperty<A, B>(string name)
        {
            var m = typeof(A).GetMethod($"get_{name}");
            if (m == null) return null;
            if (m.ReturnType != typeof(B)) return null;
            
            if (ILCapability.Available)
            {
                var arg = typeof(A);
                var dynamic = new DynamicMethod(
                    $"{typeof(A).Name}_{name}",
                    typeof(B),
                    new[] {arg},
                    typeof(A).Module,
                    true);

                var il = dynamic.GetILGenerator();
                il.DeclareLocal(typeof(B));
                if (arg.IsValueType)
                {
                    il.Emit(OpCodes.Ldarga_S, 0);
                }
                else
                {
                    il.Emit(OpCodes.Ldarg_0);
                }

                if (m.IsVirtual)
                {
                    il.Emit(OpCodes.Callvirt, m);
                }
                else
                {
                    il.Emit(OpCodes.Call, m);
                }

                il.Emit(OpCodes.Stloc_0);
                il.Emit(OpCodes.Ldloc_0);
                il.Emit(OpCodes.Ret);

                return (Func<A, B>)dynamic.CreateDelegate(typeof(Func<A, B>));
            }
            else
            {
                var larg0  = Expression.Parameter(typeof(A), "arg0");
                var expr   = Expression.Property(larg0, m);
                var lambda = Expression.Lambda<Func<A, B>>(expr, larg0);
                return lambda.Compile();
            }
        }
 
        public static Func<A, B> GetField<A, B>(string name)
        {
            var fld = typeof(A).GetField(name);
            if (fld == null) return null;
            if (fld.FieldType != typeof(B)) return null;

            if (ILCapability.Available)
            {
                var arg = typeof(A);
                var dynamic = new DynamicMethod(
                    $"{typeof(A).Name}_{name}",
                    typeof(B),
                    new[] {arg},
                    typeof(A).Module,
                    true);

                var il = dynamic.GetILGenerator();
                il.DeclareLocal(typeof(B));
                if (arg.IsValueType)
                {
                    il.Emit(OpCodes.Ldarga_S, 0);
                }
                else
                {
                    il.Emit(OpCodes.Ldarg_0);
                }

                il.Emit(OpCodes.Ldfld, fld);
                il.Emit(OpCodes.Stloc_0);
                il.Emit(OpCodes.Ldloc_0);
                il.Emit(OpCodes.Ret);

                return (Func<A, B>)dynamic.CreateDelegate(typeof(Func<A, B>));
            }
            else
            {
                var larg0  = Expression.Parameter(typeof(A), "arg0");
                var expr   = Expression.Field(larg0, fld);
                var lambda = Expression.Lambda<Func<A, B>>(expr, larg0);
                return lambda.Compile();
            }
        }

        static string PrettyFieldName(FieldInfo field) =>
            field.Name.Split('<', '>').Match(
                ()      => "",
                x       => x,
                (x, xs) => xs.Head);
    }
    
    public static class ILCapability
    {
        public static readonly bool Available;

        static ILCapability() =>
            Available = GetAvailability();

        static bool GetAvailability()
        {
            try
            {
                TestSystemExceptionCtor();
                return true;
            }
            catch (PlatformNotSupportedException)
            {
                return false;
            }
        }

        static Func<SystemException> TestSystemExceptionCtor()
        {
            var type = typeof(SystemException);
            var ctor = type.GetConstructor(new Type[0]);

            var dynamic = new DynamicMethod("CreateInstance",
                                            type,
                                            new Type[0],
                                            true);

            var il = dynamic.GetILGenerator();
            il.Emit(OpCodes.Newobj, ctor);
            il.Emit(OpCodes.Ret);

            return (Func<SystemException>)dynamic.CreateDelegate(typeof(Func<SystemException>));
        }
    }
}
