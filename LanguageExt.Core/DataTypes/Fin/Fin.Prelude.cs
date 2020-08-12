using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt;
using static LanguageExt.Prelude;
using static LanguageExt.TypeClass;
using static LanguageExt.Choice;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.IO;
using LanguageExt.TypeClasses;
using LanguageExt.ClassInstances;
using LanguageExt.Common;

namespace LanguageExt
{
    public static partial class Prelude
    {
        /// <summary>
        /// Monadic join
        /// </summary>
        [Pure]
        public static Fin<R> flatten<R>(Fin<Fin<R>> ma) =>
            ma.Bind(identity);

        /// <summary>
        /// Add the bound values of x and y, uses an Add type-class to provide the add
        /// operation for type A.  For example x.Add<TInteger,int>(y)
        /// </summary>
        /// <typeparam name="NUM">Num of A</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="x">Left hand side of the operation</param>
        /// <param name="y">Right hand side of the operation</param>
        /// <returns>Fin with y added to x</returns>
        [Pure]
        public static Fin<R> plus<NUM, R>(Fin<R> x, Fin<R> y) where NUM : struct, Num<R> =>
            from a in x
            from b in y
            select default(NUM).Plus(a, b);

        /// <summary>
        /// Find the subtract between the two bound values of x and y, uses a Subtract type-class 
        /// to provide the subtract operation for type A.  For example x.Subtract<TInteger,int>(y)
        /// </summary>
        /// <typeparam name="NUM">Num of A</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="x">Left hand side of the operation</param>
        /// <param name="y">Right hand side of the operation</param>
        /// <returns>Fin with the subtract between x and y</returns>
        [Pure]
        public static Fin<R> subtract<NUM, R>(Fin<R> x, Fin<R> y) where NUM : struct, Num<R> =>
            from a in x
            from b in y
            select default(NUM).Subtract(a, b);

        /// <summary>
        /// Find the product between the two bound values of x and y, uses a Product type-class 
        /// to provide the product operation for type A.  For example x.Product<TInteger,int>(y)
        /// </summary>
        /// <typeparam name="NUM">Num of A</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="x">Left hand side of the operation</param>
        /// <param name="y">Right hand side of the operation</param>
        /// <returns>Fin with the product of x and y</returns>
        [Pure]
        public static Fin<R> product<NUM, R>(Fin<R> x, Fin<R> y) where NUM : struct, Num<R> =>
            from a in x
            from b in y
            select default(NUM).Product(a, b);

        /// <summary>
        /// Divide the two bound values of x and y, uses a Divide type-class to provide the divide
        /// operation for type A.  For example x.Divide<TDouble,double>(y)
        /// </summary>
        /// <typeparam name="NUM">Num of A</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="x">Left hand side of the operation</param>
        /// <param name="y">Right hand side of the operation</param>
        /// <returns>Fin x / y</returns>
        [Pure]
        public static Fin<R> divide<NUM, R>(Fin<R> x, Fin<R> y) where NUM : struct, Num<R> =>
            from a in x
            from b in y
            select default(NUM).Divide(a, b);

        /// <summary>
        /// Apply
        /// </summary>
        /// <param name="fab">Function to apply the applicative to</param>
        /// <param name="fa">Applicative to apply</param>
        /// <returns>Applicative of type FB derived from Applicative of B</returns>
        [Pure]
        public static Fin<B> apply<A, B>(Fin<Func<A, B>> fab, Fin<A> fa)
        {
            if (fab.IsFail) return Fin<B>.Fail(fab.Error);
            if (fa.IsFail) return Fin<B>.Fail(fa.Error);
            return fab.Value(fa.Value);
        }

        /// <summary>
        /// Apply
        /// </summary>
        /// <param name="fab">Function to apply the applicative to</param>
        /// <param name="fa">Applicative to apply</param>
        /// <returns>Applicative of type FB derived from Applicative of B</returns>
        [Pure]
        public static Fin<B> apply<A, B>(Func<A, B> fab, Fin<A> fa) =>
            fa.Map(fab); 

