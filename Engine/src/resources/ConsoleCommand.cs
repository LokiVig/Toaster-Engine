using System;
using System.Collections.Generic;

namespace Toast.Engine.Resources;

/// <summary>
/// </summary>
public class ConsoleCommand
{
    public string alias;
    public List<object> args = new();

    public Action onCall;
    public Action<List<object>> onArgsCall;

    public bool enabled = true;
}