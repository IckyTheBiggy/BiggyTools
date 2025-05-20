using System.Diagnostics;
using Spectre.Console;

namespace BiggyTools.FFmpeg
{
    public class Utils
    {
        public static void StartRencode()
        {
            var input = Files.Helper.GetFileWithFzf();
            var quality = AnsiConsole.Ask<int>("Enter [yellow]CQP Quality (e.g. 20)[/]");

            if (input == null) return;
            
            RunFFmpeg(input, quality);
        }

        public static void RunFFmpeg(string inputFile, int cqpQuality)
        {
            var outputFileName = GetOutputFilename(inputFile, cqpQuality);

            var ffmpegArgs = $"-i \"{inputFile}\" -c:v hevc_nvenc -rc vbr -cq {cqpQuality} -c:a copy -map 0 \"{outputFileName}\"";

            var startInfo = new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = ffmpegArgs,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var process = new Process { StartInfo = startInfo };
            process.OutputDataReceived += (_, e) => Console.WriteLine(e.Data);
            process.ErrorDataReceived += (_, e) => Console.Error.WriteLine(e.Data);

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();
        }

        public static string GetOutputFilename(string inputFilePath, int cqpQuality)
        {
            var directory = Path.GetDirectoryName(inputFilePath);
            var fileNameWithoutExt = Path.GetFileNameWithoutExtension(inputFilePath);
            var extention = Path.GetExtension(inputFilePath);

            var newFilename = $"{fileNameWithoutExt} [Re-Encoded(CQP: {cqpQuality})]{extention}";

            return Path.Combine(directory ?? "", newFilename);   
        }
    }
}