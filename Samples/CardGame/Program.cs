using CardGame;
using LanguageExt;
using Console = System.Console;

// Play the game!
Game.play
    .Run(GameState.Zero) // Runs the GameM 
    .Run()               // Runs the IO
    .Ignore();

Console.WriteLine("GAME OVER");
