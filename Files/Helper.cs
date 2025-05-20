using System.Diagnostics;

namespace BiggyTools.Files
{
    public class Helper
    {
        public static string? GetFileWithFzf(string searchDirectory = ".")
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "bash",
                Arguments = $"-c \"find {searchDirectory} -type f | fzf\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var process = new Process { StartInfo = startInfo };
            process.Start();

            string? selectedFile = process.StandardOutput.ReadLine();
            process.WaitForExit();

            return string.IsNullOrWhiteSpace(selectedFile) ? null : selectedFile.Trim();
        }
    }
}