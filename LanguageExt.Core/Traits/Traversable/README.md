Traversable structures support element-wise sequencing of `Applicative` effects (thus also `Monad` effects) 
to construct new structures of the same shape as the input.

To illustrate what is meant by same shape, if the input structure is `[a]`, each output structure is a list `[b]` of 
the same length as the input. If the input is a `Tree<A>`, each output `Tree<B>` has the same graph of intermediate 
nodes and leaves. Similarly, if the input is a tuple `(x, a)`, each output is a tuple `(x, b)`, and so forth.

Every Traversable structure is both a `Functor` and `Foldable`.