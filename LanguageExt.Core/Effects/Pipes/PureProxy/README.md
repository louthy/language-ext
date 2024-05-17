This folder contains a number of micro-free-monads that allow for creation of _pure_ producers, consumers, and pipes.   They're used to facilitate the building of `Proxy` derived types without the need for typing the generic arguments endlessly.

The original Haskell Pipes library could auto-infer the generic parameter types, the system here tries to replicate manually what
Haskell can do automatically.  Hence why there are so many implementations of `SelectMany`!

Mostly you shouldn't need to care too much about the types in this folder as they're effectively 'intermediate' types.

