using Veldrid;

namespace Toast.Engine.Rendering;

public class Material
{
    public Shader shader; // The shader we wish to use
    public Texture color; // The color texture of this material
    public Texture normal; // The potential normal texture of this material
    public Texture rough; // The potential roughness texture of this material
}