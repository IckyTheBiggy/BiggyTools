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
        };

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
                        uIWindow.MinimumSize = new Vector2i(400, 300);
                        uIWindow.Run();
                        break;

                    case "Exit":
                        return;
                }
            }
        }
    }
}