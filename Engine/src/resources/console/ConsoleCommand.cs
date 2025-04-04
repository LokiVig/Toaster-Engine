﻿using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace Toast.Engine.Resources.Console;

/// <summary>
/// </summary>
public class ConsoleCommand
{
    public string alias; // The name of the command
    public string description; // The visual description for this command

    [JsonIgnore] public Action onCall = ConsoleManager.InvalidCommand; // Things to do when the command's been called
    [JsonIgnore] public Action<List<object>> onArgsCall = ConsoleManager.InvalidCommand; // Things to do when the command's been called with arguments

    public string onCallAlias; // Auto-generated alias for this command's onCall action!
    public string onArgsCallAlias; // Auto-generated alias for this command's onArgsCall action!

    public bool enabled = true; // Decides whether or not this command is currently active, ergo callable
    public CommandConditions conditions = CommandConditions.None; // Decides which conditions this command requires to be called
}

/// <summary>
/// Determines when this console command can be called
/// </summary>
public enum CommandConditions
{
    None, // No special conditions are needed
    Cheats // Cheats need to be enabled
}