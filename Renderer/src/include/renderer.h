#pragma once

////////////////////////
// C(++) standard
////////////////////////

#include <stdio.h>
#include <stdlib.h>
#include <vector>
#include <map>

////////////////////////
// OpenGL
////////////////////////

#define GLEW_STATIC
#include <GL/glew.h>

// Helps skip putting a GLEW DLL to Sys32
#pragma comment (lib, "glew32s.lib")

#include <GLFW/glfw3.h>
#include <glm/glm.hpp>

////////////////////////
// FreeType
////////////////////////

#include <ft2build.h>
#include FT_FREETYPE_H

////////////////////////
// Namespaces
////////////////////////

using namespace std;
using namespace glm;

////////////////////////
// Toaster Engine
////////////////////////

// Resources
#include "resources/bbox.h"
#include "resources/vertex.h"
#include "resources/entity.h"
#include "resources/brush.h"
#include "resources/scene.h"

// Rendering
#include "rendering/character.h"
#include "rendering/shader.h"

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
    void RenderText(const char* text, float x, float y, float scale, float r, float g, float b); // C#-accessible function to render text onto the screen
    void RenderText3D(const char* pszText, float x, float y, float z); // C#-accessible function to render text in the 3D world
    
private:
    int InitializeFont(const char* filepath, int pixelFontSize);

private:
    void DrawBrush(const Brush brush);
    void DrawEntity(const Entity entity);
    
private:
    // Game variables
    Scene* m_pScene;
    
    // Rendering window variables
    GLFWwindow* m_pWindow;
    const char* m_pszTitle;
    bool m_shuttingDown;

    // Text rendering variables
    FT_Library m_ftLibrary;
    FT_Face m_ftFace;
    map<char, Character> m_characters;

    // Mesh rendering variables
    GLuint VAO, VBO, EBO;
};
