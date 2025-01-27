#ifndef _RENDERER_H
#define _RENDERER_H

// Windows headers
#include <windows.h>
#include <windowsx.h>

// Direct3D headers
#include <d3d11.h>
#include <d3dx11.h>
#include <d3dx10.h>

// Include the Direct3D library files
#pragma comment (lib, "d3d11.lib")
#pragma comment (lib, "d3dx11.lib")
#pragma comment (lib, "d3dx10.lib")

class Renderer
{
public:
	void Initialize(const char* pszTitle);
	void Update();
	void RenderFrame();
	bool ShuttingDown();
	void Shutdown();

public:
	void OnKeyDown();
	void OnKeyUp();

private:
	void InitD3D(); // Sets up and initializes Direct3D
	void CleanD3D(); // Closes Direct3D and releases memory

public:
	HWND m_window;

private:
	IDXGISwapChain* m_pSwapChain; // Pointer to our swap chain interface
	ID3D11Device* m_pDev; // Pointer to our Direct3D device interface
	ID3D11DeviceContext* m_pDevCon; // Pointer to our Direct3D device context

	ID3D11RenderTargetView* m_pBackBuffer;
};

#endif // _RENDERER_H