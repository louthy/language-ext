The `Pipes` library of language-ext is all about compositional streams.  There are two major areas of functionality:

1. [`Core`](Core) - This is where the core compositional pipeline monad-transformers live. They are capable of lifting 
any monad (usually an effectful monad like a `IO`, `Eff`, or one of your own design) into a `producer -> pipe -> consumer` 
_fused effect_.  These fused-effects handle the flow of values through the pipeline, whilst also allowing for the 
lifted monadic effects to occur (the `IO`, `Eff`, etc).  This was originally based on the Haskell Pipes library, 
but has since been refactored to be more useful in C#-land.  It still shares many core ideas, but is its own thing now.
2. [`Concurrent`](Concurrent) - Provides three major types: `Conduit<IN, OUT>`, `Sink<IN>`, and `Source<OUT>`.  Although,
they share the same names as the Haskell Conduit library, they are unrelated in implementation.  Behind the scenes they 
use the `System.Threading.Channels.Channel` type to provide efficient threadsafe/concurrent access to a queue that also 
supports backpressure primitives.   

`Conduit<IN, OUT>` also supports `ToProducer` and `ToConsumer`.  Which allows for efficient concurrency in the more 
abstract and compositional world of the `Core` pipes.