# Artifacts is where the DLLs are compiled to 
Artifacts=/mnt/raid/dev/artifacts

# $LangExtRoot is where the source code root should be
LangExtRoot=/mnt/raid/dev/language-ext

sh clean.sh
sh docs.sh

cd $LangExtRoot || exit

echo building the artefacts
 
dotnet restore
dotnet pack LanguageExt.Core -c Release -o $Artifacts
dotnet pack LanguageExt.Streaming -c Release -o $Artifacts
dotnet pack LanguageExt.FSharp -c Release -o $Artifacts
dotnet pack LanguageExt.Parsec -c Release -o $Artifacts
dotnet pack LanguageExt.Rx -c Release -o $Artifacts
dotnet pack LanguageExt.Sys -c Release -o $Artifacts

sh ../push-language-ext.sh