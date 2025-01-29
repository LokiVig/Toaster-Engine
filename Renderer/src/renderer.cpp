#include <cstdio>

#include "include/renderer.h"

//
// DirectX11 needed functions
//

// The WindowProc function prototype
LRESULT CALLBACK WindowProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
	// Sort through the message and check what code we were given
	switch (message)
	{
		// Try not to get scared. Scariest stories.
		// There I was, enjoying my Toaster Engine built game.
		// But then... There it was.
		// The shut down message.
		// "Shut down, NOW", said the evil shut down message.
		// And the application complied.
		// It had *ruined* my savegame!
	case WM_DESTROY:
		PostQuitMessage(0);
		return 0;

		// Get outta here on messages we don't recognize
	default:
		break;
	}

	return DefWindowProc(hWnd, message, wParam, lParam);
}

void Renderer::Initialize(const char* pszTitle)
{
	// Get our instance
	HINSTANCE hInstance = GetModuleHandle(nullptr);

	// Initialize our window class
	WNDCLASSEXA wc;

	// Zero it out...
	ZeroMemory(&wc, sizeof(WNDCLASSEXA));

	// Set important variables
	wc.cbSize = sizeof(WNDCLASSEXA);
	wc.style = CS_HREDRAW | CS_VREDRAW;
	wc.lpfnWndProc = WindowProc;
	wc.hInstance = hInstance;
	wc.hCursor = LoadCursor(NULL, IDC_ARROW);
	wc.lpszClassName = "ToasterEngineWindow";

	// Register the window class
	RegisterClassExA(&wc);

	// Initialize and adjust our client window
	RECT wr = { 0,0, 1280, 720 };
	AdjustWindowRect(&wr, WS_OVERLAPPEDWINDOW, FALSE);

	// Create our window
	m_window = CreateWindowExA
	(
		NULL, // No special style
		"ToasterEngineWindow", // Class name of the window
		pszTitle, // The title viewable on the window
		WS_OVERLAPPEDWINDOW, // Some sorta style
		100, 100, // Position 100, 100
		SCREEN_WIDTH, // 1280
		SCREEN_HEIGHT, // 720
		NULL, // No parent
		NULL, // No menu
		hInstance, // Our instance
		NULL // No params
	);

	// Show the window
	ShowWindow(m_window, 1);

	// Call the function that initializes Direct3D
	InitD3D();
}

void Renderer::InitD3D()
{
	// Create a struct to hold information about the swap chain
	DXGI_SWAP_CHAIN_DESC scd;

	// Clear out the struct for us
	ZeroMemory(&scd, sizeof(DXGI_SWAP_CHAIN_DESC));

	// Fill the swap chain description struct
	scd.BufferCount = 1;								// One back buffer
	scd.BufferDesc.Format = DXGI_FORMAT_R8G8B8A8_UNORM; // Use 32-bit color
	scd.BufferDesc.Width = SCREEN_WIDTH;				// Set the backbuffer width
	scd.BufferDesc.Height = SCREEN_HEIGHT;				// Set the backbuffer height
	scd.BufferUsage = DXGI_USAGE_RENDER_TARGET_OUTPUT;	// How swap chain is to be used
	scd.OutputWindow = m_window;						// The window to be used
	scd.SampleDesc.Count = 4;							// How many multisamples
	scd.Windowed = TRUE;								// Windowed / full-screen mode
	scd.Flags = DXGI_SWAP_CHAIN_FLAG_ALLOW_MODE_SWITCH; // Allow full-screen switching

	// Create a device, device context and swap chain using the information in the scd struct
	D3D11CreateDeviceAndSwapChain
	(
		NULL,
		D3D_DRIVER_TYPE_HARDWARE,
		NULL,
		NULL,
		NULL,
		NULL,
		D3D11_SDK_VERSION,
		&scd,
		&m_pSwapChain,
		&m_pDev,
		NULL,
		&m_pDevCon
	);

	// Get the address of the back buffer
	ID3D11Texture2D* pBackBuffer;
	m_pSwapChain->GetBuffer(0, __uuidof(ID3D11Texture2D), (LPVOID*)&pBackBuffer);

	// Use the back buffer address to create the render target
	m_pDev->CreateRenderTargetView(pBackBuffer, NULL, &m_pBackBuffer);
	pBackBuffer->Release();

	// Set the render target as the back buffer
	m_pDevCon->OMSetRenderTargets(1, &m_pBackBuffer, NULL);

	// Set the viewport
	D3D11_VIEWPORT viewport;
	ZeroMemory(&viewport, sizeof(D3D11_VIEWPORT));

	viewport.TopLeftX = 0;
	viewport.TopLeftY = 0;
	viewport.Width = SCREEN_WIDTH;
	viewport.Height = SCREEN_HEIGHT;

	m_pDevCon->RSSetViewports(1, &viewport);

	InitPipeline(); // Initialize our pipeline
	InitGraphics(); // Initialize our graphics
}

