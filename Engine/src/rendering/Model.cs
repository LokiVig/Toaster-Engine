using System;

namespace Toast.Engine.Rendering;

public class Model
{
    public string path; // The direct path to the model's file
    public uint[] indices; // The indices of this model
    public Vertex[] vertices; // The vertices from the model

    /// <summary>
    /// Loads and returns a <see cref="Model"/> from a specified path.
    /// </summary>
    /// <param name="path">The path to the model we wish to load.</param>
    /// <returns>A new model with all of the information we need to render it.</returns>
    public static Model Load( string path )
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Loads and returns a <see cref="Model"/> from a specified path.
    /// </summary>
    /// <param name="path">The path to the model we wish to load.</param>
    /// <param name="model">The resulting model we've loaded.</param>
    public static void Load( string path, out Model model )
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Removes a specified model.
    /// </summary>
    /// <param name="model">The model we wish to remove.</param>
    public static void Remove( Model model )
    {
        throw new NotImplementedException();
    }
}