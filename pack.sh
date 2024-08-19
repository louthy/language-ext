echo building the docs

# $BestFormBin is where the bestform.exe is compiled to 
BestFormBin=/home/paul/Documents/dev/best-form/bestform

# $LangExtRoot is where the source code root should be (i.e. c:\dev\language-ext)
LangExtRoot=/home/paul/Documents/dev/language-ext

# $LangExtDocs is where the docs root should be (i.e. c:\dev\louthy.github.io)
LangExtDocs=/home/paul/Documents/dev/louthy.github.io

dotnet build $BestFormBin/bestform.csproj -c Release
dotnet run --project $BestFormBin -c Release --no-build "LanguageExt.Core" "$LangExtRoot\LanguageExt.Core" "$LangExtDocs\language-ext" "https://github.com/louthy/language-ext/tree/main"
dotnet run --project $BestFormBin -c Release --no-build "LanguageExt.Parsec" "$LangExtRoot\LanguageExt.Parsec" "$LangExtDocs\language-ext" "https://github.com/louthy/language-ext/tree/main"
dotnet run --project $BestFormBin -c Release --no-build "LanguageExt.FSharp" "$LangExtRoot\LanguageExt.FSharp" "$LangExtDocs\language-ext" "https://github.com/louthy/language-ext/tree/main"
dotnet run --project $BestFormBin -c Release --no-build "LanguageExt.Rx" "$LangExtRoot\LanguageExt.Rx" "$LangExtDocs\language-ext" "https://github.com/louthy/language-ext/tree/main"
dotnet run --project $BestFormBin -c Release --no-build "LanguageExt.Sys" "$LangExtRoot\LanguageExt.Sys" "$LangExtDocs\language-ext" "https://github.com/louthy/language-ext/tree/main"

echo committing them to git

cd $LangExtDocs

git add .
git commit -m "Language-ext documentation update"
git push

cd $LangExtRoot

echo building the artefacts
 
dotnet restore
dotnet pack LanguageExt.Core -c Release -o ../artifacts/bin
dotnet pack LanguageExt.FSharp -c Release -o ../artifacts/bin
dotnet pack LanguageExt.Parsec -c Release -o ../artifacts/bin
dotnet pack LanguageExt.Rx -c Release -o ../artifacts/bin
dotnet pack LanguageExt.Sys -c Release -o ../artifacts/bin
