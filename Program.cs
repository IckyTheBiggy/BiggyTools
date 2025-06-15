using BiggyTools.Debugging;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using Rendering.UI;
using Spectre.Console;

namespace BiggyTools
{
    class Program
    {
        public static void Main(string[] args)
        {
            while (true)
            {
                var option = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                .Title("[green]BiggyTools[/]")
                .PageSize(10)
                .AddChoices(new[] {
                    "Re-Encode Video",
                    "Exit"
                }));

                switch (option)
                {
                    case "Re-Encode Video":
                        FFmpeg.Utils.StartRencode();
                        break;

                    case "Exit":
                        return;
                }
            }
        }

        private static void PrintHelp()
        {
            Console.WriteLine("Usage");
            Console.WriteLine(" --tui Run in text-based moded");
            Console.WriteLine(" --gui Run in graphical mode");
            Console.WriteLine(" --help Show this help message");
        }
    }
}