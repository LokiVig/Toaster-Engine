#include "Renderer.h"
#include <d3dcompiler.h>
#include <DirectXColors.h>

HRESULT Renderer::InitWindow(HINSTANCE hInstance, int nCmdShow)
{
    std::cout << "Renderer::InitWindow(): Initializing window...\n";

    // Register class
    WNDCLASSEX wcex;
    wcex.cbSize = sizeof(WNDCLASSEX);
    wcex.style = CS_HREDRAW | CS_VREDRAW;
    wcex.lpfnWndProc = WndProc;
    wcex.cbClsExtra = 0;
    wcex.cbWndExtra = 0;
    wcex.hInstance = hInstance;
    wcex.hIcon = LoadIcon(hInstance, (LPCTSTR)IDI_APPLICATION);
    wcex.hCursor = LoadCursor(NULL, IDC_ARROW);
    wcex.hbrBackground = (HBRUSH)(COLOR_WINDOW + 1);
    wcex.lpszMenuName = L"Whag";
    wcex.lpszClassName = L"DoomNETWindowClass";
    wcex.hIconSm = LoadIcon(wcex.hInstance, IDI_APPLICATION);

    if (!RegisterClassEx(&wcex))
    {
        MessageBox(nullptr, L"Encountered an error registering the window class!", L"Error", MB_OK);
        return E_FAIL;
    }

    // Create window
    m_hInstance = hInstance;
    RECT rc = {0, 0, 1280, 720};
    AdjustWindowRect(&rc, WS_OVERLAPPEDWINDOW, FALSE);
    m_hWnd = CreateWindow(L"DoomNETWindowClass", L"Doom.NET", WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_MINIMIZEBOX,
                          CW_USEDEFAULT, CW_USEDEFAULT, rc.right - rc.left, rc.bottom - rc.top, nullptr, nullptr,
                          hInstance, nullptr);

    if (!m_hWnd)
    {
        MessageBox(nullptr, L"Encountered an error creating the window handle!", L"Error", MB_OK);
        return E_FAIL;
    }

    ShowWindow(m_hWnd, nCmdShow);

    std::cout << "Renderer::InitWindow(): Successfully initialized a window!\n\n";
    return S_OK;
}

LRESULT CALLBACK Renderer::WndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
    PAINTSTRUCT ps;
    HDC hdc;

    switch (message)
    {
    case WM_PAINT:
        hdc = BeginPaint(hWnd, &ps);
        EndPaint(hWnd, &ps);
        break;

    case WM_DESTROY:
        PostQuitMessage(0);
        break;

    default:
        return DefWindowProc(hWnd, message, wParam, lParam);
    }

    return 0;
}

