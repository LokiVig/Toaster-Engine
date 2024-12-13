#include "Renderer.h"

#define DLL_EXPORT __declspec(dllexport)

extern "C" DLL_EXPORT Renderer* CreateRenderer()
{
    return new Renderer();
}

extern "C" DLL_EXPORT void StartRenderer(Renderer* renderer, HINSTANCE hInstance, int nCmdShow)
{
    if (renderer)
    {
        renderer->WinMain(hInstance, nullptr, nullptr, nCmdShow);
    }
}

extern "C" DLL_EXPORT void RenderFrame(Renderer* renderer, Scene* scene)
{
    if (renderer)
    {
        renderer->Render(scene);
    }
}

extern "C" DLL_EXPORT void DestroyRenderer(Renderer* renderer)
{
    if (renderer)
    {
        renderer->CleanupDevice();
        delete renderer;
    }
}