        /// <summary>
        /// Apply
        /// </summary>
        /// <param name="fab">Function to apply the applicative to</param>
        /// <param name="fa">Applicative a to apply</param>
        /// <param name="fb">Applicative b to apply</param>
        /// <returns>Applicative of type FC derived from Applicative of C</returns>
        [Pure]
        public static Fin<C> apply<A, B, C>(Fin<Func<A, B, C>> fabc, Fin<A> fa, Fin<B> fb)
        {
            if (fabc.IsFail) return Fin<C>.Fail(fabc.Error);
            if (fa.IsFail) return Fin<C>.Fail(fa.Error);
            if (fb.IsFail) return Fin<C>.Fail(fb.Error);
            return fabc.Value(fa.Value, fb.Value);
        }

        /// <summary>
        /// Apply
        /// </summary>
        /// <param name="fab">Function to apply the applicative to</param>
        /// <param name="fa">Applicative a to apply</param>
        /// <param name="fb">Applicative b to apply</param>
        /// <returns>Applicative of type FC derived from Applicative of C</returns>
        [Pure]
        public static Fin<C> apply<A, B, C>(Func<A, B, C> fabc, Fin<A> fa, Fin<B> fb)
        {
            if (fa.IsFail) return Fin<C>.Fail(fa.Error);
            if (fb.IsFail) return Fin<C>.Fail(fb.Error);
            return fabc(fa.Value, fb.Value);
        }

        /// <summary>
        /// Apply
        /// </summary>
        /// <param name="fab">Function to apply the applicative to</param>
        /// <param name="fa">Applicative to apply</param>
        /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
        [Pure]
        public static Fin<Func<B, C>> apply<A, B, C>(Fin<Func<A, B, C>> fabc, Fin<A> fa)
        {
            if (fabc.IsFail) return Fin<Func<B, C>>.Fail(fabc.Error);
            if (fa.IsFail) return Fin<Func<B, C>>.Fail(fa.Error);
            return curry(fabc.Value)(fa.Value);
        }

        /// <summary>
        /// Apply
        /// </summary>
        /// <param name="fab">Function to apply the applicative to</param>
        /// <param name="fa">Applicative to apply</param>
        /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
        [Pure]
        public static Fin<Func<B, C>> apply<A, B, C>(Func<A, B, C> fabc, Fin<A> fa)
        {
            if (fa.IsFail) return Fin<Func<B, C>>.Fail(fa.Error);
            return curry(fabc)(fa.Value);
        }

        /// <summary>
        /// Apply
        /// </summary>
        /// <param name="fab">Function to apply the applicative to</param>
        /// <param name="fa">Applicative to apply</param>
        /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
        [Pure]
        public static Fin<Func<B, C>> apply<A, B, C>(Fin<Func<A, Func<B, C>>> fabc, Fin<A> fa)
        {
            if (fabc.IsFail) return Fin<Func<B, C>>.Fail(fabc.Error);
            if (fa.IsFail) return Fin<Func<B, C>>.Fail(fa.Error);
            return fabc.Value(fa.Value);
        }

        /// <summary>
        /// Apply
        /// </summary>
        /// <param name="fab">Function to apply the applicative to</param>
        /// <param name="fa">Applicative to apply</param>
        /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
        [Pure]
        public static Fin<Func<B, C>> apply<A, B, C>(Func<A, Func<B, C>> fabc, Fin<A> fa)
        {
            if (fa.IsFail) return Fin<Func<B, C>>.Fail(fa.Error);
            return fabc(fa.Value);
        }

        /// <summary>
        /// Evaluate fa, then fb, ignoring the result of fa
        /// </summary>
        /// <param name="fa">Applicative to evaluate first</param>
        /// <param name="fb">Applicative to evaluate second and then return</param>
        /// <returns>Applicative of type Fin<B></returns>
        [Pure]
        public static Fin<B> action<A, B>(Fin<A> fa, Fin<B> fb) =>
            fb;

