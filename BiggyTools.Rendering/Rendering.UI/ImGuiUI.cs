using System.Drawing;
using System.Globalization;
using System.Numerics;
using BiggyTools.Debugging;
using ImGuiNET;
using Newtonsoft.Json.Linq;
using static ImGuiNET.ImGui;

namespace Rendering.UI
{
    public static class ImGuiUI
    {
        public enum ToolType
        {
            None,
            ReEncode,
            FileSync,
            Other
        }

        public static Vector4 BackgroundClearColor = new Vector4(0f, 0f, 0f, 1f);

        private static ToolType _selectedTool = ToolType.None;
        private static int _cqpQuality = 20;

        public static void Render()
        {
            if (File.Exists("colors.json"))
            {
                var colors = LoadJsonColors("colors.json");
                ApplyTheme(colors);
            }

            Vector2 viewportSize = GetMainViewport().Size;
            Vector2 windowPos = GetMainViewport().Pos;

            SetNextWindowPos(windowPos);
            SetNextWindowSize(viewportSize);

            ImGuiWindowFlags windowFlags =
                ImGuiWindowFlags.NoMove |
                ImGuiWindowFlags.NoResize |
                ImGuiWindowFlags.NoCollapse |
                ImGuiWindowFlags.NoTitleBar |
                ImGuiWindowFlags.NoScrollbar |
                ImGuiWindowFlags.NoBackground;

            Begin("##MainWindow", windowFlags);

            float leftPanelWidth = 200f;
            float rightPanelWidth = GetContentRegionAvail().X;
            float panelHeight = GetContentRegionAvail().Y;

            if (BeginChild("##ToolListPanel", new Vector2(leftPanelWidth, panelHeight)))
            {
                Text("Biggy Tools");
                Separator();

                if (Selectable("Re-Encode", _selectedTool == ToolType.ReEncode))
                {
                    _selectedTool = ToolType.ReEncode;
                }

                if (Selectable("File-Sync", _selectedTool == ToolType.FileSync))
                {
                    _selectedTool = ToolType.FileSync;
                }

                if (Selectable("Other", _selectedTool == ToolType.Other))
                {
                    _selectedTool = ToolType.Other;
                }

                EndChild();
            }

            SameLine();

            if (BeginChild("##ToolOptionsPanel", new Vector2(rightPanelWidth, panelHeight), ImGuiChildFlags.Borders))
            {
                if (_selectedTool == ToolType.Other)
                {
                    Text("Select a tool from the left panel");
                }

                else
                {
                    Text($"Options for {_selectedTool} Tool:");
                    Separator();

                    switch (_selectedTool)
                    {
                        case ToolType.ReEncode:
                            ShowRencodeOptions();
                            break;
                        case ToolType.FileSync:
                            ShowFileSyncOptions();
                            break;
                    }
                }

                EndChild();
            }

            End();
        }

        private static void ShowRencodeOptions()
        {
            SliderInt("CQP Quality", ref _cqpQuality, 0, 50);
        }

        private static void ShowFileSyncOptions()
        {
            Text("Work in progress");
        }

        public static Dictionary<string, Vector4> LoadJsonColors(string path)
        {
            var colorDict = new Dictionary<string, Vector4>();

            try
            {
                var json = JObject.Parse(File.ReadAllText(path));

                if (json != null && json.ContainsKey("colors"))
                {
                    var colors = (JObject?)json["colors"];

                    if (colors != null)
                    {
                        foreach (var kvp in colors)
                        {
                            string key = kvp.Key;
                            string hex = kvp.Value?.ToString() ?? string.Empty;
                            var colorVec = HexToVec4(hex);
                            colorDict[key] = colorVec;
                        }

                        return colorDict;
                    }

                    else
                    {
                        Logger.LogWarning($"Colors Property in '{path}' is not a valid JSON object");
                    }
                }

                else
                {
                    Logger.LogWarning($"Colors property not found in '{path}' or JSON in null/empty");
                }
            }

            catch (FileNotFoundException)
            {
                Logger.LogError($"File not found at '{path}'");
            }

            catch (Newtonsoft.Json.JsonReaderException ex)
            {
                Logger.LogError($"Error parsing JSON from '{path}': {ex.Message}");
            }

            catch (Exception ex)
            {
                Logger.LogError($"An unexpected error occured while loading colors from '{path}': {ex.Message}");
            }

            return colorDict;
        }

        private static Vector4 HexToVec4(string hex)
        {
            if (hex.StartsWith("#"))
                hex = hex.Substring(1);

            byte r = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);

            return new Vector4(r / 255f, g / 255f, b / 255f, 1.0f);
        }

        public static void ApplyTheme(Dictionary<string, Vector4> colors)
        {
            var style = GetStyle();

            style.Colors[(int)ImGuiCol.WindowBg] = colors["color0"];
            style.Colors[(int)ImGuiCol.Text] = colors["color7"];
            style.Colors[(int)ImGuiCol.Button] = colors["color2"];
            style.Colors[(int)ImGuiCol.ButtonHovered] = colors["color3"];
            style.Colors[(int)ImGuiCol.ButtonActive] = colors["color4"];
            style.Colors[(int)ImGuiCol.Header] = colors["color5"];
            style.Colors[(int)ImGuiCol.HeaderHovered] = colors["color6"];
            style.Colors[(int)ImGuiCol.HeaderActive] = colors["color1"];

            BackgroundClearColor = colors["color0"];
        }
    }
}