using LanguageExt.Traits;

namespace DomainTypesExamples.Invariants;

public sealed class SizeLowerThan<V, F, A> : RuleK<SizeLowerThan<V, F, A>, F, A>
    where V : Const<int>
    where F : Foldable<F>
{
    public int Value => V.Value;

    public static bool Check(K<F, A> value) => 
        value.Count < V.Value;

}

public sealed class SizeHigherThan<V, F, A> : RuleK<SizeHigherThan<V, F, A>, F, A>
    where V : Const<int>
    where F : Foldable<F>
{
    public int Value => V.Value;

    public static bool Check(K<F, A> value) =>
        value.Count > V.Value;

}

public sealed class SizeEqualTo<V, F, A> : RuleK<SizeEqualTo<V, F, A>, F, A>
    where V : Const<int>
    where F : Foldable<F>
{
    public int Value => V.Value;

    public static bool Check(K<F, A> value) =>
        value.Count > V.Value;

}

public sealed class SizeLowerOrEqualTo<V, F, A> :
    Rule.ForK<F, A>.Any<SizeLowerThan<V, F, A>, SizeEqualTo<V, F, A>>,
    RuleK<SizeLowerOrEqualTo<V, F, A>, F, A>
    where V : Const<int>
    where F : Foldable<F>
{
    public int Value => V.Value;
}

public sealed class SizeHigherOrEqualTo<V, F, A> :
    Rule.ForK<F, A>.Any<SizeHigherThan<V, F, A>, SizeEqualTo<V, F, A>>,
    RuleK<SizeHigherOrEqualTo<V, F, A>, F, A>
    where V : Const<int>
    where F : Foldable<F>
{
    public int Value => V.Value;
}

public sealed class SizeBetween<MIN, MAX, F, A> : 
    Rule.ForK<F, A>.All<SizeHigherOrEqualTo<MIN, F, A>, SizeLowerOrEqualTo<MAX, F, A>>, 
    RuleK<SizeBetween<MIN, MAX, F, A>, F, A>
    where MIN : Const<int>
    where MAX : Const<int>
    where F : Foldable<F>
{
    public int Min => MIN.Value;

    public int Max => MAX.Value;
}

public sealed class StringSizeBetween<MIN, MAX> : Rule<StringSizeBetween<MIN, MAX>, string>
    where MIN : Const<int>
    where MAX : Const<int>
{
    public int Min => MIN.Value;

    public int Max => MAX.Value;

    public static bool Check(string value) =>
        value.Length >= MIN.Value && value.Length <= MAX.Value;

}
