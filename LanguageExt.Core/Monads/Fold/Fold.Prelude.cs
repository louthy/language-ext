#nullable enable

using System;

namespace LanguageExt;

public static partial class Prelude
{
    /// <summary>
    /// Fold operation that on its own doesn't do much, but when combined with the
    /// transducer based monads, will fold the stream of values flowing through
    /// the transducer
    /// </summary>
    /// <param name="schedule">Schedule which can limit the rate of processing but also the amount</param>
    /// <param name="initialState">Starting state for the fold operation</param>
    /// <param name="folder">Folding function</param>
    /// <param name="predicate">Predicate that indicates whether the fold is complete.  True means continue.</param>
    /// <typeparam name="A">Input value type</typeparam>
    /// <typeparam name="S">State type</typeparam>
    public static Fold<A, S> foldWhile<A, S>(
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder,
        Func<(S State, A Value), bool> predicate) =>
        Fold<A, S>.New(schedule, initialState, folder, predicate);
    
    /// <summary>
    /// Fold operation that on its own doesn't do much, but when combined with the
    /// transducer based monads, will fold the stream of values flowing through
    /// the transducer
    /// </summary>
    /// <param name="schedule">Schedule which can limit the rate of processing but also the amount</param>
    /// <param name="initialState">Starting state for the fold operation</param>
    /// <param name="folder">Folding function</param>
    /// <param name="stateIs">Predicate that indicates whether the fold is complete.  True means continue.</param>
    /// <typeparam name="A">Input value type</typeparam>
    /// <typeparam name="S">State type</typeparam>
    public static Fold<A, S> foldWhile<A, S>(
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder,
        Func<S, bool> stateIs) =>
        Fold<A, S>.New(schedule, initialState, folder, last => stateIs(last.State));
    
    /// <summary>
    /// Fold operation that on its own doesn't do much, but when combined with the
    /// transducer based monads, will fold the stream of values flowing through
    /// the transducer
    /// </summary>
    /// <param name="schedule">Schedule which can limit the rate of processing but also the amount</param>
    /// <param name="initialState">Starting state for the fold operation</param>
    /// <param name="folder">Folding function</param>
    /// <param name="valueIs">Predicate that indicates whether the fold is complete.  True means continue.</param>
    /// <typeparam name="A">Input value type</typeparam>
    /// <typeparam name="S">State type</typeparam>
    public static Fold<A, S> foldWhile<A, S>(
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder,
        Func<A, bool> valueIs) =>
        Fold<A, S>.New(schedule, initialState, folder, last => valueIs(last.Value));
    
    /// <summary>
    /// Fold operation that on its own doesn't do much, but when combined with the
    /// transducer based monads, will fold the stream of values flowing through
    /// the transducer
    /// </summary>
    /// <param name="schedule">Schedule which can limit the rate of processing but also the amount</param>
    /// <param name="initialState">Starting state for the fold operation</param>
    /// <param name="folder">Folding function</param>
    /// <param name="predicate">Predicate that indicates whether the fold is complete.  True means continue.</param>
    /// <typeparam name="A">Input value type</typeparam>
    /// <typeparam name="S">State type</typeparam>
    public static Fold<A, S> foldWhile<A, S>(
        S initialState,
        Func<S, A, S> folder,
        Func<(S State, A Value), bool> predicate) =>
        Fold<A, S>.New(Schedule.Forever, initialState, folder, predicate);
    
    /// <summary>
    /// Fold operation that on its own doesn't do much, but when combined with the
    /// transducer based monads, will fold the stream of values flowing through
    /// the transducer
    /// </summary>
    /// <param name="schedule">Schedule which can limit the rate of processing but also the amount</param>
    /// <param name="initialState">Starting state for the fold operation</param>
    /// <param name="folder">Folding function</param>
    /// <param name="stateIs">Predicate that indicates whether the fold is complete.  True means continue.</param>
    /// <typeparam name="A">Input value type</typeparam>
    /// <typeparam name="S">State type</typeparam>
    public static Fold<A, S> foldWhile<A, S>(
        S initialState,
        Func<S, A, S> folder,
        Func<S, bool> stateIs) =>
        Fold<A, S>.New(Schedule.Forever, initialState, folder, last => stateIs(last.State));
    
    /// <summary>
    /// Fold operation that on its own doesn't do much, but when combined with the
    /// transducer based monads, will fold the stream of values flowing through
    /// the transducer
    /// </summary>
    /// <param name="schedule">Schedule which can limit the rate of processing but also the amount</param>
    /// <param name="initialState">Starting state for the fold operation</param>
    /// <param name="folder">Folding function</param>
    /// <param name="valueIs">Predicate that indicates whether the fold is complete.  True means continue.</param>
    /// <typeparam name="A">Input value type</typeparam>
    /// <typeparam name="S">State type</typeparam>
    public static Fold<A, S> foldWhile<A, S>(
        S initialState,
        Func<S, A, S> folder,
        Func<A, bool> valueIs) =>
        Fold<A, S>.New(Schedule.Forever, initialState, folder, last => valueIs(last.Value));
    
