using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DoomNET.Entities;

public class EntityConverter : JsonConverter<Entity>
{
    public override Entity Read( ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options )
    {
        using (JsonDocument document = JsonDocument.ParseValue( ref reader ))
        {
            var type = document.RootElement.GetProperty( "type" ).GetString();
            return type switch
            {
                "player" => JsonSerializer.Deserialize<Player>(document.RootElement.GetRawText(), options),
                "testNPC" => JsonSerializer.Deserialize<TestNPC>(document.RootElement.GetRawText(), options),
                "triggerBrush" => JsonSerializer.Deserialize<TriggerBrush>( document.RootElement.GetRawText(), options ),
                _ => throw new NotSupportedException( $"Unknown type: {type}" )
            };
        }
    }

    public override void Write( Utf8JsonWriter writer, Entity value, JsonSerializerOptions options )
    {
        JsonSerializer.Serialize( writer, value, value.GetType(), options );
    }
}