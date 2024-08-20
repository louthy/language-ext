A recent C# feature is `static` interface members – this opens up some new possibilities for bending C# to make trait like functionality work.  You may have already seen the technique:

```c#
public interface Addable<SELF> where SELF : Addable<SELF>
{
    public static abstract SELF Add(SELF x, SELF y);
}
```

Note, how the Add member is `static abstract` and that the interface has a constraint that `SELF` is forced to inherit `Addable<SELF>`.

We can then create two distinct types that inherit the Addable trait:

```c#
public record MyList<A>(A[] values) : Addable<MyList<A>>
{
    public static MyList<A> Add(MyList<A> x, MyList<A> y) =>
        new (x.values.Append(y.values).ToArray());
}

public record MyString(string value) : Addable<MyString>
{
    public static MyString Add(MyString x, MyString y) =>
        new (x.value + y.value);
}
```

Language-Ext takes this idea and uses it to implement 'higher-kinded traits' (with the `K<F, A>` type being the anchor for 
them all).  

To continue reading about how this works, check out [Paul Louth's Higher-Kinds series](https://paullouth.com/higher-kinds-in-c-with-language-ext/). 

 
