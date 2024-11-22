using System;
using System.IO;

using OpenTK.Graphics.OpenGL;

namespace DoomNET.Rendering;

public class Shader
{
    private int handle;
    private bool disposed;

    public Shader(string vPath, string fPath)
    {
        if (!File.Exists(vPath))
        {
            throw new FileNotFoundException("Vertex shader not found!", vPath);
        }

        if (!File.Exists(fPath))
        {
            throw new FileNotFoundException("Fragment shader not found!", fPath);
        }
        
        string vSource = File.ReadAllText(vPath);
        string fSource = File.ReadAllText(fPath);
        
        int vShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vShader, vSource);
        
        int fShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fShader, fSource);
        
        // Compile the shaders, and check for errors
        // Vertex shader
        GL.CompileShader(vShader);

        GL.GetShaderi(vShader, ShaderParameterName.CompileStatus, out int status);

        if (status == 0)
        {
            GL.GetShaderInfoLog(vShader, out string info);
            Console.WriteLine(info);
        }
        
        // Fragment shader
        GL.CompileShader(fShader);

        GL.GetShaderi(fShader, ShaderParameterName.CompileStatus, out status);

        if (status == 0)
        {
            GL.GetShaderInfoLog(fShader, out string info);
            Console.WriteLine(info);
        }
        
        // Fill the handle
        handle = GL.CreateProgram();
        
        // Attach the shaders
        GL.AttachShader(handle, vShader);
        GL.AttachShader(handle, fShader);
        
        // Link the program
        GL.LinkProgram(handle);

        // Make sure the handle's actually linked
        GL.GetProgrami(handle, ProgramProperty.LinkStatus, out status);

        if (status == 0)
        {
            GL.GetProgramInfoLog(handle, out string info);
            Console.WriteLine(info);
        }
        
        // Now, clean up!
        // Detach and delete the shaders
        GL.DetachShader(handle, vShader);
        GL.DetachShader(handle, fShader);
        GL.DeleteShader(vShader);
        GL.DeleteShader(fShader);
    }

    public void Use()
    {
        GL.UseProgram(handle);
    }

    public int GetAttribLocation(string attribName)
    {
        return GL.GetAttribLocation(handle, attribName);        
    }

    public void SetInt(string name, int value)
    {
        int location = GL.GetUniformLocation(handle, name);
        GL.Uniform1i(location, value);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            GL.DeleteProgram(handle);
            disposed = true;
        }
    }

    ~Shader()
    {
        if (!disposed)
        {
            Console.WriteLine("GPU resource leak! Did you forget to call Dispose?\n");
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}