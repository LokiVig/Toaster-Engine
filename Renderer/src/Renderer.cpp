#include "Renderer.h"

void Renderer::Initialize()
{
    glewExperimental = true;

    if (!glfwInit())
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
}

void Renderer::Render()
{
    
}

void Renderer::Shutdown()
{
    
}