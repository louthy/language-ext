# Version 2 Release Notes (WIP)

## Bug fixes

* Fix for `Process.exists()` exception (when no process exists)
* Fix for `Lst.RemoveAt(index)` - certain tree arrangements caused this function to fail
* Fix for `HSet` (now `HashSet`) constructor bug - constructing with an enumerable always failed

## New features - LanguageExt.Core

### New collection types:

 Type                                       | Description
--------------------------------------------|--------------
`HashSet<A>`                                | Ordering is done by `GetHashCode()`.  Existence testing is with `EqualityComparer<A>.Default.Equals(a,b)`
`HashMap<A, B>`                             | Ordering is done by `GetHashCode()`.  Existence testing is with `EqualityComparer<A>.Default.Equals(a,b)`
`HashSet<EqA, A> where EqA : struct, Eq<A>` | Ordering is done by `GetHashCode()`.  Existence testing is with `default(EqA).Equals(a,b)`
`HashMap<EqA, A, B>`                        | Ordering is done by `GetHashCode()`.  Existence testing is with `default(EqA).Equals(a,b)`
`Set<OrdA, A> where OrdA : struct, Ord<A>`  | Ordering is done by `default(OrdA).Compare(a,b)`.  Existence testing is with `default(OrdA).Equals(a,b)`
`Map<EqA, A, B>`                            | Ordering is done by `default(OrdA).Compare(a,b)`.  Existence testing is with `default(OrdA).Equals(a,b)`
`Arr<A>`                                    | Immutable array.  Has the same access speed as the built-in array type, but with immutable cells.  Modification is expensive, due to the entire array being copied per operation (although for very small arrays this would be more efficient than `Lst<T>` or `Set<T>`).

As you can see above there are new type-safe key versions of `Set`, `HashSet`, `Map`, and `HashMap`.  Imagine you want to sort the value of a set of strings in a case-insensitive way (without losing information by calling `value.ToLower()`).
```c#
    var map = Set<TStringOrdinalIgnoreCase, string>(...)
```
The resulting type would be incompatible with:
```c#
    Set<TString, string>, or Set<TStringOrdinal, string>
```
And is therefore more type-safe than just using Set<string>.  Examples: https://github.com/louthy/language-ext/blob/type-classes/LanguageExt.Tests/SetTests.cs 
### Non-nullable types: 

In the ongoing quest to make it safer to write C# code, these types are all now structs and therefore can't be `null`:
```c#
    Stck<A>, 
    Que<A>, 
    Lst<A>, 
    Map<A, B>, 
    Map<Ord, A, B>, 
    HashMap<A, B>, 
    HashMap<Eq, A, B>, 
    Set<A>, 
    Set<Ord, A>
    HashSet<A, B>, 
    HashSet<Eq, A, B>, 
    Que<A>, 
    Arr<A>
```
This means you can create a member property and not initialise it and everything will 'just work':

```c#
    static class Test
    {
        public static Map<string, int> Foo;
    }

    Assert.True(Test.Foo == Map.empty<string, int>());
    Assert.True(Test.Foo == default(Map<string, int>);
```
### Serialisation fixes

`Map`, `Lst`, `Set`, `Option`, `Either`, etc.  All have serialisers that work with Json.NET (finally).  

Examples: https://github.com/louthy/language-ext/blob/type-classes/LanguageExt.Tests/SerialisationTests.cs


### NewType

NewType has gained an extra generic argument.  So this:

```c#
    class Metres : NewType<double> { ... }
```

Becomes:
```c#
    class Metres : NewType<Metres, double>  { ... } 
```
That makes lots of _stuff_ more type-safe for `NewType` derived types.  For example monadic and functor operators like `Select`, `Map`, `Bind`, `SelectMany` can now return `Metres` rather than `NewType<double>`.  Which is very important for the integrity of the type.

