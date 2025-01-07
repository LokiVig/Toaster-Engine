#include "Renderer.h"

// DEBUG
extern "C" __declspec(dllexport) int DebugFunction()
{
    return 42;
}

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