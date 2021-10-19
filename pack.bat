I:

echo building the docs
cd I:\dev\best-form\lang-ext.cmd

echo committing them to git

cd I:\dev\best-form\louthy.github.io

git add .
git commit -m "Language-ext documentation update"

cd I:\dev\best-form\language-ext

echo building the artefacts

dotnet restore
dotnet pack LanguageExt.Core -c Release -o ../../artifacts/bin
dotnet pack LanguageExt.FSharp -c Release -o ../../artifacts/bin
dotnet pack LanguageExt.Parsec -c Release -o ../../artifacts/bin
dotnet pack LanguageExt.Rx -c Release -o ../../artifacts/bin
dotnet pack LanguageExt.Sys -c Release -o ../../artifacts/bin
dotnet pack LanguageExt.CodeGen -c Release -o ../../artifacts/bin

