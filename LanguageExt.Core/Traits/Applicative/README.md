**Applicative Functors** are a _'stepping stone'_ between functors and monads – they're probably less well-known but have some 
interesting properties of their own when it comes to lifted expression evaluation. 

Two of the major uses in language-ext is to enable _automatic parallel processing of effectful computations_ and to 
_automatically collect multiple errors when validating_. Those aren't the only usages – all the higher-kinded-types, 
including the collection-types, have applicative traits.

The topic is too large to cover here, so take a look at [Paul Louth's Higher-Kinds series](https://paullouth.com/higher-kinds-in-c-with-language-ext-part-4-applicatives/) for more information.