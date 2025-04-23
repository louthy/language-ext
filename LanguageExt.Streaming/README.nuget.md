The `Streaming` library of language-ext is all about compositional streams.  There are two key types of streaming
functionality: **closed-streams** and **open-streams**...

## Closed streams

Closed streams are facilitated by the [`Pipes`](Pipes) system.  The types in the `Pipes` system are _compositional
monad-transformers_ that 'fuse' together to produce an [`EffectT<M, A>`](Pipes/EffectT).  This effect is a _closed system_,
meaning that there is no way (from the API) to directly interact with the effect from the outside: it can be executed
and will return a result if it terminates.

The pipeline components are:

* [`ProducerT<OUT, M, A>`](Pipes/ProducerT)
* [`PipeT<IN, OUT, M, A>`](Pipes/PipeT)
* [`ConsumerT<IN, M, A>`](Pipes/ConsumerT)

These are the components that fuse together (using the `|` operator) to make an [`EffectT<M, A>`](Pipes/EffectT).  The
types are _monad-transformers_ that support lifting monads with the `MonadIO` trait only (which constrains `M`).  This
makes sense, otherwise the closed-system would have no effect other than heating up the CPU.

There are also more specialised versions of the above that only support the lifting of the `Eff<RT, A>` effect-monad:

* [`Producer<RT, OUT, A>`](Pipes/Producer)
* [`Pipe<RT, IN, OUT, A>`](Pipes/Pipe)
* [`Consumer<RT, IN, A>`](Pipes/Consumer)

They all fuse together into an [`Effect<RT, A>`](Pipes/Effect).

Pipes are especially useful if you want to build reusable streaming components that you can glue together ad infinitum.
Pipes are, arguably, less useful for day-to-day stream processing, like handling events, but your mileage may vary.

_More details on the [`Pipes page`](Pipes)._

## Open streams

Open streams are closer to what most C# devs have used classically.  They are like events or `IObservable` streams.
They yield values and (under certain circumstances) accept inputs.

* [`Source`](Source) and [`SourceT`](SourceT) yield values synchronously or asynchronously depending on their construction.
* [`Sink`](Sink) and [`SinkT`](SinkT) receives values and propagates them through the channel they're attached to.
* [`Conduit`](Conduit) and [`ConduitT`](ConduitT) provides and input transducer (acts like a `Sink`), an internal buffer, and an output transducer (acts like a `Source`).

> I'm calling these 'open streams' because we can `Post` values to a `Sink`/`SinkT` and we can `Reduce` values yielded by
> `Source`/`SourceT`.  So, they are 'open' for public manipulation, unlike `Pipes` which fuse the public access away.

### [`Source`](Source)

[`Source<A>`](Source) is the 'classic stream': you can lift any of the following types into it: `System.Threading.Channels.Channel<A>`,
`IEnumerable<A>`, `IAsyncEnumerable<A>`, or singleton values.  To process a stream, you need to use one of the `Reduce`
or `ReduceAsync` variants.  These take `Reducer` delegates as arguments.  They are essentially a fold over the stream of
values, which results in an aggregated state once the stream has completed.  These reducers can be seen to play a similar
role to `Subscribe` in `IObservable` streams, but are more principled because they return a value (which we can leverage
to carry state for the duration of the stream).

`Source` also supports some built-in reducers:

* `Last` - aggregates no state, simply returns the last item yielded
* `Iter` - this forces evaluation of the stream, aggregating no state, and ignoring all yielded values.
* `Collect` - adds all yielded values to a `Seq<A>`, which is then returned upon stream completion.

### [`SourceT`](SourceT)

[`SourceT<M, A>`](SourceT) is the classic-stream _embellished_ - it turns the stream into a monad-transformer that can
lift any `MonadIO`-enabled monad (`M`), allowing side effects to be embedded into the stream in a principled way.

So, for example, to use the `IO<A>` monad with `SourceT`, simply use: `SourceT<IO, A>`.  Then you can use one of the
following `static` methods on the `SourceT` type to lift `IO<A>` effects into a stream:

* `SourceT.liftM(IO<A> effect)` creates a singleton-stream
* `SourceT.foreverM(IO<A> effect)` creates an infinite stream, repeating the same effect over and over
* `SourceT.liftM(Channel<IO<A>> channel)` lifts a `System.Threading.Channels.Channel` of effects
* `SourceT.liftM(IEnumerable<IO<A>> effects)` lifts an `IEnumerable` of effects
* `SourceT.liftM(IAsyncEnumerable<IO<A>> effects)` lifts an `IAsyncEnumerable` of effects

> Obviously, when lifting non-`IO` monads, the types above change.

`SourceT` also supports the same built-in convenience reducers as `Source` (`Last`, `Iter`, `Collect`).

