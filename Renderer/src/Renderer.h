#pragma once
#pragma comment(lib, "d3d11.lib")
#pragma comment(lib, "D3DCompiler.lib")

#include <vector>
#include <windows.h>
#include <d3d11.h>
#include <d3d11_1.h>
#include <DirectXMath.h>
#include <iostream>

using namespace DirectX;
using namespace std;

struct BBox
{
public:
    BBox() = default;

    BBox(XMFLOAT3 mins, XMFLOAT3 maxs)
        : m_mins(mins), m_maxs(maxs)
    {
    }

public:
    XMFLOAT3 m_mins = XMFLOAT3(-1, -1, -1), m_maxs = XMFLOAT3(1, 1, 1);
};

struct Vertex
{
public:
    Vertex() = default;

    Vertex(XMFLOAT3 pos, XMFLOAT2 tex)
        : m_pos(pos), m_tex(tex)
    {
    }

public:
    XMFLOAT3 m_pos;
    XMFLOAT2 m_tex;
};

class Entity
{
public:
    Entity() = default;

    Entity(XMFLOAT3 pos)
        : m_pos(pos)
    {
    }

    Entity(XMFLOAT3 pos, XMFLOAT4 rot)
        : m_pos(pos), m_rot(rot)
    {
    }

    Entity(XMFLOAT3 pos, XMFLOAT4 rot, string id)
        : m_pos(pos), m_rot(rot), m_id(id)
    {
    }

public:
    XMFLOAT3 m_pos;
    XMFLOAT4 m_rot;
    string m_id;
};

class Brush
{
public:
    Brush() = default;

    Brush(BBox bbox)
        : m_bbox(bbox)
    {
        InitializeVertices();
    }

private:
    void InitializeVertices()
    {
        float minsX = m_bbox.m_mins.x;
        float minsY = m_bbox.m_mins.y;
        float minsZ = m_bbox.m_mins.z;

        float maxsX = m_bbox.m_maxs.x;
        float maxsY = m_bbox.m_maxs.y;
        float maxsZ = m_bbox.m_maxs.z;

        m_vertices[0] = Vertex(XMFLOAT3(maxsX, maxsY, maxsZ), XMFLOAT2(1.0f, 1.0f));
        m_vertices[1] = Vertex(XMFLOAT3(minsX, maxsY, maxsZ), XMFLOAT2(1.0f, 1.0f));
        m_vertices[2] = Vertex(XMFLOAT3(maxsX, maxsY, minsZ), XMFLOAT2(1.0f, 1.0f));
        m_vertices[3] = Vertex(XMFLOAT3(minsX, maxsY, minsZ), XMFLOAT2(1.0f, 1.0f));
        m_vertices[4] = Vertex(XMFLOAT3(maxsX, minsY, maxsZ), XMFLOAT2(1.0f, 1.0f));
        m_vertices[5] = Vertex(XMFLOAT3(minsX, minsY, maxsZ), XMFLOAT2(1.0f, 1.0f));
        m_vertices[6] = Vertex(XMFLOAT3(maxsX, minsY, minsZ), XMFLOAT2(1.0f, 1.0f));
        m_vertices[7] = Vertex(XMFLOAT3(minsX, minsY, minsZ), XMFLOAT2(1.0f, 1.0f));
    }

public:
    BBox m_bbox;
    Vertex m_vertices[8] = {};
};

class Scene
{
public:
    Scene() = default;

public:
    vector<Entity> m_entities;
    vector<Brush> m_brushes;
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
            throw exception("Renderer initialization failed: InitWindow failed!");
        }

        if (FAILED(InitDevice()))
        {
            CleanupDevice();
            throw exception("Renderer initialization failed: InitDevice failed!");
        }

        // Main message loop
        MSG msg = {0};

        while (msg.message != WM_QUIT)
        {
            if (PeekMessage(&msg, nullptr, 0, 0, PM_REMOVE))
            {
                TranslateMessage(&msg);
                DispatchMessage(&msg);
            }
            // else
            // {
            //     Render(m_pScene);
            // }
        }

        CleanupDevice();

        return (int)msg.wParam;
    }

    HRESULT InitWindow(HINSTANCE hInstance, int nCmdShow);
    HRESULT InitDevice();
    HRESULT CompileShaderFromFile(const WCHAR* szFileName, LPCSTR szEntryPoint, LPCSTR szShaderModel,
                                  ID3DBlob** ppBlobOut);
    void CleanupDevice();
    static LRESULT CALLBACK WndProc(HWND, UINT, WPARAM, LPARAM);
    void Render(Scene* scene);

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
    Scene* m_pScene = nullptr;
};
