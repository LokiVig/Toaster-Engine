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
        printf("(Renderer.dll) SetScene(Renderer*, Scene*): ERROR; Input renderer is invalid!\n");
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
        printf("(Renderer.dll) RenderFrame(Renderer*): ERROR; Input renderer is invalid!\n");
        return;
    }
}

extern "C" __declspec(dllexport) void RenderText(Renderer* pRenderer, const char* text, float x, float y, float scale, float r = 1.0f, float g = 1.0f, float b = 1.0f)
{
    if (pRenderer)
    {
        pRenderer->RenderText(text, x, y, scale, r, g, b);
    }
    else
    {
        printf("(Renderer.dll) RenderText(Renderer*, string, int, int): ERROR; Input renderer is invalid!\n");
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
        printf("(Renderer.dll) ShutdownRenderer(Renderer*): ERROR; Input renderer is invalid!\n");
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
        printf("(Renderer.dll) RendererShuttingDown(Renderer*): ERROR; No renderer found! Assuming true...\n");
        return true;
    }
}