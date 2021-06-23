using System;

namespace LanguageExt.Sys.Test
{
    internal enum TestTimeSpecType
    {
        Fixed,
        Running
    }

    /// <summary>
    /// Unit tests often need to run at a specified time, or sometimes need time to stand still
    /// This is a simple specification builder for Sys.Test.TimeIO that allows you to pass that
    /// requirement through the runtime.
    /// </summary>
    public class TestTimeSpec
    {
        internal readonly TestTimeSpecType Type;
        internal readonly DateTime Start;
        internal readonly DateTime Specified;

        TestTimeSpec(TestTimeSpecType type, DateTime start, DateTime specified) =>
            (Type, Start, Specified) = (type, start.ToUniversalTime(), specified.ToUniversalTime());

        /// <summary>
        /// Time never passes, it has a constant value of DateTime.Now (as it was set when you call this method)
        /// </summary>
        public static TestTimeSpec FixedFromNow() =>
            FixedFromSpecified(DateTime.UtcNow);

        /// <summary>
        /// Time never passes, it has a constant value of `state`
        /// </summary>
        public static TestTimeSpec FixedFromSpecified(DateTime state) =>
            new TestTimeSpec(TestTimeSpecType.Fixed, DateTime.UtcNow, state);

        /// <summary>
        /// Time passes normally, this is a straight pass-through to DateTime.Now  
        /// </summary>
        public static TestTimeSpec RunningFromNow() =>
            RunningFromSpecified(DateTime.UtcNow);

        /// <summary>
        /// Time passes normally, but it is set with an initial state and flows from there  
        /// </summary>
        public static TestTimeSpec RunningFromSpecified(DateTime state) =>
            new TestTimeSpec(TestTimeSpecType.Running, DateTime.UtcNow, state);
    }
}