        /// <summary>
        /// Returns the state of the Fin provided
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="value">Either to check</param>
        /// <returns>True if the Fin is in a Succ state</returns>
        [Pure]
        public static bool isSucc<A>(Fin<A> value) =>
            value.IsSucc;

        /// <summary>
        /// Returns the state of the Fin provided
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="value">Either to check</param>
        /// <returns>True if the Fin is in an Error state</returns>
        [Pure]
        public static bool isFail<A>(Fin<A> value) =>
            value.IsFail;

        /// <summary>
        /// Fin constructor
        /// Constructs a Fin in a success state
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="value">Success value</param>
        /// <returns>A new Fin instance</returns>
        [Pure]
        public static Fin<A> FinSucc<A>(A value) =>
            Fin<A>.Succ(value);

        /// <summary>
        /// Fin constructor
        /// Constructs a Fin in a failure state
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="value">Failure value</param>
        /// <returns>A new Fin instance</returns>
        [Pure]
        public static Fin<A> FinFail<A>(Error value) =>
            Fin<A>.Fail(value);

        /// <summary>
        /// Executes the Fail function if the Fin is in a failure state.
        /// Returns the bound value if the Fin is in a success state.
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="Fail">Function to generate a Fail value if in the failure state</param>
        /// <returns>Returns an unwrapped bound value</returns>
        [Pure]
        public static A ifFail<A>(Fin<A> ma, Func<Error, A> Fail) =>
            ma.IfFail(Fail);

        /// <summary>
        /// Returns the alternative if the Fin is in a failure state.
        /// Returns the bound value if the Fin is in a success state.
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="alternative">Value to return if in the failure state</param>
        /// <returns>Returns an unwrapped value</returns>
        [Pure]
        public static A ifFail<A>(Fin<A> ma, A alternative) =>
            ma.IfFail(alternative);

        /// <summary>
        /// Executes the Fail action if the Fin is in a failure state.
        /// </summary>
        /// <param name="Fail">Function to generate a value if in the failure state</param>
        /// <returns>Returns an unwrapped value</returns>
        [Pure]
        public static Unit ifFail<A>(Fin<A> ma, Action<Error> Left) =>
           ma.IfFail(Left);

        /// <summary>
        /// Invokes the Succ action if the Fin is in a success state, otherwise does nothing
        /// </summary>
        /// <param name="Succ">Action to invoke</param>
        /// <returns>Unit</returns>
        [Pure]
        public static Unit ifSucc<A>(Fin<A> ma, Action<A> Succ) =>
           ma.IfSucc(Succ);

        /// <summary>
        /// Invokes the Succ or Fail function depending on the state of the Fin provided
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <typeparam name="B">Return type</typeparam>
        /// <param name="ma">Fin to match</param>
        /// <param name="Succ">Function to invoke if in a Succ state</param>
        /// <param name="Fail">Function to invoke if in a Fail state</param>
        /// <returns>The return value of the invoked function</returns>
        [Pure]
        public static B match<A, B>(Fin<A> ma, Func<A, B> Succ, Func<Error, B> Fail) =>
            ma.Match(Succ, Fail);

        /// <summary>
        /// Invokes the Succ or Fail action depending on the state of the Fin provided
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ma">Fin to match</param>
        /// <param name="Succ">Action to invoke if in a Succ state</param>
        /// <param name="Fail">Action to invoke if in a Fail state</param>
        /// <returns>Unit</returns>
        public static Unit match<A>(Fin<A> ma, Action<A> Succ, Action<Error> Fail) =>
            ma.Match(Succ, Fail);

        /// <summary>
        /// <para>
        /// Fin types are like lists of 0 or 1 items, and therefore follow the 
        /// same rules when folding.
        /// </para><para>
        /// In the case of lists, 'Fold', when applied to a binary
        /// operator, a starting value(typically the left-identity of the operator),
        /// and a list, reduces the list using the binary operator, from left to
        /// right:
        /// </para>
        /// </summary>
        /// <typeparam name="S">Aggregate state type</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="folder">Folder function, applied if structure is in a Succ state</param>
        /// <returns>The aggregate state</returns>
        [Pure]
        public static S fold<S, A>(Fin<A> ma, S state, Func<S, A, S> folder) =>
            ma.Fold(state, folder);

