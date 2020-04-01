using System;
using System.IO;
using OpenTK.Graphics.OpenGL;

namespace Engine.cgimin.helpers
{
    public class ShaderCompiler
    {

        public static int CreateShaderProgram(string pathVS, string pathFS)
        {

            int vertexObject;
            int fragmentObject;
            int program;

            // Shader Files (Text) werden eingelesen 
            string vs = File.ReadAllText(pathVS);
            string fs = File.ReadAllText(pathFS);

            int status_code;
            string info;

            // Vertex und Fragment Shader werden errstellt
            vertexObject = GL.CreateShader(ShaderType.VertexShader);
            fragmentObject = GL.CreateShader(ShaderType.FragmentShader);

            // Vertex-Shader wird compiliert
            GL.ShaderSource(vertexObject, vs);
            GL.CompileShader(vertexObject);
            GL.GetShaderInfoLog(vertexObject, out info);
            GL.GetShader(vertexObject, ShaderParameter.CompileStatus, out status_code);

            if (status_code != 1)
                throw new ApplicationException(info);

            // Fragment-Shader wird compiliert
            GL.ShaderSource(fragmentObject, fs);
            GL.CompileShader(fragmentObject);
            GL.GetShaderInfoLog(fragmentObject, out info);
            GL.GetShader(fragmentObject, ShaderParameter.CompileStatus, out status_code);

            if (status_code != 1)
                throw new ApplicationException(info);

            // Das Shader-Programm wird aus Vertex-Shader und Fragment-Shader zusammengesetzt
            program = GL.CreateProgram();
            GL.AttachShader(program, fragmentObject);
            GL.AttachShader(program, vertexObject);

            return program;
        }


    }
}
