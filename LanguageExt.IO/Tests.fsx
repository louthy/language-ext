#load "IO.fs"

open LanguageExt.IO
open LanguageExt.IO.MonadIO
open LanguageExt.IO.PreludeIO

let test = io {
    do! putStr "What is your name?"
    let! name = getLine
    do! putStrLn ("Hello, " + name)
}

let x = interpretIO test


