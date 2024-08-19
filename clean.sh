echo cleaing bin, obj, and /louthy.github.io/language-ext

# $BestFormBin is where the bestform.exe is compiled to 
Artifacts=/home/paul/Documents/dev/artifacts

# $BestFormBin is where the bestform.exe is compiled to 
BestFormBin=/home/paul/Documents/dev/best-form/bestform

# $LangExtRoot is where the source code root should be (i.e. c:\dev\language-ext)
LangExtRoot=/home/paul/Documents/dev/language-ext

# $LangExtDocs is where the docs root should be (i.e. c:\dev\louthy.github.io)
LangExtDocs=/home/paul/Documents/dev/louthy.github.io

rm -rf $Artifacts

rm -rf $LangExtRoot/LanguageExt.Core/bin
rm -rf $LangExtRoot/LanguageExt.Parsec/bin
rm -rf $LangExtRoot/LanguageExt.FSharp/bin
rm -rf $LangExtRoot/LanguageExt.Rx/bin
rm -rf $LangExtRoot/LanguageExt.Sys/bin

rm -rf $LangExtRoot/LanguageExt.Core/obj
rm -rf $LangExtRoot/LanguageExt.Parsec/obj
rm -rf $LangExtRoot/LanguageExt.FSharp/obj
rm -rf $LangExtRoot/LanguageExt.Rx/obj
rm -rf $LangExtRoot/LanguageExt.Sys/obj

rm -rf $LangExtDocs/language-ext

echo cleaned