There is a variant that takes an additional generic argument `PRED`.  Which is constrained to be a `struct` of `Pred<A>`.  This is called in the base constructor:
```c#
    if (!default(PRED).True(value)) throw new ArgumentOutOfRangeException(nameof(value), value, $"Argument failed {typeof(NEWTYPE).Name} NewType predicate");
```
So you should be able to see that this allows validation to be embedded into the type.  Here's an example from the new Process system client code:
```c#
    public class ClientConnectionId : NewType<ClientConnectionId, string, StrLen<I10, I100>>
    {
        public ClientConnectionId(string value) : base(value)
        { }
    }
```
`ClientConnectionId` is like a session token, and `StrLen<I10, I100>` means the string must be `10` to `100` chars long.  By embedding the validation into the type, there is no 'get out of jail free' cards where a loophole can be found in the type.  And it also becomes fundamentally (in the type system) a different type to say `NewType<ClientConnectionId, string, StrLen<I0, I100>>`; so any function that wants to work with the client connection token must accept either `ClientConnectionId` or its base type of `NewType<ClientConnectionId, string, StrLen<I10, I100>>`.  I think this is a pretty powerful concept for improving the safety of C# types in general, and takes the original `NewType` idea to the next level.

One breaking change is that the `Value` property has been made `protected`.  That means you can expose it if you like when you declare the `NewType`, but by default it's not visible outside of the class.  There is an explicit cast operator.  So for the `Metres` example above:
```c#
    Metres m = Metres.New(100);
    double x = (double)m * 2.0;
```
I think that creates a _barrier to entry_ that is high enough to make it much more attractive to use `Map`, `Bind`, etc. and therefore keep the value in context, i.e:
```c#
    Metres m = Metres.New(100);
    Metres x = m.Map(v => v * 2);
```
### NumType

