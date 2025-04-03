using System;
using System.Collections.Generic;

using Toast.Engine.Resources.Audio;
using Toast.Engine.Resources.Console;

namespace Toast.Engine.Attributes;

/// <summary>
/// Marks this method as a console command usable in the engine through the console.<br/>
/// The method <b>NEEDS</b> to be static to be accessed and registered as a console command,<br/>
/// while this may give some issues, it's, as far as I know, the only way to do it.<br/>
/// <br/>
/// To allow for argumented console commands, use <see cref="List{T}"/> with <see langword="object"/> as its type.<br/>
/// This does mean you will have to specify the arg count in the method and the arg types to<br/>
/// some extent (check <see cref="AudioManager.PlaySound(List{object})"/> for a great example),<br/>
/// but it should be easy to figure out types and argument counts.<br/>
/// <br/>
/// Otherwise, if you want a console command that has no arguments, have your method feature no arguments.<br/>
/// Those commands are mainly for toggleable options or for otherwise visual options (e.g. displaying all commands or other values).
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class ConsoleCommandAttribute : Attribute
{
    public string Alias; // The alias of the console command
    public string Description; // The description of the console command
    public CommandConditions Conditions; // The special conditions we need to meet to call this console command

    /// <summary>
    /// Defines a console command with an <paramref name="alias"/>.
    /// </summary>
    /// <param name="alias">The alias of this console command.</param>
    public ConsoleCommandAttribute( string alias )
    {
        Alias = alias;
    }

    /// <summary>
    /// Defines a console command with an <paramref name="alias"/> and a <paramref name="description"/>.
    /// </summary>
    /// <param name="alias">The alias of this console command.</param>
    /// <param name="description">The description of this console command, shortly describing what it does.</param>
    public ConsoleCommandAttribute( string alias, string description ) 
        : this(alias)
    {
        Description = description;
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
        Conditions = conditions;
    }
}