#pragma once

#include <vector>

using namespace std;

struct Vector2
{
public:
    Vector2(float x, float y)
        : x(x), y(y)
    {
    }
    
public:
    float x;
    float y;
};

struct Vector3
{
public:
    Vector3(float x, float y, float z)
        : x(x), y(y), z(z)
    {
    }
    
public:
    float x;
    float y;
    float z;
};

struct Quaternion
{
public:
    Quaternion(float x, float y, float z, float w)
        : x(x), y(y), z(z), w(w)
    {
    }

public:
    float x;
    float y;
    float z;
    float w;
};

struct BBox
{
public:
    BBox() = default;

public:
    Vector3 maxs;
    Vector3 mins;
};

struct Vertex
{
public:
    Vertex() = default;

    Vertex(Vector3 pos, Vector2 uv)
        : pos(pos), uv(uv)
    {
    }

public:
    Vector3 pos;
    Vector2 uv;
};

struct Brush
{
public:
    Brush() = default;

public:
    void GenerateVertices()
    {
        vertices[0] = Vertex(Vector3(bbox.maxs.x, bbox.maxs.y, bbox.maxs.z), Vector2(1, 1));
        vertices[1] = Vertex(Vector3(bbox.mins.x, bbox.maxs.y, bbox.maxs.z), Vector2(1, 1));
        vertices[2] = Vertex(Vector3(bbox.maxs.x, bbox.maxs.y, bbox.mins.z), Vector2(1, 1));
        vertices[3] = Vertex(Vector3(bbox.mins.x, bbox.maxs.y, bbox.mins.z), Vector2(1, 1));
        vertices[4] = Vertex(Vector3(bbox.maxs.x, bbox.mins.y, bbox.maxs.z), Vector2(1, 1));
        vertices[5] = Vertex(Vector3(bbox.mins.x, bbox.mins.y, bbox.maxs.z), Vector2(1, 1));
        vertices[6] = Vertex(Vector3(bbox.maxs.x, bbox.mins.y, bbox.mins.z), Vector2(1, 1));
        vertices[7] = Vertex(Vector3(bbox.mins.x, bbox.mins.y, bbox.mins.z), Vector2(1, 1));
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
    Vector3 pos;
    Quaternion rot;
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
