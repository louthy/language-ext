> If you find this feature confusing at first, and it wouldn't be surprising as
  it's quite a complex idea, there are some examples in the [EffectsExample sample in the repo](https://github.com/louthy/language-ext/blob/main/Samples/EffectsExamples/Examples/TextFileChunkStreamExample.cs)

Conventional stream programming forces you to choose only two of the
following three features:

1. **Effects**
2. **Streaming**
3. **Composability**

If you sacrifice _Effects_ you get `IEnumerable`, which you
can transform using composable functions in constant space, but without
interleaving effects (other than of the _imperative kind_).

If you sacrifice _Streaming_ you get `Traverse` and `Sequence`, which are 
composable and effectful, but do not return a single result until the whole 
list has first been processed and loaded into memory.

If you sacrifice _Composability_ you write a tightly coupled for loops,
and fire off imperative side-effects like they're going out of style.  Which 
is streaming and effectful, but is not modular or separable.

`Pipes` gives you all three features: effectful, streaming, and composable
programming.  `Pipes` also provides a wide variety of stream programming
abstractions which are all subsets of a single unified machinery:

* Effectful [`Producer`](Producer) and [`ProducerT`](ProducerT),
* Effectful [`Consumer`](Consumer) and [`ConsumerT`](ConsumerT),
* Effectful [`Pipe`](Pipe) and [`PipeT`](PipeT) (like Unix pipes)
* Effectful [`Effect`](Effect) and [`EffectT`](EffectT) 

> The `T` suffix types (`ProducerT`, `ConsumerT`, `PipeT`, and `EffectT`) are the 
> more generalist monad-transformers.  They can lift any monad `M` you like into them,
> supplementing the behaviour of `Pipes` with the behaviour of `M`.  The non-`T` 
> suffix types (`Producer`, `Consumer`, `Pipe`, and `Effect`) only support the lifting
> of the `Eff<RT, A>` type.  They're slightly easier to use, just less flexible.

All of these are connectable and you can combine them together in clever and
unexpected ways because they all share the same underlying type: [`PipeT`](PipeT).

The pipes ecosystem decouples stream processing stages from each other so
that you can mix and match diverse stages to produce useful streaming
programs.  If you are a library writer, pipes lets you package up streaming
components into a reusable interface.  If you are an application writer,
pipes lets you connect pre-made streaming components with minimal effort to
produce a highly-efficient program that streams data in constant memory.

To enforce loose coupling, components can only communicate using two commands:

* `yield`: Send output data
* `awaiting`: Receive input data

Pipes has four types of components built around these two commands:

* [`Producer`](Producer) and [`ProducerT`](ProducerT) yield values downstream and can only do so using: `Producer.yield` and `ProducerT.yield`.
* [`Consumer`](Consumer) and [`ConsumerT`](ConsumerT) await values from upstream and can only do so using: `Consumer.awaiting` and `ConsumerT.awaiting`.
* [`Pipe`](Pipe) and [`PipeT`](PipeT) can both await and yield, using: `Pipe.awaiting`, `PipeT.awaiting`, `Pipe.yield`, and `PipeT.yield`. 
* [`Effect`](Effect) and [`EffectT`](EffectT) can neither yield nor await and they model non-streaming components.

Pipes uses parametric polymorphism (i.e. generics) to overload all operations. The operator `|` connects `Producer`, `Consumer`, and `Pipe` by 'fusing' 
them together.  Eventually they will 'fuse' together into an `Effect` or `EffectT`.  This final state can be `.Run()`, you must fuse to an `Effect` or 
`EffectT` to be able to invoke any of the pipelines.

`Producer`, `ProducerT`, `Consumer`, `ConsumerT`, `Pipe`, `Effect`, and `EffectT` are all special cases of a
single underlying type: [`PipeT`](PipeT).  

You can think of it as having the following shape:
  
    PipeT<IN, OUT, M, A>

          Upstream | Downstream
              +---------+
              |         |
         IN --►         --► OUT  -- Information flowing downstream
              |    |    |
              +----|----+
                   |
                   A
        
 Pipes uses type synonyms to hide unused inputs or outputs and clean up
 type signatures.  These type synonyms come in two flavors:

 * Concrete type synonyms that explicitly close unused inputs and outputs of
   the `Proxy` type.

 * Polymorphic type synonyms that don't explicitly close unused inputs or
   outputs.

 The concrete type synonyms use `Unit` to close unused inputs and `Void` (the
 uninhabited type) to close unused outputs:

 * `EffectT`: explicitly closes both ends, forbidding `awaiting` and `yield`:

        EffectT<M, A> = PipeT<Unit, Void, M, A>
         
                 Upstream | Downstream

                     +---------+
                     |         |
              Unit --►         --► Void
                     |    |    |
                     +----|----+
                          |
                          A

 * `ProducerT`: explicitly closes the upstream end, forbidding `awaiting`:

        ProducerT<OUT, M, A> = PipeT<Unit, OUT, M, A>
         
                 Upstream | Downstream

                     +---------+
                     |         |
              Unit --►         --► OUT
                     |    |    |
                     +----|----+
                          |
                          A
   
 * `ConsumerT`: explicitly closes the downstream end, forbidding `yield`:

         ConsumerT<IN, M, A> = PipeT<IN, Void, M, A>
        
                 Upstream | Downstream

                     +---------+
                     |         |
                IN --►         --► Void
                     |    |    |
                     +----|----+
                          |
                          A
          
When you compose `PipeT` using `|` all you are doing is placing them
side by side and fusing them laterally.  For example, when you compose a
`ProducerT`, `PipeT`, and a `ConsumerT`, you can think of information flowing
like this:

                ProducerT                     PipeT                  ConsumerT
             +-------------+             +------------+           +-------------+
             |             |             |            |           |             |
      Unit --►  readLine   --►  string --►  parseInt  --►  int  --►  writeLine  --► Void
             |      |      |             |      |     |           |      |      |
             +------|------+             +------|-----+           +------|------+
                    |                           |                        |  
                    A                           A                        A

 Composition fuses away the intermediate interfaces, leaving behind an `EffectT`:

                            EffectT
            +-------------------------------------+
            |                                     |
     Unit --►   readLine | parseInt | writeLine   --► Void
            |                                     |
            +------------------|------------------+
                               |
                               A

This `EffectT` can be `Run()` which will return the composed underlying `M` type.  Or, 
if it's an `Effect` will return the composed underlying `Eff<RT, A>`.