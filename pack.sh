sh clean.sh
sh docs.sh

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