    /// <summary>
    /// Fold operation that on its own doesn't do much, but when combined with the
    /// transducer based monads, will fold the stream of values flowing through
    /// the transducer
    /// </summary>
    /// <param name="schedule">Schedule which can limit the rate of processing but also the amount</param>
    /// <param name="initialState">Starting state for the fold operation</param>
    /// <param name="folder">Folding function</param>
    /// <param name="predicate">Predicate that indicates whether the fold is complete.  True means complete.</param>
    /// <typeparam name="A">Input value type</typeparam>
    /// <typeparam name="S">State type</typeparam>
    public static Fold<A, S> foldUntil<A, S>(
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder,
        Func<(S State, A Value), bool> predicate) =>
        foldWhile(schedule, initialState, folder, not(predicate));
    
    /// <summary>
    /// Fold operation that on its own doesn't do much, but when combined with the
    /// transducer based monads, will fold the stream of values flowing through
    /// the transducer
    /// </summary>
    /// <param name="schedule">Schedule which can limit the rate of processing but also the amount</param>
    /// <param name="initialState">Starting state for the fold operation</param>
    /// <param name="folder">Folding function</param>
    /// <param name="stateIs">Predicate that indicates whether the fold is complete.  True means complete.</param>
    /// <typeparam name="A">Input value type</typeparam>
    /// <typeparam name="S">State type</typeparam>
    public static Fold<A, S> foldUntil<A, S>(
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder,
        Func<S, bool> stateIs) =>
        foldWhile(schedule, initialState, folder, not(stateIs));
    
    /// <summary>
    /// Fold operation that on its own doesn't do much, but when combined with the
    /// transducer based monads, will fold the stream of values flowing through
    /// the transducer
    /// </summary>
    /// <param name="schedule">Schedule which can limit the rate of processing but also the amount</param>
    /// <param name="initialState">Starting state for the fold operation</param>
    /// <param name="folder">Folding function</param>
    /// <param name="valueIs">Predicate that indicates whether the fold is complete.  True means continue.</param>
    /// <typeparam name="A">Input value type</typeparam>
    /// <typeparam name="S">State type</typeparam>
    public static Fold<A, S> foldUntil<A, S>(
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder,
        Func<A, bool> valueIs) =>
        foldWhile(schedule, initialState, folder, not(valueIs));
    
    /// <summary>
    /// Fold operation that on its own doesn't do much, but when combined with the
    /// transducer based monads, will fold the stream of values flowing through
    /// the transducer
    /// </summary>
    /// <param name="schedule">Schedule which can limit the rate of processing but also the amount</param>
    /// <param name="initialState">Starting state for the fold operation</param>
    /// <param name="folder">Folding function</param>
    /// <param name="predicate">Predicate that indicates whether the fold is complete.  True means complete.</param>
    /// <typeparam name="A">Input value type</typeparam>
    /// <typeparam name="S">State type</typeparam>
    public static Fold<A, S> foldUntil<A, S>(
        S initialState,
        Func<S, A, S> folder,
        Func<(S State, A Value), bool> predicate) =>
        foldWhile(initialState, folder, not(predicate));
    
    /// <summary>
    /// Fold operation that on its own doesn't do much, but when combined with the
    /// transducer based monads, will fold the stream of values flowing through
    /// the transducer
    /// </summary>
    /// <param name="schedule">Schedule which can limit the rate of processing but also the amount</param>
    /// <param name="initialState">Starting state for the fold operation</param>
    /// <param name="folder">Folding function</param>
    /// <param name="stateIs">Predicate that indicates whether the fold is complete.  True means complete.</param>
    /// <typeparam name="A">Input value type</typeparam>
    /// <typeparam name="S">State type</typeparam>
    public static Fold<A, S> foldUntil<A, S>(
        S initialState,
        Func<S, A, S> folder,
        Func<S, bool> stateIs) =>
        foldWhile(initialState, folder, not(stateIs));
    
    /// <summary>
    /// Fold operation that on its own doesn't do much, but when combined with the
    /// transducer based monads, will fold the stream of values flowing through
    /// the transducer
    /// </summary>
    /// <param name="schedule">Schedule which can limit the rate of processing but also the amount</param>
    /// <param name="initialState">Starting state for the fold operation</param>
    /// <param name="folder">Folding function</param>
    /// <param name="valueIs">Predicate that indicates whether the fold is complete.  True means complete.</param>
    /// <typeparam name="A">Input value type</typeparam>
    /// <typeparam name="S">State type</typeparam>
    public static Fold<A, S> foldUntil<A, S>(
        S initialState,
        Func<S, A, S> folder,
        Func<A, bool> valueIs) =>
        foldWhile(initialState, folder, not(valueIs));
}
