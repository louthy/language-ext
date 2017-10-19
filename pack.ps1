$version=gitversion /output json /showvariable FullSemVer
write-host "VERSION is $version"
dotnet restore
dotnet pack LanguageExt.Core -c Release /p:Version=$version /p:PackageVersion=$version -o ../../artifacts/bin
dotnet pack LanguageExt.FSharp -c Release /p:Version=$version /p:PackageVersion=$version -o ../../artifacts/bin
dotnet pack LanguageExt.Parsec -c Release /p:Version=$version /p:PackageVersion=$version -o ../../artifacts/bin
