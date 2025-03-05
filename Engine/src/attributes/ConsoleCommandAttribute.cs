using System;

using Toast.Engine.Resources.Console;

namespace Toast.Engine.Attributes;

/// <summary>
/// Marks this method as a console command usable in the engine through the console.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class ConsoleCommandAttribute : Attribute
{
    public string alias; // The alias of the console command
    public string description; // The description of the console command
    public CommandConditions conditions; // The special conditions we need to meet to call this console command

    /// <summary>
    /// Defines a console command with an <paramref name="alias"/>.
    /// </summary>
    /// <param name="alias">The alias of this console command.</param>
    public ConsoleCommandAttribute( string alias )
    {
        this.alias = alias;
    }

    /// <summary>
    /// Defines a console command with an <paramref name="alias"/> and a <paramref name="description"/>.
    /// </summary>
    /// <param name="alias">The alias of this console command.</param>
    /// <param name="description">The description of this console command, shortly describing what it does.</param>
    public ConsoleCommandAttribute( string alias, string description ) 
        : this(alias)
    {
        this.description = description;
    }

    /// <summary>
    /// Defines a console command with an <paramref name="alias"/>, a <paramref name="description"/>, and special <paramref name="conditions"/>.
    /// </summary>
    /// <param name="alias">The alias of this console command.</param>
    /// <param name="description">The description of this console command, shortly describing what it does.</param>
    /// <param name="conditions">The special conditions the command needs to be called.</param>
    public ConsoleCommandAttribute( string alias, string description, CommandConditions conditions = CommandConditions.None) 
        : this(alias, description)
    {
        this.conditions = conditions;
    }
}