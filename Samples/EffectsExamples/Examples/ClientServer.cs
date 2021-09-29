using LanguageExt;
using LanguageExt.Sys;
using LanguageExt.Pipes;
using LanguageExt.Common;
using LanguageExt.Sys.Traits;
using LanguageExt.Effects.Traits;
using static LanguageExt.Prelude;
using static LanguageExt.Pipes.Proxy;

namespace EffectsExamples
{
    /// <summary>
    /// Client / server effect example
    /// </summary>
    /// <remarks>Simple request/response example.  The client sends 3 values to the server and it increments them</remarks>
    public class ClientServer<RT> where RT : 
        struct, 
        HasCancel<RT>,
        HasConsole<RT>
    {
        public static Aff<RT, Unit> main =>
            mainEffect.RunEffect();
        
        /// <summary>
        /// Compose the client and server together to make an enclosed effect
        /// </summary>
        public static Effect<RT, Unit> mainEffect =>
            incrementer | oneTwoThree;

        /// <summary>
        /// Server receives a request (as the argument to the function), increments the value and responds
        /// </summary>
        static Server<RT, int, int, Unit> incrementer(int question) =>
            from _1 in Server.lift<RT, int, int, Unit>(Console<RT>.writeLine($"Server received: {question}"))
            from _2 in Server.lift<RT, int, int, Unit>(Console<RT>.writeLine($"Server responded: {question + 1}"))
            from nq in Server.respond<RT, int, int>(question + 1)
            select unit;

        /// <summary>
        /// Client enumerates through three items, sending each item to the server as a request.  The following line
        /// handles the response.
        /// </summary>
        static Client<RT, int, int, Unit> oneTwoThree =>
            from qn in Client.enumerate<RT, int, int, int>(Seq(1, 2, 3))
            from _1 in Client.lift<RT, int, int, Unit>(Console<RT>.writeLine($"Client requested: {qn}"))
            from an in Client.request<RT, int, int>(qn)
            from _2 in Client.lift<RT, int, int, Unit>(Console<RT>.writeLine($"Client received: {an}"))
            select unit;
    }
}
