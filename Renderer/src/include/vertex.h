#pragma once

struct Vertex
{
public:
    Vertex() = default;

    Vertex(vec3 pos, vec2 uv)
        : pos(pos), uv(uv)
    {
    }

public:
    vec3 pos;
    vec2 uv;
};