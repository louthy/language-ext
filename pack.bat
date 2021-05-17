dotnet restore
dotnet pack LanguageExt.Core -c Release -o ../../artifacts/bin
REM dotnet pack LanguageExt.FSharp -c Release -o ../../artifacts/bin
REM dotnet pack LanguageExt.Parsec -c Release -o ../../artifacts/bin
REM dotnet pack LanguageExt.Rx -c Release -o ../../artifacts/bin
REM dotnet pack LanguageExt.CodeGen -c Release -o ../../artifacts/bin
dotnet pack LanguageExtAnalysers.Package -c Release -o ../../artifacts/bin
