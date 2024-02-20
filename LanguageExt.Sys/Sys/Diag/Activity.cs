using System;
using System.Diagnostics;
using LanguageExt.Effects.Traits;
using LanguageExt.Sys.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt.Sys.Diag;

/// <summary>
/// An `Activity` has an operation name, an ID, a start time and duration, tags, and baggage.
/// 
/// Activities should be created by calling the `span` functions, configured as necessary.  Each `span` function
/// takes an `Eff` or `Aff` operation to run (which is the activity).  The runtime system will maintain the parent-
/// child relationships for the activities, and maintains the 'current' activity.
/// </summary>
/// <typeparam name="RT">runtime</typeparam>
public static class Activity<RT>
    where RT : HasActivitySource<RT>, HasIO<RT>
{
    static Eff<RT, Activity> startActivity(
        string name,
        ActivityKind activityKind,
        HashMap<string, object> activityTags,
        Seq<ActivityLink> activityLinks,
        DateTimeOffset startTime,
        ActivityContext? parentContext = default) =>
        from rt in runtime<RT>()
        from src in rt.ActivitySourceEff
        from act in Eff.use(
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
    public static Eff<RT, A> span<A>(string name, Eff<RT, A> operation) =>
        span(name, ActivityKind.Internal, default, default, DateTimeOffset.Now, operation);

    /// <summary>
    /// Creates a new activity if there are active listeners for it, using the specified name, activity kind, parent
    /// activity context, tags, optional activity link and optional start time.
    /// </summary>
    /// <param name="name">The operation name of the activity.</param>
    /// <param name="activityKind">The activity kind.</param>
    /// <param name="operation">The operation to whose activity will be traced</param>
    /// <returns>The result of the `operation`</returns>
    public static Eff<RT, A> span<A>(
        string name,
        ActivityKind activityKind,
        Eff<RT, A> operation) => 
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
    public static Eff<RT, A> span<A>(
        string name,
        ActivityKind activityKind,
        HashMap<string, object> activityTags,
        Eff<RT, A> operation) => 
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
    public static Eff<RT, TA> span<TA>(
        string name,
        ActivityKind activityKind,
        HashMap<string, object> activityTags,
        Seq<ActivityLink> activityLinks,
        DateTimeOffset startTime,
        Eff<RT, TA> operation) =>
        from a in startActivity(name, activityKind, activityTags, activityLinks, startTime)
        from r in localEff<RT, RT, TA>(rt => rt.WithActivity(a), operation)
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
    public static Eff<RT, A> span<A>(
        string name,
        ActivityKind activityKind,
        ActivityContext parentContext,
        HashMap<string, object> activityTags,
        Seq<ActivityLink> activityLinks,
        DateTimeOffset startTime,
        Eff<RT, A> operation) =>
        from a in startActivity(
            name,
            activityKind,
            activityTags,
            activityLinks,
            startTime,
            parentContext)
        from r in localEff<RT, RT, A>(rt => rt.WithActivity(a), operation)
        select r;

    /// <summary>
    /// Set the state trace string
    /// </summary>
    /// <param name="traceStateString">Trace state string</param>
    /// <returns>Unit effect</returns>
    public static Eff<RT, Unit> setTraceState(string traceStateString) =>
        lift((RT rt) =>
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
    public static Eff<RT, Option<string>> traceState =>
        lift((RT rt) => Optional(rt.CurrentActivity?.TraceStateString));

    /// <summary>
    /// Read the trace ID of the current activity
    /// </summary>
    public static Eff<RT, Option<ActivityTraceId>> traceId =>
        lift((RT rt) => Optional(rt.CurrentActivity?.TraceId));

    /// <summary>
    /// Add baggage to the current activity
    /// </summary>
    /// <param name="key">Baggage key</param>
    /// <param name="value">Baggage value</param>
    /// <returns>Unit effect</returns>
    public static Eff<RT, Unit> addBaggage(string key, string? value) =>
        lift((RT rt) =>
             {
                 rt.CurrentActivity?.AddBaggage(key, value);
                 return unit;
             });

    /// <summary>
    /// Read the baggage of the current activity
    /// </summary>
    public static Eff<RT, HashMap<string, string?>> baggage =>
        lift((RT rt) =>
                 rt.CurrentActivity is not null
                     ? rt.CurrentActivity.Baggage.ToHashMap()
                     : HashMap<string, string?>());

    /// <summary>
    /// Add tag to the current activity
    /// </summary>
    /// <param name="name">Tag name</param>
    /// <param name="value">Tag value</param>
    /// <returns>Unit effect</returns>
    public static Eff<RT, Unit> addTag(string name, string? value) =>
        lift((RT rt) =>
             {
                 rt.CurrentActivity?.AddTag(name, value);
                 return unit;
             });

    /// <summary>
    /// Add tag to the current activity
    /// </summary>
    /// <param name="name">Tag name</param>
    /// <param name="value">Tag value</param>
    public static Eff<RT, Unit> addTag(string name, object? value) =>
        lift((RT rt) =>
             {
                 rt.CurrentActivity?.AddTag(name, value);
                 return unit;
             });

    /// <summary>
    /// Read the tags of the current activity
    /// </summary>
    public static Eff<RT, HashMap<string, string?>> tags =>
        lift((RT rt) =>
                 rt.CurrentActivity is not null
                     ? rt.CurrentActivity.Tags.ToHashMap()
                     : HashMap<string, string?>());

    /// <summary>
    /// Read the tags of the current activity
    /// </summary>
    public static Eff<RT, HashMap<string, object?>> tagObjects =>
        lift((RT rt) =>
                 rt.CurrentActivity is not null
                     ? rt.CurrentActivity.TagObjects.ToHashMap()
                     : HashMap<string, object?>());

    /// <summary>
    /// Read the context of the current activity
    /// </summary>
    /// <remarks>None if there is no current activity</remarks>
    public static Eff<RT, Option<ActivityContext>> context =>
        lift((RT rt) => Optional(rt.CurrentActivity?.Context));

    /// <summary>
    /// Read the duration of the current activity
    /// </summary>
    /// <remarks>None if there is no current activity</remarks>
    public static Eff<RT, Option<TimeSpan>> duration =>
        lift((RT rt) => Optional(rt.CurrentActivity?.Duration));

    /// <summary>
    /// Add an event to the current activity
    /// </summary>
    /// <param name="event">Event</param>
    public static Eff<RT, Unit> addEvent(ActivityEvent @event) =>
        lift((RT rt) =>
             {
                 rt.CurrentActivity?.AddEvent(@event);
                 return unit;
             });

    /// <summary>
    /// Read the events of the current activity
    /// </summary>
    public static Eff<RT, Seq<ActivityEvent>> events =>
        lift((RT rt) =>
                rt.CurrentActivity is not null
                     ? rt.CurrentActivity.Events.ToSeq()
                     : Seq<ActivityEvent>());

    /// <summary>
    /// Read the ID of the current activity
    /// </summary>
    /// <remarks>None if there is no current activity</remarks>
    public static Eff<RT, Option<string>> id =>
        lift((RT rt) => Optional(rt.CurrentActivity?.Id));

    /// <summary>
    /// Read the kind of the current activity
    /// </summary>
    /// <remarks>None if there is no current activity</remarks>
    public static Eff<RT, Option<ActivityKind>> kind =>
        lift((RT rt) => Optional(rt.CurrentActivity?.Kind));

    /// <summary>
    /// Read the links of the current activity
    /// </summary>
    public static Eff<RT, Seq<ActivityLink>> links =>
        lift((RT rt) =>
                rt.CurrentActivity is not null
                     ? rt.CurrentActivity.Links.ToSeq()
                     : Seq<ActivityLink>());

    /// <summary>
    /// Read the current activity
    /// </summary>
    /// <remarks>None if there is no current activity</remarks>
    public static Eff<RT, Option<Activity>> current =>
        lift((RT rt) => Optional(rt.CurrentActivity));

    /// <summary>
    /// Read the parent ID of the current activity
    /// </summary>
    /// <remarks>None if there is no current activity</remarks>
    public static Eff<RT, Option<string>> parentId =>
        lift((RT rt) => Optional(rt.CurrentActivity?.ParentId));

    /// <summary>
    /// Read the parent span ID of the current activity
    /// </summary>
    /// <remarks>None if there is no current activity</remarks>
    public static Eff<RT, Option<ActivitySpanId>> parentSpanId =>
        lift((RT rt) => Optional(rt.CurrentActivity?.ParentSpanId));

    /// <summary>
    /// Read the recorded flag of the current activity
    /// </summary>
    /// <remarks>None if there is no current activity</remarks>
    public static Eff<RT, Option<bool>> recorded =>
        lift((RT rt) => Optional(rt.CurrentActivity?.Recorded));

    /// <summary>
    /// Read the display-name of the current activity
    /// </summary>
    /// <remarks>None if there is no current activity</remarks>
    public static Eff<RT, Option<string>> displayName =>
        lift((RT rt) => Optional(rt.CurrentActivity?.DisplayName));

    /// <summary>
    /// Read the operation-name of the current activity
    /// </summary>
    /// <remarks>None if there is no current activity</remarks>
    public static Eff<RT, Option<string>> operationName =>
        lift((RT rt) => Optional(rt.CurrentActivity?.OperationName));

    /// <summary>
    /// Read the root ID of the current activity
    /// </summary>
    /// <remarks>None if there is no current activity</remarks>
    public static Eff<RT, Option<string>> rootId =>
        lift((RT rt) => Optional(rt.CurrentActivity?.RootId));

    /// <summary>
    /// Read the span ID of the current activity
    /// </summary>
    /// <remarks>None if there is no current activity</remarks>
    public static Eff<RT, Option<ActivitySpanId>> spanId =>
        lift((RT rt) => Optional(rt.CurrentActivity?.SpanId));

    /// <summary>
    /// Read the start-time of the current activity
    /// </summary>
    /// <remarks>None if there is no current activity</remarks>
    public static Eff<RT, Option<DateTime>> startTimeUTC =>
        lift((RT rt) => Optional(rt.CurrentActivity?.StartTimeUtc));
}
