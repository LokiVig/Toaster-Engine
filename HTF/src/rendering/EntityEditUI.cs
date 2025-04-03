using ImGuiNET;

using Toast.Engine;
using Toast.Engine.Entities;

namespace Toast.HTF.Rendering;

public class EntityEditUI
{
    public static void Display( ref bool open, Entity targetEnt )
    {
        if ( ImGui.Begin( $"Entity Edit - Editing {(targetEnt != null ? targetEnt : "N/A")}", ref open, ImGuiWindowFlags.NoDocking ) )
        {
            EngineManager.ToggleInput( !ImGui.IsWindowFocused() );



            ImGui.End();
        }
    }
}