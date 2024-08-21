echo cleaning the docs

rm -rf $LangExtDocs/language-ext-v4/LanguageExt.Core
rm -rf $LangExtDocs/language-ext-v4/LanguageExt.Parsec
rm -rf $LangExtDocs/language-ext-v4/LanguageExt.FSharp
rm -rf $LangExtDocs/language-ext-v4/LanguageExt.Rx
rm -rf $LangExtDocs/language-ext-v4/LanguageExt.Sys

echo building the docs

# $BestFormBin is where the bestform.exe is compiled to 
BestFormBin=/home/paul/Documents/dev/best-form/bestform

# $LangExtRoot is where the source code root should be (i.e. c:\dev\language-ext)
LangExtRoot=/home/paul/Documents/dev/language-ext-v4-latest

# $LangExtDocs is where the docs root should be (i.e. c:\dev\louthy.github.io)
LangExtDocs=/home/paul/Documents/dev/louthy.github.io

dotnet build $BestFormBin/bestform.csproj -c Release
dotnet run --project $BestFormBin -c Release --no-build "LanguageExt.Core" "$LangExtRoot/LanguageExt.Core" "$LangExtDocs/language-ext-v4" "https://github.com/louthy/language-ext/tree/v4-latest"
dotnet run --project $BestFormBin -c Release --no-build "LanguageExt.Parsec" "$LangExtRoot/LanguageExt.Parsec" "$LangExtDocs/language-ext-v4" "https://github.com/louthy/language-ext/tree/v4-latest"
dotnet run --project $BestFormBin -c Release --no-build "LanguageExt.FSharp" "$LangExtRoot/LanguageExt.FSharp" "$LangExtDocs/language-ext-v4" "https://github.com/louthy/language-ext/tree/v4-latest"
dotnet run --project $BestFormBin -c Release --no-build "LanguageExt.Rx" "$LangExtRoot/LanguageExt.Rx" "$LangExtDocs/language-ext-v4" "https://github.com/louthy/language-ext/tree/v4-latest"
dotnet run --project $BestFormBin -c Release --no-build "LanguageExt.Sys" "$LangExtRoot/LanguageExt.Sys" "$LangExtDocs/language-ext-v4" "https://github.com/louthy/language-ext/tree/v4-latest"
