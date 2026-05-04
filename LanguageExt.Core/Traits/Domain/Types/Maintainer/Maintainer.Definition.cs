namespace LanguageExt.Traits.Domain;

public interface Maintainer<SELF> : 
    DomainSet<SELF>
    where SELF : Maintainer<SELF>
{
    static abstract Seq<SELF> All { get; }
}

public interface Maintainer<SELF, REPR> : 
    Maintainer<SELF>
    where SELF : Maintainer<SELF, REPR>
{
    REPR To();
}
