using System;
using LanguageExt;
using LanguageExt.Common;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace TestBed;

[Obsolete]
public class Issue1270
{
    public record Command;
    
    EitherT<Error, IO, Command> GetData1(Command x) =>
        throw new NotImplementedException();
    
    EitherT<Error, IO, Command> GetData2(Command x) =>
        throw new NotImplementedException();
    
    EitherT<Error, IO, Command> GetData3(Command x) =>
        throw new NotImplementedException();
    
    EitherT<Error, IO, Command> GetData4(Command x) =>
        throw new NotImplementedException();
    
    EitherT<Error, IO, Unit> ProcessData1(Command x) =>
        throw new NotImplementedException();
    
    EitherT<Error, IO, Unit> ProcessData2(Command x) =>
        throw new NotImplementedException();
    
    EitherT<Error, IO, Unit> ProcessData3(Command x) =>
        throw new NotImplementedException();
    
    EitherT<Error, IO, Unit> ProcessData4(Command x) =>
        throw new NotImplementedException();

public EitherT<Error, IO, Unit> MainFunction(Command cmd) =>
    // Early out is good
   (from data1 in GetData1(cmd)
    from data2 in GetData2(cmd)
    from data3 in GetData3(cmd)
    from data4 in GetData4(cmd)
    
    // No early out anymore
    let process1 = ProcessData1(data1)
    let process2 = ProcessData2(data2)
    let process3 = ProcessData3(data3)
    let process4 = ProcessData4(data4)

    // Collect the lefts from the processes.  If there aren't any
    // then return Right, else collect the Errors together and return
    // Left
    
    from errors in Seq(process1, process2, process3, process4).Lefts()
    from result in guard(errors.IsEmpty, Fail(Error.Many(errors))) 

    select result).As();
        
}
