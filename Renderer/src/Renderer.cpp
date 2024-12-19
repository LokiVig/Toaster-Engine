#include "Renderer.h"

void Renderer::Initialize()
{
    glewExperimental = true;

    if (glfwInit() == 0)
    {
        (void)fprintf(stderr, "Failed to initialize GLFW\n");
        return;
    }

    glfwWindowHint(GLFW_SAMPLES, 4);
    glfwWindowHint(GLFW_CONTEXT_VERSION_MAJOR, 3);
    glfwWindowHint(GLFW_CONTEXT_VERSION_MINOR, 3);
    glfwWindowHint(GLFW_OPENGL_FORWARD_COMPAT, GL_TRUE);
    glfwWindowHint(GLFW_OPENGL_PROFILE, GLFW_OPENGL_CORE_PROFILE);

    GLFWwindow* window; 
    window = glfwCreateWindow(1280, 720, "Doom.NET", NULL, NULL);
    m_pWindow = window;

    if (!window)
    {
        (void)fprintf(stderr, "Failed to open GLFW window. If you have an Intel GPU, they are not 3.3 compatible.\n");
        glfwTerminate();
        return;
    }

    glfwMakeContextCurrent(window);

    if (glewInit() != GLEW_OK)
    {
        (void)fprintf(stderr, "Failed to initialize GLEW.\n");
        return;
    }

    glfwSetInputMode(window, GLFW_STICKY_KEYS, GL_TRUE);
}

void Renderer::Render()
{
    if (glfwGetKey(m_pWindow, GLFW_KEY_ESCAPE) == GLFW_PRESS || glfwWindowShouldClose(m_pWindow))
    {
        return;
    }
    
    glClear(GL_COLOR_BUFFER_BIT);

    for (Brush brush : m_pScene->m_brushes)
    {
        // Draw brushes
    }

    for (Entity entity : m_pScene->m_entities)
    {
        // Draw entities
    }
    
    glfwSwapBuffers(m_pWindow);
    glfwPollEvents();
}

void Renderer::Shutdown()
{
    
}