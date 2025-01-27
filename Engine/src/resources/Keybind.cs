namespace Toast.Engine.Resources;

public class Keybind
{
    public string alias; // Name of the keybind, should allow for editing through the console (lets us change which key does this keybind)
    public Key key; // The key that we want to use for this keybind
    public string commandAlias; // The name of the console command we wish to perform
}