#ifndef _RENDERER_H
#define _RENDERER_H

#include <Windows.h>
#include <windowsx.h>

class Renderer
{
public:
	void Initialize(const char* pszTitle);
	void Update();
	bool ShuttingDown();
	void Shutdown();

public:
	void OnKeyDown();
	void OnKeyUp();

public:
	HWND m_window;
};

#endif // _RENDERER_H