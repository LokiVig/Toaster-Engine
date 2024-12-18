#pragma once

#include <stdio.h>
#include <stdlib.h>
#include <vector>

#include <GL/glew.h>

#include <GLFW/glfw3.h>

#include <glm/glm.hpp>

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
    GLFWwindow* m_pWindow;
};
