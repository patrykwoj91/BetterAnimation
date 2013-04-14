using System;

namespace BetterSkinned
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (SkinnedGame game = new SkinnedGame())
            {
                game.Run();
            }
        }
    }
}

