#pragma once

struct Brush
{
public:
    Brush() = default;

public:
    void GenerateVertices()
    {
        vertices.resize(8);
        
        vertices[0] = Vertex(vec3(bbox.maxs.x, bbox.maxs.y, bbox.maxs.z), vec2(1, 1));
        vertices[1] = Vertex(vec3(bbox.mins.x, bbox.maxs.y, bbox.maxs.z), vec2(1, 1));
        vertices[2] = Vertex(vec3(bbox.maxs.x, bbox.maxs.y, bbox.mins.z), vec2(1, 1));
        vertices[3] = Vertex(vec3(bbox.mins.x, bbox.maxs.y, bbox.mins.z), vec2(1, 1));
        vertices[4] = Vertex(vec3(bbox.maxs.x, bbox.mins.y, bbox.maxs.z), vec2(1, 1));
        vertices[5] = Vertex(vec3(bbox.mins.x, bbox.mins.y, bbox.maxs.z), vec2(1, 1));
        vertices[6] = Vertex(vec3(bbox.maxs.x, bbox.mins.y, bbox.mins.z), vec2(1, 1));
        vertices[7] = Vertex(vec3(bbox.mins.x, bbox.mins.y, bbox.mins.z), vec2(1, 1));
    }

public:
    BBox bbox;
    vector<Vertex> vertices;
};