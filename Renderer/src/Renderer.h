#pragma once

#define GLFW_INCLUDE_VULKAN
#include <GLFW/glfw3.h>

#define GLM_FORCE_DEPTH_ZERO_TO_ONE
#include <glm/vec2.hpp>
#include <glm/vec3.hpp>
#include <glm/vec4.hpp>
#include <glm/mat4x4.hpp>

#include <iostream>
#include <vector>

using namespace std;
using namespace glm;

#define WINDOW_WIDTH 1280
#define WINDOW_HEIGHT 720

struct BBox
{
public:
    BBox() = default;

public:
    vec3 maxs;
    vec3 mins;
};

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

struct Brush
{
public:
    Brush() = default;

public:
    void GenerateVertices()
    {
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

struct Entity
{
public:
    Entity() = default;

public:
    vec3 pos;
    vec4 rot;
};

class Scene
{
public:
    Scene() = default;

public:
    vector<Brush> m_brushes = vector<Brush>();
    vector<Entity> m_entities = vector<Entity>();
};

class Renderer
{
public:
    Renderer(Scene* pScene)
        : m_pScene(pScene)
    {
        // Call the initialize function, to let us start our renderer
        Initialize();
    }

public:
    void Initialize();
    void Render();
    void Shutdown();

private:
    Scene* m_pScene;
};
