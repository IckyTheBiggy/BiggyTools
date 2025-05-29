using System.Numerics;
using ImGuiNET;
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

        private static ToolType _selectedTool = ToolType.None;
        private static int _cqpQuality = 20;

        public static void Render()
        {
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
    }
}