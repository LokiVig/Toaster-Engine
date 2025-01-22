using System;
using System.Numerics;
using System.Collections.Generic;

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
        open = true;

        if ( ImGui.Begin( "- Debug Menu -", ref open, ImGuiWindowFlags.NoSavedSettings | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoDocking ) )
        {
            // Set the default window size
            ImGui.SetWindowSize( new Vector2( 750, 500 ) );

            // Display framerate / frametime
            ImGui.Text( $"Frametime: {1000 / ImGui.GetIO().Framerate:.##}ms" );
            ImGui.Text( $"Framerate: {ImGui.GetIO().Framerate:.#}FPS" );

            // Entities \\
            if ( ImGui.TreeNodeEx( "Entities", ImGuiTreeNodeFlags.Framed | ImGuiTreeNodeFlags.DefaultOpen ) )
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

                        if ( ImGui.TreeNodeEx( "Transform", ImGuiTreeNodeFlags.DefaultOpen ) )
                        {
                            ImGui.InputFloat3( "Position", ref entities[i].GetPosition() );
                            ImGui.InputFloat3( "Velocity", ref entities[i].GetVelocity() );
                            ImGui.InputFloat4( "Rotation", ref entities[i].GetRotation() );

                            ImGui.SeparatorText( "Bounding Box" );
                            {
                                ImGui.InputFloat3( "Mins", ref entities[i].GetBBox().mins );
                                ImGui.InputFloat3( "Maxs", ref entities[i].GetBBox().maxs );
                            }

                            ImGui.TreePop();
                        }

                        //
                        // Show different things depending on different entity types
                        //

                        // Sound entity
                        if ( entities[i] is SoundEntity soundEntity )
                        {
                            ImGui.SeparatorText( "Sound Entity Variables" );

                            ImGui.InputText( "Sound Path", ref soundEntity.audioPath, 2048 );
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

                            if ( ImGui.BeginCombo( "Trigger Type", $"{trigger.triggerType}", ImGuiComboFlags.WidthFitPreview ) )
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

                            if ( ImGui.BeginCombo( "Trigger By", $"{trigger.triggerBy}", ImGuiComboFlags.WidthFitPreview ) )
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

                            if ( ImGui.BeginCombo( "Trigger On", $"{trigger.triggerOn}", ImGuiComboFlags.WidthFitPreview ) )
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

                            #region ENTITY_EVENTS
                            if ( ImGui.BeginCombo( "Entity Event", $"{trigger.targetEvent}" ) )
                            {
                                if ( ImGui.Selectable( $"{EntityEvent.None}" ) )
                                {
                                    trigger.targetEvent = EntityEvent.None;
                                }

                                if ( ImGui.Selectable( $"{EntityEvent.Kill}" ) )
                                {
                                    trigger.targetEvent = EntityEvent.Kill;
                                }

                                if ( ImGui.Selectable( $"{EntityEvent.Delete}" ) )
                                {
                                    trigger.targetEvent = EntityEvent.Delete;
                                }

                                if ( ImGui.Selectable( $"{EntityEvent.SetHealth}" ) )
                                {
                                    trigger.targetEvent = EntityEvent.SetHealth;
                                }

                                if ( ImGui.Selectable( $"{EntityEvent.TakeDamage}" ) )
                                {
                                    trigger.targetEvent = EntityEvent.TakeDamage;
                                }

                                if ( ImGui.Selectable( $"{EntityEvent.SetPosition}" ) )
                                {
                                    trigger.targetEvent = EntityEvent.SetPosition;
                                }

                                if ( ImGui.Selectable( $"{EntityEvent.SetBBox}" ) )
                                {
                                    trigger.targetEvent = EntityEvent.SetBBox;
                                }

                                if ( ImGui.Selectable( $"{EntityEvent.SpawnEntity}" ) )
                                {
                                    trigger.targetEvent = EntityEvent.SpawnEntity;
                                }

                                if ( ImGui.Selectable( $"{EntityEvent.SetRotation}" ) )
                                {
                                    trigger.targetEvent = EntityEvent.SetRotation;
                                }

                                if ( ImGui.Selectable( $"{EntityEvent.PlaySound}" ) )
                                {
                                    trigger.targetEvent = EntityEvent.PlaySound;
                                }

                                if ( ImGui.Selectable( $"{EntityEvent.StopSound}" ) )
                                {
                                    trigger.targetEvent = EntityEvent.StopSound;
                                }

                                ImGui.EndCombo();
                            }
                            #endregion

                            #region TRIGGER_VALUES
                            ImGui.Separator();

                            ImGui.InputInt( "Int Value", ref trigger.iValue );
                            ImGui.InputFloat( "Float Value", ref trigger.fValue );
                            ImGui.SliderInt( "Bool Value", ref trigger.bValue, -1, 1 );
                            ImGui.InputFloat3( "Vector3 Value", ref trigger.v3Value );
                            ImGui.InputFloat4( "Vector4 Value", ref trigger.v4Value );
                            ImGui.InputFloat3( "Bounding Box Mins", ref trigger.bbValue.mins );
                            ImGui.InputFloat3( "Bounding Box Maxs", ref trigger.bbValue.maxs );
                            #endregion

                            ImGui.Separator();

                            if ( ImGui.Button( "Trigger" ) )
                            {
                                trigger.OnTrigger( null );
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
            if ( ImGui.TreeNodeEx( "Commands", ImGuiTreeNodeFlags.Framed | ImGuiTreeNodeFlags.DefaultOpen ) )
            {
                if ( ImGui.Button( "Open Console" ) )
                {
                    ConsoleUI.Open( ref open );
                }

                if ( ImGui.Button( "Exit Game" ) )
                {
                    EngineManager.EnvironmentShutdown();
                }

                ImGui.TreePop();
            }

            ImGui.End();
        }
    }
}