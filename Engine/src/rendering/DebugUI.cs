using System.Numerics;
using System.Collections.Generic;

using ImGuiNET;

using Toast.Engine.Entities;
using Toast.Engine.Resources;
using Toast.Engine.Entities.Brushes;
using Toast.Engine.Entities.Tools;
using System;

namespace Toast.Engine.Rendering;

/// <summary>
/// ImGui rendered Debug UI.<br/>
/// This should be used to describe values, change values, etc.
/// </summary>
public static class DebugUI
{
    public static void Display( ref bool open )
    {
        open = true;

        if ( ImGui.Begin( "- Debug Menu -", ref open, ImGuiWindowFlags.NoSavedSettings | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoDocking ) )
        {
            // Set the default window size
            ImGui.SetWindowSize( new Vector2( 750, 500 ) );

            // Display framerate / frametime
            ImGui.Text( $"Frametime: {1000 / ImGui.GetIO().Framerate:.##}ms" );
            ImGui.Text( $"Framerate: {ImGui.GetIO().Framerate:.#}FPS" );

            ImGui.Separator();

            // Display map information
            ImGui.Text( $"Active map: \"{EngineManager.currentFile?.path ?? "N/A"}\"" );

            // Are we CHEATING?!
            ImGui.Text( $"Cheats enabled? {EngineManager.cheatsEnabled}" );

            ImGui.Separator();

            // Entities \\
            if ( ImGui.TreeNodeEx( "Entities", ImGuiTreeNodeFlags.Framed | ImGuiTreeNodeFlags.DefaultOpen ) )
            {
                List<Entity> entities = EngineManager.currentScene?.GetEntities();

                for ( int i = 0; i < entities?.Count; i++ )
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
                            //ImGui.InputFloat4( "Rotation", entities[i].GetRotation().AsVector4() );

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

                        // Entity spawner
                        if ( entities[i] is EntitySpawner<Entity> spawner)
                        {
                            ImGui.SeparatorText( "Entity Spawner Variables" );

                            if ( ImGui.Button( "Spawn Entity" ) )
                            {
                                spawner.SpawnEntity();
                            }
                        }

                        // Sound entity
                        if ( entities[i] is SoundEntity sound )
                        {
                            ImGui.SeparatorText( "Sound Entity Variables" );

                            ImGui.InputText( "Sound Path", ref sound.audioPath, 2048 );
                            ImGui.SliderFloat( "Sound Volume", ref sound.audioVolume, 0.0f, 1.0f );
                            ImGui.Checkbox( "Sound Repeats", ref sound.audioRepeats );

                            ImGui.Separator();

                            if ( ImGui.Button( "Play Sound" ) )
                            {
                                sound.PlaySound();
                            }

                            if ( ImGui.Button( "Stop Sound" ) )
                            {
                                sound.StopSound();
                            }
                        }

                        // Trigger brush
                        if ( entities[i] is TriggerBrush trigger )
                        {
                            ImGui.SeparatorText( "Trigger Brush Variables" );

                            ImGui.Text( $"Last trigger entity: {trigger.previousTriggerEntity}" );

                            ImGui.Separator();

                            ImGui.InputText( "Target Entity", ref trigger.targetEntity, 2048 );

                            if ( ImGui.BeginCombo( "Trigger Type", $"{trigger.triggerType}", ImGuiComboFlags.WidthFitPreview ) )
                            {
                                TriggerType[] triggerTypes = Enum.GetValues<TriggerType>();

                                for ( int j = 0; j < triggerTypes.Length; j++ )
                                {
                                    if ( ImGui.Selectable( triggerTypes[j].ToString() ) )
                                    {
                                        trigger.triggerType = triggerTypes[j];
                                    }
                                }

                                ImGui.EndCombo();
                            }

                            if ( ImGui.BeginCombo( "Trigger By", $"{trigger.triggerBy}", ImGuiComboFlags.WidthFitPreview ) )
                            {
                                TriggerBy[] triggerBys = Enum.GetValues<TriggerBy>();

                                for ( int j = 0; j < triggerBys.Length; j++ )
                                {
                                    if ( ImGui.Selectable( triggerBys[j].ToString() ) )
                                    {
                                        trigger.triggerBy = triggerBys[j];
                                    }
                                }

                                ImGui.EndCombo();
                            }

                            if ( ImGui.BeginCombo( "Trigger On", $"{trigger.triggerOn}", ImGuiComboFlags.WidthFitPreview ) )
                            {
                                TriggerOn[] triggerOns = Enum.GetValues<TriggerOn>();

                                for ( int j = 0; j < triggerOns.Length; j++ )
                                {
                                    if ( ImGui.Selectable( triggerOns[j].ToString() ) )
                                    {
                                        trigger.triggerOn = triggerOns[j];
                                    }
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

                            if ( ImGui.BeginCombo( "Entity Event", $"{trigger.targetEvent}", ImGuiComboFlags.WidthFitPreview ) )
                            {
                                EntityEvent[] entityEvents = Enum.GetValues<EntityEvent>();

                                for ( int j = 0; j < entityEvents.Length; j++ )
                                {
                                    if ( ImGui.Selectable( entityEvents[j].ToString() ) )
                                    {
                                        trigger.targetEvent = entityEvents[j];
                                    }
                                }

                                ImGui.EndCombo();
                            }

                            #region TRIGGER_VALUES
                            ImGui.Separator();

                            ImGui.InputInt( "Int Value", ref trigger.iValue );
                            ImGui.InputFloat( "Float Value", ref trigger.fValue );
                            ImGui.SliderInt( "Bool Value", ref trigger.bValue, -1, 1 );
                            ImGui.InputFloat3( "Vector3 Value", ref trigger.v3Value );
                            //ImGui.InputFloat4( "Quaternion Value", ref trigger.qValue );
                            ImGui.InputFloat3( "Bounding Box Mins", ref trigger.bbValue.mins );
                            ImGui.InputFloat3( "Bounding Box Maxs", ref trigger.bbValue.maxs );
                            #endregion

                            ImGui.Separator();

                            if ( ImGui.Button( "Trigger" ) )
                            {
                                Log.Warning( "TODO: Implementation is to be done!" );
                                //trigger.OnTrigger( EngineManager.currentScene.GetPlayer() );
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
                if ( ImGui.Button( "Shutdown" ) )
                {
                    EngineManager.EngineShutdown();
                }

                ImGui.TreePop();
            }

            ImGui.End();
        }
    }
}