using System;
using System.IO;
using System.Collections.Generic;

using OpenTK.Graphics.OpenGL;

using DoomNET.Resources;

using Matrix4 = OpenTK.Mathematics.Matrix4;

namespace DoomNET.Rendering;

/// <summary>
/// Shader compiler / definer.
/// </summary>
public class Shader
{
    public int handle;
    
    private Dictionary<string, int> uniformLocations;
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
        
        // Compile the vertex shader
        CompileShader(vShader);
        
        // And the fragment shader uses the fShaderSource
        int fShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fShader, fShaderSource);
        
        // Compile the fragment shader
        CompileShader(fShader);

        // Link the shaders to a program on the GPU
        handle = GL.CreateProgram();
        
        // Attach both shaders
        GL.AttachShader(handle, vShader);
        GL.AttachShader(handle, fShader);
        
        // Then link them together
        GL.LinkProgram(handle);
        
        // Clean-up, detach and delete the shaders from memory
        GL.DetachShader(handle, vShader);
        GL.DetachShader(handle, fShader);
        GL.DeleteShader(vShader); 
        GL.DeleteShader(fShader);
        
        // Cache all the shader uniforms
        // Get the number of active uniforms in the shader
        GL.GetProgrami(handle, ProgramProperty.ActiveUniforms, out int numberOfUniforms);
        
        // Allocate the dictionary to hold the locations
        uniformLocations = new Dictionary<string, int>();
        
        // Loop over all the uniforms...
        for (int i = 0; i < numberOfUniforms; i++)
        {
            // Get the name of this uniform...
            string key = GL.GetActiveUniform(handle, (uint)i, int.MaxValue, out _, out _, out _);
            
            // Get the location...
            int location = GL.GetUniformLocation(handle, key);
            
            // And then add it to the dictionary
            uniformLocations.Add(key, location);
        }
    }

    private static void CompileShader(int shader)
    {
        // Try to compile the shader
        GL.CompileShader(shader);
        
        // Check for compilation errors
        GL.GetShaderi(shader, ShaderParameterName.CompileStatus, out int status);
        if (status != (int)All.True)
        {
            // We can use `GL.GetShaderInfoLog(shader, out string info)` to get information about the error
            GL.GetShaderInfoLog(shader, out string info);
            
            throw new Exception($"Error occurred whilst compiling shader ({shader}):\n" +
                                $"\t{info}\n");
        }
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
    /// Set a uniform int on this shader.
    /// </summary>
    /// <param name="name">The name of the uniform</param>
    /// <param name="data">The data to set</param>
    public void SetInt(string name, int data)
    {
        GL.UseProgram(handle);
        GL.Uniform1i(uniformLocations[name], data);
    }
    
    /// <summary>
    /// Set a uniform float on this shader.
    /// </summary>
    /// <param name="name">The name of the uniform</param>
    /// <param name="data">The data to set</param>
    public void SetFloat(string name, float data)
    {
        GL.UseProgram(handle);
        GL.Uniform1f(uniformLocations[name], data);
    }

    /// <summary>
    /// Set a uniform Matrix4 on this shader
    /// </summary>
    /// <param name="name">The name of the uniform</param>
    /// <param name="data">The data to set</param>
    /// <remarks>
    ///   <para>
    ///   The matrix is transposed before being sent to the shader.
    ///   </para>
    /// </remarks>
    public void SetMatrix4(string name, Matrix4 data)
    {
        GL.UseProgram(handle);
        GL.UniformMatrix4f(uniformLocations[name], uniformLocations.Count, true, ref data);
    }
    
    /// <summary>
    /// Set a uniform Vector3 on this shader.
    /// </summary>
    /// <param name="name">The name of the uniform</param>
    /// <param name="data">The data to set</param>
    public void SetVector3(string name, Vector3 data)
    {
        GL.UseProgram(handle);
        GL.Uniform3d(uniformLocations[name], data.x, data.y, data.z);
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