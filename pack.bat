C:

echo building the docs

cd C:\dev\best-form\BestForm\bin\Release\net5.0
bestform.exe "LanguageExt.Core" "C:\Dev\language-ext\LanguageExt.Core" "C:\dev\louthy.github.io\language-ext" "https://github.com/louthy/language-ext/tree/main"
bestform.exe "LanguageExt.Transformers" "C:\Dev\language-ext\LanguageExt.Transformers" "C:\dev\louthy.github.io\language-ext" "https://github.com/louthy/language-ext/tree/main"
bestform.exe "LanguageExt.Parsec" "C:\Dev\language-ext\LanguageExt.Parsec" "C:\dev\louthy.github.io\language-ext" "https://github.com/louthy/language-ext/tree/main"
bestform.exe "LanguageExt.FSharp" "C:\Dev\language-ext\LanguageExt.FSharp" "C:\dev\louthy.github.io\language-ext" "https://github.com/louthy/language-ext/tree/main"
bestform.exe "LanguageExt.Rx" "C:\Dev\language-ext\LanguageExt.Rx" "C:\dev\louthy.github.io\language-ext" "https://github.com/louthy/language-ext/tree/main"
bestform.exe "LanguageExt.Sys" "C:\Dev\language-ext\LanguageExt.Sys" "C:\dev\louthy.github.io\language-ext" "https://github.com/louthy/language-ext/tree/main"
bestform.exe "LanguageExt.SysX" "C:\Dev\language-ext\LanguageExt.SysX" "C:\dev\louthy.github.io\language-ext" "https://github.com/louthy/language-ext/tree/main"

echo committing them to git

cd C:\dev\louthy.github.io

git add .
git commit -m "Language-ext documentation update"
git push

cd C:\dev\language-ext

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

