using System;

namespace Toast.Engine.Resources.Attributes;

public class ConsoleCommandAttribute : Attribute
{
    public string alias;
    public string description;
    public bool requiresCheats;

    public ConsoleCommandAttribute( string alias, string description )
    {
        this.alias = alias;
        this.description = description;
    }

    public ConsoleCommandAttribute( string alias, string description, bool requiresCheats = false ) : this(alias, description)
    {
        this.requiresCheats = requiresCheats;
    }
}