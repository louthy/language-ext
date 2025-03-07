## Natural transformations

Natural transformations are operations that transform the _structure_ of a type.  If `Functor` is 
_structure **preserving**_ then natural-transformations are _structure **transforming**_.

A concrete example is that if we call `Map` on a `Seq〈A〉`, then the structure (the `Seq`) is 
preserved, but the values within the structure, `A`, are transformed: and that defines a `Functor`,
whereas with natural-transformations we could call `AsEnumerable()` on the `Seq〈A〉`.  The result
would preserve value-type, `A`, but transform the structure `Seq` to `IEnumerable`.

This common pattern of structure transformation is a natural-transformation. It is captured by the type `Natural〈F, G〉`.

## `Natural`

`Natural〈F, G〉` defines a single method, `Transform`, that maps a `K<F, A>` to a `K<G, A>`.  This could be `K<Try, A>` 
to `K<Option, A>` for example.

## `CoNatural`

There is also `CoNatural〈F, G〉` which is the dual of `Natural〈F, G〉`.  It has the `Transform` arrow reversed, which 
means it maps a `K<G, A>` to a `K<F, A>`.

It is _functionally exactly the same_ as `Natural`.  The reason it exists is listed below.

## `NaturalIso`

A natural-isomorphism (`NaturalIso〈F, G〉`) is a natural-transformation that can map forwards and backwards:

    F〈A〉-> G〈A〉

And the dual:

    G〈A〉-> F〈A〉

`NaturalIso` derives from:

    Natural〈F, G〉
    CoNatural〈F, G〉

> The reason it doesn't derive from `Natural〈F, G〉` and `Natural〈G, F〉` is because C# can't type-check when `F == G` and
so the dual needs a separate type: `CoNatural〈F, G〉`.

## `NaturalMono`

A natural _monomorphic_ transformation means there's one arrow between `F` and `G`.  Therefore, there's also a `CoNatural` 
between `G` and `F` (`CoNatural` is the dual of `Natural`, so its arrows are flipped, so a `CoNatural` between `G` and `F`
is isomorphic to a `Natural` between `F` and `G`).

`NaturalMono` derives from:

    Natural〈F, G〉
    CoNatural〈G, F〉

The `CoTransform` method has a default implementation.

## `NaturalEpi`

An _epimorphism_ is the _dual_ of a _monomorphism_.  So, `NaturalEpi` is the dual of `NaturalMono`.  

`NaturalEpi` derives from:

    Natural〈G, F〉
    CoNatural〈F, G〉

The `Transform` method has a default implementation.
