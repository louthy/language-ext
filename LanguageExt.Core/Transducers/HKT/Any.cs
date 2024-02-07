namespace LanguageExt.HKT;

/// <summary>
/// Any kind trait
/// </summary>
public record Any : KLift<Any>
{
    public static KStar<Any, A> Lift<A>(Transducer<Unit, A> f) => 
        new Star<A>(f);
}

/// <summary>
/// Any kind trait
/// </summary>
public record Any<A> : KLift<Any<A>, A>
{
    public static KArrow<Any<A>, A, B> Lift<B>(Transducer<A, B> f) => 
        new Arrow<A, B>(f);
}

public record Arrow<A, B>(Transducer<A, B> Transducer) : KArrow<Any<A>, A, B>
{
    public Transducer<A, B> Morphism { get; } = Transducer;
}

public record Star<A>(Transducer<Unit, A> Transducer) : KStar<Any, A>
{
    public Transducer<Unit, A> Morphism { get; } = Transducer;
}
