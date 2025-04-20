The `Streaming` library of language-ext is all about compositional streams.  There are two key types of streaming
functionality: **closed-streams** and **open-streams**...

## Closed streams

Closed streams are facilitated by the [`Pipes`](Pipes) system.  The types in the `Pipes` system are compositional
monad-transformers that 'fuse' together to produce an [`EffectT<M, A>`](Pipes/EffectT).  This effect is a _closed system_,
meaning that there is no way (from the API) to directly interact with the effect from the outside: it can be executed
and will return a result if it terminates.

The pipeline components are:

* [`ProducerT<OUT, M, A>`](Pipes/ProducerT)
* [`PipeT<IN, OUT, M, A>`](Pipes/PipeT)
* [`ConsumerT<IN, M, A>`](Pipes/ConsumerT)

These are the components that fuse together (using the `|` operator) to make an [`EffectT<M, A>`](Pipes/EffectT).  The
types are _monad-transformers_ that support the `MonadIO` trait only (which constrains `M`).  This makes sense, otherwise
the closed-system would have no effect other than heating up the CPU.

There are also more specialised versions of the above that only support the lifting of the `Eff<RT, A>` effect monad:

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
* [`Sink`](Sink) receives values and propagates them through the channel they're attached to.
* [`Conduit`](Conduit) and [`ConduitT`](ConduitT) composes a `Sink` and `Source` or `SourceT` (so `Sink -> Source` or `Sink -> SourceT`), providing inputs to the stream which yields the received values.

> I'm calling these 'open streams' because we can `Post` values to a `Sink` and we can `Reduce` values yielded by
> `Source` and `SourceT`.  So, they are 'open' for public manipulation, unlike `Pipes` which fuses the public access away.

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

## Open to closed streams

Clearly, even for 'closed systems' like the [`Pipes`](Pipes) system, it would be beneficial to be able to post values
into the streams from the outside.  And so, the _open-stream components_ can all be converted into `Pipes` components
like `ProducerT` and `ConsumerT`.

* `Conduit` and `ConduitT` support `ToProducer`, `ToProducerT`, `ToConsumer`, and `ToConsumerT`.
* `Sink` supports `ToConsumer`, and `ToConsumerT`.
* `Source` and `SourceT` supports `ToProducer`, and `ToProducerT`.

This allows for the ultimate flexibility in your choice of streaming effect. It also allows for efficient concurrency in
the more abstract and compositional world of the pipes. In fact `ProducerT.merge`, which merges many streams into one,
uses `ConduitT` internally to collect the values and to merge them into a single `ProducerT`.