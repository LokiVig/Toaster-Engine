using System;
using System.Collections.Generic;

namespace Toast.Engine.Resources;

/// <summary>
/// </summary>
public class ConsoleCommand
{
    public string alias; // The name of the command
    public string description; // The visual description for this command

    public Action onCall; // Things to do when the command's been called
    public Action<List<object>> onArgsCall; // Things to do when the command's been called with arguments

    public bool enabled = true; // Decides whether or not this command is currently active, ergo callable
}