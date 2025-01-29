#include <cstdio>

#include "include/renderer.h"

#define DLLEXPORT extern "C" __declspec(dllexport)

DLLEXPORT Renderer* Initialize(const char* pszTitle)
{
	// Return a new renderer to use
	Renderer* result = new Renderer();
	result->Initialize(pszTitle);
	return result;
}

DLLEXPORT void Update(Renderer* pRenderer)
{
	if (!pRenderer)
	{
		printf("(Renderer.dll) Update: ERROR; pRenderer is nullptr!");
		return;
	}

	// Call the renderer's update method
	// This should draw everything necessary
	pRenderer->Update();
}

DLLEXPORT bool ShuttingDown(Renderer* pRenderer)
{
	if (!pRenderer)
	{
		printf("(Renderer.dll) ShuttingDown: ERROR; pRenderer is nullptr!");
		return true;
	}

	// Get the status of the renderer
	return pRenderer->ShuttingDown();
}

DLLEXPORT void Shutdown(Renderer* pRenderer)
{
	if (!pRenderer)
	{
		printf("(Renderer.dll) Shutdown: ERROR; pRenderer is nullptr!");
		return;
	}

	// Call the renderer's shutdown method, and null it
	pRenderer->Shutdown();
	pRenderer = nullptr;
}