        /// <summary>
        /// <para>
        /// Fin types are like lists of 0 or 1 items, and therefore follow the 
        /// same rules when folding.
        /// </para><para>
        /// In the case of lists, 'Fold', when applied to a binary
        /// operator, a starting value(typically the left-identity of the operator),
        /// and a list, reduces the list using the binary operator, from left to
        /// right:
        /// </para>
        /// </summary>
        /// <typeparam name="S">Aggregate state type</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="Right">Folder function, applied if Either is in a Succ state</param>
        /// <param name="Left">Folder function, applied if Either is in a Fail state</param>
        /// <returns>The aggregate state</returns>
        [Pure]
        public static S bifold<S, A>(Fin<A> ma, S state, Func<S, A, S> Right, Func<S, Error, S> Left) =>
            ma.BiFold(state, Right, Left);

        /// <summary>
        /// Invokes a predicate on the value of the Fin if it's in the Succ state
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ma">Fin to extract the value from</param>
        /// <param name="pred">Predicate</param>
        /// <returns>True if the Fin is in a Fail state.  
        /// True if the Fin is in a Right state and the predicate returns True.  
        /// False otherwise.</returns>
        [Pure]
        public static bool forall<A>(Fin<A> ma, Func<A, bool> pred) =>
            ma.ForAll(pred);

        /// <summary>
        /// Invokes a predicate on the value of the Fin if it's in the Succ state
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ma">Fin to extract the value from</param>
        /// <param name="pred">Predicate</param>
        /// <returns>True if the Fin is in a Succ state and the predicate returns True.  False otherwise.</returns>
        [Pure]
        public static bool exists<A>(Fin<A> ma, Func<A, bool> pred) =>
            ma.Exists(pred);

        /// <summary>
        /// Maps the value in the Either if it's in a Right state
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <typeparam name="B">Mapped bound value type</typeparam>
        /// <param name="ma">Either to map</param>
        /// <param name="f">Map function</param>
        /// <returns>Mapped Either</returns>
        [Pure]
        public static Fin<B> map<A, B>(Fin<A> ma, Func<A, B> f) =>
            ma.Map(f);

        /// <summary>
        /// Bi-maps the value in the Fin 
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <typeparam name="B">Mapped value type if in Succ state</typeparam>
        /// <param name="ma">Fin to map</param>
        /// <param name="Succ">Success state map function</param>
        /// <param name="Fail">Failure state map function</param>
        /// <returns>Bi-mapped Fin</returns>
        [Pure]
        public static Fin<B> bimap<A, B>(Fin<A> ma, Func<A, B> Succ, Func<Error, Error> Fail) =>
            ma.BiMap(Succ, Fail);

        /// <summary>
        /// Monadic bind function
        /// https://en.wikipedia.org/wiki/Monad_(functional_programming)
        /// </summary>
        [Pure]
        public static Fin<B> bind<A, B>(Fin<A> ma, Func<A, Fin<B>> f) =>
            ma.Bind(f);

        /// <summary>
        /// Match over a sequence of Fins
        /// </summary>
        /// <typeparam name="A">Bound values type</typeparam>
        /// <typeparam name="B">Mapped bound values type</typeparam>
        /// <param name="xs">Sequence to match over</param>
        /// <param name="Succ">Success state match function</param>
        /// <param name="Fail">Failure state match function</param>
        /// <returns>Sequence of mapped values</returns>
        [Pure]
        public static IEnumerable<B> Match<A, B>(this IEnumerable<Fin<A>> xs, Func<A, B> Succ, Func<Error, B> Fail ) =>
            match(xs, Succ, Fail);

