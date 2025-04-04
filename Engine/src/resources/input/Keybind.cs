﻿using Veldrid;

namespace Toast.Engine.Resources.Input;

public class Keybind
{
    public string alias; // Name of the keybind, should allow for editing through the console (lets us change which key does this keybind)
    public Key key; // The key that we want to use for this keybind
    public Key comboKey = Key.Unknown; // If this keybind should blend with Ctrl, Alt, etc., this value should be filled too
    public bool down; // Determines whether or not the keybind supports being held down
    public string commandAlias; // The name of the console command we wish to perform
}