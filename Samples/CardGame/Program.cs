using CardGame;
using LanguageExt;
using Console = System.Console;

// Play the game!
Game.play
    .Run(GameState.Zero) // Runs the Game 
    .Run()               // Runs the IO
    .Ignore();           // Discard the result

Console.WriteLine("GAME OVER");
