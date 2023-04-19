using System;

namespace LanguageExt;

public interface Trait { }
public interface Eq : Trait { }
public interface Ord : Eq { }
public interface Show : Trait { }
public interface Read : Trait { }
public interface Generic : Trait { }
public interface Lens : Trait { }
public interface Functor : Trait { }
public interface Monad : Trait { }
public interface Record : Ord, Show, Read, Generic, Lens { }
public interface Union : Ord, Show, Read, Generic, Lens { }

public interface deriving<A> 
    where A : Trait 
{ }
public interface deriving<A, B> 
    where A : Trait 
    where B : Trait 
{ }
public interface deriving<A, B, C> 
    where A : Trait 
    where B : Trait 
    where C : Trait 
{ }
public interface deriving<A, B, C, D> 
    where A : Trait 
    where B : Trait 
    where C : Trait 
    where D : Trait 
{ }
public interface deriving<A, B, C, D, E> 
    where A : Trait 
    where B : Trait 
    where C : Trait 
    where D : Trait 
    where E : Trait 
{ }
public interface deriving<A, B, C, D, E, F> 
    where A : Trait 
    where B : Trait 
    where C : Trait 
    where D : Trait 
    where E : Trait 
    where F : Trait 
{ }
public interface deriving<A, B, C, D, E, F, G> 
    where A : Trait 
    where B : Trait 
    where C : Trait 
    where D : Trait 
    where E : Trait 
    where F : Trait 
    where G : Trait 
{ }
public interface deriving<A, B, C, D, E, F, G, H> 
    where A : Trait 
    where B : Trait 
    where C : Trait 
    where D : Trait 
    where E : Trait 
    where F : Trait 
    where G : Trait 
    where H : Trait 
{ }
