using System;
using System.Diagnostics;
using LanguageExt.Effects.Traits;
using LanguageExt.Sys.Traits;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt.Sys.Diag;

/// <summary>
/// An `Activity` has an operation name, an ID, a start time and duration, tags, and baggage.
/// 
/// Activities should be created by calling the `span` functions, configured as necessary.  Each `span` function
/// takes an `Eff` or `Aff` operation to run (which is the activity).  The runtime system will maintain the parent-
/// child relationships for the activities, and maintains the 'current' activity.
/// </summary>
/// <typeparam name="M">Reader, Resource, and monad trait</typeparam>
/// <typeparam name="RT">Runtime</typeparam>
public static class Activity<M, RT>
    where M : Reader<M, RT>, Resource<M>, Monad<M>
    where RT : HasActivitySource<RT>, HasIO<RT>
{
    static K<M, Activity> startActivity(
        string name,
        ActivityKind activityKind,
        HashMap<string, object> activityTags,
        Seq<ActivityLink> activityLinks,
        DateTimeOffset startTime,
        ActivityContext? parentContext = default) =>
        from rt in Reader.ask<M, RT>()
        from src in rt.ActivitySourceEff
        from act in Resource.use<M, Activity>(
            src.StartActivity(
                name,
                activityKind,
                rt.CurrentActivity != null
                    ? parentContext ?? rt.CurrentActivity.Context
                    : default,
                activityTags,
                activityLinks,
                startTime))
        select act;

    /// <summary>
    /// Creates a new activity if there are active listeners for it, using the specified name, activity kind, parent
    /// activity context, tags, optional activity link and optional start time.
    /// </summary>
    /// <param name="name">The operation name of the activity.</param>
    /// <param name="operation">The operation to whose activity will be traced</param>
    /// <returns>The result of the `operation`</returns>
    public static K<M, A> span<A>(string name, K<M, A> operation) =>
        span(name, ActivityKind.Internal, default, default, DateTimeOffset.Now, operation);

    /// <summary>
    /// Creates a new activity if there are active listeners for it, using the specified name, activity kind, parent
    /// activity context, tags, optional activity link and optional start time.
    /// </summary>
    /// <param name="name">The operation name of the activity.</param>
    /// <param name="activityKind">The activity kind.</param>
    /// <param name="operation">The operation to whose activity will be traced</param>
    /// <returns>The result of the `operation`</returns>
    public static K<M, A> span<A>(
        string name,
        ActivityKind activityKind,
        K<M, A> operation) => 
        span(name, activityKind, default, default, DateTimeOffset.Now, operation);

    /// <summary>
    /// Creates a new activity if there are active listeners for it, using the specified name, activity kind, parent
    /// activity context, tags, optional activity link and optional start time.
    /// </summary>
    /// <param name="name">The operation name of the activity.</param>
    /// <param name="activityKind">The activity kind.</param>
    /// <param name="activityTags">The optional tags list to initialise the created activity object with.</param>
    /// <param name="operation">The operation to whose activity will be traced</param>
    /// <returns>The result of the `operation`</returns>
    public static K<M, A> span<A>(
        string name,
        ActivityKind activityKind,
        HashMap<string, object> activityTags,
        K<M, A> operation) => 
        span(name, activityKind, activityTags, default, DateTimeOffset.Now, operation);

    /// <summary>
    /// Creates a new activity if there are active listeners for it, using the specified name, activity kind, parent
    /// activity context, tags, optional activity link and optional start time.
    /// </summary>
    /// <param name="name">The operation name of the activity.</param>
    /// <param name="activityKind">The activity kind.</param>
    /// <param name="activityTags">The optional tags list to initialise the created activity object with.</param>
    /// <param name="activityLinks">The optional `ActivityLink` list to initialise the created activity object with.</param>
    /// <param name="startTime">The optional start timestamp to set on the created activity object.</param>
    /// <param name="operation">The operation to whose activity will be traced</param>
    /// <returns>The result of the `operation`</returns>
    public static K<M, TA> span<TA>(
        string name,
        ActivityKind activityKind,
        HashMap<string, object> activityTags,
        Seq<ActivityLink> activityLinks,
        DateTimeOffset startTime,
        K<M, TA> operation) =>
        from a in startActivity(name, activityKind, activityTags, activityLinks, startTime)
        from r in Reader.local<M, RT, TA>(rt => rt.WithActivity(a), operation)
        select r;

    /// <summary>
    /// Creates a new activity if there are active listeners for it, using the specified name, activity kind, parent
    /// activity context, tags, optional activity link and optional start time.
    /// </summary>
    /// <param name="name">The operation name of the activity.</param>
    /// <param name="activityKind">The activity kind.</param>
    /// <param name="parentContext">The parent `ActivityContext` object to initialize the created activity object
    /// with</param>
    /// <param name="activityTags">The optional tags list to initialise the created activity object with.</param>
    /// <param name="activityLinks">The optional `ActivityLink` list to initialise the created activity object with.</param>
    /// <param name="startTime">The optional start timestamp to set on the created activity object.</param>
    /// <param name="operation">The operation to whose activity will be traced</param>
    /// <returns>The result of the `operation`</returns>
    public static K<M, A> span<A>(
        string name,
        ActivityKind activityKind,
        ActivityContext parentContext,
        HashMap<string, object> activityTags,
        Seq<ActivityLink> activityLinks,
        DateTimeOffset startTime,
        K<M, A> operation) =>
        from a in startActivity(
            name,
            activityKind,
            activityTags,
            activityLinks,
            startTime,
            parentContext)
        from r in Reader.local<M, RT, A>(rt => rt.WithActivity(a), operation)
        select r;

    /// <summary>
    /// Set the state trace string
    /// </summary>
    /// <param name="traceStateString">Trace state string</param>
    /// <returns>Unit effect</returns>
    public static K<M, Unit> setTraceState(string traceStateString) =>
        Reader.asks<M, RT, Unit>(
            rt =>
            {
                if (rt.CurrentActivity is not null)
                {
                    rt.CurrentActivity.TraceStateString = traceStateString;
                }
                return unit;
            });

    /// <summary>
    /// Read the trace-state string of the current activity
    /// </summary>
    public static K<M, Option<string>> traceState =>
        Reader.asks<M, RT, Option<string>>(
            rt => Optional(rt.CurrentActivity?.TraceStateString));

    /// <summary>
    /// Read the trace ID of the current activity
    /// </summary>
    public static K<M, Option<ActivityTraceId>> traceId =>
        Reader.asks<M, RT, Option<ActivityTraceId>>(
            rt => Optional(rt.CurrentActivity?.TraceId));

    /// <summary>
    /// Add baggage to the current activity
    /// </summary>
    /// <param name="key">Baggage key</param>
    /// <param name="value">Baggage value</param>
    /// <returns>Unit effect</returns>
    public static K<M, Unit> addBaggage(string key, string? value) =>
        Reader.asks<M, RT, Unit>(
            rt =>
            {
                rt.CurrentActivity?.AddBaggage(key, value);
                return unit;
            });

    /// <summary>
    /// Read the baggage of the current activity
    /// </summary>
    public static K<M, HashMap<string, string?>> baggage =>
        Reader.asks<M, RT, HashMap<string, string?>>(
            rt => rt.CurrentActivity is not null
                      ? rt.CurrentActivity.Baggage.ToHashMap()
                      : HashMap<string, string?>());

    /// <summary>
    /// Add tag to the current activity
    /// </summary>
    /// <param name="name">Tag name</param>
    /// <param name="value">Tag value</param>
    /// <returns>Unit effect</returns>
    public static K<M, Unit> addTag(string name, string? value) =>
        Reader.asks<M, RT, Unit>(
            rt =>
            {
                rt.CurrentActivity?.AddTag(name, value);
                return unit;
            });

    /// <summary>
    /// Add tag to the current activity
    /// </summary>
    /// <param name="name">Tag name</param>
    /// <param name="value">Tag value</param>
    public static K<M, Unit> addTag(string name, object? value) =>
        Reader.asks<M, RT, Unit>(
            rt =>
            {
                rt.CurrentActivity?.AddTag(name, value);
                return unit;
            });

    /// <summary>
    /// Read the tags of the current activity
    /// </summary>
    public static K<M, HashMap<string, string?>> tags =>
        Reader.asks<M, RT, HashMap<string, string?>>(
            rt => rt.CurrentActivity is not null
                ? rt.CurrentActivity.Tags.ToHashMap()
                : HashMap<string, string?>());

    /// <summary>
    /// Read the tags of the current activity
    /// </summary>
    public static K<M, HashMap<string, object?>> tagObjects =>
        Reader.asks<M, RT, HashMap<string, object?>>(
            rt => rt.CurrentActivity is not null
                      ? rt.CurrentActivity.TagObjects.ToHashMap()
                      : HashMap<string, object?>());

    /// <summary>
    /// Read the context of the current activity
    /// </summary>
    /// <remarks>None if there is no current activity</remarks>
    public static K<M, Option<ActivityContext>> context =>
        Reader.asks<M, RT, Option<ActivityContext>>(
            rt => Optional(rt.CurrentActivity?.Context));

    /// <summary>
    /// Read the duration of the current activity
    /// </summary>
    /// <remarks>None if there is no current activity</remarks>
    public static K<M, Option<TimeSpan>> duration =>
        Reader.asks<M, RT, Option<TimeSpan>>(
            rt => Optional(rt.CurrentActivity?.Duration));

    /// <summary>
    /// Add an event to the current activity
    /// </summary>
    /// <param name="event">Event</param>
    public static K<M, Unit> addEvent(ActivityEvent @event) =>
        Reader.asks<M, RT, Unit>(
            rt =>
            {
                rt.CurrentActivity?.AddEvent(@event);
                return unit;
            });

    /// <summary>
    /// Read the events of the current activity
    /// </summary>
    public static K<M, Seq<ActivityEvent>> events =>
        Reader.asks<M, RT, Seq<ActivityEvent>>(
            rt => rt.CurrentActivity is not null
                      ? rt.CurrentActivity.Events.ToSeq()
                      : Seq<ActivityEvent>());

    /// <summary>
    /// Read the ID of the current activity
    /// </summary>
    /// <remarks>None if there is no current activity</remarks>
    public static K<M, Option<string>> id =>
        Reader.asks<M, RT, Option<string>>(
            rt => Optional(rt.CurrentActivity?.Id));

    /// <summary>
    /// Read the kind of the current activity
    /// </summary>
    /// <remarks>None if there is no current activity</remarks>
    public static K<M, Option<ActivityKind>> kind =>
        Reader.asks<M, RT, Option<ActivityKind>>(
            rt => Optional(rt.CurrentActivity?.Kind));

    /// <summary>
    /// Read the links of the current activity
    /// </summary>
    public static K<M, Seq<ActivityLink>> links =>
        Reader.asks<M, RT, Seq<ActivityLink>>(
            rt => rt.CurrentActivity is not null
                      ? rt.CurrentActivity.Links.ToSeq()
                      : Seq<ActivityLink>());

    /// <summary>
    /// Read the current activity
    /// </summary>
    /// <remarks>None if there is no current activity</remarks>
    public static K<M, Option<Activity>> current =>
        Reader.asks<M, RT, Option<Activity>>(
            rt => Optional(rt.CurrentActivity));

    /// <summary>
    /// Read the parent ID of the current activity
    /// </summary>
    /// <remarks>None if there is no current activity</remarks>
    public static K<M, Option<string>> parentId =>
        Reader.asks<M, RT, Option<string>>(
            rt => Optional(rt.CurrentActivity?.ParentId));

    /// <summary>
    /// Read the parent span ID of the current activity
    /// </summary>
    /// <remarks>None if there is no current activity</remarks>
    public static K<M, Option<ActivitySpanId>> parentSpanId =>
        Reader.asks<M, RT, Option<ActivitySpanId>>(
            rt => Optional(rt.CurrentActivity?.ParentSpanId));

    /// <summary>
    /// Read the recorded flag of the current activity
    /// </summary>
    /// <remarks>None if there is no current activity</remarks>
    public static K<M, Option<bool>> recorded =>
        Reader.asks<M, RT, Option<bool>>(
            rt => Optional(rt.CurrentActivity?.Recorded));

    /// <summary>
    /// Read the display-name of the current activity
    /// </summary>
    /// <remarks>None if there is no current activity</remarks>
    public static K<M, Option<string>> displayName =>
        Reader.asks<M, RT, Option<string>>(
            rt => Optional(rt.CurrentActivity?.DisplayName));

    /// <summary>
    /// Read the operation-name of the current activity
    /// </summary>
    /// <remarks>None if there is no current activity</remarks>
    public static K<M, Option<string>> operationName =>
        Reader.asks<M, RT, Option<string>>(
            rt => Optional(rt.CurrentActivity?.OperationName));

    /// <summary>
    /// Read the root ID of the current activity
    /// </summary>
    /// <remarks>None if there is no current activity</remarks>
    public static K<M, Option<string>> rootId =>
        Reader.asks<M, RT, Option<string>>(
            rt => Optional(rt.CurrentActivity?.RootId));

    /// <summary>
    /// Read the span ID of the current activity
    /// </summary>
    /// <remarks>None if there is no current activity</remarks>
    public static K<M, Option<ActivitySpanId>> spanId =>
        Reader.asks<M, RT, Option<ActivitySpanId>>(
            rt => Optional(rt.CurrentActivity?.SpanId));

    /// <summary>
    /// Read the start-time of the current activity
    /// </summary>
    /// <remarks>None if there is no current activity</remarks>
    public static K<M, Option<DateTime>> startTimeUTC =>
        Reader.asks<M, RT, Option<DateTime>>(
            rt => Optional(rt.CurrentActivity?.StartTimeUtc));
}
