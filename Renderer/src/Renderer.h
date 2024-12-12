#pragma once
#pragma comment(lib, "d3d11.lib")

#include <windows.h>
#include <d3d11.h>
#include <d3d11_1.h>
#include <iostream>

struct Vector2
{
public:
    Vector2()
    {
        m_x = m_y = 1;
    }

    Vector2(float xy)
    {
        m_x = m_y = xy;
    }

    Vector2(float x, float y)
    {
        m_x = x;
        m_y = y;
    }

public:
    float m_x, m_y;
};

struct Vector3
{
public:
    Vector3()
    {
        m_x = m_y = m_z = 1;
    }
    
    Vector3(float xyz)
    {
        m_x = m_y = m_z = xyz;
    }

    Vector3(float x, float y, float z)
    {
        m_x = x;
        m_y = y;
        m_z = z;
    }

public:
    float m_x, m_y, m_z;
};

struct Quaternion
{
public:
    Quaternion(float xyzw)
    {
        m_x = m_y = m_z = m_w = xyzw;
    }

    Quaternion(float x, float y, float z, float w)
    {
        m_x = x;
        m_y = y;
        m_z = z;
        m_w = w;
    }

public:
    float m_x, m_y, m_z, m_w;
};

struct BBox
{
public:
    BBox(Vector3 mins, Vector3 maxs)
        : m_mins(mins), m_maxs(maxs)
    {
    }

public:
    Vector3 m_mins, m_maxs;
};

struct Vertex
{
public:
    Vector3 m_pos;
};

class Entity
{
public:
    Entity(Vector3 pos, Quaternion rot)
        : m_pos(pos), m_rot(rot)
    {
    }

public:
    Vector3 m_pos;
    Quaternion m_rot;
};

class Brush
{
public:
    Brush(BBox bbox)
        : m_bbox(bbox)
    {
        InitializeVertices();
    }

public:
    BBox m_bbox;
    Vertex m_vertices[8] = {};

private:
    void InitializeVertices()
    {
        {
            Vector3(m_bbox.m_maxs.m_x, m_bbox.m_maxs.m_y, m_bbox.m_maxs.m_z), Vector2()
        }
    }
};

class Renderer
{
public:
    INT WINAPI WinMain(_In_ HINSTANCE hInstance, _In_opt_ HINSTANCE hPrevInstance, _In_ LPSTR lpCmdLine,
                       _In_ int nCmdShow)
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