void Renderer::InitPipeline()
{
	// Load and compile the two shaders
	ID3D10Blob* VS, * PS;
	D3DX11CompileFromFile(L"resources/shaders/shaders.shader", 0, 0, "VShader", "vs_4_0", 0, 0, 0, &VS, 0, 0);
	D3DX11CompileFromFile(L"resources/shaders/shaders.shader", 0, 0, "PShader", "ps_4_0", 0, 0, 0, &PS, 0, 0);

	// Encapsulate both shaders into shader objects
	m_pDev->CreateVertexShader(VS->GetBufferPointer(), VS->GetBufferSize(), NULL, &m_pVS);
	m_pDev->CreatePixelShader(PS->GetBufferPointer(), PS->GetBufferSize(), NULL, &m_pPS);

	// Set the shader objects
	m_pDevCon->VSSetShader(m_pVS, 0, 0);
	m_pDevCon->PSSetShader(m_pPS, 0, 0);

	// Create the input elements
	D3D11_INPUT_ELEMENT_DESC ied[] =
	{
		{"POSITION", 0, DXGI_FORMAT_R32G32B32_FLOAT, 0, 0, D3D11_INPUT_PER_VERTEX_DATA, 0},
		{"COLOR", 0, DXGI_FORMAT_R32G32B32A32_FLOAT, 0, 12, D3D11_INPUT_PER_VERTEX_DATA, 0},
	};

	// Create the input layout
	m_pDev->CreateInputLayout(ied, 2, VS->GetBufferPointer(), VS->GetBufferSize(), &m_pLayout);
	m_pDevCon->IASetInputLayout(m_pLayout);
}

void Renderer::InitGraphics()
{
	VERTEX triangle[] =
	{
		{  0.0f,  0.5f, 0.0f, D3DXCOLOR(1.0f, 0.0f, 0.0f, 1.0f)},
		{ 0.45f, -0.5f, 0.0f, D3DXCOLOR(0.0f, 1.0f, 0.0f, 1.0f)},
		{-0.45f, -0.5f, 0.0f, D3DXCOLOR(0.0f, 0.0f, 1.0f, 1.0f)}
	};

	D3D11_BUFFER_DESC bd;
	ZeroMemory(&bd, sizeof(bd));

	bd.Usage = D3D11_USAGE_DYNAMIC;				// Write access access by CPU and GPU
	bd.ByteWidth = sizeof(VERTEX) * 3;			// Size is the VERTEX struct * 3
	bd.BindFlags = D3D11_BIND_VERTEX_BUFFER;	// Use as vertex buffer
	bd.CPUAccessFlags = D3D11_CPU_ACCESS_WRITE; // Allow CPU to write in buffer

	m_pDev->CreateBuffer(&bd, NULL, &m_pVBuffer); // Create the buffer

	// Filling the vertex buffer
	D3D11_MAPPED_SUBRESOURCE ms;
	m_pDevCon->Map(m_pVBuffer, NULL, D3D11_MAP_WRITE_DISCARD, NULL, &ms); // Map the buffer
	memcpy(ms.pData, triangle, sizeof(triangle));														 // Copy the data
	m_pDevCon->Unmap(m_pVBuffer, NULL);																 // Unmap the buffer
}

void Renderer::Update()
{
	// This struct holds our messages
	MSG msg;

	// Check to see if any messages are waiting in the queue
	if (PeekMessage(&msg, NULL, 0, 0, PM_REMOVE))
	{
		// Translate keystroke messages into the right format
		TranslateMessage(&msg);

		// Send the message to the WindowProc function
		DispatchMessage(&msg);

		// Check to see if it's time to quit
		if (msg.message == WM_QUIT)
		{
			m_window = NULL;
		}
	}

	// Call our rendering functions
	RenderFrame();
}

void Renderer::RenderFrame()
{
	// Clear the back buffer to a deep blue
	m_pDevCon->ClearRenderTargetView(m_pBackBuffer, D3DXCOLOR(0.0f, 0.2f, 0.4f, 1.0f));

	// Select which vertex buffer to display
	UINT stride = sizeof(VERTEX);
	UINT offset = 0;
	m_pDevCon->IASetVertexBuffers(0, 1, &m_pVBuffer, &stride, &offset);

	// Select which primitive type we're using
	m_pDevCon->IASetPrimitiveTopology(D3D10_PRIMITIVE_TOPOLOGY_TRIANGLELIST);

	// Draw the vertex buffer to the back buffer
	m_pDevCon->Draw(3, 0);

	// Switch the back buffer and the front buffer
	m_pSwapChain->Present(0, 0);
}

bool Renderer::ShuttingDown()
{
	// Is the window closing?
	// This is determined by whether or not the window even exists
	return !m_window;
}

void Renderer::Shutdown()
{
	// Call our function to clean Direct3D values
	CleanD3D();
}

void Renderer::CleanD3D()
{
	// Switch to windowed mode
	m_pSwapChain->SetFullscreenState(FALSE, NULL);

	// Close and release all existing COM objects
	m_pVS->Release();
	m_pPS->Release();
	m_pSwapChain->Release();
	m_pBackBuffer->Release();
	m_pDev->Release();
	m_pDevCon->Release();
}

void Renderer::OnKeyDown()
{

}

void Renderer::OnKeyUp()
{

}