#include <cstdio>

#include <renderer.h>

#define DLLEXPORT extern "C" __declspec(dllexport)

DLLEXPORT Renderer* Initialize(const char* pszTitle)
{
	// Return a new renderer to use
	Renderer* result = new Renderer();
	result->Initialize();
	return result;
}

DLLEXPORT void Update(Renderer* pRenderer)
{
	if (!pRenderer)
	{
		printf("(Renderer.dll) Update(Renderer*): ERROR; pRenderer is nullptr!");
		return;
	}

	// Call the renderer's update method
	// This should draw everything necessaryB
	pRenderer->Update();
}

DLLEXPORT void SetFullscreen(Renderer* pRenderer, bool fullscreen)
{
	if (!pRenderer)
	{
		printf("(Renderer.dll) SetFullscreen(Renderer*, bool): ERROR; pRenderer is nullptr!");
	}
}

DLLEXPORT bool ShuttingDown(Renderer* pRenderer)
{
	if (!pRenderer)
	{
		printf("(Renderer.dll) ShuttingDown(Renderer*): ERROR; pRenderer is nullptr!");
		return true;
	}

	// Get the status of the renderer
	return false /*pRenderer->ShuttingDown()*/;
}

DLLEXPORT void Shutdown(Renderer* pRenderer)
{
	if (!pRenderer)
	{
		printf("(Renderer.dll) Shutdown(Renderer*): ERROR; pRenderer is nullptr!");
		return;
	}

	// Call the renderer's shutdown method, and null it
	pRenderer->Shutdown();
	pRenderer = nullptr;
}