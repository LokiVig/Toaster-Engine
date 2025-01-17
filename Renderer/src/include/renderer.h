#pragma once

////////////////////////
// C++ standard
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

// Helps skip putting a GLEW DLL in Sys32
#pragma comment (lib, "glew32s.lib")

#include <GLFW/glfw3.h>
#include <glm/glm.hpp>
#include <glm/gtc/matrix_transform.hpp>

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
#include "rendering/camera.h"

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
    void DrawText(const char* text, float x, float y, float scale, float r, float g, float b); // C#-accessible function to render text onto the screen
    void RenderText3D(const char* pszText, float x, float y, float z); // C#-accessible function to render text in the 3D world

public:
    bool KeyDown(int key);

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

    // General rendering variables
    Camera* m_pCamera;

    // Text rendering variables
    FT_Library m_ftLibrary{ nullptr };
    FT_Face m_ftFace{ nullptr };
    Shader* m_txtShader;
    map<char, Character> m_characters;

    // Mesh rendering variables
    GLuint VAO, VBO, EBO;
};
