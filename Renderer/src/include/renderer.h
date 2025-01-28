#include "directx.h"

using namespace DirectX;

class Renderer
{
public:

public:
	const LONG m_windowWidth = 1280;
	const LONG m_windowHeight = 720;

	LPCSTR m_windowClassName = "ToasterEngineWindowClass";
	LPCSTR m_windowName = NULL;

	HWND m_windowHandle = NULL;

	const BOOL m_enableVSync = TRUE;

	// Direct3D device and swap chain
	ID3D11Device* m_pDevice = nullptr;
	ID3D11DeviceContext* m_pDeviceContext = nullptr;
	IDXGISwapChain* m_pSwapChain = nullptr;

	// Render target view for the back buffer of the swap chain
	ID3D11RenderTargetView* m_pRenderTargetView = nullptr;

	// Depth / stencil view for use as a depth buffer
	ID3D11DepthStencilView* m_pDepthStencilView = nullptr;

	// A texture to associate to the depth stencil view
	ID3D11Texture2D* m_pDepthStencilBuffer = nullptr;

	// Vertex buffer data
	ID3D11InputLayout* m_pInputLayout = nullptr;
	ID3D11Buffer* m_pVertexBuffer = nullptr;
	ID3D11Buffer* m_pIndexBuffer = nullptr;

	// Shader data
	ID3D11VertexShader* m_pVertexShader = nullptr;
	ID3D11PixelShader* m_pPixelShader = nullptr;

	// Shader resources
	enum ConstantBuffer
	{
		CB_Application,
		CB_Frame,
		CB_Object,
		NumConstantBuffers
	};

	ID3D11Buffer* m_pConstantBuffers[NumConstantBuffers];

	// Define the functionality of the depth / stencil stages
	ID3D11DepthStencilState* m_pDepthStencilState = nullptr;

	// Define the functionality of the rasterizer stage
	ID3D11RasterizerState* m_pRasterizerState = nullptr;
	D3D11_VIEWPORT m_pViewport = { 0 };
};