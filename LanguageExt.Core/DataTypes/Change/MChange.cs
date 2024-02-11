using LanguageExt.TypeClasses;

namespace LanguageExt;

/// <summary>
/// `Monoid` instance for `Change`
/// </summary>
public struct MChange<A> : Monoid<Change<A>>
{
    public static Change<A> Append(Change<A> x, Change<A> y) =>
        (x, y) switch
        {
            (NoChange<A> _, _)                                     => y,
            (_, NoChange<A> _)                                     => x,
            (_, EntryRemoved<A> _)                                 => y,
            (EntryRemoved<A> (var vx), EntryAdded<A> (var vy))     => Change<A>.Mapped(vx, vy),
            (EntryAdded<A> _, EntryMappedTo<A>(var vz))            => Change<A>.Added(vz),
            (EntryMappedFrom<A>(var vx), EntryMappedTo<A>(var vz)) => Change<A>.Mapped(vx, vz),
            _                                                      => y 
        };

    public static Change<A> Empty => 
        Change<A>.None;
}
