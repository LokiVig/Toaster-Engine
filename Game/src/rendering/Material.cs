using OpenTK.Graphics.OpenGL;

namespace DoomNET.Rendering;

public struct Material
{
	public Shader shader; // Defines which shader this material uses - NEEDED!
	
	public Texture colorTexture; // Defines the main color - NEEDED!
	public Texture normalTexture; // Defines the normal map of this material, needed?
	public Texture roughTexture; // Defines the roughness map of this material, needed?

	public Material(string colorPath, string normalPath = null, string roughPath = null)
	{
		// // Initialize the shader
		// shader.Use();
		//
		// //
		// // Texture initialization
		// //
		//
		// // Color texture initialization
		// colorTexture = Texture.LoadFromFile(colorPath);
		// colorTexture.Use(TextureUnit.Texture0);
		//
		// // If we have designated a normal map...
		// if (normalPath != null)
		// {
		// 	// Initialize the normal map
		// 	normalTexture = Texture.LoadFromFile(normalPath);
		// 	normalTexture.Use(TextureUnit.Texture1);
		// }
		//
		// // If we have designated a roughness map...
		// if (roughPath != null)
		// {
		// 	// Initialize the roughness map
		// 	roughTexture = Texture.LoadFromFile(roughPath);
		// 	roughTexture.Use(TextureUnit.Texture2);
		// }
	}
}