### [`Sink`](Sink)

[`Sink<A>`](Sink) provides a way to accept many input values.  The values are buffered until consumed.  The sink can be
thought of as a `System.Threading.Channels.Channel` (which is the buffer that collects the values) that happens to
manipulate the values being posted to the buffer just before they are stored.

> This manipulation is possible because the `Sink` is a `CoFunctor` (contravariant functor).  This is the dual of `Functor`:
we can think of `Functor.Map` as converting a value from `A -> B`.  Whereas `CoFunctor.Comap` converts from `B -> A`.

So, to manipulate values coming into the `Sink`, use `Comap`.  It will give you a new `Sink` with the manipulation 'built-in'.

### [`SinkT`](SinkT)

[`SinkT<M, A>`](SinkT) provides a way to accept many input values.  The values are buffered until consumed.  The sink can
be thought of as a `System.Threading.Channels.Channel` (which is the buffer that collects the values) that happens to
manipulate the values being posted to the buffer just before they are stored.

> This manipulation is possible because the `SinkT` is a `CoFunctor` (contravariant functor).  This is the dual of `Functor`:
we can think of `Functor.Map` as converting a value from `A -> B`.  Whereas `CoFunctor.Comap` converts from `B -> A`.

So, to manipulate values coming into the `SinkT`, use `Comap`.  It will give you a new `SinkT` with the manipulation 'built-in'.

`SinkT` is also a transformer that lifts types of `K<M, A>`.

### [`Conduit`](Conduit)

`Conduit<A, B>` can be pictured as so:

    +----------------------------------------------------------------+
    |                                                                |
    |  A --> Transducer --> X --> Buffer --> X --> Transducer --> B  |
    |                                                                |
    +----------------------------------------------------------------+

* A value of `A` is posted to the `Conduit` (via `Post`)
* It flows through an input `Transducer`, mapping the `A` value to `X` (an internal type you can't see)
* The `X` value is then stored in the conduit's internal buffer (a `System.Threading.Channels.Channel`)
* Any invocation of `Reduce` will force the consumption of the values in the buffer
* Flowing each value `X` through the output `Transducer`

So the input and output transducers allow for pre and post-processing of values as they flow through the conduit.  
`Conduit` is a `CoFunctor`, call `Comap` to manipulate the pre-processing transducer. `Conduit` is also a `Functor`, call
`Map` to manipulate the post-processing transducer.  There are other non-trait, but common behaviours, like `FoldWhile`,
`Filter`, `Skip`, `Take`, etc.

> `Conduit` supports access to a `Sink` and a `Source` for more advanced processing.

### [`ConduitT`](Conduit)

`ConduitT<M, A, B>` can be pictured as so:

    +------------------------------------------------------------------------------------------+
    |                                                                                          |
    |  K<M, A> --> TransducerM --> K<M, X> --> Buffer --> K<M, X> --> TransducerM --> K<M, B>  |
    |                                                                                          |
    +------------------------------------------------------------------------------------------+

* A value of `K<M, A>` is posted to the `Conduit` (via `Post`)
* It flows through an input `TransducerM`, mapping the `K<M, A>` value to `K<M, X>` (an internal type you can't see)
* The `K<M, X>` value is then stored in the conduit's internal buffer (a `System.Threading.Channels.Channel`)
* Any invocation of `Reduce` will force the consumption of the values in the buffer
* Flowing each value `K<M, A>` through the output `TransducerM`

So the input and output transducers allow for pre and post-processing of values as they flow through the conduit.  
`ConduitT` is a `CoFunctor`, call `Comap` to manipulate the pre-processing transducer. `Conduit` is also a `Functor`, call
`Map` to manipulate the post-processing transducer.  There are other non-trait, but common behaviours, like `FoldWhile`,
`Filter`, `Skip`, `Take`, etc.

> `ConduitT` supports access to a `SinkT` and a `SourceT` for more advanced processing.

## Open to closed streams

Clearly, even for 'closed systems' like the [`Pipes`](Pipes) system, it would be beneficial to be able to post values
into the streams from the outside.  And so, the _open-stream components_ can all be converted into `Pipes` components
like `ProducerT` and `ConsumerT`.

* `Conduit` and `ConduitT` support `ToProducer`, `ToProducerT`, `ToConsumer`, and `ToConsumerT`.
* `Sink` and `SinkT` supports `ToConsumer`, and `ToConsumerT`.
* `Source` and `SourceT` supports `ToProducer`, and `ToProducerT`.

This allows for the ultimate flexibility in your choice of streaming effect. It also allows for efficient concurrency in
the more abstract and compositional world of the pipes. In fact `ProducerT.merge`, which merges many streams into one,
uses `ConduitT` internally to collect the values and to merge them into a single `ProducerT`.