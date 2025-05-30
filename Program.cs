using BiggyTools.Debugging;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using Rendering.UI;
using Spectre.Console;

namespace BiggyTools
{
    class Program
    {
        static NativeWindowSettings nativeWindowSettings = new NativeWindowSettings
        {
            ClientSize = new Vector2i(800, 600),
            Title = "BiggyTools"
            //AutoLoadBindings = false
        };

        private static bool _useGui;

        public static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: BiggyTools --tui | --gui | --help");
                return;
            }

            string arg = args[0].ToLower();

            switch (arg)
            {
                case "--tui":
                    _useGui = false;
                    break;
                case "--gui":
                    _useGui = true;
                    break;
                case "--help":
                    PrintHelp();
                    break;
            }

            while (true)
            {
                var option = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                .Title("[green]BiggyTools[/]")
                .PageSize(10)
                .AddChoices(new[] {
                    "Re-Encode Video",
                    "UI",
                    "Exit"
                }));

                switch (option)
                {
                    case "Re-Encode Video":
                        FFmpeg.Utils.StartRencode();
                        break;

                    case "UI":
                        UIWindow uIWindow = new UIWindow(GameWindowSettings.Default, nativeWindowSettings);
                        uIWindow.MinimumSize = new Vector2i(600, 350);
                        uIWindow.Run();
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