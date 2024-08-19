echo building the docs

set root=

:: %LangExtRoot% is where the source code root should be (i.e. c:\dev\language-ext)
:: %LangExtDocs% is where the docs root should be (i.e. c:\dev\louthy.github.io)

Q:
cd Q:\Dev\best-form\bestform\bin\Release\net8.0
bestform.exe "LanguageExt.Core" "%LangExtRoot%\LanguageExt.Core" "%LangExtDocs%\language-ext" "https://github.com/louthy/language-ext/tree/main"
bestform.exe "LanguageExt.Parsec" "%LangExtRoot%\LanguageExt.Parsec" "%LangExtDocs%\language-ext" "https://github.com/louthy/language-ext/tree/main"
bestform.exe "LanguageExt.FSharp" "%LangExtRoot%\LanguageExt.FSharp" "%LangExtDocs%\language-ext" "https://github.com/louthy/language-ext/tree/main"
bestform.exe "LanguageExt.Rx" "%LangExtRoot%\LanguageExt.Rx" "%LangExtDocs%\language-ext" "https://github.com/louthy/language-ext/tree/main"
bestform.exe "LanguageExt.Sys" "%LangExtRoot%\LanguageExt.Sys" "%LangExtDocs%\language-ext" "https://github.com/louthy/language-ext/tree/main"

echo committing them to git

cd %LangExtDocs%

git add .
git commit -m "Language-ext documentation update"
git push

cd %LangExtRoot%

echo building the artefacts

dotnet restore
dotnet pack LanguageExt.Core -c Release -o ../../artifacts/bin
dotnet pack LanguageExt.FSharp -c Release -o ../../artifacts/bin
dotnet pack LanguageExt.Parsec -c Release -o ../../artifacts/bin
dotnet pack LanguageExt.Rx -c Release -o ../../artifacts/bin
dotnet pack LanguageExt.Sys -c Release -o ../../artifacts/bin

