using System.Drawing;
using BiggyTools.Debugging;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using Spectre.Console;

namespace Rendering.UI
{
    public static class Background3D
    {
        private static int _vao;
        private static int _vbo;
        private static int _ebo;
        private static int _shaderProgram;
        private static float _rotationAngle = 0.0f;
        private static double _time;

        private static readonly float[] _cubeVertices =
        {
            -0.5f, -0.5f, -0.5f, 0.0f, 0.0f, -1.0f, 0.0f, 0.0f
        };

        private static readonly uint[] _cubeIndicies =
        {
            0, 1, 2, 3, 0
        };

        private static Matrix4 _viewMatrix;
        private static Matrix4 _projectionMatrix;

        public static void HandleOnLoad(GameWindow window)
        {
            GL.Enable(EnableCap.DepthTest);

            _vao = GL.GenVertexArray();
            GL.BindVertexArray(_vao);

            _vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, _cubeVertices.Length * sizeof(float), _cubeVertices, BufferUsageHint.StaticDraw);

            _ebo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _cubeIndicies.Length * sizeof(uint), _cubeIndicies, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 6 * sizeof(float));
            GL.EnableVertexAttribArray(2);

            string vertexShaderSource = @"
                #version 330 core
                layout (location = 0) in vec3 aPos;
                layout (location = 1) in vec3 aNormal;
                layout (location = 2) in vec2 aTexCoords;
                
                out vec3 Normal;
                out vec3 FragPos;
                out vec2 TexCoords;
                
                uniform mat4 model;
                uniform mat4 view;
                uniform mat4 projection;
                
                void main()
                {
                    FragPos = vec3(model * vec4(aPos, 1.0));
                    Normal = mat3(transpose(inverse(model))) * aNormal;
                    TexCoords = aTexCoords;
                    gl_Position = projection * view * model * vec4(aPos, 1.0);
                }";

            string fragmentShaderSource = @"
                #version 330 core
                out vec4 FragColor;
                
                in vec3 Normal;
                in vec3 FragPos;
                in vec2 TexCoords;
                
                uniform vec3 lightPos;
                uniform vec3 viewPos;
                uniform vec3 lightColor;
                uniform vec3 objectColor;
                
                void main()
                {
                    vec3 ambient = 0.1f * lightColor;

                    vec3 norm = normalize(Normal);
                    vec3 lightDir = normalize(lightPos - FragPos);
                    float diff = max(dot(norm, lightDir), 0.0);
                    vec3 diffuse = diff * lightColor;

                    vec3 viewDir = normalize(viewPos - FragPos);
                    vec3 reflectDir = reflect(-lightDir, norm);
                    float spec = pow(max(dot(viewDir, reflectDir), 0.0), 32);
                    vec3 specular = spec * lightColor * 0.5;
                    
                    vec3 result = (ambient + diffuse + specular) * objectColor;
                    FragColor = vec4(result, 1.0);
                }";

            _shaderProgram = CreateAndCompileShader(vertexShaderSource, fragmentShaderSource);

            _viewMatrix = Matrix4.LookAt(new Vector3(0, 0, 2), new Vector3(0, 0, 0), new Vector3(0, 1, 0));
            _projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45f), (float)window.Size.X / window.Size.Y, 0.1f, 100f);

            GL.UseProgram(_shaderProgram);
            GL.UniformMatrix4(GL.GetUniformLocation(_shaderProgram, "view"), false, ref _viewMatrix);
            GL.UniformMatrix4(GL.GetUniformLocation(_shaderProgram, "projection"), false, ref _projectionMatrix);

            GL.Uniform3(GL.GetUniformLocation(_shaderProgram, "lightPos"), new Vector3(1.2f, 1.0f, 2.0f));
            GL.Uniform3(GL.GetUniformLocation(_shaderProgram, "viewPos"), new Vector3(0, 0, 2));
            GL.Uniform3(GL.GetUniformLocation(_shaderProgram, "lightColor"), new Vector3(1.0f, 1.0f, 1.0f));
            GL.Uniform3(GL.GetUniformLocation(_shaderProgram, "objectColor"), new Vector3(0.3f, 0.6f, 0.8f));
        }

        public static void HandleResize(ResizeEventArgs e, GameWindow window)
        {
            GL.Viewport(0, 0, e.Width, e.Height);

            _projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45f), (float)window.Size.X / window.Size.Y, 0.1f, 100f);
            GL.UseProgram(_shaderProgram);
            GL.UniformMatrix4(GL.GetUniformLocation(_shaderProgram, "projection"), false, ref _projectionMatrix);
        }

        public static void HandleOnRenderFrame(FrameEventArgs args)
        {
            _time += args.Time;
            _rotationAngle = (float)_time * 0.5f;

            GL.UseProgram(_shaderProgram);
            GL.BindVertexArray(_vao);

            Matrix4 modelMatrix = Matrix4.Identity;
            modelMatrix *= Matrix4.CreateRotationY(_rotationAngle);

            GL.UniformMatrix4(GL.GetUniformLocation(_shaderProgram, "model"), false, ref modelMatrix);

            GL.DrawElements(PrimitiveType.Triangles, _cubeIndicies.Length, DrawElementsType.UnsignedInt, 0);

            
        }

        public static void HandleUnload()
        {
            GL.DeleteProgram(_shaderProgram);
            GL.DeleteVertexArray(_vao);
            GL.DeleteBuffer(_vbo);
            GL.DeleteBuffer(_ebo);
        }

        private static int CreateAndCompileShader(string vertexShaderSource, string fragmentShaderSource)
        {
            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vertexShaderSource);
            GL.CompileShader(vertexShader);
            CheckShaderError(vertexShader, "Vertex Shader");

            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fragmentShaderSource);
            GL.CompileShader(fragmentShader);
            CheckShaderError(fragmentShader, "Fragment Shader");

            int program = GL.CreateProgram();
            GL.AttachShader(program, vertexShader);
            GL.AttachShader(program, fragmentShader);
            GL.LinkProgram(program);
            CheckProgramError(program);

            GL.DetachShader(program, vertexShader);
            GL.DetachShader(program, fragmentShader);
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);

            return program;
        }

        private static void CheckShaderError(int shader, string type)
        {
            GL.GetShader(shader, ShaderParameter.CompileStatus, out int status);
            if (status == 0)
            {
                string infoLog = GL.GetShaderInfoLog(shader);
                Logger.LogError($"Background3D::Error Compiling{type} shader: {infoLog}");
            }
        }

        private static void CheckProgramError(int program)
        {
            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out int status);
            if (status == 0)
            {
                string infoLog = GL.GetProgramInfoLog(program);
                Logger.LogError($"Background3D::Error linking shader program: {infoLog}");
            }
        }
    }
}