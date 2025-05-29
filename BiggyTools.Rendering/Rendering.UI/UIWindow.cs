using System.Drawing;
using BiggyTools.Debugging;
using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace Rendering.UI
{
    public class UIWindow : GameWindow
    {
        private ImGuiController _imGuiController;

        public UIWindow(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {

        }

        protected override void OnLoad()
        {
            base.OnLoad();

            ImGui.CreateContext();
            ImGui.SetCurrentContext(ImGui.GetCurrentContext());

            Color4 clearColor = new Color4(ImGuiUI.BackgroundClearColor.X, ImGuiUI.BackgroundClearColor.Y, ImGuiUI.BackgroundClearColor.Z, ImGuiUI.BackgroundClearColor.W);
            GL.ClearColor(clearColor);

            Background3D.HandleOnLoad(this);

            _imGuiController = new ImGuiController(ClientSize.X, ClientSize.Y);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Background3D.HandleOnRenderFrame(args);

            _imGuiController.Update(this, (float)args.Time);

            ImGuiUI.Render();

            _imGuiController.Render();

            SwapBuffers();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
            _imGuiController.WindowResized(ClientSize.X, ClientSize.Y);

            Background3D.HandleResize(e, this);
        }

        protected override void OnUnload()
        {
            base.OnUnload();

            _imGuiController.Dispose();

            Background3D.HandleUnload();
        }

        public override void Close()
        {
            base.Close();

            Logger.Log("UIWindow::Closed Window");
        }
    }
}