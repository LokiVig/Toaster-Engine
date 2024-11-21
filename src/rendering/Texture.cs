using System;
using System.IO;

using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics.Vulkan;

using StbImageSharp;

namespace DoomNET.Rendering;

public class Texture
{
    public readonly int handle;

    public Texture(int handle)
    {
        this.handle = handle;
    }

    /// <summary>
    /// Activate this texture.
    /// </summary>
    public void Use(TextureUnit unit)
    {
        GL.ActiveTexture(unit);
        GL.BindTexture(TextureTarget.Texture2d, handle);
    }
    
    public static Texture LoadFromFile(string path)
    {
        if (!Path.Exists(path)) // Make sure the texture path exists
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path) ?? throw new InvalidOperationException());
        }
        
        // Generate handle
        int _handle = GL.GenTexture();
        
        // Bind the handle
        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2d, _handle);
        
        // OpenGL has its texture origin in the lower left corner, instead of the top left corner,
        // so we tell StbImageSharp to flip the image when loading
        StbImage.stbi_set_flip_vertically_on_load(1);
        
        // Use a stream to read the file and pass it to StbImageSharp to load
        using (Stream stream = File.OpenRead(path))
        {
            ImageResult image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
            GL.TexImage2D(TextureTarget.Texture2d, 0, InternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
        }
        
        // Now that our texture is loaded, we can set a few settings to affect how the image appears on rendering
        GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
        
        // Generate mipmaps
        GL.GenerateMipmap(TextureTarget.Texture2d);

        return new Texture(_handle);
    }
}