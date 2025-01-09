#include "Renderer.h"

extern "C" __declspec(dllexport) Renderer* CreateRenderer(Scene* pScene)
{
    return new Renderer(pScene);
}

extern "C" __declspec(dllexport) void RenderFrame(Renderer* renderer)
{
    if (renderer)
    {
        renderer->Render();
    }
}

extern "C" __declspec(dllexport) void ShutdownRenderer(Renderer* renderer)
{
    if (renderer)
    {
        renderer->Shutdown();
    }
}

extern "C" __declspec(dllexport) bool RendererShuttingDown(Renderer* renderer)
{
    if (renderer)
    {
        return renderer->ShuttingDown();
    }
    else // No renderer means nothing to run with... Faulty!
    {
        printf("RendererShuttingDown(Renderer*): ERROR; No renderer found! Assuming true...\n");
        return true;
    }
}