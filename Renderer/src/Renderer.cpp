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

    m_pWindow = glfwCreateWindow(WINDOW_WIDTH, WINDOW_HEIGHT, WINDOW_TITLE, nullptr, nullptr);

    if (!m_pWindow)
    {
        printf("Renderer::Initialize(): ERROR; Failed to open GLFW window. If you have an Intel GPU, they are not 3.3 compatible.\n");
        glfwTerminate();
        return;
    }

    glfwMakeContextCurrent(m_pWindow);

    if (glewInit() != GLEW_OK)
    {
        printf("Renderer::Initialize(): ERROR; Failed to initialize GLEW.\n");
        glfwTerminate();
        return;
    }

    glfwSetInputMode(m_pWindow, GLFW_STICKY_KEYS, GL_TRUE);
}

void Renderer::Render()
{
    // Whenever we get told to shut down, call the shutdown method
    if (glfwGetKey(m_pWindow, GLFW_KEY_ESCAPE) == GLFW_PRESS || glfwWindowShouldClose(m_pWindow))
    {
        Shutdown();
        return;
    }
    
    glClear(GL_COLOR_BUFFER_BIT);

    if (m_pScene)
    {
        // for (Brush brush : m_pScene->m_brushes)
        // {
        //     // Draw brushes
        // }
        //
        // for (Entity entity : m_pScene->m_entities)
        // {
        //     // Draw entities
        // }
    }
    
    glfwSwapBuffers(m_pWindow);
    glfwPollEvents();
}

bool Renderer::ShuttingDown()
{
    return m_shuttingDown;
}

void Renderer::Shutdown()
{
    // Ensure the window is destroyed if it exists
    if (m_pWindow)
    {
        glfwDestroyWindow(m_pWindow);
        m_pWindow = nullptr; // Prevent dangling pointer
    }

    // Terminate GLFW to release any allocated resources
    glfwTerminate();
    
    // Tell our C# game that we're shutting down
    m_shuttingDown = true;
    ShuttingDown();
}