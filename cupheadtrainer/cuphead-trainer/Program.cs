using System;
using System.Threading;
using CupheadTrainer.Core;

namespace CupheadTrainer
{
    /// <summary>
    /// Entry point for the Cuphead Trainer application.
    /// Provides a console interface to activate cheats.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Cuphead Trainer v1.0");
            Console.WriteLine("--------------------");
            Console.WriteLine("Commands:");
            Console.WriteLine("  infhp   - Toggle infinite HP");
            Console.WriteLine("  infcoins - Toggle infinite coins");
            Console.WriteLine("  speed   - Toggle speed multiplier (2x)");
            Console.WriteLine("  exit    - Quit trainer");
            Console.WriteLine();

            var trainer = new TrainerCore();
            trainer.Initialize();

            bool running = true;
            while (running)
            {
                Console.Write("> ");
                string? input = Console.ReadLine()?.Trim().ToLower();

                switch (input)
                {
                    case "infhp":
                        trainer.ToggleInfiniteHP();
                        Console.WriteLine("Infinite HP toggled.");
                        break;
                    case "infcoins":
                        trainer.ToggleInfiniteCoins();
                        Console.WriteLine("Infinite coins toggled.");
                        break;
                    case "speed":
                        trainer.ToggleSpeedMultiplier();
                        Console.WriteLine("Speed multiplier toggled.");
                        break;
                    case "exit":
                        running = false;
                        Console.WriteLine("Exiting...");
                        break;
                    default:
                        Console.WriteLine("Unknown command.");
                        break;
                }
            }

            trainer.Cleanup();
        }
    }
}
