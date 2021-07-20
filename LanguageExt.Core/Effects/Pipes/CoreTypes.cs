using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.Effects.Traits;

namespace LanguageExt.Pipes
{
    /// <summary>
    /// The pipes library decouples stream processing stages from each other so
    /// that you can mix and match diverse stages to produce useful streaming
    /// programs.  If you are a library writer, pipes lets you package up streaming
    /// components into a reusable interface.  If you are an application writer,
    /// pipes lets you connect pre-made streaming components with minimal effort to
    /// produce a highly-efficient program that streams data in constant memory.
    ///
    /// To enforce loose coupling, components can only communicate using two commands:
    /// 
    /// * `yield`: Send output data
    /// * `await`: Receive input data
    ///
    /// Pipes has four types of components built around these two commands:
    /// 
    /// * `Producer` can only `yield` values and they model streaming sources
    /// * `Consumer` can only `await` values and they model streaming sinks
    /// * `Pipe` can both `yield` and `await` values and they model stream transformations
    /// * `Effect` can neither `yield` nor `await` and they model non-streaming components
    /// 
    /// Pipes uses parametric polymorphism (i.e. generics) to overload all operations.
    ///
    /// You've probably noticed this overloading already:
    /// 
    ///  * `yield` works within both `Producer` and a `Pipe`
    ///  * `await` works within both `Consumer` and `Pipe`
    ///  * `|` connects `Producer`, `Consumer`, and `Pipe` in varying ways
    /// 
    /// This overloading is great when it works, but when connections fail they
    /// produce type errors that appear intimidating at first.  This section
    /// explains the underlying types so that you can work through type errors
    /// intelligently.
    /// 
    /// `Producer`, `Consumer`, `Pipe`, and `Effect` are all special cases of a
    /// single underlying type: `Proxy`.  This overarching type permits fully
    /// bidirectional communication on both an upstream and downstream interface.
    /// 
    /// You can think of it as having the following shape:
    /// 
    ///      Proxy a' a b' b m r
    ///
    ///        Upstream | Downstream
    ///            +---------+
    ///            |         |
    ///        a' ◄--       ◄-- b'  -- Information flowing upstream
    ///            |         |
    ///        a  --►       --► b   -- Information flowing downstream
    ///            |    |    |
    ///            +----|----+
    ///                 v
    ///                 r
    /// 
    ///  The four core types do not use the upstream flow of information.  This means
    ///  that the `a'` and `b'` in the above diagram go unused unless you use the
    ///  more advanced features.
    /// 
    ///  Pipes uses type synonyms to hide unused inputs or outputs and clean up
    ///  type signatures.  These type synonyms come in two flavors:
    /// 
    ///  * Concrete type synonyms that explicitly close unused inputs and outputs of
    ///    the 'Proxy' type
    /// 
    ///  * Polymorphic type synonyms that don't explicitly close unused inputs or
    ///    outputs
    /// 
    ///  The concrete type synonyms use `()` to close unused inputs and `Void` (the
    ///  uninhabited type) to close unused outputs:
    /// 
    ///  * `Effect`: explicitly closes both ends, forbidding `await` and `yield`
    /// 
    ///      type Effect = Proxy X () () X
    ///       
    ///        Upstream | Downstream
    ///            +---------+
    ///            |         |
    ///      Void ◄--       ◄-- ()
    ///            |         |
    ///        () --►       --► Void
    ///            |    |    |
    ///            +----|----+
    ///                 v
    ///                 r
    /// 
    ///  * `Producer`: explicitly closes the upstream end, forbidding `await`
    /// 
    ///      type Producer b = Proxy X () () b
    ///       
    ///        Upstream | Downstream
    ///            +---------+
    ///            |         |
    ///      Void ◄--       ◄-- ()
    ///            |         |
    ///        () --►       --► b
    ///            |    |    |
    ///            +----|----+
    ///                 v
    ///                 r
    /// 
    ///  * `Consumer`: explicitly closes the downstream end, forbidding `yield`
    /// 
    ///      type Consumer a = Proxy () a () X
    ///     
    ///        Upstream | Downstream
    ///            +---------+
    ///            |         |
    ///        () ◄--       ◄-- ()
    ///            |         |
    ///        a  --►       --► Void
    ///            |    |    |
    ///            +----|----+
    ///                 v
    ///                 r
    /// 
    ///  Pipe: marks both ends open, allowing both `await` and `yield`
    /// 
    ///        type Pipe a b = Proxy () a () b
    ///       
    ///        Upstream | Downstream
    ///            +---------+
    ///            |         |
    ///        () ◄--       ◄-- ()
    ///            |         |
    ///        a  --►       --► b
    ///            |    |    |
    ///            +----|----+
    ///                 v
    ///                 r
    /// 
    /// When you compose `Proxy` using `|` all you are doing is placing them
    /// side by side and fusing them laterally.  For example, when you compose a
    /// `Producer`, `Pipe`, and a `Consumer`, you can think of information flowing
    /// like this:
    /// 
    ///             Producer                Pipe                 Consumer
    ///          +-----------+          +----------+          +------------+
    ///          |           |          |          |          |            |
    ///    Void ◄--         ◄--   ()   ◄--        ◄--   ()   ◄--          ◄-- ()
    ///          |  stdinLn  |          |  take 3  |          |  stdoutLn  |
    ///      () --►         --► String --►        --► String --►          --► Void
    ///          |     |     |          |    |     |          |      |     |
    ///          +-----|-----+          +----|-----+          +------|-----+
    ///                v                     v                       v
    ///                ()                    ()                      ()
    /// 
    ///  Composition fuses away the intermediate interfaces, leaving behind an `Effect`:
    /// 
    ///                         Effect
    ///          +-----------------------------------+
    ///          |                                   |
    ///    Void ◄--                                 ◄-- ()
    ///          |  stdinLn >-> take 3 >-> stdoutLn  |
    ///      () --►                                 --► Void
    ///          |                                   |
    ///          +----------------|------------------+
    ///                           v
    ///                           () 
    /// </summary>
    public interface Proxy<in RT, in A1, in A, in B1, in B, out R>  where RT : struct, HasCancel<RT>
    {
        Proxy<RT, A1, A, B1, B, R> ToProxy();
    }

