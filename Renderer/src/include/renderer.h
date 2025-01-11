#pragma once

// C(++) standard
#include <stdio.h>
#include <stdlib.h>
#include <vector>

// OpenGL
#define GLEW_STATIC
#include <GL/glew.h>

// Helps skip putting a GLEW DLL to Sys32
#pragma comment (lib, "glew32s.lib")

#include <GLFW/glfw3.h>
#include <glm/glm.hpp>

// Namespaces
using namespace std;
using namespace glm;

// Toaster Engine
#include "resources/bitmap.h"
#include "resources/bbox.h"
#include "resources/vertex.h"
#include "resources/entity.h"
#include "resources/brush.h"
#include "resources/scene.h"

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
    GLuint m_fontTexture;

    GLuint VAO, VBO, EBO;
};
