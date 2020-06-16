using System;
using System.Collections.Generic;
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

            var dynamic = new DynamicMethod("CreateInstance",
                                            ctorInfo.DeclaringType,
                                            ctorParams.Select(p => p.ParameterType).ToArray(),
                                            true);

            var il = dynamic.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Newobj, ctorInfo);
            il.Emit(OpCodes.Ret);

            return (Func<A, R>)dynamic.CreateDelegate(typeof(Func<A, R>));
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

            var dynamic = new DynamicMethod("CreateInstance",
                                            ctorInfo.DeclaringType,
                                            ctorParams.Select(p => p.ParameterType).ToArray(),
                                            true);

            var il = dynamic.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Newobj, ctorInfo);
            il.Emit(OpCodes.Ret);

            return (Func<A, B, R>)dynamic.CreateDelegate(typeof(Func<A, B, R>));
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

            var dynamic = new DynamicMethod("CreateInstance",
                                            ctorInfo.DeclaringType,
                                            ctorParams.Select(p => p.ParameterType).ToArray(),
                                            true);

            var il = dynamic.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Newobj, ctorInfo);
            il.Emit(OpCodes.Ret);

            return (Func<A, B, C, R>)dynamic.CreateDelegate(typeof(Func<A, B, C, R>));
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

            var ctorParams = ctorInfo.GetParameters();

            var dynamic = new DynamicMethod("CreateInstance",
                                            ctorInfo.DeclaringType,
                                            ctorParams.Select(p => p.ParameterType).ToArray(),
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

        /// <summary>
        /// Emits the IL to invoke a static method
        /// </summary>
        public static Option<Func<object, R>> Func1<TYPE, R>(Type arg1, Func<MethodInfo, bool> methodPred = null)
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
                    if (ps[0].ParameterType != arg1) return false;
                    return methodPred(x);
                })
                .FirstOrDefault();

            if (methodInfo == null) return None;

            var methodParams = methodInfo.GetParameters();

            var dynamic = new DynamicMethod("CreateInstance",
                                            typeof(R),
                                            methodParams.Select(p => typeof(object)).ToArray(),
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

            var methodParams = methodInfo.GetParameters();

            var dynamic = new DynamicMethod("CreateInstance",
                                            typeof(R),
                                            methodParams.Select(p => p.ParameterType).ToArray(),
                                            true);

            var il = dynamic.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, methodInfo);
            il.Emit(OpCodes.Ret);

            return (Func<A, R>)dynamic.CreateDelegate(typeof(Func<A, R>));
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

            var methodParams = methodInfo.GetParameters();

            var dynamic = new DynamicMethod("CreateInstance",
                                            typeof(R),
                                            methodParams.Select(p => p.ParameterType).ToArray(),
                                            true);

            var il = dynamic.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Call, methodInfo);
            il.Emit(OpCodes.Ret);

            return (Func<A, B, R>)dynamic.CreateDelegate(typeof(Func<A, B, R>));
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

            var methodParams = methodInfo.GetParameters();

            var dynamic = new DynamicMethod("CreateInstance",
                                            typeof(R),
                                            methodParams.Select(p => p.ParameterType).ToArray(),
                                            true);

            var il = dynamic.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Call, methodInfo);
            il.Emit(OpCodes.Ret);

            return (Func<A, B, C, R>)dynamic.CreateDelegate(typeof(Func<A, B, C, R>));
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

            var methodParams = methodInfo.GetParameters();

            var dynamic = new DynamicMethod("CreateInstance",
                                            typeof(R),
                                            methodParams.Select(p => p.ParameterType).ToArray(),
                                            true);

            var il = dynamic.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Ldarg_3);
            il.Emit(OpCodes.Call, methodInfo);
            il.Emit(OpCodes.Ret);

            return (Func<A, B, C, D, R >)dynamic.CreateDelegate(typeof(Func<A, B, C, D, R>));
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

            // Implement FNV 1a hashing algoritm - https://en.wikipedia.org/wiki/Fowler%E2%80%93Noll%E2%80%93Vo_hash_function#FNV-1a_hash
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
                        yield return new[] { comparer };
                    }
                    else
                    { 
                        var fnull = Expression.Constant(null, f.FieldType);

                        yield return new[] {
                            Expression.IfThen(
                                Expression.And(
                                    Expression.ReferenceEqual(Expression.PropertyOrField(self, f.Name), fnull),
                                    Expression.IsFalse(Expression.ReferenceEqual(Expression.PropertyOrField(other, f.Name), fnull))),
                                Expression.Return(returnTarget, Minus1)),

                            Expression.IfThen(
                                Expression.And(
                                    Expression.ReferenceEqual(Expression.PropertyOrField(other, f.Name), fnull),
                                    Expression.IsFalse(Expression.ReferenceEqual(Expression.PropertyOrField(self, f.Name), fnull))),
                                Expression.Return(returnTarget, Plus1)),

                            Expression.IfThenElse(
                                Expression.ReferenceEqual(
                                    Expression.PropertyOrField(self, f.Name),
                                    Expression.PropertyOrField(other, f.Name)),
                                Expression.Assign(ord, Zero),
                                comparer)
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

        public static Func<A, string> ToString<A>(bool includeBase)
        {
            var isValueType = typeof(A).GetTypeInfo().IsValueType;
            var dynamic = new DynamicMethod("FieldsToString", typeof(string), new[] { typeof(A) },true);
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


        public static Action<A, SerializationInfo> GetObjectData<A>(bool includeBase)
        {
            var isValueType = typeof(A).GetTypeInfo().IsValueType;
            var dynamic = new DynamicMethod("GetObjectData", null, new[] { typeof(A), typeof(SerializationInfo) }, true);
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

        public static Action<A, SerializationInfo> SetObjectData<A>(bool includeBase)
        {
            var dynamic = new DynamicMethod("SetObjectData", null, new[] { typeof(A), typeof(SerializationInfo) }, typeof(A), true);
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

            var arg = typeof(A);
            var dynamic = new DynamicMethod($"{typeof(A).Name}_{name}",
                typeof(B),
                new[] {arg},
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

            return (Func<A, B>) dynamic.CreateDelegate(typeof(Func<A, B>));
        }
 
        public static Func<A, B> GetField<A, B>(string name)
        {
            var fld = typeof(A).GetField(name);
            if (fld == null) return null;
            if (fld.FieldType != typeof(B)) return null;

            var arg = typeof(A);
            var dynamic = new DynamicMethod($"{typeof(A).Name}_{name}",
                typeof(B),
                new[] {arg},
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

            return (Func<A, B>) dynamic.CreateDelegate(typeof(Func<A, B>));
        }

        static string PrettyFieldName(FieldInfo field) =>
            field.Name.Split('<', '>').Match(
                ()      => "",
                x       => x,
                (x, xs) => xs.Head);
    }
}
