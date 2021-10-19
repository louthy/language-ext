I:

echo building the docs

cd I:\dev\best-form\BestForm\bin\Release\net5.0
bestform.exe "LanguageExt.Core" "I:\Dev\language-ext\LanguageExt.Core" "I:\dev\louthy.github.io\language-ext" "https://github.com/louthy/language-ext/tree/main"
bestform.exe "LanguageExt.Parsec" "I:\Dev\language-ext\LanguageExt.Parsec" "I:\dev\louthy.github.io\language-ext" "https://github.com/louthy/language-ext/tree/main"
bestform.exe "LanguageExt.FSharp" "I:\Dev\language-ext\LanguageExt.FSharp" "I:\dev\louthy.github.io\language-ext" "https://github.com/louthy/language-ext/tree/main"
bestform.exe "LanguageExt.Rx" "I:\Dev\language-ext\LanguageExt.Rx" "I:\dev\louthy.github.io\language-ext" "https://github.com/louthy/language-ext/tree/main"
bestform.exe "LanguageExt.Sys" "I:\Dev\language-ext\LanguageExt.Sys" "I:\dev\louthy.github.io\language-ext" "https://github.com/louthy/language-ext/tree/main"

echo committing them to git

cd I:\dev\louthy.github.io

git add .
git commit -m "Language-ext documentation update"
git push

cd I:\dev\language-ext

echo building the artefacts

dotnet restore
dotnet pack LanguageExt.Core -c Release -o ../../artifacts/bin
dotnet pack LanguageExt.FSharp -c Release -o ../../artifacts/bin
dotnet pack LanguageExt.Parsec -c Release -o ../../artifacts/bin
dotnet pack LanguageExt.Rx -c Release -o ../../artifacts/bin
dotnet pack LanguageExt.Sys -c Release -o ../../artifacts/bin
dotnet pack LanguageExt.CodeGen -c Release -o ../../artifacts/bin

