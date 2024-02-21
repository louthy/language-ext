using System;
using System.Diagnostics;
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
public class Activity<M, RT>
    where M :
        State<M, RT>,
        Resource<M>,
        Monad<M>

    where RT :
        Has<M, ActivitySourceIO>,
        Mutates<M, RT, ActivityEnv>
{
    static readonly K<M, ActivitySourceIO> trait =
        State.getsM<M, RT, ActivitySourceIO>(e => e.Trait);

    static K<M, Unit> mutate(Func<ActivityEnv, ActivityEnv> f) =>
        State.getsM<M, RT, Unit>(e => e.Modify(f));

    static K<M, ActivityEnv> env =>
        State.getsM<M, RT, ActivityEnv>(e => e.Get);

    static K<M, Activity?> currentActivity =>
        env.Map(e => e.Activity);

    static K<M, Activity> startActivity(
        string name,
        ActivityKind activityKind,
        HashMap<string, object> activityTags,
        Seq<ActivityLink> activityLinks,
        DateTimeOffset startTime,
        ActivityContext? parentContext = default) =>
        from src in trait
        from cur in currentActivity
        from act in Resource.use<M, Activity>(
            src.StartActivity(
                name,
                activityKind,
                cur is null
                    ? default
                    : parentContext ?? cur.Context,
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
        from r in State.bracket<M, RT, TA>(mutate(e => e with { Activity = a }), operation)
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
        from r in State.bracket<M, RT, A>(mutate(e => e with { Activity = a }), operation)
        select r;

    /// <summary>
    /// Set the state trace string
    /// </summary>
    /// <param name="traceStateString">Trace state string</param>
    /// <returns>Unit effect</returns>
    public static K<M, Unit> setTraceState(string traceStateString) =>
        currentActivity.Map(
            a =>
            {
                if (a is not null) a.TraceStateString = traceStateString;
                return unit;
            });

    /// <summary>
    /// Read the trace-state string of the current activity
    /// </summary>
    public static K<M, Option<string>> traceState =>
        currentActivity.Map(a => Optional(a?.TraceStateString));

    /// <summary>
    /// Read the trace ID of the current activity
    /// </summary>
    public static K<M, Option<ActivityTraceId>> traceId =>
        currentActivity.Map(a => Optional(a?.TraceId));

    /// <summary>
    /// Add baggage to the current activity
    /// </summary>
    /// <param name="key">Baggage key</param>
    /// <param name="value">Baggage value</param>
    /// <returns>Unit effect</returns>
    public static K<M, Unit> addBaggage(string key, string? value) =>
        currentActivity.Map(
            a =>
            {
                a?.AddBaggage(key, value);
                return unit;
            });

    /// <summary>
    /// Read the baggage of the current activity
    /// </summary>
    public static K<M, HashMap<string, string?>> baggage =>
        currentActivity.Map(
            a => a is not null
                     ? a.Baggage.ToHashMap()
                     : HashMap<string, string?>());

    /// <summary>
    /// Add tag to the current activity
    /// </summary>
    /// <param name="name">Tag name</param>
    /// <param name="value">Tag value</param>
    /// <returns>Unit effect</returns>
    public static K<M, Unit> addTag(string name, string? value) =>
        currentActivity.Map(
            a =>
            {
                a?.AddTag(name, value);
                return unit;
            });

    /// <summary>
    /// Add tag to the current activity
    /// </summary>
    /// <param name="name">Tag name</param>
    /// <param name="value">Tag value</param>
    public static K<M, Unit> addTag(string name, object? value) =>
        currentActivity.Map(
            a =>
            {
                a?.AddTag(name, value);
                return unit;
            });

    /// <summary>
    /// Read the tags of the current activity
    /// </summary>
    public static K<M, HashMap<string, string?>> tags =>
        currentActivity.Map(
            a => a is not null
                     ? a.Tags.ToHashMap()
                     : HashMap<string, string?>());

    /// <summary>
    /// Read the tags of the current activity
    /// </summary>
    public static K<M, HashMap<string, object?>> tagObjects =>
        currentActivity.Map(
            a => a is not null
                     ? a.TagObjects.ToHashMap()
                     : HashMap<string, object?>());

    /// <summary>
    /// Read the context of the current activity
    /// </summary>
    /// <remarks>None if there is no current activity</remarks>
    public static K<M, Option<ActivityContext>> context =>
        currentActivity.Map(a => Optional(a?.Context));

    /// <summary>
    /// Read the duration of the current activity
    /// </summary>
    /// <remarks>None if there is no current activity</remarks>
    public static K<M, Option<TimeSpan>> duration =>
        currentActivity.Map(a => Optional(a?.Duration));

    /// <summary>
    /// Add an event to the current activity
    /// </summary>
    /// <param name="event">Event</param>
    public static K<M, Unit> addEvent(ActivityEvent @event) =>
        currentActivity.Map(
            a =>
            {
                a?.AddEvent(@event);
                return unit;
            });

    /// <summary>
    /// Read the events of the current activity
    /// </summary>
    public static K<M, Seq<ActivityEvent>> events =>
        currentActivity.Map(
            a => a is not null
                     ? a.Events.ToSeq()
                     : Seq<ActivityEvent>());

    /// <summary>
    /// Read the ID of the current activity
    /// </summary>
    /// <remarks>None if there is no current activity</remarks>
    public static K<M, Option<string>> id =>
        currentActivity.Map(a => Optional(a?.Id));

    /// <summary>
    /// Read the kind of the current activity
    /// </summary>
    /// <remarks>None if there is no current activity</remarks>
    public static K<M, Option<ActivityKind>> kind =>
        currentActivity.Map(a => Optional(a?.Kind));

    /// <summary>
    /// Read the links of the current activity
    /// </summary>
    public static K<M, Seq<ActivityLink>> links =>
        currentActivity.Map(
            a => a is not null
                     ? a.Links.ToSeq()
                     : Seq<ActivityLink>());

    /// <summary>
    /// Read the current activity
    /// </summary>
    /// <remarks>None if there is no current activity</remarks>
    public static K<M, Option<Activity>> current =>
        currentActivity.Map(a => Optional(a));

    /// <summary>
    /// Read the parent ID of the current activity
    /// </summary>
    /// <remarks>None if there is no current activity</remarks>
    public static K<M, Option<string>> parentId =>
        currentActivity.Map(a => Optional(a?.ParentId));

    /// <summary>
    /// Read the parent span ID of the current activity
    /// </summary>
    /// <remarks>None if there is no current activity</remarks>
    public static K<M, Option<ActivitySpanId>> parentSpanId =>
        currentActivity.Map(a => Optional(a?.ParentSpanId));

    /// <summary>
    /// Read the recorded flag of the current activity
    /// </summary>
    /// <remarks>None if there is no current activity</remarks>
    public static K<M, Option<bool>> recorded =>
        currentActivity.Map(a => Optional(a?.Recorded));

    /// <summary>
    /// Read the display-name of the current activity
    /// </summary>
    /// <remarks>None if there is no current activity</remarks>
    public static K<M, Option<string>> displayName =>
        currentActivity.Map(a => Optional(a?.DisplayName));

    /// <summary>
    /// Read the operation-name of the current activity
    /// </summary>
    /// <remarks>None if there is no current activity</remarks>
    public static K<M, Option<string>> operationName =>
        currentActivity.Map(a => Optional(a?.OperationName));

    /// <summary>
    /// Read the root ID of the current activity
    /// </summary>
    /// <remarks>None if there is no current activity</remarks>
    public static K<M, Option<string>> rootId =>
        currentActivity.Map(a => Optional(a?.RootId));

    /// <summary>
    /// Read the span ID of the current activity
    /// </summary>
    /// <remarks>None if there is no current activity</remarks>
    public static K<M, Option<ActivitySpanId>> spanId =>
        currentActivity.Map(a => Optional(a?.SpanId));

    /// <summary>
    /// Read the start-time of the current activity
    /// </summary>
    /// <remarks>None if there is no current activity</remarks>
    public static K<M, Option<DateTime>> startTimeUTC =>
        currentActivity.Map(a => Optional(a?.StartTimeUtc));
}
