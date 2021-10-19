using System;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;

namespace LanguageExt
{
    public static partial class Prelude
    {
        /// <summary>
        /// Func type inference helper
        /// 
        /// Try it with lambdas, instead of doing:
        /// 
        ///     Func&lt;int,int,int&gt; add = (int x, int y) => x + y;
        /// 
        /// You can use this function and do:
        /// 
        ///     var add = fun((int x, int y) => x + y);
        /// 
        /// </summary>
        /// <param name="f">Function to infer</param>
        /// <returns>The same func you gave it, but allows the type system to work out what f is</returns>
        [Pure]
        public static Func<R> fun<R>(Func<R> f) => f;

        /// <summary>
        /// Func type inference helper
        /// 
        /// Try it with lambdas, instead of doing:
        /// 
        ///     Func&lt;int,int,int&gt; add = (int x, int y) => x + y;
        /// 
        /// You can use this function and do:
        /// 
        ///     var add = fun((int x, int y) => x + y);
        /// 
        /// </summary>
        /// <param name="f">Function to infer</param>
        /// <returns>The same func you gave it, but allows the type system to work out what f is</returns>
        [Pure]
        public static Func<T1, R> fun<T1, R>(Func<T1, R> f) => f;

        /// <summary>
        /// Func type inference helper
        /// 
        /// Try it with lambdas, instead of doing:
        /// 
        ///     Func&lt;int,int,int&gt; add = (int x, int y) => x + y;
        /// 
        /// You can use this function and do:
        /// 
        ///     var add = fun((int x, int y) => x + y);
        /// 
        /// </summary>
        /// <param name="f">Function to infer</param>
        /// <returns>The same func you gave it, but allows the type system to work out what f is</returns>
        [Pure]
        public static Func<T1, T2, R> fun<T1, T2, R>(Func<T1, T2, R> f) => f;

        /// <summary>
        /// Func type inference helper
        /// 
        /// Try it with lambdas, instead of doing:
        /// 
        ///     Func&lt;int,int,int&gt; add = (int x, int y) => x + y;
        /// 
        /// You can use this function and do:
        /// 
        ///     var add = fun((int x, int y) => x + y);
        /// 
        /// </summary>
        /// <param name="f">Function to infer</param>
        /// <returns>The same func you gave it, but allows the type system to work out what f is</returns>
        [Pure]
        public static Func<T1, T2, T3, R> fun<T1, T2, T3, R>(Func<T1, T2, T3, R> f) => f;

        /// <summary>
        /// Func type inference helper
        /// 
        /// Try it with lambdas, instead of doing:
        /// 
        ///     Func&lt;int,int,int&gt; add = (int x, int y) => x + y;
        /// 
        /// You can use this function and do:
        /// 
        ///     var add = fun((int x, int y) => x + y);
        /// 
        /// </summary>
        /// <param name="f">Function to infer</param>
        /// <returns>The same func you gave it, but allows the type system to work out what f is</returns>
        [Pure]
        public static Func<T1, T2, T3, T4, R> fun<T1, T2, T3, T4, R>(Func<T1, T2, T3, T4, R> f) => f;

        /// <summary>
        /// Func type inference helper
        /// 
        /// Try it with lambdas, instead of doing:
        /// 
        ///     Func&lt;int,int,int&gt; add = (int x, int y) => x + y;
        /// 
        /// You can use this function and do:
        /// 
        ///     var add = fun((int x, int y) => x + y);
        /// 
        /// </summary>
        /// <param name="f">Function to infer</param>
        /// <returns>The same func you gave it, but allows the type system to work out what f is</returns>
        [Pure]
        public static Func<T1, T2, T3, T4, T5, R> fun<T1, T2, T3, T4, T5, R>(Func<T1, T2, T3, T4, T5, R> f) => f;

        /// <summary>
        /// Func type inference helper
        /// 
        /// Try it with lambdas, instead of doing:
        /// 
        ///     Func&lt;int,int,int&gt; add = (int x, int y) => x + y;
        /// 
        /// You can use this function and do:
        /// 
        ///     var add = fun((int x, int y) => x + y);
        /// 
        /// </summary>
        /// <param name="f">Function to infer</param>
        /// <returns>The same func you gave it, but allows the type system to work out what f is</returns>
        [Pure]
        public static Func<T1, T2, T3, T4, T5, T6, R> fun<T1, T2, T3, T4, T5, T6, R>(Func<T1, T2, T3, T4, T5, T6, R> f) => f;

