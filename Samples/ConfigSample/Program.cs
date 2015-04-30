using static System.Console;
using static System.Configuration.ConfigurationManager;
using LanguageExt;
using static LanguageExt.Prelude;

namespace ConfigSample
{
    class Program
    {
        static void Main(string[] args)
        {
            match(Config.GetValue("sample1"),
                Some: v => WriteLine(v),
                None: () => WriteLine("sample1: not found")
            );

            var res = match(Config.GetValue("sample2"),
                        Some: v => v,
                        None: () => "sample2: not found"
                      );

            WriteLine(res);

            match(Config.GetValue("sampleX"),
                Some: v => WriteLine(v),
                None: () => WriteLine("sampleX: not found")
            );

            match(Config.GetIntegerValue("sample3"),
                Right: v => WriteLine("Int of " + v),
                Left: t => WriteLine(t)
                );

            match(Config.GetIntegerValue("sample4"),
                Right: v => WriteLine("Int of " + v),
                Left: t => WriteLine(t)
                );

            match(Config.GetIntegerValue("sampleX"),
                Right: v => WriteLine("Int of " + v),
                Left: t => WriteLine(t)
                );

            /// WILL CRASH
            match(Config.GetIntegerValue(null),
                Right: v => WriteLine("Int of " + v),
                Left: t => WriteLine(t)
                );

        }
    }

    static class Config
    {
        /// <summary>
        /// Reads the configuration setting fro App.config
        /// If the setting is 'null' then the result will be None otherwise it
        /// will be Some(the config value).  
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static Option<string> GetValue(Some<string> key) => AppSettings[key];

        /// <summary>
        /// Attempts to get the config value, if it exists it attempts to convert
        /// it to an int and return it as Right(the int) otherwise it returns
        /// Left(the string).
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static Either<string, int> GetIntegerValue(Some<string> key) =>
            match
            (
                GetValue(key),
                Some: str =>
                    match
                    (
                        parseInt(str),
                        Some: result => Right<string, int>(result),
                        None: () => Left<string, int>("Not a valid int: " + str)
                    ),
                None: () => Left<string, int>("Key doesn't exist: " + key)
            );
    }
}
