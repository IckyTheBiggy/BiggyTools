using System.Diagnostics;
using Spectre.Console;

namespace BiggyTools.FFmpeg
{
    public class Utils
    {
        private static string? _encoderType;
        private static bool _compatabilityMode = false;

        public static void StartRencode()
        {
            var input = Files.Helper.GetFileWithFzf();
            var mode = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                .Title("[green]Re-Encode With Compatability Mode(H.264, mp4)[/]")
                .PageSize(8)
                .AddChoices(new[] {
                    "no",
                    "yes"
                }));

            var quality = AnsiConsole.Ask<int>("Enter [yellow]CQP Quality (e.g. 20)[/]");

            switch (mode)
            {
                case "no":
                    _encoderType = "hevc_nvenc";
                    break;

                case "yes":
                    _encoderType = "h264_nvenc";
                    _compatabilityMode = true;
                    break;
            }

            if (input == null) return;
            if (_encoderType == null) return;

            RunFFmpeg(input, _encoderType, quality);
        }

        public static void RunFFmpeg(string inputFile, string encoderType, int cqpQuality)
        {
            var outputFileName = GetOutputFilename(inputFile, cqpQuality);

            var ffmpegArgs = $"-i \"{inputFile}\" -c:v {encoderType} -rc vbr -cq {cqpQuality} -c:a copy -map 0 \"{outputFileName}\"";

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

            if (_compatabilityMode)
            {
                extention = ".mp4";
            }

            var newFilename = $"{fileNameWithoutExt} [Re-Encoded(CQP:{cqpQuality})]{extention}";

            return Path.Combine(directory ?? "", newFilename);   
        }
    }
}