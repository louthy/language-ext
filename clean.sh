echo cleaing bin, obj, and /louthy.github.io/language-ext

# Artifacts is where the DLLs are compiled to 
Artifacts=/media/paul/raid/dev/artifacts

# $LangExtRoot is where the source code root should be (i.e. c:\dev\language-ext)
LangExtRoot=/media/paul/raid/dev/language-ext

rm -rf $Artifacts

rm -rf $LangExtRoot/LanguageExt.Core/bin
rm -rf $LangExtRoot/LanguageExt.Pipes/bin
rm -rf $LangExtRoot/LanguageExt.Parsec/bin
rm -rf $LangExtRoot/LanguageExt.FSharp/bin
rm -rf $LangExtRoot/LanguageExt.Rx/bin
rm -rf $LangExtRoot/LanguageExt.Sys/bin

rm -rf $LangExtRoot/LanguageExt.Core/obj
rm -rf $LangExtRoot/LanguageExt.Pipes/obj
rm -rf $LangExtRoot/LanguageExt.Parsec/obj
rm -rf $LangExtRoot/LanguageExt.FSharp/obj
rm -rf $LangExtRoot/LanguageExt.Rx/obj
rm -rf $LangExtRoot/LanguageExt.Sys/obj

echo cleaned