        /// <summary>
        /// Match over a sequence of Fins
        /// </summary>
        /// <remarks>Bottom values are dropped</remarks>
        /// <typeparam name="A">Bound values type</typeparam>
        /// <typeparam name="B">Mapped bound values type</typeparam>
        /// <param name="xs">Sequence to match over</param>
        /// <param name="Succ">Success state match function</param>
        /// <param name="Fail">Failure state match function</param>
        /// <returns>Sequence of mapped values</returns>
        [Pure]
        public static IEnumerable<B> match<A, B>(IEnumerable<Fin<A>> xs, Func<A, B> Succ, Func<Error, B> Fail)
        {
            foreach (var item in xs)
            {
                if (item.IsBottom) continue;
                if (item.IsSucc) yield return Succ(item.Value);
                if (item.IsFail) yield return Fail(item.Error);
            }
        }

        /// <summary>
        /// Extracts from a list of Fins all the Succ elements.
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="xs">Sequence of Fins</param>
        /// <returns>An enumerable of A</returns>
        [Pure]
        public static IEnumerable<A> succs<A>(IEnumerable<Fin<A>> xs) =>
            xs.Where(x => x.IsSucc).Select(x => x.Value);

        /// <summary>
        /// Extracts from a list of Fins all the Succ elements.
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="xs">Sequence of Fins</param>
        /// <returns>An enumerable of A</returns>
        [Pure]
        public static Seq<A> succs<A>(Seq<Fin<A>> xs) =>
            xs.Where(x => x.IsSucc).Select(x => x.Value);

        /// <summary>
        /// Extracts from a list of Fins all the Fail elements.
        /// </summary>
        /// <remarks>Bottom values are dropped</remarks>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="xs">Sequence of Fins</param>
        /// <returns>An enumerable of Errors</returns>
        [Pure]
        public static IEnumerable<Error> fails<A>(IEnumerable<Fin<A>> xs) =>
            xs.Where(x => x.IsFail).Select(x => x.Error);

        /// <summary>
        /// Extracts from a list of Fins all the Fail elements.
        /// </summary>
        /// <remarks>Bottom values are dropped</remarks>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="xs">Sequence of Fins</param>
        /// <returns>An enumerable of Errors</returns>
        [Pure]
        public static Seq<Error> fails<A>(Seq<Fin<A>> xs) =>
            xs.Filter(x => x.IsFail).Map(x => x.Error);


        /// <summary>
        /// Partitions a list of 'Fin' into two lists.
        /// All the Fail elements are extracted, in order, to the first
        /// component of the output.  Similarly the Succ elements are extracted
        /// to the second component of the output.
        /// </summary>
        /// <remarks>Bottom values are dropped</remarks>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="xs">Fin list</param>
        /// <returns>A tuple containing the an enumerable of Erorr and an enumerable of Succ</returns>
        [Pure]
        public static (IEnumerable<Error> Fails, IEnumerable<A> Succs) partition<A>(IEnumerable<Fin<A>> xs)
        {
            var fs = new List<Error>();
            var rs = new List<A>();
            
            foreach(var x in xs)
            {
                if (x.IsSucc)
                {
                    rs.Add(x.Value);
                }
                if (x.IsFail)
                {
                    fs.Add(x.Error);
                }
            }

            return (fs, rs);
        }

        /// <summary>
        /// Partitions a list of 'Fin' into two lists.
        /// All the Fail elements are extracted, in order, to the first
        /// component of the output.  Similarly the Succ elements are extracted
        /// to the second component of the output.
        /// </summary>
        /// <remarks>Bottom values are dropped</remarks>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="xs">Fin list</param>
        /// <returns>A tuple containing the an enumerable of Erorr and an enumerable of Succ</returns>
        [Pure]
        public static (Seq<Error> Fails, Seq<A> Succs) partition<A>(Seq<Fin<A>> xs)
        {
            var fs = Seq<Error>();
            var rs = Seq<A>();
            
            foreach(var x in xs)
            {
                if (x.IsSucc)
                {
                    rs = rs.Add(x.Value);
                }
                if (x.IsFail)
                {
                    fs = fs.Add(x.Error);
                }
            }

            return (fs, rs);
        }
    }
}
