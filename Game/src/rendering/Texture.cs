// using System;
using System.IO;

using OpenTK.Graphics.OpenGL;

using StbImageSharp;

namespace DoomNET.Rendering;

public class Texture
{
    public const string MISSINGTEXTURE = "./textures/dev/missing_texture.png";
    
    private int handle;

    public Texture(int handle)
    {
        this.handle = handle;
    }

    /// <summary>
    /// Load a texture from a set path.<br/>
    /// If the path is not found, the texture will be set to the default missing texture.
    /// </summary>
    public static Texture LoadFromFile(string path)
    {
        // Generate the handle
        int _handle = GL.GenTexture();

        // Bind the handle
        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2d, _handle);

        // OpenGL has its texture origin in the lower left corner instead of the top left corner, so we tell StbImageSharp to flip the image when loading
        StbImage.stbi_set_flip_vertically_on_load(1);

        // Open a stream, read the path, and pass it to StbImageSharp to load
        Stream stream = File.OpenRead(path);

        // If the file doesn't actually exist, we should load "resources/textures/dev/missing_texture.png"!
        if (stream.Length == 0)
        {
            stream = File.OpenRead("resources/textures/dev/missing_texture.png");
        }
        
        // Get the image through StbImageSharp
        ImageResult image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
        
        // Bind the texture to our handle!
        GL.TexImage2D(TextureTarget.Texture2d, 0, InternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
        
        // Now that our texture is loaded, set a few settings to affect how the image appears on rendering
        GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.NearestMipmapNearest);
        GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
        
        // Set the wrapping mode
        GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
        
        // Generate its mipmaps
        GL.GenerateMipmap(TextureTarget.Texture2d);

        return new Texture(_handle);
    }

    /// <summary>
    /// Activate texture.
    /// </summary>
    public void Use(TextureUnit unit)
    {
        GL.ActiveTexture(unit);
        GL.BindTexture(TextureTarget.Texture2d, handle);
    }
}