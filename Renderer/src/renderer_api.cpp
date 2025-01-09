#include "include/renderer.h"

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
    else
    {
        printf("RenderFrame(Renderer*): ERROR; Input renderer is invalid!\n");
    }
}

extern "C" __declspec(dllexport) void RenderText(Renderer* renderer, const char* text, int x, int y)
{
    if (renderer)
    {
        renderer->RenderText(text, x, y);
    }
    else
    {
        printf("RenderText(Renderer*, string, int, int): ERROR; Input renderer is invalid!\n");
    }
}

extern "C" __declspec(dllexport) void ShutdownRenderer(Renderer* renderer)
{
    if (renderer)
    {
        renderer->Shutdown();
    }
    else
    {
        printf("ShutdownRenderer(Renderer*): ERROR; Input renderer is invalid!\n");
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