With the new type-classes and class-instances (see later), it's now possible to write generic code for numeric-types.  And so I have created a variant of `NewType` called `NumType`.  Numeric types like `int` are the kind of types that are very commonly made into `NewType` derived types (along with `string`), but previously there wasn't a good story for doing arithmetic on those types.  Now with the `NumType` it is possible.  They work in exactly the same way as `NewTypes`, but you must specify a `Num<A>` class-instance (below it's `TDouble`):
```c#
    public class Metres : NumType<Metres, TDouble, double> { 
        public Metres(double x) : base(x) {}
    }
```
That gives you these extras over `NumType`:
```c#
    operator: + * / -
    Product()
    Divide()
    Plus()
    Subtract()
    Abs()
    Signum()
    Min()
    Max()
    Sum()
```
You can also use a predicate with `NumType`:
```c#
    public class Age : NumType<Age, TInt, int, Range<int, TInt, I0, I120>> 
    { 
        public Age(int x) : base(x)  {}
    }
```
### ValueTuple and Tuple

A new feature of C# 7 is syntax for tuples.  Instead of:
```c#
    Tuple.Create(a, b)
```
You just do:
```c#
    (a, b)
```
You can also give them names:
```c#
    (K Key, V Value)
```
So wherever in lang-ext tuples are used, they now accept (or return with names) `ValueTuple`.  `ValueTuple` is the `struct` version of `Tuple` that C# is using to back the new syntax.

This is particularly nice for `Map`:
```c#
    var map = Map(
        (1, "Paul"), 
        (2, "Steve"), 
        (3, "Stan"), 
        (4, "Tanzeel"), 
        (5, "Dan"), 
        (6, "Andreas"));
```
Also `Map` now descends from `IEnumerable<(K Key, V Value)>`.

Tuples look like they're going to be a lot more important in C# going forwards, so I have created extension methods and `Prelude` functions for `Tuple` and `ValueTuple` with up to 7 items.
```c#
    // Add an item to a tuple
    var (a,b,c)   = (1, 2).Add(3);
    var (a,b,c,d) = (1, 2).Add(3).Add("Hello");
```
If your tuple contains Semigroups (like `Lst` and `int` for example) then you can call `Append`:
```c#
    var (a, b) = append<TLst<int>, TInt, Lst<int>, int>(
                    (List(1,2,3), 3),
                    (List(4,5,6,7), 4));

    // ([1,2,3,4,5,6,7], 7)
```
Or:
```c#
    var list = append<TLst<int>, Lst<int>>( (List(1,2,3), List(4,5,6,7)) );

    // [1,2,3,4,5,6,7]
```
`Head` and `Tail`:
```c#
    var a  = ("a", 123, true).Head();   // "a"
    var bc = ("a", 123, true).Tail();   // (123, true)
```
`Sum` and `Product`:
```c#
    var a = (100, 200, 300).Sum<TInt, int>();  // 600
    var b = (10, 10, 10).Product<TInt, int>(); // 1000
```
`Contains`:
```c#
    var a = (1,2,3,4,5).Contains<EqInt, int>(3);  // true
```
Mapping:
```c#
    x = x.Map( tuple => tuple );
    
    x = x.BiMap(a  => a * 2, b => b * 3);
    x = x.TriMap(a  => a * 2, b => b * 3, c => c + " add");
    x = x.QuadMap(a  => a * 2, b => b * 3, c => "over", d => "ride");
    // etc.

    x = x.MapFirst(a => a * 2);  // just maps the first item and leaves the rest alone
    x = x.MapSecond(b => b * 3);  
    x = x.MapThird(c => c + " add");
    x = x.MapFourth(d => "change");  
    // etc.

    var (a, b) = (100, "text").MapFirst(x => x * 2); // (200, "text")
```
Also:
```c#
    Iter, Fold, BiFold, BiFoldBack, TriFold, TriFoldBack, etc.
```
### Cond

`Cond` allows for building conditional expressions that can be used fluently.  It also seamlessly steps between synchronous and asynchronous behaviour without any need for ceremony.  It's a technique that I'd like to add to the other types (although that will mean an `OptionAsync`, `EitherAsync`, etc. so not free).

Here's a simple example:
```c#
var cond = Cond<int>(x => x == 4)
               .Then(true)
               .Else(false);
```
That can be run like so:
```c#
bool result = cond(4); // True
bool result = cond(0); // False
```
Or,
```c#
bool result = 4.Apply(cond); // True
bool result = 0.Apply(cond); // False
```
Here's a slightly more complex  example:
```c#
    var vowels = Subj<char>().Map(Char.ToLower)
                             .Any(x => x == 'a', x => x == 'e', x => x == 'i', x => x == 'o', x => x == 'u')
                             .Then("Is a vowel")
                             .Else("Is a consonant");

    var x = vowels('a'); // "Is a vowel"
```
This can then be tagged onto anything that returns a char or a `Task<char>`:
```c#
    var res = GetCharFromRemoteServer().Apply(vowels);   // Task<string>
```
See the [pull request](https://github.com/louthy/language-ext/pull/179) that led to this feature for more examples.

Thanks to @ncthbrt for the suggestion and initial implementation.
    
### Type-classes

It's hinted at above, but there are now type-classes (interfaces) for:

 Type-class | Functions | Description
------------|-----------|-------------
`BiFoldable<F, A, B>` | `BiFold`, `BiFoldBack` | for folding types that have two values (`Tuple<A,B>` or `Either<L,R>` for example)
`BiFunctor<FAB, FR, A, B, R>` | `BiMap` | as above, but for projection
`Choice<CH, A, B>` | `IsChoice1`, `IsChoice2`, `Match`, `MatchUnsafe`, `IsBottom`, `IsUnsafe` | Represents a type (`CH`) with two possible values (a discriminated union of two types basically - like `Either<L, R>`).  Allows for generalisation of code that requires a binary choice, but doesn't want to be locked down to using `Either` (`Option` for example is a `Choice<Option<A>, Unit, A>`)
`Const<A>` | `Value` | Used for providing constants to the predicates for `NewType`
`Eq<A>`| `Equals`, `GetHashCode` | Equality type-class
`Floating<A>` | `Pi`, `Exp`, `Sqrt`, `Log`, `Pow`, `LogBase`, `Sin`, `Cos`, `Tan`, `Asin`, `Acos`, `Atan`, `Sinh`, `Cosh`, `Tanh`, `Asinh`, `Acosh`, `Atanh` | Floating point number type-class.  Derives from `Fractional<A> -> Num<A> -> Ord<A> -> Eq<A>, Monoid<A> -> Semigroup<A>`.  
`Foldable<FA, A>` | `Fold`, `FoldBack`, `Count` | standard type-class for types that can be folded (like `Lst`, `Option`, etc.)
`Fraction<A>` | `FromRational` | Provides a way to get an `A` from `A / A`
`Functor<FA, FB, A, B>` | `Map` | standard mapping projection function
`Liftable<LA, A>` | `Lift`, `LiftSeq` | For lifting value(s) of `A` into the container type of `LA`
`Monad<MA, A>` | `Bind`, `Return`, `FromSeq`, `Fail` | Monad type-class
`MonadPlus<MA, A>` | Superclass of `Monad<MA, A>` | Adds `Plus` and `Zero`, which allows for `Filter`/`Where` to be derived.
`MonadReader<Env, E>` | `Ask`, `Local`, `Reader` | Reader monad type-class, allows for an environment to be passed through a computation
`MonadState<S, E>` | `Get`, `Put`, `State` | State monad type-class, allows for a state to be passed through a computation, one that can be set (using `Put`).
`MonadWriter<MonoidW, W, A>` | `Tell`, `Listen` | Writer monad type-class, allows for aggregating outout (like a log for example).  The old version forced the use of `IEnumerable` to collect the output from `Tell` calls.  This is because I didn't have a `Monoid` type-class (`Monoid` provides `Append` and `Empty`).  So `MonadWriter` isn't limited to lists any more, it's limited to monoids: numbers, strings, etc.  Obviously you can implement your own monoids too.
`Num<A>` | `Abs`, `Signum`, `FromInteger`, `Plus`, `Subtract`, `Product`, `Divide`, `Negate` | Represents a numeric type.  Derives from `Ord<A> -> Eq<A>, Monoid<A> -> Semigroup<A>`.
`Optional<OA, A>` | IsSome, IsNone, Match, MatchUnsafe, IsUnsafe | Represents an optional value.  Like the `Choice` type, this allows unification of values that have an optional result (`Option`, `Either`, `Try`, `TryOption`, etc.)
`Ord<A>` | `Compare` | Ordering type-class
`Pred<A>` | `True` | Predicate type-class, used by `NewType` and `NumType` for validation, but essentially can be used to represent any predicate expression.
`Semigroup<A>` | `Append` | Type-class that provides one function: `Append`, types that fit in to this: `Lst`, `int`, `string`, etc.
`TriFunctor` | `TriMap` | Three item version of `Functor`

### Class-instances

Class instances implement the type-class interfaces, are structs, and can be invoked by `default(TClassInstance).Foo()`.  This allows for ad-hoc polymorphic behaviours to be applied to sealed types.

 Type-class              | Class instances
-------------------------|-----------------
`Const<A>`               | `ChA` - `ChZ`, `Cha` - `Chz`, `Ch0` - `Ch9`, `ChSpace`, `ChTab`, `ChCR`, `ChLF`
                         | `D0`, `D1`, `DNeg`
                         | `I0`-`I256`, `I300`, `I320`, `I384`, `I400`, `I480`, `I500`, `I512`, `I600`, `I640`, `I700`, `I768`, ..., `I1073741824`, `IMax`, `IMin`.
`Eq<A>`                  | `TChar`, `EqArr`, `EqArray`, `EqBigInt`, `EqBool`, `EqChar`, `EqChoice`, `EqDateTime`, `EqDecimal`, `EqDefault`, `EqDouble`, `EqFloat`, `EqGuid`, `EqHashSet`, `EqInt`, `EqLong`, `EqLst`, `EqNewType`, `EqNumType`, `EqOpt`, `EqQue`, `EqSeq`, `EqShort`, `EqStck`, `EqString`, `EqTry`, `EqTryOpt`
`Floating<A>`            | `TFloat`, `TDouble`, `TDecimal`
`Foldable<FA, A>`        | `FoldTuple`, `MArr`, `MArray`, `MEither`, `MEitherUnsafe`, `MHashSet`, `MLst`, `MNullable`, `MOption`, `MOptionUnsafe`, `MQue`, `MSeq`, `MSet`, `MStck`, `MTry`, `MTryOption`
`Functor<FA, FB, A, B>`  | `FArr`, `FArray`, `FEither`, `FEitherUnsafe`, `FHashMap`, `FLst`, `FMap`, `FNullable`, `FOption`, `FOptionUnsafe`, `FQue`, `FSeq`, `FSet`, `FStck`, `FTupleBi`, `FTupleFst`, `FTupleSnd`, `FTupleThrd`, `FTupleTri`
`Monad<MA, A>`           | `MReader`, `MRWS`, `MState`, `MWriter`, `MArr`, `MArray`, `MEither`, `MEitherUnsafe`, `MHashSet`, `MLst`, `MNullable`, `MOption`, `MOptionUnsafe`, `MQue`, `MSeq`, `MSet`, `MStck`, `MTry`, `MTryOption`
`MonadPlus<MA, A>`       | `MArr`, `MArray`, `MEither`, `MEitherUnsafe`, `MHashSet`, `MLst`, `MNullable`, `MOption`, `MOptionUnsafe`, `MQue`, `MSeq`, `MSet`, `MStck`, `MTry`, `MTryOption`
`Monoid<A>`              | `TInt`, `TLong`, `TShort`, `TString`, `TLst`, `TFloat`, `TDouble`, `TChar`, `All`, `Any`, `MHashMap`, `MMap`, `Product`, `Sum`
`Num<A>`                 | `TInt`, `TLong`, `TShort`, `TFloat`, `TDouble`, `TDecimal`, `TBigInt`, `TChar`,
`Ord<A>`                 | `TInt`, `TLong`, `TShort`, `TString`, `TFloat`, `TDouble`, `TDecimal`, `TChar`,`OrdArr`, `OrdArray`, `OrdBigInt`, `OrdBool`, `OrdChar`, `OrdChoice`, `OrdDateTime`, `OrdDecimal`, `OrdDefault`, `OrdDouble`, `OrdFloat`, `OrdGuid`, `OrdInt`, `OrdLong`, `OrdLst`, `OrdNewType`, `OrdNumType`, `OrdOpt`, `OrdQue`, `OrdSeq`, `OrdSet`, `OrdShort`, `OrdStck`, `OrdString`, `OrdTry`, `OrdTryOpt`
`Pred<A>`                | `CharSatisfy<MIN, MAX>`, `Char<CH>`, `Letter`, `Digit`, `Space`, `AlphaNum`, `StrLen<NMin, NMax>`, `True<A>`, `False<A>`, `Equal<A, EQ, CONST>`, `Exists<A, Term1, Term2>`, `ForAll<A, Term1, Term2>`, `GreaterThan<A, ORD, CONST>`, `LessThan<A, ORD, CONST>`, `GreaterOrEq<A, ORD, CONST>`, `LessOrEq<A, ORD, CONST>`, `Range<A, ORD, MIN, MAX>`
`Semigroup<A>`           | `TInt`, `TLong`, `TShort`, `TString`, `TLst`, `TFloat`, `TDouble`, `TDecimal`, `TBigInt`, `TChar`,`Min<ORD, A>`, `Max<ORD, A>`, `All`, `Any`, `MHashMap`, `MMap`, `Product`, `Sum`, `TArr`, `TArray`, 

### Reader/Writer/State

These monads are now classes and wrap the computation delegate.  Therefore they can't now be invoked directly.  You must now call `Run(...)` on the type.  Their output is in the form:

**`Reader<Env, A>`**
```c#
    TryOption<A> result = reader.Run(env);
```
The `TryOption<A>` captures the possible return states: 
    * `Some(x)` - success
    * `None` - computation is in a 'bottom' state (usually because of a `where` filter).
    * `Fail(ex)` - An exception was thrown.

The `TryOption<A>` also memoises the result.  So keep a reference to it to avoid running the computation twice.  Match on the `TryOption` to extract the value.  

**`Writer<MonoidW, W, O>`**
```c#
    (TryOption<A> Value, W Output) = writer.Run();
```
The `Writer` monad returns the optional value as well as any logged output.

**`State<S, A>`**
```c#
    (TryOption<A> Value, S State) = state.Run(initialState);
```
The `State` monad returns the optional value as well as any stored state.

### Reflect.Util 

Although this doesn't really fit with the core message of the library, it is very useful.  There are 4 functions for building a `Func<...,R>` which invokes the constructor for a type.  
```c#
    var ticks = new DateTime(2017, 1, 1).Ticks;

    // Builds a delegate to call new DateTime(long);
    var ctor = Reflect.Util.CtorInvoke<long, DateTime>();

    DateTime res = ctor(ticks);

    Assert.True(res.Ticks == ticks);
```
The generated delegate doesn't use reflection, IL is emitted directly.  So invocation is as fast as calling `new DateTime(ticks)`.  The four `CtorInvoke` functions are for up to four arguments.

The `NewType` system uses it to build a fast strongly-typed constructor:
```c#
    public abstract class NewType<NEWTYPE, A, PRED> :
        IEquatable<NEWTYPE>,
        IComparable<NEWTYPE>
        where PRED    : struct, Pred<A>
        where NEWTYPE : NewType<NEWTYPE, A, PRED>
    {
        protected readonly A Value;

        /// <summary>
        /// Constructor function
        /// </summary>
        public static readonly Func<A, NEWTYPE> New = Reflect.Util.CtorInvoke<A, NEWTYPE>();

        ...
    }
```
So for example:
```c#
   class Metres : NewType<Metres, float> 
   { 
       public Metres(float x) : base(x) {} 
   }

   var ms = Metres.New(100);
```
