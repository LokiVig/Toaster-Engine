namespace Toast.Engine.Resources;

public class Model
{
    public string path; // The direct path to the model's file
    public ushort[] indices; // The amount of indices from this model
    public Vertex[] vertices; // The vertices from the model

    /// <summary>
    /// Loads and returns a <see cref="Model"/> from a specified path.
    /// </summary>
    /// <param name="path">The path to the model we wish to load.</param>
    /// <returns>A new model with all of the information we need to render it.</returns>
    public static Model Load( string path )
    {
        return null;
    }

    /// <summary>
    /// Removes a specified model from another object.
    /// </summary>
    /// <param name="model">The model we wish to remove.</param>
    public static void Remove(Model model)
    {

    }
}