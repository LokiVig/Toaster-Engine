using System.Linq;
using System.Collections.Generic;

using Toast.Engine.Entities;

namespace Toast.Engine.Resources;

/// <summary>
/// A scene with a list of entities and brushes.
/// </summary>
public class Scene
{
	private List<Entity> entities;
	private List<Brush> brushes;

	public Scene(List<Entity> entities, List<Brush> brushes)
	{
		this.entities = entities;
		this.brushes = brushes;
	}

	/// <summary>
	/// Load variables from the argument WTF file
	/// </summary>
	public static Scene LoadFromFile(WTF file)
	{
		return new Scene(file.GetEntities(), file.GetBrushes());
	}

	/// <summary>
	/// Load variables from a WTF file loaded from the argument path
	/// </summary>
	public static Scene LoadFromFile(string path)
	{
		WTF file = WTF.LoadFile(path);
		return new Scene(file.GetEntities(), file.GetBrushes());
	}

	/// <summary>
	/// Remove the entity from the entities list
	/// </summary>
	/// <param name="entity">Desired entity to remove</param>
	public void RemoveEntity(Entity entity)
	{
		entities.Remove(entity);
	}

	/// <summary>
	/// Remove the entity from the entities list
	/// </summary>
	/// <param name="id">Desired entity to remove</param>
	public void RemoveEntity(string id)
	{
		foreach (Entity entity in entities.ToList())
		{
			if (entity.GetID() == id)
			{
				entities.Remove(entity);
			}
		}
	}

	/// <summary>
	/// Returns this scenes list of entities
	/// </summary>
	public List<Entity> GetEntities()
	{
		return entities;
	}

	/// <summary>
	/// Gets the player entity from the scenes list of entities
	/// </summary>
	public Player GetPlayer()
	{
		// Check every entity in our entities list
		foreach (Entity ent in entities)
		{
			if (ent is Player)
			{
				return ent as Player;
			}
		}

		// If there is no player
		// Should this really be possible?
		return null;
	}

	/// <summary>
	/// Returns this scenes list of brushes
	/// </summary>
	public List<Brush> GetBrushes()
	{
		return brushes;
	}

	/// <summary>
	/// Add an entity to the scene.
	/// </summary>
	/// <param name="ent">A specific entity to add to this scene.</param>
	/// <typeparam name="T">Anything that is an entity should be added to the scene.</typeparam>
	public void AddEntity<T>(T ent) where T : Entity
	{
		entities.Add(ent);
	}

	/// <summary>
	/// Finds an entity according to their ID
	/// </summary>
	/// <param name="id">A specific ID of an entity</param>
	/// <returns>The desired entity appropriate to the argument ID</returns>
	public T FindEntity<T>(string id) where T : Entity
	{
		// Get the id of every entity
		for (int i = 0; i < entities.Count; i++)
		{
			// If entities[i]'s ID fits with the input ID, return that entity
			if (entities[i].GetID() == id && entities[i] is T entity)
			{
				return entity;
			}
		}

		// We didn't find an entity with that ID! Return null
		return null;
	}

	/// <inheritdoc cref="FindEntity{T}"/>
	public Entity FindEntity(string id)
	{
		// Get the id of every entity
		for (int i = 0; i < entities.Count; i++)
		{
			// If entities[i]'s ID fits with the input ID, return that entity
			if (entities[i].GetID() == id)
			{
				return entities[i];
			}
		}

		// We didn't find an entity with that ID! Return null
		return null;
	}
}