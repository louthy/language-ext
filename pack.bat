Q:

echo building the docs

cd Q:\dev\best-form\BestForm\bin\Release\net5.0
bestform.exe "LanguageExt.Core" "Q:\Dev\language-ext\LanguageExt.Core" "Q:\dev\louthy.github.io\language-ext" "https://github.com/louthy/language-ext/tree/main"
bestform.exe "LanguageExt.Transformers" "Q:\Dev\language-ext\LanguageExt.Transformers" "Q:\dev\louthy.github.io\language-ext" "https://github.com/louthy/language-ext/tree/main"
bestform.exe "LanguageExt.Parsec" "Q:\Dev\language-ext\LanguageExt.Parsec" "Q:\dev\louthy.github.io\language-ext" "https://github.com/louthy/language-ext/tree/main"
bestform.exe "LanguageExt.FSharp" "Q:\Dev\language-ext\LanguageExt.FSharp" "Q:\dev\louthy.github.io\language-ext" "https://github.com/louthy/language-ext/tree/main"
bestform.exe "LanguageExt.Rx" "Q:\Dev\language-ext\LanguageExt.Rx" "Q:\dev\louthy.github.io\language-ext" "https://github.com/louthy/language-ext/tree/main"
bestform.exe "LanguageExt.Sys" "Q:\Dev\language-ext\LanguageExt.Sys" "Q:\dev\louthy.github.io\language-ext" "https://github.com/louthy/language-ext/tree/main"
bestform.exe "LanguageExt.SysX" "Q:\Dev\language-ext\LanguageExt.SysX" "Q:\dev\louthy.github.io\language-ext" "https://github.com/louthy/language-ext/tree/main"

echo committing them to git

cd Q:\dev\louthy.github.io

git add .
git commit -m "Language-ext documentation update"
git push

cd Q:\dev\language-ext

echo building the artefacts

dotnet restore
dotnet pack LanguageExt.Core -c Release -o ../../artifacts/bin
dotnet pack LanguageExt.Transformers -c Release -o ../../artifacts/bin
dotnet pack LanguageExt.FSharp -c Release -o ../../artifacts/bin
dotnet pack LanguageExt.Parsec -c Release -o ../../artifacts/bin
dotnet pack LanguageExt.Rx -c Release -o ../../artifacts/bin
dotnet pack LanguageExt.Sys -c Release -o ../../artifacts/bin
dotnet pack LanguageExt.SysX -c Release -o ../../artifacts/bin
dotnet pack LanguageExt.CodeGen -c Release -o ../../artifacts/bin