        /// <summary>
        /// Func type inference helper
        /// 
        /// Try it with lambdas, instead of doing:
        /// 
        ///     Func&lt;int,int,int&gt; add = (int x, int y) => x + y;
        /// 
        /// You can use this function and do:
        /// 
        ///     var add = fun((int x, int y) => x + y);
        /// 
        /// </summary>
        /// <param name="f">Function to infer</param>
        /// <returns>The same func you gave it, but allows the type system to work out what f is</returns>
        [Pure]
        public static Func<T1, T2, T3, T4, T5, T6, T7, R> fun<T1, T2, T3, T4, T5, T6, T7, R>(Func<T1, T2, T3, T4, T5, T6, T7, R> f) => f;

        /// <summary>
        /// Action type inference helper and converts it to a Func that returns a Unit instead of void
        /// 
        /// Try it with lambdas, instead of doing:
        /// 
        ///     Func&lt;string,Unit&gt; putStr = (string x) => { Console.WriteLine(x); return unit; }
        /// 
        /// You can use this function and do:
        /// 
        ///     var putStr = fun((string x) => Console.WriteLine(x) );
        /// 
        /// </summary>
        /// <param name="f">Function to infer</param>
        /// <returns>Func that returns a Unit</returns>
        [Pure]
        public static Func<Unit> fun(Action f) => () => { f(); return unit; };

        /// <summary>
        /// Action type inference helper and converts it to a Func that returns a Unit instead of void
        /// 
        /// Try it with lambdas, instead of doing:
        /// 
        ///     Func&lt;string,Unit&gt; putStr = (string x) => { Console.WriteLine(x); return unit; }
        /// 
        /// You can use this function and do:
        /// 
        ///     var putStr = fun((string x) => Console.WriteLine(x) );
        /// 
        /// </summary>
        /// <param name="f">Function to infer</param>
        /// <returns>Func that returns a Unit</returns>
        [Pure]
        public static Func<T1, Unit> fun<T1>(Action<T1> f) => (a1) => { f(a1); return unit; };

        /// <summary>
        /// Action type inference helper and converts it to a Func that returns a Unit instead of void
        /// 
        /// Try it with lambdas, instead of doing:
        /// 
        ///     Func&lt;string,Unit&gt; putStr = (string x) => { Console.WriteLine(x); return unit; }
        /// 
        /// You can use this function and do:
        /// 
        ///     var putStr = fun((string x) => Console.WriteLine(x) );
        /// 
        /// </summary>
        /// <param name="f">Function to infer</param>
        /// <returns>Func that returns a Unit</returns>
        [Pure]
        public static Func<T1, T2, Unit> fun<T1, T2>(Action<T1, T2> f) => (a1, a2) => { f(a1, a2); return unit; };

        /// <summary>
        /// Action type inference helper and converts it to a Func that returns a Unit instead of void
        /// 
        /// Try it with lambdas, instead of doing:
        /// 
        ///     Func&lt;string,Unit&gt; putStr = (string x) => { Console.WriteLine(x); return unit; }
        /// 
        /// You can use this function and do:
        /// 
        ///     var putStr = fun((string x) => Console.WriteLine(x) );
        /// 
        /// </summary>
        /// <param name="f">Function to infer</param>
        /// <returns>Func that returns a Unit</returns>
        [Pure]
        public static Func<T1, T2, T3, Unit> fun<T1, T2, T3>(Action<T1, T2, T3> f) => (a1, a2, a3) => { f(a1, a2, a3); return unit; };

        /// <summary>
        /// Action type inference helper and converts it to a Func that returns a Unit instead of void
        /// 
        /// Try it with lambdas, instead of doing:
        /// 
        ///     Func&lt;string,Unit&gt; putStr = (string x) => { Console.WriteLine(x); return unit; }
        /// 
        /// You can use this function and do:
        /// 
        ///     var putStr = fun((string x) => Console.WriteLine(x) );
        /// 
        /// </summary>
        /// <param name="f">Function to infer</param>
        /// <returns>Func that returns a Unit</returns>
        [Pure]
        public static Func<T1, T2, T3, T4, Unit> fun<T1, T2, T3, T4>(Action<T1, T2, T3, T4> f) => (a1, a2, a3, a4) => { f(a1, a2, a3, a4); return unit; };

