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
	wc.hbrBackground = (HBRUSH)COLOR_WINDOW;
	wc.lpszClassName = (LPCSTR)"ToasterEngineWindow";

	// Register the window class
	RegisterClassExA(&wc);

	// Create our window
	m_window = CreateWindowExA
	(
		NULL, // No special style
		"ToasterEngineWindow", // Class name of the window
		(LPCSTR)pszTitle, // The title viewable on the window
		WS_OVERLAPPEDWINDOW, // Some sorta style
		100, 100, // Position 100, 100
		1280, 720, // 1280x720
		NULL, // No parent
		NULL, // No menu
		hInstance, // Our instance
		NULL // No params
	);

	// Show the window
	ShowWindow(m_window, 1);
}

void Renderer::Update()
{
	// This struct holds our messages
	MSG msg;

	// While we're getting our messages...
	while (GetMessage(&msg, NULL, 0, 0))
	{
		// Translate the message
		TranslateMessage(&msg);

		// Dispatch the message
		DispatchMessage(&msg);
	}
}

bool Renderer::ShuttingDown()
{
	// Is the window closing?
	// This is determined by whether or not the window even exists
	return !m_window;
}

void Renderer::Shutdown()
{
	// Nullify the window
	m_window = NULL;
}

void Renderer::OnKeyDown()
{

}

void Renderer::OnKeyUp()
{

}