using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LanguageExt;
using LanguageExt.UnitsOfMeasure;
using static LanguageExt.Prelude;
using static LanguageExt.Process;
using static LanguageExt.Strategy;

namespace Strategies
{
    class Program
    {
        static void Main(string[] args)
        {
            Test1();
        }

        static void Test1()
        {
            Func<ProcessId> setup = () =>
                spawn<string>(
                    "test1",
                    msg =>
                    {
                        Console.WriteLine(msg);
                        failwith<Unit>("fail");
                    });

            var supervisor = spawn<ProcessId, Unit>(
                "test1-supervisor",
                setup,
                (pid, _)  => {
                    tell(pid, "test1");
                    return pid;
                },
                Strategy: OneForOne(
                    Retries(5),
                    Always(Directive.Restart),
                    Redirect(
                        When<Restart>(MessageDirective.ForwardToSelf))));

            tell(supervisor, unit);

            Console.WriteLine("Test 1: Press enter when messages stop");
            Console.ReadKey();
        }
    }
}
