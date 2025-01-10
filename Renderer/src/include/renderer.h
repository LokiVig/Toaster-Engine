#pragma once

#include "resources/resources.h"

// Window info
#define WINDOW_WIDTH 1280
#define WINDOW_HEIGHT 720

class Renderer
{
public:
    Renderer(const char* pszTitle = nullptr)
        : m_pszTitle(pszTitle), m_pWindow(nullptr)
    {
        Initialize();
    }

public:
    void Initialize();
    void SetScene(Scene* pScene);
    void Render();
    void Shutdown();
    bool ShuttingDown();

public:
    void RenderText(const char* pszText, int x, int y); // C#-accessible function to render text onto the screen
    void RenderText3D(const char* pszText, float x, float y, float z); // C#-accessible function to render text in the 3D world
    
private:
    void DrawBrush(const Brush brush);
    void DrawEntity(const Entity entity);
    
private:
    Scene* m_pScene;
    GLFWwindow* m_pWindow;
    const char* m_pszTitle;
    bool m_shuttingDown;

    GLuint VAO, VBO, EBO;
};
