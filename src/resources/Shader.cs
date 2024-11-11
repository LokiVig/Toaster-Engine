using OpenTK.Graphics.OpenGL4;
using System;
using System.IO;

namespace DoomNET.Resources;

public class Shader : IDisposable
{
    public int handle;
    private bool disposedValue = false;

    public Shader(string vertexPath, string fragmentPath)
    {
        string vertexSource = File.ReadAllText(vertexPath);
        string fragmentSource = File.ReadAllText(fragmentPath);

        int vertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertexShader, vertexSource);

        int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragmentShader, fragmentSource);

        // Compiling and error checking

        GL.CompileShader(vertexShader);

        GL.GetShader(vertexShader, ShaderParameter.CompileStatus, out int vSuccess);

        if (vSuccess == 0)
        {
            Console.WriteLine(GL.GetShaderInfoLog(vertexShader));
        }

        GL.CompileShader(fragmentShader);

        GL.GetShader(fragmentShader, ShaderParameter.CompileStatus, out int fSuccess);

        if (fSuccess == 0)
        {
            Console.WriteLine(GL.GetShaderInfoLog(fragmentShader));
        }

        // Using the shaders

        handle = GL.CreateProgram();

        GL.AttachShader(handle, vertexShader);
        GL.AttachShader(handle, fragmentShader);

        GL.LinkProgram(handle);

        GL.GetProgram(handle, GetProgramParameterName.LinkStatus, out int success);

        if (success == 0)
        {
            Console.WriteLine(GL.GetProgramInfoLog(handle));
        }

        GL.DetachShader(handle, vertexShader);
        GL.DetachShader(handle, fragmentShader);
        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);
    }

    public void Use()
    {
        GL.UseProgram(handle);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            GL.DeleteProgram(handle);

            disposedValue = true;
        }
    }

    ~Shader()
    {
        if (!disposedValue)
        {
            Console.WriteLine("GPU resource leak! Did you forget to call Dispose()?");
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}