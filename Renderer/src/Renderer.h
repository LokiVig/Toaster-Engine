#pragma once

#include "include/resources.h"

#define WINDOW_WIDTH 1280
#define WINDOW_HEIGHT 720
#define WINDOW_TITLE "Toaster Engine - Game"

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
        : m_pScene(pScene), m_pWindow(nullptr)
    {
        Initialize();
    }

public:
    void Initialize();
    void Render();
    void Shutdown();
    bool ShuttingDown();

private:
    Scene* m_pScene;
    GLFWwindow* m_pWindow;
    bool m_shuttingDown;
};
