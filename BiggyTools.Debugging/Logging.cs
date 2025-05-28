using Spectre.Console;

namespace BiggyTools.Debugging
{
    public static class Logger
    {
        public static void Log(string text)
        {
            PrintLog("Log::" + text);
        }

        public static void LogWarning(string text)
        {
            PrintLog("Warning::" + text);
        }

        public static void LogError(string text)
        {
            PrintLog("Error::" + text);   
        }

        private static void PrintLog(string text)
        {
            AnsiConsole.Write(text);
        }
    }
}