using Spectre.Console;

namespace BiggyTools
{
    class Program
    {
        public static void Main(string[] args)
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
            }
        }
    }
}