HRESULT Renderer::InitDevice()
{
    HRESULT hr = S_OK;

    RECT rc;
    GetClientRect(m_hWnd, &rc);
    UINT width = rc.right - rc.left;
    UINT height = rc.bottom - rc.top;
    UINT createDeviceFlags = 0;
#ifdef _DEBUG
    createDeviceFlags |= D3D11_CREATE_DEVICE_DEBUG;
#endif // _DEBUG

    D3D_DRIVER_TYPE driverTypes[] =
    {
        D3D_DRIVER_TYPE_HARDWARE,
        D3D_DRIVER_TYPE_WARP,
        D3D_DRIVER_TYPE_REFERENCE,
    };
    UINT numDriverTypes = ARRAYSIZE(driverTypes);

    D3D_FEATURE_LEVEL featureLevels[] =
    {
        D3D_FEATURE_LEVEL_11_1,
        D3D_FEATURE_LEVEL_11_0,
        D3D_FEATURE_LEVEL_10_1,
        D3D_FEATURE_LEVEL_10_0,
    };
    UINT numFeatureLevels = ARRAYSIZE(featureLevels);

    std::cout << "Renderer::InitDevice(): Initializing devices...\n";

    for (UINT driverTypeIndex = 0; driverTypeIndex < numDriverTypes; driverTypeIndex++)
    {
        m_driverType = driverTypes[driverTypeIndex];
        hr = D3D11CreateDevice(nullptr, m_driverType, nullptr, createDeviceFlags, featureLevels, numFeatureLevels,
                               D3D11_SDK_VERSION, &m_pDevice, &m_featureLevel, &m_pImmediateContext);

        if (hr == E_INVALIDARG)
        {
            // DirectX 11.0 platforms will not recognize D3D_FEATURE_LEVEL_11_1 so we need to retry without it
            hr = D3D11CreateDevice(nullptr, m_driverType, nullptr, createDeviceFlags, &featureLevels[1],
                                   numFeatureLevels - 1,
                                   D3D11_SDK_VERSION, &m_pDevice, &m_featureLevel, &m_pImmediateContext);
        }

        if (SUCCEEDED(hr))
        {
            std::cout << "Renderer::InitDevice(): Successfully created devices needed for every driver type!\n";
            break;
        }
    }

    if (FAILED(hr))
    {
        MessageBox(nullptr, L"Encountered an error creating the necessary devices!", L"Error", MB_OK);
        return hr;
    }

    // Obtain DXGI factory from device (since we used nullptr for pAdapter above)
    IDXGIFactory* dxgiFactory = nullptr;
    {
        IDXGIDevice* dxgiDevice = nullptr;
        hr = m_pDevice->QueryInterface(__uuidof(IDXGIDevice), reinterpret_cast<void**>(&dxgiDevice));

        if (SUCCEEDED(hr))
        {
            IDXGIAdapter* adapter = nullptr;
            hr = dxgiDevice->GetAdapter(&adapter);

            if (SUCCEEDED(hr))
            {
                hr = adapter->GetParent(__uuidof(IDXGIFactory1), reinterpret_cast<void**>(&dxgiFactory));
                adapter->Release();
            }

            dxgiDevice->Release();
        }
    }

    if (FAILED(hr))
    {
        MessageBox(nullptr, L"Encountered an error creating factory!", L"Error", MB_OK);
        return hr;
    }

    std::cout << "Renderer::InitDevice(): Successfully created factory!\n";

    // Create swap chain
    IDXGIFactory2* dxgiFactory2 = nullptr;
    hr = dxgiFactory->QueryInterface(__uuidof(IDXGIFactory2), reinterpret_cast<void**>(&dxgiFactory2));

    if (dxgiFactory2)
    {
        // DirectX 11.1 or later
        hr = m_pDevice->QueryInterface(__uuidof(ID3D11Device1), reinterpret_cast<void**>(&m_pDevice1));

        if (SUCCEEDED(hr))
        {
            (void)m_pImmediateContext->QueryInterface(__uuidof(ID3D11DeviceContext1),
                                                      reinterpret_cast<void**>(&m_pImmediateContext1));
        }

        DXGI_SWAP_CHAIN_DESC1 sd = {};
        sd.Width = width;
        sd.Height = height;
        sd.Format = DXGI_FORMAT_R8G8B8A8_UNORM;
        sd.SampleDesc.Count = 1;
        sd.SampleDesc.Quality = 0;
        sd.BufferUsage = DXGI_USAGE_RENDER_TARGET_OUTPUT;
        sd.BufferCount = 1;

        hr = dxgiFactory2->CreateSwapChainForHwnd(m_pDevice, m_hWnd, &sd, nullptr, nullptr, &m_pSwapChain1);

        if (SUCCEEDED(hr))
        {
            hr = m_pSwapChain1->QueryInterface(__uuidof(IDXGISwapChain), reinterpret_cast<void**>(&m_pSwapChain));
        }

        dxgiFactory2->Release();
    }
    else
    {
        // DirectX 11.0 systems
        DXGI_SWAP_CHAIN_DESC sd = {};
        sd.BufferCount = 1;
        sd.BufferDesc.Width = width;
        sd.BufferDesc.Height = height;
        sd.BufferDesc.Format = DXGI_FORMAT_R8G8B8A8_UNORM;
        sd.BufferDesc.RefreshRate.Numerator = 60;
        sd.BufferDesc.RefreshRate.Denominator = 1;
        sd.BufferUsage = DXGI_USAGE_RENDER_TARGET_OUTPUT;
        sd.OutputWindow = m_hWnd;
        sd.SampleDesc.Count = 1;
        sd.SampleDesc.Quality = 0;
        sd.Windowed = TRUE;

        hr = dxgiFactory->CreateSwapChain(m_pDevice, &sd, &m_pSwapChain);
    }

    dxgiFactory->MakeWindowAssociation(m_hWnd, DXGI_MWA_NO_ALT_ENTER);

    dxgiFactory->Release();

    if (FAILED(hr))
    {
        MessageBox(nullptr, L"Encountered an error creating the swap chain!", L"Error", MB_OK);
        return hr;
    }

    std::cout << "Renderer::InitDevice(): Successfully created swap chain!\n";

    // Create a render target view
    ID3D11Texture2D* pBackBuffer = nullptr;
    hr = m_pSwapChain->GetBuffer(0, __uuidof(ID3D11Texture2D), reinterpret_cast<void**>(&pBackBuffer));

    if (FAILED(hr))
    {
        MessageBox(nullptr, L"Encountered an error creating the back buffer!", L"Error", MB_OK);
        return hr;
    }

    std::cout << "Renderer::InitDevice(): Successfully created back buffer!\n";

    hr = m_pDevice->CreateRenderTargetView(pBackBuffer, nullptr, &m_pRenderTargetView);
    pBackBuffer->Release();

    if (FAILED(hr))
    {
        MessageBox(nullptr, L"Encountered an error creating the render target!", L"Error", MB_OK);
        return hr;
    }

    std::cout << "Renderer::InitDevice(): Successfully created render target!\n";

    m_pImmediateContext->OMSetRenderTargets(1, &m_pRenderTargetView, nullptr);

    // Set up the viewport
    D3D11_VIEWPORT vp;
    vp.Width = (FLOAT)width;
    vp.Height = (FLOAT)height;
    vp.MinDepth = 0.0f;
    vp.MaxDepth = 1.0f;
    vp.TopLeftX = 0;
    vp.TopLeftY = 0;
    m_pImmediateContext->RSSetViewports(1, &vp);

    std::cout << "Renderer::InitDevice(): Successfully created viewport!\n";

    // Compile the vertex shader
    ID3DBlob* pVSBlob = nullptr;
    hr = CompileShaderFromFile(L"base.fxh", "VS", "vs_4_0", &pVSBlob);

    if (FAILED(hr))
    {
        MessageBox(nullptr,
                   L"The FX file cannot be compiled. Please run the game from the directory that contains the FX file.",
                   L"Error", MB_OK);
        return hr;
    }

    std::cout << "Renderer::InitDevice(): Successfully initialized all devices!\n\n";

    return S_OK;
}

