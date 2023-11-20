### language-ext migration notes

> `error CS0305: Using the generic type 'NewType<NEWTYPE, A>' requires 2 type arguments`

`NewType` and the new `NumType` and `FloatType` now have an additional generic argument.  So if you defined your `NewType` in `v1` as this: 
```c#
    public class Metres : NewType<int>
    {
        public Metres(int x) : base(x)
        {}
    }
```
Then you should add an extra generic argument to the inherited `NewType`:
```c#
    public class Metres : NewType<Metres, int>
    {
        public Metres(int x) : base(x)
        {}
    }
```
This allows the sub-type to be used in the base-type's methods (like `Select`, `Map`, `Where`, etc.) so that they can return `Metres` rather than `NewType<int>`.

> `The type 'ValueTuple<>' is defined in an assembly that is not referenced. You must add a reference to assembly 'System.ValueTuple,`

There is new C# language support for tuples ("like", "so").  Language-Ext adds loads of extension methods to make them even more useful.  However the underlying typle `System.ValueTuple` must be added to your projects from nu-get.

> `error CS0305: Using the generic type 'Trans<OuterMonad, OuterType, InnerMonad, InnerType, A>' requires 5 type arguments`

The namespace `LanguageExt.Trans` is now deprecated.  Remove any usings.

> `error CS1750: A value of type '<null>' cannot be used as a default parameter because there are no standard conversions to type `

Many types have become structs (like `Map`, `Lst`, etc).  So if you see this error remove the ` = null`.  If you need to assign anything to remove the warnings then either use `default`, or the built-in empty constructors.  e.g.
```c#
    Map<string, int> xs = Map<string, int>();        // If you're using static LanguageExt.Prelude
    Map<string, int> xs = Map.empty<string, int>();
    Map<string, int> xs = default(Map<string, int>);
```

> `'Prelude.Map<K, V>(Tuple<K, V>, params Tuple<K, V>[])' cannot be inferred from the usage. Try specifying the type arguments explicitly.`

The constructor functions for `Map`, `Set`, `List`, etc. have all been standardised.  If you previously passed an `IEnumerable<...>` to any one of them, then you'll now need to either call `TYPE.createRange(...)` on the type, or `toTYPE(...)`.  For example:
```c#
    IEnumerable<(int, string)> xs = new [] { (1, "A"), (2, "B"), (1, "C") };

    var m1 = toMap(xs);
    var m2 = Map.createRange(xs);
```

> `error CS0104: 'HashSet<>' is an ambiguous reference between 'System.Collections.Generic.HashSet<T>' and 'LanguageExt.HashSet<A>'`

`HSet` has been renamed, because it's a terrible name.  And so having a reference to `System.Collections.Generic` and `LanguageExt` in the same code file will cause ambiguities that the linker can't resolve.

Remove one of the usings, and then use aliases for the type you want.  i.e.

```c#
    using G = System.Collections.Generic;

    var hs = new G.HashSet<int>();
```

> `error CS0122: 'NewType<SELF, A, PRED>.Value' is inaccessible due to its protection level`

Previously the `Value` property was public.  If you want to make it public you should now opt-in by exposing the `protected`  `Value` property.  Alternatives are to use an explicit cast to the bound value type to get at the `Value`, or use `Map(x => ...)`
