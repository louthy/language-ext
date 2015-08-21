// TODO: Work-in-progress
// Not sure yet whether to go the whole hog and have an IO interpreter for all common IO
// or buid one just for the Process system (to facilitate transactional IO).  

namespace LanguageExt.IO

open System.IO
open LanguageExt

type IO<'a> =
    | PutChar       of char * 'a
    | PutStr        of string * 'a
    | PutStrLn      of string * 'a
    | Print         of obj * 'a
    | GetChar       of (char -> 'a)
    | GetLine       of (string -> 'a)
    | GetContents   of (string -> 'a)
    //| Tell          of ProcessId * obj * ProcessId * 'a

type FreeIO<'a> = 
    | Pure of 'a
    | FreeIO of IO<FreeIO<'a>>

[<AutoOpen>]
module MonadIO =

    let rec interpretIO (term : FreeIO<'a>) : 'a = 
        match term with 
        | FreeIO(PutChar(c,next))        -> stdout.Write(c)
                                            interpretIO next
        | FreeIO(PutStr(s,next))         -> stdout.Write(s)
                                            interpretIO next
        | FreeIO(PutStrLn(s,next))       -> stdout.WriteLine(s)
                                            interpretIO next
        | FreeIO(Print(x,next))          -> printfn "%A" x
                                            interpretIO next
        | FreeIO(GetChar f)              -> let read = stdin.Read() |> char
                                            interpretIO (f read)
        | FreeIO(GetLine f)              -> let read = stdin.ReadLine()
                                            interpretIO (f read)
        | FreeIO(GetContents f)          -> let read = stdin.ReadToEnd()
                                            interpretIO (f read)
        | Pure a                         -> a

    let mapIO (f : 'a -> 'b) (term : IO<'a>) : IO<'b> =
        match term with
        | PutChar(c,v)          -> PutChar(c,f v)
        | PutStr(s,v)           -> PutStr(s,f v)
        | PutStrLn(c,v)         -> PutStrLn(c,f v)
        | Print(x,v)            -> Print(x,f v)
        | GetChar fn            -> GetChar(f << fn)
        | GetLine fn            -> GetLine(f << fn)
        | GetContents fn        -> GetContents(f << fn)

    let rec bind (f : 'a -> FreeIO<'b>) (term : FreeIO<'a>) : FreeIO<'b> =
        match term with
            | Pure value -> f value
            | FreeIO t -> FreeIO (mapIO (bind f) t)

    let liftF (term:IO<'a>) : FreeIO<'a> =
        FreeIO (mapIO Pure term)

    let (>>=) = fun term f -> bind f term
    let (>>.) = fun t1 t2 -> t1 >>= fun _ -> t2

    type IOBuilder() =
        member x.Bind(term, f) = bind f term
        member x.Return(value) = Pure value
        member x.Combine(term1, term2) = term1 >>. term2
        member x.Zero() = Pure ()
        member x.Delay(f) = f()
    let io = new IOBuilder()

[<AutoOpen>]
module PreludeIO =
    let putChar c            = liftF (PutChar(c,()))
    let putStr s             = liftF (PutStr(s,()))
    let putStrLn s           = liftF (PutStrLn(s,()))
    let print x              = liftF (Print(x,()))
    let getChar              = liftF (GetChar id)
    let getLine              = liftF (GetLine id)
    let getContents          = liftF (GetContents id)
