#pragma once

#include "resources/resources.h"

// Window info
#define WINDOW_WIDTH 1280
#define WINDOW_HEIGHT 720
#define WINDOW_TITLE "Toaster Engine - Game"

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

public:
    void RenderText(const char* text, int x, int y); // C#-accessible function to render text onto the screen
    void RenderText3D(const char* text, float x, float y, float z); // C#-accessible function to render text in the 3D world
    
private:
    void RenderBrush(Brush brush);
    void RenderEntity(Entity entity);
    
private:
    Scene* m_pScene;
    GLFWwindow* m_pWindow;
    bool m_shuttingDown;
};
