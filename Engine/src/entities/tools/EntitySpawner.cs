using System;
using Toast.Engine.Resources;

namespace Toast.Engine.Entities;

public class EntitySpawner<T> : ToolEntity where T : Entity, new()
{
	public EntitySpawner()
	{
		SetBBox(new BBox(new Vector3(-8, -8, -8), new Vector3(8, 8, 8)));
	}

	public EntitySpawner(Vector3 position) : base(position)
	{
		SetBBox(new BBox(new Vector3(-8, -8, -8), new Vector3(8, 8, 8)));
	}

	/// <summary>
	/// Spawn an entity, based on the given argument.
	/// </summary>
	/// <returns>The recently spawned entity.</returns>
	public T SpawnEntity()
	{
		T ent = new T();

		// Make sure the desired entity is not another spawner!
		if (ent is EntitySpawner<T>)
		{
			Console.WriteLine("Can't spawn another spawner!\n");
			return null;
		}

		// The entity should spawn!
		ent.Spawn();
		// Set the entity's position to our position
		ent.SetPosition(position);

		// Add the newly spawned entity to the current scene
		EngineProgram.currentScene?.AddEntity(ent);

		// Generate the entity's ID
		ent.CreateID();

		Console.WriteLine($"Spawned entity {ent}.\n" +
		                  $"\tSource: {this}\n" +
		                  $"\tPosition: {ent.GetPosition()}\n" +
		                  $"\tRotation: {ent.GetRotation()}\n" +
		                  $"\tBBox: {ent.GetBBox()}\n");

		// And return the entity we just spawned
		return ent;
	}

	public override string ToString()
	{
		return $"EntitySpawner<{typeof(T)}> (\"{GetID()}\")";
	}
}

public class EntitySpawner : Entity
{
	public override EntityType type => EntityType.Tool;

	public EntitySpawner()
	{
		SetBBox(new BBox(new Vector3(-8, -8, -8), new Vector3(8, 8, 8)));
	}

	public EntitySpawner(Vector3 position) : base(position)
	{
		SetBBox(new BBox(new Vector3(-8, -8, -8), new Vector3(8, 8, 8)));
	}

	/// <summary>
	/// Spawn an entity, based on the given argument.
	/// </summary>
	/// <param name="ent">The entity we wish to spawn.</param>
	/// <returns>The recently spawned entity.</returns>
	public Entity SpawnEntity(Entity ent)
	{
		// Make sure the desired entity is not a tool entity!
		if (ent is ToolEntity)
		{
			Console.WriteLine("Can't spawn a tool entity!\n");
			return null;
		}

		// The entity should spawn!
		ent.Spawn();
		// Set the entity's position to our position
		ent.SetPosition(position);

		// Add the newly spawned entity to the current scene
		EngineProgram.currentScene?.AddEntity(ent);

		// Generate the entity's ID
		ent.CreateID();

		Console.WriteLine($"Spawned entity {ent}.\n" +
		                  $"\tSource: {this}\n" +
		                  $"\tPosition: {ent.GetPosition()}\n" +
		                  $"\tRotation: {ent.GetRotation()}\n" +
		                  $"\tBBox: {ent.GetBBox()}\n");

		// And return the entity we just spawned
		return ent;
	}
}