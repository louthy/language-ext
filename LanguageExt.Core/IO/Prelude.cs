using System;
using System.IO;
using LanguageExt.Common;
using LanguageExt.TypeClasses;
using System.Collections.Generic;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.Interfaces;

namespace LanguageExt
{
    /// <summary>
    /// IO prelude
    /// </summary>
    public static partial class Prelude
    {
        /// <summary>
        /// Sequentially run IO operations, returning the result of the last one
        /// </summary>
        public static IO<Env, R> io<Env, A, R>(
            IO<Env, A> ma,
            IO<Env, R> mr) where Env : struct, HasCancel<Env> =>
            from a in ma
            from r in mr
            select r;
        
        /// <summary>
        /// Sequentially run IO operations, returning the result of the last one
        /// </summary>
        public static IO<Env, R> io<Env, A, B, R>(
            IO<Env, A> ma,
            IO<Env, B> mb,
            IO<Env, R> mr) where Env : struct, HasCancel<Env> =>
            from a in ma
            from b in mb
            from r in mr
            select r;
        
        /// <summary>
        /// Sequentially run IO operations, returning the result of the last one
        /// </summary>
        public static IO<Env, R> io<Env, A, B, C, R>(
            IO<Env, A> ma,
            IO<Env, B> mb,
            IO<Env, C> mc,
            IO<Env, R> mr) where Env : struct, HasCancel<Env> =>
            from a in ma
            from b in mb
            from c in mc
            from r in mr
            select r;
        
        /// <summary>
        /// Sequentially run IO operations, returning the result of the last one
        /// </summary>
        public static IO<Env, R> io<Env, A, B, C, D, R>(
            IO<Env, A> ma,
            IO<Env, B> mb,
            IO<Env, C> mc,
            IO<Env, D> md,
            IO<Env, R> mr) where Env : struct, HasCancel<Env> =>
            from a in ma
            from b in mb
            from c in mc
            from d in md
            from r in mr
            select r;
        
        /// <summary>
        /// Sequentially run IO operations, returning the result of the last one
        /// </summary>
        public static IO<Env, R> io<Env, A, B, C, D, E, R>(
            IO<Env, A> ma,
            IO<Env, B> mb,
            IO<Env, C> mc,
            IO<Env, D> md,
            IO<Env, E> me,
            IO<Env, R> mr) where Env : struct, HasCancel<Env> =>
            from a in ma
            from b in mb
            from c in mc
            from d in md
            from e in me
            from r in mr
            select r;
        
        /// <summary>
        /// Sequentially run IO operations, returning the result of the last one
        /// </summary>
        public static IO<Env, R> io<Env, A, B, C, D, E, F, R>(
            IO<Env, A> ma,
            IO<Env, B> mb,
            IO<Env, C> mc,
            IO<Env, D> md,
            IO<Env, E> me,
            IO<Env, F> mf,
            IO<Env, R> mr) where Env : struct, HasCancel<Env> =>
            from a in ma
            from b in mb
            from c in mc
            from d in md
            from e in me
            from f in mf
            from r in mr
            select r;
        
        /// <summary>
        /// Sequentially run IO operations, returning the result of the last one
        /// </summary>
        public static IO<Env, R> io<Env, A, B, C, D, E, F, G, R>(
            IO<Env, A> ma,
            IO<Env, B> mb,
            IO<Env, C> mc,
            IO<Env, D> md,
            IO<Env, E> me,
            IO<Env, F> mf,
            IO<Env, G> mg,
            IO<Env, R> mr) where Env : struct, HasCancel<Env> =>
            from a in ma
            from b in mb
            from c in mc
            from d in md
            from e in me
            from f in mf
            from r in mr
            select r;
 
        
        // SIO
        
        /// <summary>
        /// Sequentially run IO operations, returning the result of the last one
        /// </summary>
        public static SIO<Env, R> io<Env, A, R>(
            SIO<Env, A> ma,
            SIO<Env, R> mr) =>
            from a in ma
            from r in mr
            select r;
        
        /// <summary>
        /// Sequentially run IO operations, returning the result of the last one
        /// </summary>
        public static SIO<Env, R> io<Env, A, B, R>(
            SIO<Env, A> ma,
            SIO<Env, B> mb,
            SIO<Env, R> mr) =>
            from a in ma
            from b in mb
            from r in mr
            select r;
        
        /// <summary>
        /// Sequentially run IO operations, returning the result of the last one
        /// </summary>
        public static SIO<Env, R> io<Env, A, B, C, R>(
            SIO<Env, A> ma,
            SIO<Env, B> mb,
            SIO<Env, C> mc,
            SIO<Env, R> mr) =>
            from a in ma
            from b in mb
            from c in mc
            from r in mr
            select r;
        
        /// <summary>
        /// Sequentially run IO operations, returning the result of the last one
        /// </summary>
        public static SIO<Env, R> io<Env, A, B, C, D, R>(
            SIO<Env, A> ma,
            SIO<Env, B> mb,
            SIO<Env, C> mc,
            SIO<Env, D> md,
            SIO<Env, R> mr) =>
            from a in ma
            from b in mb
            from c in mc
            from d in md
            from r in mr
            select r;
        
        /// <summary>
        /// Sequentially run IO operations, returning the result of the last one
        /// </summary>
        public static SIO<Env, R> io<Env, A, B, C, D, E, R>(
            SIO<Env, A> ma,
            SIO<Env, B> mb,
            SIO<Env, C> mc,
            SIO<Env, D> md,
            SIO<Env, E> me,
            SIO<Env, R> mr) =>
            from a in ma
            from b in mb
            from c in mc
            from d in md
            from e in me
            from r in mr
            select r;
        
        /// <summary>
        /// Sequentially run IO operations, returning the result of the last one
        /// </summary>
        public static SIO<Env, R> io<Env, A, B, C, D, E, F, R>(
            SIO<Env, A> ma,
            SIO<Env, B> mb,
            SIO<Env, C> mc,
            SIO<Env, D> md,
            SIO<Env, E> me,
            SIO<Env, F> mf,
            SIO<Env, R> mr) =>
            from a in ma
            from b in mb
            from c in mc
            from d in md
            from e in me
            from f in mf
            from r in mr
            select r;
        
        /// <summary>
        /// Sequentially run IO operations, returning the result of the last one
        /// </summary>
        public static SIO<Env, R> io<Env, A, B, C, D, E, F, G, R>(
            SIO<Env, A> ma,
            SIO<Env, B> mb,
            SIO<Env, C> mc,
            SIO<Env, D> md,
            SIO<Env, E> me,
            SIO<Env, F> mf,
            SIO<Env, G> mg,
            SIO<Env, R> mr) =>
            from a in ma
            from b in mb
            from c in mc
            from d in md
            from e in me
            from f in mf
            from g in mg
            from r in mr
            select r;
    }
}
