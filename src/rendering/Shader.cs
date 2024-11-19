using System;
using System.IO;
using OpenTK.Graphics.OpenGL;

namespace DoomNET.Rendering;

/// <summary>
/// Shader compiler / definer.
/// </summary>
public class Shader
{
    public int handle;
    
    private bool disposedValue = false;

    /// <summary>
    /// Create and compile shaders.
    /// </summary>
    /// <param name="vPath">The path to the vertex shader.</param>
    /// <param name="fPath">The path to the fragment shader.</param>
    public Shader(string vPath, string fPath)
    {
        if (!Path.Exists(vPath)) // Make sure the vertex shader path exists
        {
            Directory.CreateDirectory(Path.GetDirectoryName(vPath) ?? throw new InvalidOperationException());
        }
        
        if (!Path.Exists(fPath)) // Make sure the fragment shader path exists
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fPath) ?? throw new InvalidOperationException());
        }
        
        string vShaderSource = File.ReadAllText(vPath); // Vertex (*.vert) shader
        string fShaderSource = File.ReadAllText(fPath); // Fragment (*.frag) shader
        
        // Generate our shaders, and bind them to their source code
        // Vertex shader uses the vShaderSource
        int vShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vShader, vShaderSource);
        
        // And the fragment shader uses the fShaderSource
        int fShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fShader, fShaderSource);
        
        // Compile the shaders and check for errors
        // Compile the vertex shader
        GL.CompileShader(vShader);
        
        // FIXME: GL.GetShader(int, ShaderParameter, out int) seems to be removed... What now?
        
        // Compile the fragment shader
        GL.CompileShader(fShader);
        
        // FIXME: GL.GetShader(int, ShaderParameter, out int) seems to be removed... What now?

        // Link the shaders to a program on the GPU
        handle = GL.CreateProgram();
        
        GL.AttachShader(handle, vShader);
        GL.AttachShader(handle, fShader);
        
        GL.LinkProgram(handle);
        
        // FIXME: Can't check for errors using GL.GetProgram(int, GetProgramParameterName, out int)...
        
        // Clean-up, detach and delete the shaders from memory
        GL.DetachShader(handle, vShader);
        GL.DetachShader(handle, fShader);
        GL.DeleteShader(vShader); 
        GL.DeleteShader(fShader);
    }
    
    /// <summary>
    /// Allows the usage of the compiled shaders.
    /// </summary>
    public void Use()
    {
        GL.UseProgram(handle);
    }

    /// <summary>
    /// Get the location of a specified shader attribute.
    /// </summary>
    /// <param name="attribName">The name of the desired attribute.</param>
    /// <returns>The desired attribute's location as <see langword="int"/>.</returns>
    public int GetAttribLocation(string attribName)
    {
        return GL.GetAttribLocation(handle, attribName);
    }

    /// <summary>
    /// Dispose of this shader.
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            // Delete the program
            GL.DeleteProgram(handle);

            // We've successfully disposed of this shader class
            disposedValue = true;
        }
    }

    ~Shader()
    {
        if (!disposedValue)
        {
            Console.Error.WriteLine("GPU resource leak! Did you forget to call Dispose()?\n");
        }
    }
    
    /// <summary>
    /// Dispose of this shader.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}