    public partial class Pure<RT, A1, A, B1, B, R> : Proxy<RT, A1, A, B1, B, R> where RT : struct, HasCancel<RT>
    {
        public readonly R Value;

        [Pure, MethodImpl(Proxy.mops)]
        public Proxy<RT, A1, A, B1, B, R> ToProxy() => this;

        public Pure(R value) =>
            Value = value;

        [Pure, MethodImpl(Proxy.mops)]
        public void Deconstruct(out R value) =>
            value = Value;
    }

    public partial class Request<RT, A1, A, B1, B, R> : Proxy<RT, A1, A, B1, B, R> where RT : struct, HasCancel<RT>
    {
        public readonly A1 Value;
        public readonly Func<A, Proxy<RT, A1, A, B1, B, R>> Fun;
        
        [MethodImpl(Proxy.mops)]
        public Request(A1 value, Func<A, Proxy<RT, A1, A, B1, B, R>> fun) =>
            (Value, Fun) = (value, fun);

        [Pure, MethodImpl(Proxy.mops)]
        public Proxy<RT, A1, A, B1, B, R> ToProxy() => this;

        [Pure, MethodImpl(Proxy.mops)]
        public void Deconstruct(out A1 value, out Func<A, Proxy<RT, A1, A, B1, B, R>> fun) =>
            (value, fun) = (Value, Fun);
    }

    public partial class Respond<RT, A1, A, B1, B, R> : Proxy<RT, A1, A, B1, B, R> where RT : struct, HasCancel<RT>
    {
        public readonly B Value;
        public readonly Func<B1, Proxy<RT, A1, A, B1, B, R>> Fun;
        
        [MethodImpl(Proxy.mops)]
        public Respond(B value, Func<B1, Proxy<RT, A1, A, B1, B, R>> fun) =>
            (Value, Fun) = (value, fun);

        [Pure, MethodImpl(Proxy.mops)]
        public Proxy<RT, A1, A, B1, B, R> ToProxy() => this;

        [Pure, MethodImpl(Proxy.mops)]
        public void Deconstruct(out B value, out Func<B1, Proxy<RT, A1, A, B1, B, R>> fun) =>
            (value, fun) = (Value, Fun);
    }

    public partial class M<RT, A1, A, B1, B, R> : Proxy<RT, A1, A, B1, B, R>  where RT : struct, HasCancel<RT>
    {
        public readonly Aff<RT, Proxy<RT, A1, A, B1, B, R>> Value;
        