        /// <summary>
        /// Action type inference helper and converts it to a Func that returns a Unit instead of void
        /// 
        /// Try it with lambdas, instead of doing:
        /// 
        ///     Func&lt;string,Unit&gt; putStr = (string x) => { Console.WriteLine(x); return unit; }
        /// 
        /// You can use this function and do:
        /// 
        ///     var putStr = fun((string x) => Console.WriteLine(x) );
        /// 
        /// </summary>
        /// <param name="f">Function to infer</param>
        /// <returns>Func that returns a Unit</returns>
        [Pure]
        public static Func<T1, T2, T3, T4, T5, Unit> fun<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> f) => (a1, a2, a3, a4, a5) => { f(a1, a2, a3, a4, a5); return unit; };

        /// <summary>
        /// Action type inference helper and converts it to a Func that returns a Unit instead of void
        /// 
        /// Try it with lambdas, instead of doing:
        /// 
        ///     Func&lt;string,Unit&gt; putStr = (string x) => { Console.WriteLine(x); return unit; }
        /// 
        /// You can use this function and do:
        /// 
        ///     var putStr = fun((string x) => Console.WriteLine(x) );
        /// 
        /// </summary>
        /// <param name="f">Function to infer</param>
        /// <returns>Func that returns a Unit</returns>
        [Pure]
        public static Func<T1, T2, T3, T4, T5, T6, Unit> fun<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> f) => (a1, a2, a3, a4, a5, a6) => { f(a1, a2, a3, a4, a5, a6); return unit; };

        /// <summary>
        /// Action type inference helper and converts it to a Func that returns a Unit instead of void
        /// 
        /// Try it with lambdas, instead of doing:
        /// 
        ///     Func&lt;string,Unit&gt; putStr = (string x) => { Console.WriteLine(x); return unit; }
        /// 
        /// You can use this function and do:
        /// 
        ///     var putStr = fun((string x) => Console.WriteLine(x) );
        /// 
        /// </summary>
        /// <param name="f">Function to infer</param>
        /// <returns>Func that returns a Unit</returns>
        [Pure]
        public static Func<T1, T2, T3, T4, T5, T6, T7, Unit> fun<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> f) => (a1, a2, a3, a4, a5, a6, a7) => { f(a1, a2, a3, a4, a5, a6, a7); return unit; };

        /// <summary>
        /// Action type inference helper
        /// 
        /// Try it with lambdas, instead of doing:
        /// 
        ///     Action&lt;string&gt; putStr = (string x) => Console.WriteLine(x);
        /// 
        /// You can use this function and do:
        /// 
        ///     var putStr = act((string x) => Console.WriteLine(x));
        /// 
        /// </summary>
        /// <param name="f">Action to infer</param>
        /// <returns>The same Action you gave it, but allows the type system to work out what f is</returns>
        [Pure]
        public static Action act(Action f) => f;

        /// <summary>
        /// Action type inference helper
        /// 
        /// Try it with lambdas, instead of doing:
        /// 
        ///     Action&lt;string&gt; putStr = (string x) => Console.WriteLine(x);
        /// 
        /// You can use this function and do:
        /// 
        ///     var putStr = act((string x) => Console.WriteLine(x));
        /// 
        /// </summary>
        /// <param name="f">Action to infer</param>
        /// <returns>The same Action you gave it, but allows the type system to work out what f is</returns>
        [Pure]
        public static Action<T1> act<T1>(Action<T1> f) => f;

        /// <summary>
        /// Action type inference helper
        /// 
        /// Try it with lambdas, instead of doing:
        /// 
        ///     Action&lt;string&gt; putStr = (string x) => Console.WriteLine(x);
        /// 
        /// You can use this function and do:
        /// 
        ///     var putStr = act((string x) => Console.WriteLine(x));
        /// 
        /// </summary>
        /// <param name="f">Action to infer</param>
        /// <returns>The same Action you gave it, but allows the type system to work out what f is</returns>
        [Pure]
        public static Action<T1, T2> act<T1, T2>(Action<T1, T2> f) => f;

        /// <summary>
        /// Action type inference helper
        /// 
        /// Try it with lambdas, instead of doing:
        /// 
        ///     Action&lt;string&gt; putStr = (string x) => Console.WriteLine(x);
        /// 
        /// You can use this function and do:
        /// 
        ///     var putStr = act((string x) => Console.WriteLine(x));
        /// 
        /// </summary>
        /// <param name="f">Action to infer</param>
        /// <returns>The same Action you gave it, but allows the type system to work out what f is</returns>
        [Pure]
        public static Action<T1, T2, T3> act<T1, T2, T3>(Action<T1, T2, T3> f) => f;

        /// <summary>
        /// Action type inference helper
        /// 
        /// Try it with lambdas, instead of doing:
        /// 
        ///     Action&lt;string&gt; putStr = (string x) => Console.WriteLine(x);
        /// 
        /// You can use this function and do:
        /// 
        ///     var putStr = act((string x) => Console.WriteLine(x));
        /// 
        /// </summary>
        /// <param name="f">Action to infer</param>
        /// <returns>The same Action you gave it, but allows the type system to work out what f is</returns>
        [Pure]
        public static Action<T1, T2, T3, T4> act<T1, T2, T3, T4>(Action<T1, T2, T3, T4> f) => f;

        /// <summary>
        /// Action type inference helper
        /// 
        /// Try it with lambdas, instead of doing:
        /// 
        ///     Action&lt;string&gt; putStr = (string x) => Console.WriteLine(x);
        /// 
        /// You can use this function and do:
        /// 
        ///     var putStr = act((string x) => Console.WriteLine(x));
        /// 
        /// </summary>
        /// <param name="f">Action to infer</param>
        /// <returns>The same Action you gave it, but allows the type system to work out what f is</returns>
        [Pure]
        public static Action<T1, T2, T3, T4, T5> act<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> f) => f;

        /// <summary>
        /// Action type inference helper
        /// 
        /// Try it with lambdas, instead of doing:
        /// 
        ///     Action&lt;string&gt; putStr = (string x) => Console.WriteLine(x);
        /// 
        /// You can use this function and do:
        /// 
        ///     var putStr = act((string x) => Console.WriteLine(x));
        /// 
        /// </summary>
        /// <param name="f">Action to infer</param>
        /// <returns>The same Action you gave it, but allows the type system to work out what f is</returns>
        [Pure]
        public static Action<T1, T2, T3, T4, T5, T6> act<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> f) => f;

        /// <summary>
        /// Action type inference helper
        /// 
        /// Try it with lambdas, instead of doing:
        /// 
        ///     Action&lt;string&gt; putStr = (string x) => Console.WriteLine(x);
        /// 
        /// You can use this function and do:
        /// 
        ///     var putStr = act((string x) => Console.WriteLine(x));
        /// 
        /// </summary>
        /// <param name="f">Action to infer</param>
        /// <returns>The same Action you gave it, but allows the type system to work out what f is</returns>
        [Pure]
        public static Action<T1, T2, T3, T4, T5, T6, T7> act<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> f) => f;

        /// <summary>
        /// Func type inference helper; converts it to an Action by dropping the return value
        /// 
        /// Try it with lambdas, instead of doing:
        /// 
        ///     Func&lt;string, string&gt; thereIs = ...
        /// 
        ///     Action&lt;string,Unit&gt; thereIsNoReturn = (string x) => { thereis(x); };
        /// 
        /// You can use this function and do:
        /// 
        ///     var thereIsNoReturn = act(thereIs);
        /// 
        /// </summary>
        /// <param name="f">Function to infer</param>
        /// <returns>Action that is the same as the Func passed in, but with the return type dropped</returns>
        [Pure]
        public static Action act<R>(Func<R> f) => () => f();

        /// <summary>
        /// Func type inference helper; converts it to an Action by dropping the return value
        /// 
        /// Try it with lambdas, instead of doing:
        /// 
        ///     Func&lt;string, string&gt; thereIs = ...
        /// 
        ///     Action&lt;string,Unit&gt; thereIsNoReturn = (string x) => { thereis(x); };
        /// 
        /// You can use this function and do:
        /// 
        ///     var thereIsNoReturn = act(thereIs);
        /// 
        /// </summary>
        /// <param name="f">Function to infer</param>
        /// <returns>Action that is the same as the Func passed in, but with the return type dropped</returns>
        [Pure]
        public static Action<T1> act<T1, R>(Func<T1, R> f) => a1 => f(a1);

        /// <summary>
        /// Func type inference helper; converts it to an Action by dropping the return value
        /// 
        /// Try it with lambdas, instead of doing:
        /// 
        ///     Func&lt;string, string&gt; thereIs = ...
        /// 
        ///     Action&lt;string,Unit&gt; thereIsNoReturn = (string x) => { thereis(x); };
        /// 
        /// You can use this function and do:
        /// 
        ///     var thereIsNoReturn = act(thereIs);
        /// 
        /// </summary>
        /// <param name="f">Function to infer</param>
        /// <returns>Action that is the same as the Func passed in, but with the return type dropped</returns>
        [Pure]
        public static Action<T1, T2> act<T1, T2, R>(Func<T1, T2, R> f) => (a1, a2) => f(a1, a2);

        /// <summary>
        /// Func type inference helper; converts it to an Action by dropping the return value
        /// 
        /// Try it with lambdas, instead of doing:
        /// 
        ///     Func&lt;string, string&gt; thereIs = ...
        /// 
        ///     Action&lt;string,Unit&gt; thereIsNoReturn = (string x) => { thereis(x); };
        /// 
        /// You can use this function and do:
        /// 
        ///     var thereIsNoReturn = act(thereIs);
        /// 
        /// </summary>
        /// <param name="f">Function to infer</param>
        /// <returns>Action that is the same as the Func passed in, but with the return type dropped</returns>
        [Pure]
        public static Action<T1, T2, T3> act<T1, T2, T3, R>(Func<T1, T2, T3, R> f) => (a1, a2, a3) => f(a1, a2, a3);

        /// <summary>
        /// Func type inference helper; converts it to an Action by dropping the return value
        /// 
        /// Try it with lambdas, instead of doing:
        /// 
        ///     Func&lt;string, string&gt; thereIs = ...
        /// 
        ///     Action&lt;string,Unit&gt; thereIsNoReturn = (string x) => { thereis(x); };
        /// 
        /// You can use this function and do:
        /// 
        ///     var thereIsNoReturn = act(thereIs);
        /// 
        /// </summary>
        /// <param name="f">Function to infer</param>
        /// <returns>Action that is the same as the Func passed in, but with the return type dropped</returns>
        [Pure]
        public static Action<T1, T2, T3, T4> act<T1, T2, T3, T4, R>(Func<T1, T2, T3, T4, R> f) => (a1, a2, a3, a4) => f(a1, a2, a3, a4);

        /// <summary>
        /// Func type inference helper; converts it to an Action by dropping the return value
        /// 
        /// Try it with lambdas, instead of doing:
        /// 
        ///     Func&lt;string, string&gt; thereIs = ...
        /// 
        ///     Action&lt;string,Unit&gt; thereIsNoReturn = (string x) => { thereis(x); };
        /// 
        /// You can use this function and do:
        /// 
        ///     var thereIsNoReturn = act(thereIs);
        /// 
        /// </summary>
        /// <param name="f">Function to infer</param>
        /// <returns>Action that is the same as the Func passed in, but with the return type dropped</returns>
        [Pure]
        public static Action<T1, T2, T3, T4, T5> act<T1, T2, T3, T4, T5, R>(Func<T1, T2, T3, T4, T5, R> f) => (a1, a2, a3, a4, a5) => f(a1, a2, a3, a4, a5);

        /// <summary>
        /// Func type inference helper; converts it to an Action by dropping the return value
        /// 
        /// Try it with lambdas, instead of doing:
        /// 
        ///     Func&lt;string, string&gt; thereIs = ...
        /// 
        ///     Action&lt;string,Unit&gt; thereIsNoReturn = (string x) => { thereis(x); };
        /// 
        /// You can use this function and do:
        /// 
        ///     var thereIsNoReturn = act(thereIs);
        /// 
        /// </summary>
        /// <param name="f">Function to infer</param>
        /// <returns>Action that is the same as the Func passed in, but with the return type dropped</returns>
        [Pure]
        public static Action<T1, T2, T3, T4, T5, T6> act<T1, T2, T3, T4, T5, T6, R>(Func<T1, T2, T3, T4, T5, T6, R> f) => (a1, a2, a3, a4, a5, a6) => f(a1, a2, a3, a4, a5, a6);

        /// <summary>
        /// Func type inference helper; converts it to an Action by dropping the return value
        /// 
        /// Try it with lambdas, instead of doing:
        /// 
        ///     Func&lt;string, string&gt; thereIs = ...
        /// 
        ///     Action&lt;string,Unit&gt; thereIsNoReturn = (string x) => { thereis(x); };
        /// 
        /// You can use this function and do:
        /// 
        ///     var thereIsNoReturn = act(thereIs);
        /// 
        /// </summary>
        /// <param name="f">Function to infer</param>
        /// <returns>Action that is the same as the Func passed in, but with the return type dropped</returns>
        [Pure]
        public static Action<T1, T2, T3, T4, T5, T6, T7> act<T1, T2, T3, T4, T5, T6, T7, R>(Func<T1, T2, T3, T4, T5, T6, T7, R> f) => (a1, a2, a3, a4, a5, a6, a7) => f(a1, a2, a3, a4, a5, a6, a7);

        /// <summary>
        /// Expression inference
        /// </summary>
        /// <returns>Same expression passed in, just gives the type system a chance to infer</returns>
        [Pure]
        public static Expression<Func<R>> expr<R>(Expression<Func<R>> f) => f;

        /// <summary>
        /// Expression inference
        /// </summary>
        /// <returns>Same expression passed in, just gives the type system a chance to infer</returns>
        [Pure]
        public static Expression<Func<T1, R>> expr<T1, R>(Expression<Func<T1, R>> f) => f;

        /// <summary>
        /// Expression inference
        /// </summary>
        /// <returns>Same expression passed in, just gives the type system a chance to infer</returns>
        [Pure]
        public static Expression<Func<T1, T2, R>> expr<T1, T2, R>(Expression<Func<T1, T2, R>> f) => f;

        /// <summary>
        /// Expression inference
        /// </summary>
        /// <returns>Same expression passed in, just gives the type system a chance to infer</returns>
        [Pure]
        public static Expression<Func<T1, T2, T3, R>> expr<T1, T2, T3, R>(Expression<Func<T1, T2, T3, R>> f) => f;

        /// <summary>
        /// Expression inference
        /// </summary>
        /// <returns>Same expression passed in, just gives the type system a chance to infer</returns>
        [Pure]
        public static Expression<Func<T1, T2, T3, T4, R>> expr<T1, T2, T3, T4, R>(Expression<Func<T1, T2, T3, T4, R>> f) => f;

        /// <summary>
        /// Expression inference
        /// </summary>
        /// <returns>Same expression passed in, just gives the type system a chance to infer</returns>
        [Pure]
        public static Expression<Func<T1, T2, T3, T4, T5, R>> expr<T1, T2, T3, T4, T5, R>(Expression<Func<T1, T2, T3, T4, T5, R>> f) => f;

        /// <summary>
        /// Expression inference
        /// </summary>
        /// <returns>Same expression passed in, just gives the type system a chance to infer</returns>
        [Pure]
        public static Expression<Func<T1, T2, T3, T4, T5, T6, R>> expr<T1, T2, T3, T4, T5, T6, R>(Expression<Func<T1, T2, T3, T4, T5, T6, R>> f) => f;

        /// <summary>
        /// Expression inference
        /// </summary>
        /// <returns>Same expression passed in, just gives the type system a chance to infer</returns>
        [Pure]
        public static Expression<Func<T1, T2, T3, T4, T5, T6, T7, R>> expr<T1, T2, T3, T4, T5, T6, T7, R>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, R>> f) => f;

        /// <summary>
        /// Expression inference
        /// </summary>
        /// <returns>Same expression passed in, just gives the type system a chance to infer</returns>
        [Pure]
        public static Expression<Action> expr(Expression<Action> f) => f;

        /// <summary>
        /// Expression inference
        /// </summary>
        /// <returns>Same expression passed in, just gives the type system a chance to infer</returns>
        [Pure]
        public static Expression<Action<T1>> expr<T1>(Expression<Action<T1>> f) => f;

        /// <summary>
        /// Expression inference
        /// </summary>
        /// <returns>Same expression passed in, just gives the type system a chance to infer</returns>
        [Pure]
        public static Expression<Action<T1, T2>> expr<T1, T2>(Expression<Action<T1, T2>> f) => f;

        /// <summary>
        /// Expression inference
        /// </summary>
        /// <returns>Same expression passed in, just gives the type system a chance to infer</returns>
        [Pure]
        public static Expression<Action<T1, T2, T3>> expr<T1, T2, T3>(Expression<Action<T1, T2, T3>> f) => f;

        /// <summary>
        /// Expression inference
        /// </summary>
        /// <returns>Same expression passed in, just gives the type system a chance to infer</returns>
        [Pure]
        public static Expression<Action<T1, T2, T3, T4>> expr<T1, T2, T3, T4>(Expression<Action<T1, T2, T3, T4>> f) => f;

        /// <summary>
        /// Expression inference
        /// </summary>
        /// <returns>Same expression passed in, just gives the type system a chance to infer</returns>
        [Pure]
        public static Expression<Action<T1, T2, T3, T4, T5>> expr<T1, T2, T3, T4, T5>(Expression<Action<T1, T2, T3, T4, T5>> f) => f;

        /// <summary>
        /// Expression inference
        /// </summary>
        /// <returns>Same expression passed in, just gives the type system a chance to infer</returns>
        [Pure]
        public static Expression<Action<T1, T2, T3, T4, T5, T6>> expr<T1, T2, T3, T4, T5, T6>(Expression<Action<T1, T2, T3, T4, T5, T6>> f) => f;

        /// <summary>
        /// Expression inference
        /// </summary>
        /// <returns>Same expression passed in, just gives the type system a chance to infer</returns>
        [Pure]
        public static Expression<Action<T1, T2, T3, T4, T5, T6, T7>> expr<T1, T2, T3, T4, T5, T6, T7>(Expression<Action<T1, T2, T3, T4, T5, T6, T7>> f) => f;

        /// <summary>
        /// Function composition
        /// </summary>
        /// <returns>b(a(()))</returns>
        [Pure]
        public static Func<T2> compose<T1, T2>(Func<T1> a, Func<T1, T2> b) =>
            () => b(a());
        
        /// <summary>
        /// Function composition
        /// </summary>
        /// <returns>b(a(v))</returns>
        [Pure]
        public static Func<T1, T3> compose<T1, T2, T3>(Func<T1, T2> a, Func<T2, T3> b) =>
            v => b(a(v));

        /// <summary>
        /// Function composition
        /// </summary>
        /// <returns>c(b(a(v)))</returns>
        [Pure]
        public static Func<T1, T4> compose<T1, T2, T3, T4>(Func<T1, T2> a, Func<T2, T3> b, Func<T3, T4> c) =>
            v => c(b(a(v)));

        /// <summary>
        /// Function composition
        /// </summary>
        /// <returns>c(b(a(v)))</returns>
        [Pure]
        public static Func<T1, T5> compose<T1, T2, T3, T4, T5>(Func<T1, T2> a, Func<T2, T3> b, Func<T3, T4> c, Func<T4, T5> d) =>
            v => d(c(b(a(v))));

        /// <summary>
        /// Function composition
        /// </summary>
        /// <returns>c(b(a(v)))</returns>
        [Pure]
        public static Func<T1, T6> compose<T1, T2, T3, T4, T5, T6>(Func<T1, T2> a, Func<T2, T3> b, Func<T3, T4> c, Func<T4, T5> d, Func<T5, T6> e) =>
            v => e(d(c(b(a(v)))));

        /// <summary>
        /// Function composition
        /// </summary>
        /// <returns>c(b(a(v)))</returns>
        [Pure]
        public static Func<T1, T7> compose<T1, T2, T3, T4, T5, T6, T7>(Func<T1, T2> a, Func<T2, T3> b, Func<T3, T4> c, Func<T4, T5> d, Func<T5, T6> e, Func<T6, T7> f) =>
            v => f(e(d(c(b(a(v))))));
    }
}
