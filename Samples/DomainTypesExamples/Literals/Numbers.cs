global using N10 =
    DomainTypesExamples.Literals.N<
        DomainTypesExamples.Literals.N1,
        DomainTypesExamples.Literals.N0>;

global using N12 =
    DomainTypesExamples.Literals.N<
        DomainTypesExamples.Literals.N1,
        DomainTypesExamples.Literals.N2>;

global using N23 =
    DomainTypesExamples.Literals.N<
        DomainTypesExamples.Literals.N2,
        DomainTypesExamples.Literals.N3>;

global using N59 =
    DomainTypesExamples.Literals.N<
        DomainTypesExamples.Literals.N5,
        DomainTypesExamples.Literals.N9>;

global using N60 =
    DomainTypesExamples.Literals.N<
        DomainTypesExamples.Literals.N6,
        DomainTypesExamples.Literals.N0>;

global using N80 =
    DomainTypesExamples.Literals.N<
        DomainTypesExamples.Literals.N8,
        DomainTypesExamples.Literals.N0>;

global using N128 =
    DomainTypesExamples.Literals.N<
        DomainTypesExamples.Literals.N1,
        DomainTypesExamples.Literals.N2,
        DomainTypesExamples.Literals.N8>;

global using N480 =
    DomainTypesExamples.Literals.N<
        DomainTypesExamples.Literals.N4,
        DomainTypesExamples.Literals.N8,
        DomainTypesExamples.Literals.N0>;

global using N720 =
    DomainTypesExamples.Literals.N<
        DomainTypesExamples.Literals.N7,
        DomainTypesExamples.Literals.N2,
        DomainTypesExamples.Literals.N0>;

using LanguageExt.Traits;

namespace DomainTypesExamples.Literals;

public sealed class N0 : Const<int>
{
    public static int Value => 0;
}

public sealed class N1 : Const<int>
{
    public static int Value => 1;
}

public sealed class N2 : Const<int>
{
    public static int Value => 2;
}

public sealed class N3 : Const<int>
{
    public static int Value => 3;
}

public sealed class N4 : Const<int>
{
    public static int Value => 4;
}

public sealed class N5 : Const<int>
{
    public static int Value => 5;
}

public sealed class N6 : Const<int>
{
    public static int Value => 6;
}

public sealed class N7 : Const<int>
{
    public static int Value => 7;
}

public sealed class N8 : Const<int>
{
    public static int Value => 8;
}

public sealed class N9 : Const<int>
{
    public static int Value => 9;
}


public sealed class N<D10, D1> : Const<int>
    where D10 : Const<int>
    where D1 : Const<int>
{
    public static int Value =>
        D10.Value * 10 +
        D1.Value;
}

public sealed class N<D100, D10, D1> : Const<int>
    where D100 : Const<int>
    where D10 : Const<int>
    where D1 : Const<int>
{
    public static int Value =>
        D100.Value * 100 +
        D10.Value * 10 +
        D1.Value;
}

public sealed class NNeg<N> : Const<int>
    where N : Const<int>
{
    public static int Value => -N.Value;
}
