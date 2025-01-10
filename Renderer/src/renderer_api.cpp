#include "include/renderer.h"

extern "C" __declspec(dllexport) Renderer* CreateRenderer(const char* pszTitle = nullptr)
{
    return new Renderer(pszTitle);
}

extern "C" __declspec(dllexport) void SetScene(Renderer* pRenderer, Scene* pScene)
{
    if (pRenderer)
    {
        pRenderer->SetScene(pScene);
    }
    else
    {
        printf("SetScene(Renderer*, Scene*): ERROR; Input renderer is invalid!\n");
        return;
    }
}

extern "C" __declspec(dllexport) void RenderFrame(Renderer* pRenderer)
{
    if (pRenderer)
    {
        pRenderer->Render();
    }
    else
    {
        printf("RenderFrame(Renderer*): ERROR; Input renderer is invalid!\n");
        return;
    }
}

extern "C" __declspec(dllexport) void RenderText(Renderer* pRenderer, const char* text, int x, int y)
{
    if (pRenderer)
    {
        pRenderer->RenderText(text, x, y);
    }
    else
    {
        printf("RenderText(Renderer*, string, int, int): ERROR; Input renderer is invalid!\n");
        return;
    }
}

extern "C" __declspec(dllexport) void ShutdownRenderer(Renderer* pRenderer)
{
    if (pRenderer)
    {
        pRenderer->Shutdown();
    }
    else
    {
        printf("ShutdownRenderer(Renderer*): ERROR; Input renderer is invalid!\n");
        return;
    }
}

extern "C" __declspec(dllexport) bool RendererShuttingDown(Renderer* pRenderer)
{
    if (pRenderer)
    {
        return pRenderer->ShuttingDown();
    }
    else // No renderer means nothing to run with... Faulty!
    {
        printf("RendererShuttingDown(Renderer*): ERROR; No renderer found! Assuming true...\n");
        return true;
    }
}