        [MethodImpl(Proxy.mops)]
        public M(Aff<RT, Proxy<RT, A1, A, B1, B, R>> value) =>
            Value = value;
        
        [Pure, MethodImpl(Proxy.mops)]
        public Proxy<RT, A1, A, B1, B, R> ToProxy() => this;

        [Pure, MethodImpl(Proxy.mops)]
        public void Deconstruct(out Aff<RT, Proxy<RT, A1, A, B1, B, R>> value) =>
            value = Value;
    }
    
    /// <summary>
    /// Producers are effectful streams of input.  Specifically, a `Producer` is a
    /// monad transformer that extends the base IO monad with a new `yield` command.
    /// This `yield` command lets you send output downstream to an anonymous handler,
    /// decoupling how you generate values from how you consume them.
    /// </summary>
    public partial class Producer<RT, OUT, R> : Proxy<RT, Void, Unit, Unit, OUT, R> where RT : struct, HasCancel<RT>
    {
        public readonly Proxy<RT, Void, Unit, Unit, OUT, R> Value;
        
        [MethodImpl(Proxy.mops)]
        public Producer(Proxy<RT, Void, Unit, Unit, OUT, R> value) =>
            Value = value;

        [Pure, MethodImpl(Proxy.mops)]
        public Proxy<RT, Void, Unit, Unit, OUT, R> ToProxy() =>
            Value.ToProxy();
        
        // Pipe composition, analogous to the Unix pipe operator
        [Pure, MethodImpl(Proxy.mops)]
        public static Effect<RT, R> operator |(Producer<RT, OUT, R> p1, Consumer<RT, OUT, R> p2) => 
            Proxy.compose(p1, p2);
        
        // Pipe composition, analogous to the Unix pipe operator
        [Pure, MethodImpl(Proxy.mops)]
        public static Effect<RT, R> operator |(Producer<RT, OUT, R> p1, Consumer<OUT, R> p2) => 
            Proxy.compose(p1, p2);
        
        [Pure, MethodImpl(Proxy.mops)]
        public void Deconstruct(out Proxy<RT, Void, Unit, Unit, OUT, R> value) =>
            value = Value;

        [Pure, MethodImpl(Proxy.mops)]
        public static implicit operator Producer<RT, OUT, R>(Producer<OUT, R> p) =>
            p.Interpret<RT, OUT, R>();
    }

    /// <summary>
    /// Consumers only await
    /// </summary>
    public partial class Consumer<RT, IN, R> : Proxy<RT, Unit, IN, Unit, Void, R>  where RT : struct, HasCancel<RT>
    {
        public readonly Proxy<RT, Unit, IN, Unit, Void, R> Value;
        
        [MethodImpl(Proxy.mops)]
        public Consumer(Proxy<RT, Unit, IN, Unit, Void, R> value) =>
            Value = value;

        [Pure, MethodImpl(Proxy.mops)]
        public Proxy<RT, Unit, IN, Unit, Void, R> ToProxy() =>
            Value.ToProxy();

        [Pure, MethodImpl(Proxy.mops)]
        public void Deconstruct(out Proxy<RT, Unit, IN, Unit, Void, R> value) =>
            value = Value;

        [Pure, MethodImpl(Proxy.mops)]
        public static implicit operator Consumer<RT, IN, R>(Consumer<IN, R> c) =>
            c.Interpret<RT, IN, R>();
    }

    /// <summary>
    /// Pipes both await and yield
    /// </summary>
    /// <remarks>
    ///     Upstream | Downstream
    ///         +---------+
    ///         |         |
    ///     () <==       <== ()
    ///         |         |
    ///     A  ==>       ==> B
    ///         |    |    |
    ///         +----|----+
    ///              R
    /// </remarks>
    public partial class Pipe<RT, IN, OUT, R> : Proxy<RT, Unit, IN, Unit, OUT, R> where RT : struct, HasCancel<RT>
    {
        public readonly Proxy<RT, Unit, IN, Unit, OUT, R> Value;
        
        [MethodImpl(Proxy.mops)]
        public Pipe(Proxy<RT, Unit, IN, Unit, OUT, R> value) =>
            Value = value;
        
