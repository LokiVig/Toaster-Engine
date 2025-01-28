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

// Toaster Engine
#include "vertex.h"

// Macros
#define SCREEN_WIDTH 1280
#define SCREEN_HEIGHT 720

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
	void InitPipeline(); // Initializes the pipeline
	void InitGraphics(); // Initializes our graphics stuff
	void CleanD3D(); // Closes Direct3D and releases memory

public:
	HWND m_window;

private:
	IDXGISwapChain* m_pSwapChain; // Pointer to our swap chain interface
	ID3D11Device* m_pDev; // Pointer to our Direct3D device interface
	ID3D11DeviceContext* m_pDevCon; // Pointer to our Direct3D device context

	ID3D11RenderTargetView* m_pBackBuffer; // Pointer to our back buffer

	ID3D11InputLayout* m_pLayout; // Pointer to our input layout

	ID3D11Buffer* m_pVBuffer; // Pointer to our vertex buffer

	ID3D11VertexShader* m_pVS; // Pointer to our vertex shader
	ID3D11PixelShader* m_pPS; // Pointer to our pixel shader
};

#endif // _RENDERER_H