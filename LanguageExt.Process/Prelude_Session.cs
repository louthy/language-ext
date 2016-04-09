using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using static LanguageExt.Prelude;
using LanguageExt.UnitsOfMeasure;
using System.Security.Cryptography;

namespace LanguageExt
{
    /// <summary>
    /// 
    ///     Process: Session functions
    /// 
    ///     These functions facilitate the use of sessions that live from
    ///     Process to Process.  Essentially if there's an active Session
    ///     ID then it will be packaged with each message that is sent via
    ///     tell or ask.  
    /// 
    /// </summary>
    public static partial class Process
    {
        /// <summary>
        /// Starts a new session in the Process system with the specified
        /// session ID
        /// </summary>
        /// <param name="sid">Session ID</param>
        /// <param name="timeout">Session timeout</param>
        public static string sessionStart(string sid, Time timeout, SystemName system = default(SystemName))
        {
            ActorContext.System(system).Sessions.Start(sid, (int)(timeout/1.Seconds()));
            ActorContext.SessionId = sid;
            return sid;
        }

        /// <summary>
        /// Starts a new session in the Process system
        /// NOTE: This function is asynchronous
        /// </summary>
        /// <param name="timeout">Session timeout</param>
        /// <returns>Session ID of the newly created session</returns>
        public static string sessionStart(Time timeout) =>
            sessionStart(makeSessionId(), timeout);

        /// <summary>
        /// Ends a session in the Process system with the specified
        /// session ID
        /// NOTE: This function is asynchronous
        /// </summary>
        /// <param name="sid">Session ID</param>
        public static Unit sessionStop(string sid, SystemName system = default(SystemName)) =>
            ActorContext.System(system).Sessions.Stop(sid);

        /// <summary>
        /// Touch a session
        /// Time-stamps the session so that its time-to-expiry is reset
        /// </summary>
        /// <param name="sid">Session ID</param>
        public static Unit sessionTouch(string sid, SystemName system = default(SystemName)) =>
            ActorContext.System(system).Sessions.Touch(sid);

        /// <summary>
        /// Gets the current session ID
        /// </summary>
        /// <remarks>Also touches the session so that its time-to-expiry 
        /// is reset</remarks>
        /// <returns>Optional session ID</returns>
        public static Option<string> sessionId(SystemName system = default(SystemName)) 
        {
            var sid = ActorContext.SessionId;
            sid.IfSome(x => sessionTouch(x, system));
            return sid;
        }

        /// <summary>
        /// Set the meta-data to store with the session, this is typically
        /// user credentials when they've logged in.  But can be anything.  It is a 
        /// key/value store that is sync'd around the cluster.
        /// NOTE: This function is asynchronous
        /// </summary>
        /// <param name="sid">Session ID</param>
        /// <param name="key">Key</param>
        /// <param name="value">Data value </param>
        public static Unit sessionSetData(string sid, string key, object value, SystemName system = default(SystemName))
        {
            var time = (from session in ActorContext.System(system).Sessions.GetSession(sid)
                        from data in session.Data.Find(key)
                        select data.Time)
                       .IfNone(0L);

            if(time == 0L)
            {
                throw new Exception("Session not started");
            }

            return ActorContext.System(system).Sessions.SetData(time + 1, sid, key, value);
        }

        /// <summary>
        /// Clear the meta-data key stored with the session
        /// NOTE: This function is asynchronous
        /// </summary>
        /// <param name="sid">Session ID</param>
        /// <param name="key">Key</param>
        public static Unit sessionClearData(string sid, string key, SystemName system = default(SystemName))
        {
            var time = (from session in ActorContext.System(system).Sessions.GetSession(sid)
                        from data in session.Data.Find(key)
                        select data.Time)
                       .IfNone(0L);

            if (time == 0L)
            {
                return unit;
            }

            return ActorContext.System(system).Sessions.ClearData(time, sid, key);
        }

        /// <summary>
        /// Get the meta-data stored with the session.  
        /// <para>
        /// The session system allows concurrent updates from across the
        /// cluster or from within the app-domain (from multiple processes).
        /// To maintain the integrity of the data in any one session, the system 
        /// uses a Vector Clock per-key.
        /// </para>
        /// <para>
        /// That means that if two Processes update the session from the
        /// same 'time' start point, then there will be a conflict and the session 
        /// will  contain both values stored against the key.  
        /// </para>
        /// <para>
        /// It is up to you 
        /// to decide on the best approach to resolving the conflict.  Calling Head() / HeadOrNone() 
        /// on the result will get the value that was written first, calling Last() 
        /// will get the value that was written last.
        /// However, being first or last doesn't necessarily make a value 'right', in
        /// an asynchronous system the last value could be the newest or oldest.
        /// Both value commits had the same start point, so if the consistency
        /// of the session data is important to you then you should implement
        /// a more robust strategy to deal with value conflicts, if integrity doesn't
        /// really matter, call HeadOrNone().
        /// </para>
        /// </summary>
        /// <param name="sid">Session ID</param>
        public static Lst<T> sessionGetData<T>(string sid, string key, SystemName system = default(SystemName)) =>
            (from session in ActorContext.System(system).Sessions.GetSession(sid)
             from vector in session.Data.Find(key)
             select vector.Vector.Map(obj =>
                obj is T
                    ? (T)obj
                    : default(T)))
            .IfNone(List.empty<T>())
            .Filter(notnull);

        /// <summary>
        /// Returns True if there is an active session
        /// </summary>
        /// <returns></returns>
        public static bool hasSession() =>
            ActorContext.SessionId.IsSome;

        /// <summary>
        /// Acquires a session for the duration of invocation of the 
        /// provided function
        /// </summary>
        /// <param name="sid">Session ID</param>
        /// <param name="f">Function to invoke</param>
        /// <returns>Result of the function</returns>
        public static R withSession<R>(string sid, Func<R> f, SystemName system = default(SystemName)) =>
            InMessageLoop
                ? ActorContext.System(system).WithContext<R>(
                    ActorContext.Request.Self,
                    ActorContext.Request.Self.Actor.Parent,
                    Process.Sender,
                    ActorContext.Request.CurrentRequest,
                    ActorContext.Request.CurrentMsg,
                    Some(sid),
                    f)
                : raiseUseInMsgLoopOnlyException<R>(nameof(withSession));

        /// <summary>
        /// Acquires a session for the duration of invocation of the 
        /// provided action
        /// </summary>
        /// <param name="sid">Session ID</param>
        /// <param name="f">Action to invoke</param>
        public static Unit withSession(string sid, Action f) =>
            withSession(sid, fun(f));

        const int DefaultSessionIdSizeInBytes = 32;

        /// <summary>
        /// Make a cryptographically strong session ID
        /// </summary>
        /// <param name="sizeInBytes">Size in bytes.  This is not the final string length, the final length depends
        /// on the Base64 encoding of a byte-array sizeInBytes long.  As a guide a 64 byte session ID turns into
        /// an 88 character string.</returns>
        static string makeSessionId(int sizeInBytes = DefaultSessionIdSizeInBytes) =>
            randomBase64(sizeInBytes);
    }
}
