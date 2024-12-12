#pragma once
#pragma comment(lib, "d3d11.lib")

#include <windows.h>
#include <d3d11.h>
#include <d3d11_1.h>

class Renderer
{
public:
    INT WINAPI WinMain(_In_ HINSTANCE hInstance, _In_opt_ HINSTANCE hPrevInstance, _In_ LPSTR lpCmdLine, _In_ int nCmdShow)
    {
        UNREFERENCED_PARAMETER(hPrevInstance);
        UNREFERENCED_PARAMETER(lpCmdLine);

        if (FAILED(InitWindow(hInstance, nCmdShow)))
        {
            return 0;
        }

        if (FAILED(InitDevice()))
        {
            CleanupDevice();
            return 0;
        }

        // Main message loop
        MSG msg = {0};

        while (WM_QUIT != msg.message)
        {
            if (PeekMessage(&msg, nullptr, 0, 0, PM_REMOVE))
            {
                TranslateMessage(&msg);
                DispatchMessage(&msg);
            }
            else
            {
                Render();
            }
        }

        CleanupDevice();

        return (int)msg.wParam;
    }
    
    HRESULT InitWindow(HINSTANCE hInstance, int nCmdShow);
    HRESULT InitDevice();
    void CleanupDevice();
    static LRESULT CALLBACK WndProc(HWND, UINT, WPARAM, LPARAM);
    void Render();
    
private:
    HINSTANCE m_hInstance = nullptr;
    HWND m_hWnd = nullptr;
    D3D_DRIVER_TYPE m_driverType = D3D_DRIVER_TYPE_NULL;
    D3D_FEATURE_LEVEL m_featureLevel = D3D_FEATURE_LEVEL_11_0;
    ID3D11Device* m_pDevice = nullptr;
    ID3D11Device1* m_pDevice1 = nullptr;
    ID3D11DeviceContext* m_pImmediateContext = nullptr;
    ID3D11DeviceContext1* m_pImmediateContext1 = nullptr;
    IDXGISwapChain* m_pSwapChain = nullptr;
    IDXGISwapChain1* m_pSwapChain1 = nullptr;
    ID3D11RenderTargetView* m_pRenderTargetView = nullptr;
};