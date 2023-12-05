/*
#nullable enable

using System;
using LanguageExt.Transducers;

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
    public static Transducer<A, S> foldWhile<A, S>(
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder,
        Func<(S State, A Value), bool> predicate) =>
        Transducer.foldWhile(schedule, initialState, folder, predicate);
    
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
    public static Transducer<A, S> foldWhile<A, S>(
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder,
        Func<S, bool> stateIs) =>
        Transducer.foldWhile(schedule, initialState, folder, stateIs);
    
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
    public static Transducer<A, S> foldWhile<A, S>(
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder,
        Func<A, bool> valueIs) =>
        Transducer.foldWhile(schedule, initialState, folder, valueIs);
    
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
    public static Transducer<A, S> foldWhile<A, S>(
        S initialState,
        Func<S, A, S> folder,
        Func<(S State, A Value), bool> predicate) =>
        Transducer.foldWhile(initialState, folder, predicate);
    
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
    public static Transducer<A, S> foldWhile<A, S>(
        S initialState,
        Func<S, A, S> folder,
        Func<S, bool> stateIs) =>
        Transducer.foldWhile(initialState, folder, stateIs);
    
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
    public static Transducer<A, S> foldWhile<A, S>(
        S initialState,
        Func<S, A, S> folder,
        Func<A, bool> valueIs) =>
        Transducer.foldWhile(initialState, folder, valueIs);
    
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
    public static Transducer<A, S> foldUntil<A, S>(
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder,
        Func<(S State, A Value), bool> predicate) =>
        Transducer.foldUntil(schedule, initialState, folder, predicate);
    
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
    public static Transducer<A, S> foldUntil<A, S>(
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder,
        Func<S, bool> stateIs) =>
        Transducer.foldUntil(schedule, initialState, folder, stateIs);
    
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
    public static Transducer<A, S> foldUntil<A, S>(
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder,
        Func<A, bool> valueIs) =>
        Transducer.foldUntil(schedule, initialState, folder, valueIs);
    
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
    public static Transducer<A, S> foldUntil<A, S>(
        S initialState,
        Func<S, A, S> folder,
        Func<(S State, A Value), bool> predicate) =>
        Transducer.foldUntil(initialState, folder, predicate);
    
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
    public static Transducer<A, S> foldUntil<A, S>(
        S initialState,
        Func<S, A, S> folder,
        Func<S, bool> stateIs) =>
        Transducer.foldUntil(initialState, folder, stateIs);
    
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
    public static Transducer<A, S> foldUntil<A, S>(
        S initialState,
        Func<S, A, S> folder,
        Func<A, bool> valueIs) =>
        Transducer.foldUntil(initialState, folder, valueIs);
}
*/
