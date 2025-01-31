using ImGuiNET;

using Toast.Engine.Entities;

namespace Toast.WTFEdit.Rendering;

public class EntityEditUI
{
    public static void Display( ref bool open, Entity targetEnt )
    {
        if ( ImGui.Begin( $"Entity Edit - Editing {(targetEnt != null ? targetEnt : "N/A")}", ref open, ImGuiWindowFlags.NoDocking ) )
        {


            ImGui.End();
        }
    }
}