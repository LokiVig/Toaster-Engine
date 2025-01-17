#include "include/renderer.h"

extern "C" __declspec(dllexport) Renderer* Renderer_Initialize(const char* pszTitle = nullptr)
{
    return new Renderer(pszTitle);
}

extern "C" __declspec(dllexport) void Renderer_SetScene(Renderer* pRenderer, Scene* pScene)
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

extern "C" __declspec(dllexport) void Renderer_RenderFrame(Renderer* pRenderer)
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

extern "C" __declspec(dllexport) void Renderer_DrawText(Renderer* pRenderer, const char* text, float x, float y, float scale, float r = 1.0f, float g = 1.0f, float b = 1.0f)
{
    if (pRenderer)
    {
        pRenderer->DrawText(text, x, y, scale, r, g, b);
    }
    else
    {
        printf("(Renderer.dll) RenderText(Renderer*, string, int, int): ERROR; Input renderer is invalid!\n");
        return;
    }
}

extern "C" __declspec(dllexport) void Renderer_Shutdown(Renderer* pRenderer)
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

extern "C" __declspec(dllexport) bool Renderer_ShuttingDown(Renderer* pRenderer)
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