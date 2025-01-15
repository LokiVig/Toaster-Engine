#pragma once

struct Vertex
{
public:
    Vertex() = default;

    Vertex(vec3 position, vec2 texCoords)
        : m_position(position), m_texCoords(texCoords)
    {
        m_normal = vec3(0, 0, 0);
    }

public:
    vec3 m_position;
    vec3 m_normal;
    vec2 m_texCoords;
};