void Renderer::Render(Scene* scene)
{
    if (scene)
    {
        // Set our scene correctly
        m_pScene = scene;
    }

    m_pImmediateContext->ClearRenderTargetView(m_pRenderTargetView, DirectX::Colors::MidnightBlue);
    m_pSwapChain->Present(0, 0);
}

void Renderer::CleanupDevice()
{
    if (m_pImmediateContext)
    {
        m_pImmediateContext->ClearState();
    }

    if (m_pRenderTargetView)
    {
        m_pRenderTargetView->Release();
    }

    if (m_pSwapChain)
    {
        m_pSwapChain->Release();
    }

    if (m_pSwapChain1)
    {
        m_pSwapChain1->Release();
    }

    if (m_pImmediateContext)
    {
        m_pImmediateContext->Release();
    }

    if (m_pImmediateContext1)
    {
        m_pImmediateContext1->Release();
    }

    if (m_pDevice)
    {
        m_pDevice->Release();
    }

    if (m_pDevice1)
    {
        m_pDevice1->Release();
    }
}

HRESULT Renderer::CompileShaderFromFile(const WCHAR* szFileName, LPCSTR szEntryPoint, LPCSTR szShaderModel,
                                        ID3DBlob** ppBlobOut)
{
    HRESULT hr = S_OK;

    DWORD dwShaderFlags = D3DCOMPILE_ENABLE_STRICTNESS;

#ifdef _DEBUG
    // Set the D3DCOMPILE_DEBUG flag to embed debug information in the shaders.
    // Setting this flag improves the shader debugging experience, but still allows 
    // the shaders to be optimized and to run exactly the way they will run in 
    // the release configuration of this program
    dwShaderFlags |= D3DCOMPILE_DEBUG;

    // Disable optimizations to further improve shader debugging
    dwShaderFlags |= D3DCOMPILE_SKIP_OPTIMIZATION;
#endif // _DEBUG

    ID3DBlob* pErrorBlob = nullptr;
    hr = D3DCompileFromFile(szFileName, nullptr, nullptr, szEntryPoint, szShaderModel, dwShaderFlags, 0, ppBlobOut,
                            &pErrorBlob);

    if (FAILED(hr))
    {
        if (pErrorBlob)
        {
            OutputDebugStringA(reinterpret_cast<const char*>(pErrorBlob->GetBufferPointer()));
            pErrorBlob->Release();
        }

        return hr;
    }

    if (pErrorBlob)
    {
        pErrorBlob->Release();
    }

    return S_OK;
}
