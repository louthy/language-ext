using CardGame;
using LanguageExt;
using Console = System.Console;

// Play the game!
Game.play
    .Run(Game.Zero).As() // Runs the StateT
    .Run()               // Runs the OptionT
    .Run()               // Runs the IO
    .Ignore();           // We don't care about the result
    
Console.WriteLine("GAME OVER");
