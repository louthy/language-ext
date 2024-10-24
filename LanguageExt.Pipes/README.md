_This feature of language-ext is based on the wonderful work of 
[Gabriella Gonzalez](https://twitter.com/GabriellaG439) on the [Haskell Pipes 
library](https://hackage.haskell.org/package/pipes-4.3.16/docs/Pipes.html).  I have 
had to make some significant changes to make it work in C#, but the essence is the 
same, and the core types and composition of the components is exactly the same._

* If you find this feature confusing at first, and it wouldn't be surprising as
  it's quite a complex idea, there are some examples in the [EffectsExample sample in the repo](https://github.com/louthy/language-ext/blob/main/Samples/EffectsExamples/Examples/TextFileChunkStreamExample.cs)

Conventional stream programming forces you to choose only two of the
following three features:

1. **Effects**
2. **Streaming**
3. **Composability**

If you sacrifice _Effects_ you get `IEnumerable`, which you
can transform using composable functions in constant space, but without
interleaving effects (other than of the _imperative kind_).

If you sacrifice _Streaming_ you get 'Traverse' and  'Sequence', which are 
composable and effectful, but do not return a single result until the whole 
list has first been processed and loaded into memory.

If you sacrifice _Composability_ you write a tightly coupled for loops,
and fire off imperative side-effects like they're going out of style.  Which 
is streaming and effectful, but is not modular or separable.

`Pipes` gives you all three features: effectful, streaming, and composable
programming.  `Pipes` also provides a wide variety of stream programming
abstractions which are all subsets of a single unified machinery:

* effectful [`Producer`](Producer),
* effectful [`Consumer`](Consumer),
* effectful [`Pipe`](Pipe) (like Unix pipes)

On top of that, `Pipes` has more advanced features, including bi-directional
streaming.  This comes into play when fusing clients and servers:

* effectful [`Client`](Client),
* effectful [`Server`](Server),

All of these are connectable and you can combine them together in clever and
unexpected ways because they all share the same underlying type.

The pipes ecosystem decouples stream processing stages from each other so
that you can mix and match diverse stages to produce useful streaming
programs.  If you are a library writer, pipes lets you package up streaming
components into a reusable interface.  If you are an application writer,
pipes lets you connect pre-made streaming components with minimal effort to
produce a highly-efficient program that streams data in constant memory.

To enforce loose coupling, components can only communicate using two commands:

* [`yield`](#Proxy_0_yield_1): Send output data
* [`awaiting`](#Proxy_0_awaiting_1): Receive input data

Pipes has four types of components built around these two commands:

* [`Producer`](Producer) can only [`yield`](#Proxy_0_yield_1) values and they model streaming sources
* [`Consumer`](Consumer) can only be [`awaiting`](#Proxy_0_awaiting_1) values and they model streaming sinks
* [`Pipe`](Pipe) can both [`yield`](#Proxy_0_yield_1) and be [`awaiting`](#Proxy_0_awaiting_1) values and they model stream transformations
* [`Effect`](Effect) can neither [`yield`](#Proxy_0_yield_1) nor be [`awaiting`](#Proxy_0_awaiting_1) and they model non-streaming components

Pipes uses parametric polymorphism (i.e. generics) to overload all operations.

You've probably noticed this overloading already:

 * [`yield`](#Proxy_0_yield_1) works within both [`Producer`](Producer) and a [`Pipe`](Pipe)
 * [`Consumer`](Consumer) works within both [`Consumer`](Consumer) and [`Pipe`](Pipe)
 * The operator `|` connects [`Producer`](Producer), [`Consumer`](Consumer), and [`Pipe`](Pipe) in varying ways

_This overloading is great when it works, but when connections fail they
produce type errors that appear intimidating at first.  This section
explains the underlying types so that you can work through type errors
intelligently._

`Producer`, `Consumer`, `Pipe`, and `Effect` are all special cases of a
single underlying type: [`Proxy`](#LanguageExt.Pipes_0_Proxy_6).  This overarching type permits fully
bidirectional communication on both an upstream and downstream interface.

You can think of it as having the following shape:
  
    Proxy<RT, UOut, UIn, DIn, DOut, A>

          Upstream | Downstream
              +---------+
              |         |
        UOut ◄--       ◄-- DIn   -- Information flowing upstream
              |         |
        UIn --►        --► DOut  -- Information flowing downstream
              |    |    |
              +----|----+
                   |
                   A
        
 The four core types do not use the upstream flow of information.  This means
 that the `UOut` and `DIn` in the above diagram go unused unless you use the
 more advanced features.

 Pipes uses type synonyms to hide unused inputs or outputs and clean up
 type signatures.  These type synonyms come in two flavors:

 * Concrete type synonyms that explicitly close unused inputs and outputs of
   the `Proxy` type

 * Polymorphic type synonyms that don't explicitly close unused inputs or
   outputs

 The concrete type synonyms use `Unit` to close unused inputs and `Void` (the
 uninhabited type) to close unused outputs:

 * `Effect`: explicitly closes both ends, forbidding `awaiting` and `yield`

        Effect<RT, A> = Proxy<RT, Void, Unit, Unit, Void, A>
         
                 Upstream | Downstream
                     +---------+
                     |         |
               Void ◄--       ◄-- Unit
                     |         |
               Unit --►       --► Void
                     |    |    |
                     +----|----+
                          |
                          A

 * `Producer`: explicitly closes the upstream end, forbidding `awaiting`

        Producer<RT, OUT, A> = Proxy<RT, Void, Unit, Unit, OUT, A>
         
                 Upstream | Downstream
                     +---------+
                     |         |
               Void ◄--       ◄-- Unit
                     |         |
               Unit --►       --► OUT
                     |    |    |
                     +----|----+
                          |
                          A
   
 * `Consumer`: explicitly closes the downstream end, forbidding `yield`

         Consumer<RT, IN, A> = Proxy<RT, Unit, IN, Unit, Void, A>
        
                 Upstream | Downstream
                     +---------+
                     |         |
               Unit ◄--       ◄-- Unit
                     |         |
                 IN --►       --► Void
                     |    |    |
                     +----|----+
                          |
                          A
          
 * `Pipe`: marks both ends open, allowing both `awaiting` and `yield`

         Pipe<RT, IN, OUT, A> = Proxy<RT, Unit, IN, Unit, OUT, A>
          
                 Upstream | Downstream
                     +---------+
                     |         |
               Unit ◄--       ◄-- Unit
                     |         |
                 IN --►       --► OUT
                     |    |    |
                     +----|----+
                          |
                          A
          
When you compose `Proxy` using `|` all you are doing is placing them
side by side and fusing them laterally.  For example, when you compose a
`Producer`, `Pipe`, and a `Consumer`, you can think of information flowing
like this:

                Producer                Pipe                 Consumer
             +------------+          +------------+          +-------------+
             |            |          |            |          |             |
       Void ◄--          ◄--  Unit   ◄--         ◄--  Unit  ◄--           ◄-- Unit
             |  readLine  |          |  parseInt  |          |  writeLine  |
       Unit --►         --► string  --►          --► string --►           --► Void
             |     |      |          |    |       |          |      |      |
             +-----|------+          +----|-------+          +------|------+
                   v                     v                       v
                   ()                    ()                      ()

 Composition fuses away the intermediate interfaces, leaving behind an `Effect`:

                           Effect
            +-----------------------------------+
            |                                   |
      Void ◄--                                 ◄-- Unit
            |  readLine | parseInt | writeLine  |
      Unit --►                                 --► Void
            |                                   |
            +----------------|------------------+
                            Unit
