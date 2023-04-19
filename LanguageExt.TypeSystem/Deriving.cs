using System;

namespace LanguageExt.TypeSystem;

public abstract record Deriving
{
    public static readonly Deriving Eq = new DeriveEq();
    public static readonly Deriving Ord = new DeriveOrd();
    public static readonly Deriving Show = new DeriveShow();
    public static readonly Deriving Read = new DeriveRead();
    public static readonly Deriving Generic = new DeriveGeneric();
    public static readonly Deriving Lens = new DeriveLens();
    public static readonly Deriving Functor = new DeriveFunctor();
    public static readonly Deriving Applicative = new DeriveApplicative();
    public static readonly Deriving Monad = new DeriveMonad();
    public static readonly Deriving Record = new DeriveRecord();
    public static readonly Deriving Union = new DeriveUnion();

    public static Deriving FromString(string name) =>
        name switch
        {
            "Eq" => Eq,
            "Ord" => Ord,
            "Show" => Show,
            "Read" => Read,
            "Generic" => Generic,
            "Lens" => Lens,
            "Functor" => Functor,
            "Applicative" => Applicative,
            "Monad" => Monad,
            "Record" => Record,
            "Union" => Union,
            _ => throw new NotSupportedException()
        };
}

public record DeriveEq : Deriving;
public record DeriveOrd : DeriveEq;
public record DeriveShow : Deriving;
public record DeriveRead : Deriving;
public record DeriveGeneric : Deriving;
public record DeriveLens : Deriving;
public record DeriveFunctor : Deriving;
public record DeriveApplicative : DeriveFunctor;
public record DeriveMonad : DeriveApplicative;
public record DeriveRecord : Deriving;
public record DeriveUnion : Deriving;