        [Pure, MethodImpl(Proxy.mops)]
        public Proxy<RT, Unit, IN, Unit, OUT, R> ToProxy() =>
            Value.ToProxy();

        [Pure, MethodImpl(Proxy.mops)]
        public void Deconstruct(out Proxy<RT, Unit, IN, Unit, OUT, R> value) =>
            value = Value;
        
        // 'Pipe' composition, analogous to the Unix pipe operator
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<RT, OUT, R> operator |(Producer<RT, IN, R> p1, Pipe<RT, IN, OUT, R> p2) =>
            Proxy.compose(p1, p2);
        
        // 'Pipe' composition, analogous to the Unix pipe operator
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<RT, OUT, R> operator |(Producer<IN, R> p1, Pipe<RT, IN, OUT, R> p2) =>
            Proxy.compose(p1, p2);
        
        // Pipe composition, analogous to the Unix pipe operator
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<RT, IN, R> operator |(Pipe<RT, IN, OUT, R> p1, Consumer<OUT, R> p2) =>
            Proxy.compose(p1, p2);
        
        // Pipe composition, analogous to the Unix pipe operator
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<RT, OUT, R> operator |(Producer<OUT, IN> p1, Pipe<RT, IN, OUT, R> p2) =>
            Proxy.compose(p1, p2);
        
        // Pipe composition, analogous to the Unix pipe operator
        [Pure, MethodImpl(Proxy.mops)]
        public Pipe<RT, IN, C, R> Then<C>(Pipe<RT, OUT, C, R> p2) =>
            Proxy.compose(this, p2);

        [Pure, MethodImpl(Proxy.mops)]
        public static implicit operator Pipe<RT, IN, OUT, R>(Pipe<IN, OUT, R> p) =>
            p.Interpret<RT, IN, OUT, R>();
    }

    /// <summary>
    /// Clients only request and never respond
    /// </summary>
    public partial class Client<RT, A, B, R> : Proxy<RT, A, B, Unit, Unit, R> where RT : struct, HasCancel<RT>
    {
        public readonly Proxy<RT, A, B, Unit, Unit, R> Value;

        [MethodImpl(Proxy.mops)]
        public Client(Proxy<RT, A, B, Unit, Unit, R> value) =>
            Value = value;
 
        [Pure, MethodImpl(Proxy.mops)]
        public Proxy<RT, A, B, Unit, Unit, R> ToProxy() =>
            Value.ToProxy();

        [Pure, MethodImpl(Proxy.mops)]
        public void Deconstruct(out Proxy<RT, A, B, Unit, Unit, R> value) =>
            value = Value;
    }

    /// <summary>
    /// Servers only respond and never request
    /// </summary>
    public partial class Server<RT, A, B, R> : Proxy<RT, Unit, Unit, A, B, R>  where RT : struct, HasCancel<RT>
    {
        public readonly Proxy<RT, Unit, Unit, A, B, R> Value;
    
        [MethodImpl(Proxy.mops)]
        public Server(Proxy<RT, Unit, Unit, A, B, R> value) =>
            Value = value;
        
        [Pure, MethodImpl(Proxy.mops)]
        public Proxy<RT, Unit, Unit, A, B, R> ToProxy() =>
            Value.ToProxy();

        [Pure, MethodImpl(Proxy.mops)]
        public void Deconstruct(out Proxy<RT, Unit, Unit, A, B, R> value) =>
            value = Value;
    }

    /// <summary>
    /// Effects represent a 'fused' set of producer, pipes, and consumer into one type
    /// It neither yields nor awaits, but represents an entire effect system
    /// </summary>
    public partial class Effect<RT, R> : Proxy<RT, Void, Unit, Unit, Void, R> where RT : struct, HasCancel<RT>
    {
        public readonly Proxy<RT, Void, Unit, Unit, Void, R> Value;

        [MethodImpl(Proxy.mops)]
        public Effect(Proxy<RT, Void, Unit, Unit, Void, R> value) =>
            Value = value;
        
        [Pure, MethodImpl(Proxy.mops)]
        public Proxy<RT, Void, Unit, Unit, Void, R> ToProxy() =>
            Value.ToProxy();

        [Pure, MethodImpl(Proxy.mops)]
        public void Deconstruct(out Proxy<RT, Void, Unit, Unit, Void, R> value) =>
            value = Value;
    }
}
