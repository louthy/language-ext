`Prelude` is a
`static partial class`, this type is loaded with functions for constructing the key data types, as well
as many of the things you'd expect in a functional programming language's prelude.  Note, the `Prelude` type
extends into many other parts of the source-tree.  It's the same type, but spread all over the code-base.
And so, you may see `Prelude` in other areas of the documentation: it's the same type.

Because it's so fundamental, you'll want to add this to the top of every code file:

    using static LanguageExt.Prelude;

So what's in here?  Well, apart from the modules listed below, there's the data-type constructors, for example: 

    Option<int> mx = Some(100);

    Seq<int> mx = Seq(1, 2, 3);

As well as the `camelCase` versions of the fluent-methods attached to each type:

    var items = Seq(1, 2, 3);
    
    // Fluent version
    var sum = items.Fold(0, (s, x) => s + x);
    
    // Prelude static function
    var sum = fold(items, 0, (s, x) => s + x);

There is _mostly_ a 1-to-1 mapping between the fluent methods and the `Prelude` static functions ... mostly.