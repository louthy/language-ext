#nullable enable
using System;
using System.Linq;
using System.Threading;
using System.Collections;
using System.ComponentModel;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using LanguageExt.ClassInstances;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace LanguageExt
{
    /// <summary>
    /// Wraps up a version vector, making it easier to work with and not generics hell
    /// </summary>
    /// <typeparam name="Actor">Actor type</typeparam>
    /// <typeparam name="V">Value type</typeparam>
    public abstract record Version<Actor, V>
    {
        /// <summary>
        /// Perform a write to the vector.  This increases the vector-clock by 1 for the `actor` provided.
        /// </summary>
        /// <param name="value">Value to write</param>
        public abstract Version<Actor, V> Write(Actor actor, long timeStamp, V value);

        /// <summary>
        /// Perform a write to the vector.  This increases the vector-clock by 1 for the `actor` provided.
        /// </summary>
        /// <param name="value">Value to write</param>
        public Version<Actor, V> Write(Actor actor, V value) =>
            Write(actor, DateTime.UtcNow.Ticks, value);

        /// <summary>
        /// Perform a delete to the vector.  This increases the vector-clock by 1 for the `actor` provided.
        /// </summary>
        /// <param name="value">Value to write</param>
        public abstract Version<Actor, V> Delete(Actor actor, long timeStamp);

        /// <summary>
        /// Perform a delete to the vector.  This increases the vector-clock by 1 for the `actor` provided.
        /// </summary>
        /// <param name="value">Value to write</param>
        public Version<Actor, V> Delete(Actor actor) =>
            Delete(actor, DateTime.UtcNow.Ticks);

        /// <summary>
        /// Get the value if there is one
        /// </summary>
        public abstract Option<V> Value { get; }
    }

    /// <summary>
    /// Internal: Helper functions for mapping between the non-generics Version and the generics heavy VersionVector
    /// </summary>
    internal static class Version
    {
        public static Version<Actor, V> ToVersion<ConflictV, OrdActor, Actor, V>(this VersionVector<ConflictV, OrdActor, TLong, Actor, long, V> vector)
            where OrdActor   : struct, Ord<Actor>
            where ConflictV : struct, Conflict<V> =>
            vector.Value.IsSome
                ? new VersionValueVector<ConflictV, OrdActor, Actor, V>(vector)
                : new VersionDeletedVector<ConflictV, OrdActor, Actor, V>(vector);

        public static VersionVector<ConflictV, OrdActor, TLong, Actor, long, V>? ToVector<ConflictV, OrdActor, Actor, V>(this Version<Actor, V> version)
            where OrdActor : struct, Ord<Actor>
            where ConflictV : struct, Conflict<V> =>
            version switch
            {
                VersionValueVector<ConflictV, OrdActor, Actor, V> vv   => vv.Vector,
                VersionDeletedVector<ConflictV, OrdActor, Actor, V> vd => vd.Vector,
                _                                                      => null
            };
    }

    /// <summary>
    /// Abstract representation of a version vector with a value
    /// </summary>
    /// <typeparam name="Actor">Actor type</typeparam>
    /// <typeparam name="V">Value type</typeparam>
    internal abstract record VersionSome<Actor, V>(V value) : Version<Actor, V>
    {
        /// <summary>
        /// Get the value if there is one
        /// </summary>
        public override Option<V> Value =>
            value;
    }

    /// <summary>
    /// Abstract representation of a version vector without a value (it either never existed or has been deleted)
    /// </summary>
    /// <typeparam name="Actor">Actor type</typeparam>
    /// <typeparam name="V">Value type</typeparam>
    internal abstract record VersionNone<Actor, V> : Version<Actor, V>
    {

        /// <summary>
        /// Get the value if there is one
        /// </summary>
        public override Option<V> Value =>
            None;
    }

    /// <summary>
    /// Representation of a version vector that never existed
    /// </summary>
    /// <typeparam name="Actor">Actor type</typeparam>
    /// <typeparam name="V">Value type</typeparam>
    internal record VersionNeverExistedVector<ConflictV, OrdActor, Actor, V> : VersionNone<Actor, V>
        where OrdActor  : struct, Ord<Actor>
        where ConflictV : struct, Conflict<V>
    {
        public static Version<Actor, V> Default = new VersionNeverExistedVector<ConflictV, OrdActor, Actor, V>();

        /// <summary>
        /// Perform a write to the vector.  This increases the vector-clock by 1 for the `actor` provided.
        /// </summary>
        /// <param name="value">Value to write</param>
        public override Version<Actor, V> Write(Actor actor, long timeStamp, V value) =>
            new VersionValueVector<ConflictV, OrdActor, Actor, V>(
                new VersionVector<ConflictV, OrdActor, TLong, Actor, long, V>(
                    value,
                    timeStamp,
                    VectorClock.Single<OrdActor, TLong, Actor, long>(actor, 1L)));

        /// <summary>
        /// Perform a write to the vector.  This increases the vector-clock by 1 for the `actor` provided.
        /// </summary>
        /// <param name="value">Value to write</param>
        public override Version<Actor, V> Delete(Actor actor, long timeStamp) =>
            this;
    }

    /// <summary>
    /// Representation of a version vector that existed but has since had its value deleted
    /// </summary>
    /// <typeparam name="Actor">Actor type</typeparam>
    /// <typeparam name="V">Value type</typeparam>
    internal record VersionDeletedVector<ConflictV, OrdActor, Actor, V>(VersionVector<ConflictV, OrdActor, TLong, Actor, long, V> Vector) : VersionNone<Actor, V>
        where OrdActor : struct, Ord<Actor>
        where ConflictV : struct, Conflict<V>
    {
        /// <summary>
        /// Perform a write to the vector.  This increases the vector-clock by 1 for the `actor` provided.
        /// </summary>
        /// <param name="value">Value to write</param>
        public override Version<Actor, V> Write(Actor actor, long timeStamp, V value) =>
            new VersionValueVector<ConflictV, OrdActor, Actor, V>(Vector.Put(actor, timeStamp, value));
        
        /// <summary>
        /// Perform a write to the vector.  This increases the vector-clock by 1 for the `actor` provided.
        /// </summary>
        /// <param name="value">Value to write</param>
        public override Version<Actor, V> Delete(Actor actor, long timeStamp) =>
            new VersionDeletedVector<ConflictV, OrdActor, Actor, V>(Vector.Put(actor, timeStamp, None));
    }

    /// <summary>
    /// Representation of a version vector with a value
    /// </summary>
    /// <typeparam name="Actor">Actor type</typeparam>
    /// <typeparam name="V">Value type</typeparam>
    internal record VersionValueVector<ConflictV, OrdActor, Actor, V>(VersionVector<ConflictV, OrdActor, TLong, Actor, long, V> Vector) : VersionSome<Actor, V>(Vector.Value.Value)
        where OrdActor : struct, Ord<Actor>
        where ConflictV : struct, Conflict<V>
    {
        /// <summary>
        /// Perform a write to the vector.  This increases the vector-clock by 1 for the `actor` provided.
        /// </summary>
        /// <param name="value">Value to write</param>
        public override Version<Actor, V> Write(Actor actor, long timeStamp, V value) =>
            new VersionValueVector<ConflictV, OrdActor, Actor, V>(Vector.Put(actor, timeStamp, value));
        
        /// <summary>
        /// Perform a write to the vector.  This increases the vector-clock by 1 for the `actor` provided.
        /// </summary>
        /// <param name="value">Value to write</param>
        public override Version<Actor, V> Delete(Actor actor, long timeStamp) =>
            new VersionDeletedVector<ConflictV, OrdActor, Actor, V>(Vector.Put(actor, timeStamp, None));
    }
}
#nullable disable
