using System;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace TestBed;

public class Issue1270
{
    public record Command;
    
    EitherAsync<Error, Command> GetData1(Command x) =>
        throw new NotImplementedException();
    
    EitherAsync<Error, Command> GetData2(Command x) =>
        throw new NotImplementedException();
    
    EitherAsync<Error, Command> GetData3(Command x) =>
        throw new NotImplementedException();
    
    EitherAsync<Error, Command> GetData4(Command x) =>
        throw new NotImplementedException();
    
    EitherAsync<Error, Unit> ProcessData1(Command x) =>
        throw new NotImplementedException();
    
    EitherAsync<Error, Unit> ProcessData2(Command x) =>
        throw new NotImplementedException();
    
    EitherAsync<Error, Unit> ProcessData3(Command x) =>
        throw new NotImplementedException();
    
    EitherAsync<Error, Unit> ProcessData4(Command x) =>
        throw new NotImplementedException();

public EitherAsync<Error, Unit> MainFunction(Command cmd) =>
    // Early out is good
    from data1 in GetData1(cmd)
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
    from result in Seq(process1, process2, process3, process4)
                       .Lefts()
                       .Map(errors => errors.IsEmpty
                           ? Right<Error, Unit>(unit)
                           : Left<Error, Unit>(Error.Many(errors)))
                       .ToAsync()

    select result;
        
}
