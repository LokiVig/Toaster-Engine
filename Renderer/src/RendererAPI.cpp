#include "Renderer.h"

#define DLL_EXPORT __declspec(dllexport)

extern "C" DLL_EXPORT Renderer* CreateRenderer(Scene* pScene)
{
    return new Renderer(pScene);
}

extern "C" DLL_EXPORT void RenderFrame(Renderer* renderer)
{
    if (renderer)
    {
        renderer->Render();
    }
}

extern "C" DLL_EXPORT void ShutdownRenderer(Renderer* renderer)
{
    if (renderer)
    {
        renderer->Shutdown();
    }
}