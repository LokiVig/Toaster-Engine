using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

using ImGuiNET;

using Toast.Engine.Entities;
using Toast.Engine.Entities.Brushes;
using Toast.Engine.Entities.Tools;

namespace Toast.Engine.Rendering;

/// <summary>
/// ImGui rendered Debug UI.<br/>
/// This should be used to describe values, change values, etc.
/// </summary>
public static class DebugUI
{
    public static void Open( ref bool open )
    {
        if ( ImGui.Begin( "- Debug Menu -", ref open, ImGuiWindowFlags.NoSavedSettings ) )
        {
            // Set the default window size
            ImGui.SetWindowSize( new Vector2( 750, 500 ) );

            // Entities \\
            if ( ImGui.TreeNodeEx( "Entities", ImGuiTreeNodeFlags.Framed ) )
            {
                List<Entity> entities = EngineManager.currentScene.GetEntities();

                for ( int i = 0; i < entities.Count; i++ )
                {
                    // Create a tree node for every entity
                    if ( ImGui.TreeNodeEx( $"{entities[i]}", ImGuiTreeNodeFlags.Framed ) )
                    {
                        ImGui.Text( $"Type: \"{entities[i].type}\"" );
                        ImGui.Text( $"Alive? {entities[i].IsAlive()}" );
                        ImGui.DragFloat( "Health", ref entities[i].GetHealth() );

                        ImGui.SeparatorText( "Transform" );
                        {
                            ImGui.InputFloat3( "Position", ref entities[i].GetPosition() );
                            ImGui.InputFloat3( "Velocity", ref entities[i].GetVelocity() );
                            ImGui.InputFloat4( "Rotation", ref entities[i].GetRotation() );
                        }

                        //
                        // Show different things depending on different entity types
                        //

                        // Sound entity
                        if ( entities[i] is SoundEntity soundEntity )
                        {
                            ImGui.SeparatorText( "Sound Entity Variables" );

                            ImGui.InputText( "Sound Path", ref soundEntity.audioPath, 2048 );
                            ImGui.InputText( "Sound Alias", ref soundEntity.audioAlias, 2048 );
                            ImGui.SliderFloat( "Sound Volume", ref soundEntity.audioVolume, 0.0f, 1.0f );
                            ImGui.Checkbox( "Sound Repeats", ref soundEntity.audioRepeats );

                            ImGui.Separator();

                            if ( ImGui.Button( "Play Sound" ) )
                            {
                                soundEntity.PlaySound();
                            }

                            if ( ImGui.Button( "Stop Sound" ) )
                            {
                                soundEntity.StopSound();
                            }
                        }

                        // Trigger brush
                        if ( entities[i] is TriggerBrush trigger )
                        {
                            ImGui.SeparatorText( "Trigger Brush Variables" );

                            ImGui.InputText( "Target Entity", ref trigger.targetEntity, 2048 );

                            if ( ImGui.BeginCombo( "Trigger Type", $"{trigger.triggerType}" ) )
                            {
                                if ( ImGui.Selectable( $"{TriggerType.Once}" ) )
                                {
                                    trigger.triggerType = TriggerType.Once;
                                }

                                if ( ImGui.Selectable( $"{TriggerType.Multiple}" ) )
                                {
                                    trigger.triggerType = TriggerType.Multiple;
                                }

                                if ( ImGui.Selectable( $"{TriggerType.Count}" ) )
                                {
                                    trigger.triggerType = TriggerType.Count;
                                }

                                ImGui.EndCombo();
                            }

                            if ( ImGui.BeginCombo( "Trigger By", $"{trigger.triggerBy}" ) )
                            {
                                if ( ImGui.Selectable( $"{TriggerBy.All}" ) )
                                {
                                    trigger.triggerBy = TriggerBy.All;
                                }

                                if ( ImGui.Selectable( $"{TriggerBy.Player}" ) )
                                {
                                    trigger.triggerBy = TriggerBy.Player;
                                }

                                if ( ImGui.Selectable( $"{TriggerBy.NPC}" ) )
                                {
                                    trigger.triggerBy = TriggerBy.NPC;
                                }

                                ImGui.EndCombo();
                            }

                            if ( ImGui.BeginCombo( "Trigger On", $"{trigger.triggerOn}" ) )
                            {
                                if ( ImGui.Selectable( $"{TriggerOn.Trigger}" ) )
                                {
                                    trigger.triggerOn = TriggerOn.Trigger;
                                }

                                if ( ImGui.Selectable( $"{TriggerOn.Enter}" ) )
                                {
                                    trigger.triggerOn = TriggerOn.Enter;
                                }

                                if ( ImGui.Selectable( $"{TriggerOn.Exit}" ) )
                                {
                                    trigger.triggerOn = TriggerOn.Exit;
                                }

                                ImGui.EndCombo();
                            }

                            if ( trigger.triggerType == TriggerType.Count )
                            {
                                ImGui.Separator();

                                ImGui.InputInt( "Max Trigger Count", ref trigger.triggerCount );
                                ImGui.Text( $"Current Trigger Count: {trigger.TriggeredCount()}" );
                            }
                            else if ( trigger.triggerType == TriggerType.Once )
                            {
                                ImGui.Separator();

                                ImGui.Text( $"Has Been Triggered? {trigger.HasBeenTriggered()}" );
                            }

                            ImGui.Separator();

                            byte[] triggererBuffer = new byte[2048];
                            ImGui.InputText( "Entity Triggering This Trigger", triggererBuffer, (uint)triggererBuffer.Length );

                            if ( ImGui.Button( "Trigger" ) )
                            {
                                trigger.OnTrigger(EngineManager.currentScene.FindEntity(Encoding.Default.GetString(triggererBuffer)));
                            }
                        }

                        ImGui.Separator();

                        if ( ImGui.Button( "Kill Entity" ) )
                        {
                            entities[i].OnEvent( EntityEvent.Kill );
                        }

                        if ( ImGui.Button( "Remove Entity" ) )
                        {
                            entities[i].Remove();
                        }

                        ImGui.TreePop();
                    }
                }

                ImGui.TreePop();
            }

            // Debug Commands \\
            if ( ImGui.TreeNodeEx( "Commands", ImGuiTreeNodeFlags.Framed ) )
            {
                if ( ImGui.Button( "Exit Game" ) )
                {
                    Environment.Exit( 255 );
                }

                ImGui.TreePop();
            }

            ImGui.Text( $"Frametime: {1000 / ImGui.GetIO().Framerate:.##}ms" );
            ImGui.Text( $"Framerate: {ImGui.GetIO().Framerate:.#}FPS" );

            ImGui.End();
        }
    }
}