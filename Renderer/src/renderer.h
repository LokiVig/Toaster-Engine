#pragma once

#include <string>
#include <wrl/client.h>
#include <d3d12.h>
#include <dxgi.h>
#include <dxgi1_4.h>
#include <d3d12shader.h>
#include <d3dcompiler.h>>

using namespace Microsoft::WRL;

#ifdef BUILD_DLL
#define DLL_EXPORT __declspec(dllexport)
#else
#define DLL_EXPORT __declspec(dllimport)
#endif // BUILD_DLL

struct Vector2
{
public:
    Vector2()
    {
        x = y = 0;
    }

    Vector2(float x, float y)
    {
        this->x = x;
        this->y = y;
    }

public:
    float x;
    float y;
};

struct Vector3
{
public:
    Vector3()
    {
        x = y = z = 0;
    }

    Vector3(float x, float y, float z)
    {
        this->x = x;
        this->y = y;
        this->z = z;
    }

public:
    float x, y, z;
};

struct Quaternion
{
public:
    Quaternion()
    {
        x = y = z = w = 0;
    }

    Quaternion(float x, float y, float z, float w)
    {
        this->x = x;
        this->y = y;
        this->z = z;
        this->w = w;
    }

public:
    float x, y, z, w;
};

struct BBox
{
public:
    BBox()
    {
        mins = {-1, -1, -1};
        maxs = {1, 1, 1};
    }

    BBox(Vector3 mins, Vector3 maxs)
    {
        this->mins = mins;
        this->maxs = maxs;
    }

public:
    Vector3 mins;
    Vector3 maxs;
};

struct Vertex
{
    Vector3 pos;
    float texX, texY;
};

class Entity
{
public:
    Entity()
    {
        m_pos = Vector3();
        m_rot = Quaternion();
        m_box = BBox();
    }

    Entity(Vector3 pos)
    {
        m_pos = pos;
        m_rot = Quaternion();
        m_box = BBox();
    }

    Entity(Vector3 pos, Quaternion rot)
    {
        m_pos = pos;
        m_rot = rot;
        m_box = BBox();
    }

    Entity(Vector3 pos, Quaternion rot, BBox box)
    {
        m_pos = pos;
        m_rot = rot;
        m_box = box;
    }

public:
    Vector3 m_pos;
    Quaternion m_rot;
    BBox m_box;
};

class Brush
{
    float posX, posY, posZ;
    float boxX, boxY, boxZ;
    Vertex vertices[8];
};

class Scene
{
    Entity* entities;
    Brush* brushes;
};

class Renderer
{
private:
#define FRAMECOUNT 2

    // Game-wise variables
    Scene* m_scene;

    // Pipeline objects
    D3D12_VIEWPORT m_viewport;
    D3D12_RECT m_scissorRect;
    ComPtr<IDXGISwapChain> m_swapChain;
    ComPtr<ID3D12Device> m_device;
    ComPtr<ID3D12Resource> m_renderTargets[FRAMECOUNT];
    ComPtr<ID3D12CommandAllocator> m_commandAllocator;
    ComPtr<ID3D12CommandQueue> m_commandQueue;
    ComPtr<ID3D12RootSignature> m_rootSignature;
    ComPtr<ID3D12PipelineState> m_pipelineState;
    ComPtr<ID3D12GraphicsCommandList> m_commandList;
    UINT m_rtvDescriptorSize;

    // App resources
    ID3D12Resource* m_vertexBuffer;
    D3D12_VERTEX_BUFFER_VIEW m_vertexBufferView;

    // Synchronization objects
    UINT m_frameIndex;
    HANDLE m_fenceEvent;
    ID3D12Fence* m_fence;
    UINT64 m_fenceValue;

private:
    void LoadPipeline() // Loads all of the important pipeline items.
    {
#if defined(_DEBUG)
        // Enable the D3D12 debug layer.
        {
            ID3D12Debug* debugController;
            if (SUCCEEDED(D3D12GetDebugInterface(IID_PPV_ARGS(&debugController))))
            {
                debugController->EnableDebugLayer();
            }
        }
#endif // _DEBUG

        IDXGIFactory4* factory;
        ThrowIfFailed(CreateDXGIFactory1(IID_PPV_ARGS(&factory)));

        if (m_useWarpDevice)
        {
            IDXGIAdapter* warpAdapter;
            ThrowIfFailed(factory->EnumWarpAdapter(IID_PPV_ARGS(&warpAdapter)));

            ThrowIfFailed(D3D12CreateDevice(
                warpAdapter.Get(),
                D3D_FEATURE_LEVEL_11_0,
                IID_PPV_ARGS(&m_device)
            ));
        }
        else
        {
            ComPtr<IDXGIAdapter1> hardwareAdapter;
            GetHardwareAdapter(factory.Get(), &hardwareAdapter);

            ThrowIfFailed(D3D12CreateDevice(
                hardwareAdapter.Get(),
                D3D_FEATURE_LEVEL_11_0,
                IID_PPV_ARGS(&m_device)
            ));
        }

        // Describe and create the command queue.
        D3D12_COMMAND_QUEUE_DESC queueDesc = {};
        queueDesc.Flags = D3D12_COMMAND_QUEUE_FLAG_NONE;
        queueDesc.Type = D3D12_COMMAND_LIST_TYPE_DIRECT;

        ThrowIfFailed(m_device->CreateCommandQueue(&queueDesc, IID_PPV_ARGS(&m_commandQueue)));

        // Describe and create the swap chain.
        DXGI_SWAP_CHAIN_DESC swapChainDesc = {};
        swapChainDesc.BufferCount = FrameCount;
        swapChainDesc.BufferDesc.Width = m_width;
        swapChainDesc.BufferDesc.Height = m_height;
        swapChainDesc.BufferDesc.Format = DXGI_FORMAT_R8G8B8A8_UNORM;
        swapChainDesc.BufferUsage = DXGI_USAGE_RENDER_TARGET_OUTPUT;
        swapChainDesc.SwapEffect = DXGI_SWAP_EFFECT_FLIP_DISCARD;
        swapChainDesc.OutputWindow = Win32Application::GetHwnd();
        swapChainDesc.SampleDesc.Count = 1;
        swapChainDesc.Windowed = TRUE;

        ComPtr<IDXGISwapChain> swapChain;
        ThrowIfFailed(factory->CreateSwapChain(
            m_commandQueue.Get(), // Swap chain needs the queue so that it can force a flush on it.
            &swapChainDesc,
            &swapChain
        ));

        ThrowIfFailed(swapChain.As(&m_swapChain));

        // This sample does not support fullscreen transitions.
        ThrowIfFailed(factory->MakeWindowAssociation(Win32Application::GetHwnd(), DXGI_MWA_NO_ALT_ENTER));

        m_frameIndex = m_swapChain->GetCurrentBackBufferIndex();

        // Create descriptor heaps.
        {
            // Describe and create a render target view (RTV) descriptor heap.
            D3D12_DESCRIPTOR_HEAP_DESC rtvHeapDesc = {};
            rtvHeapDesc.NumDescriptors = FrameCount;
            rtvHeapDesc.Type = D3D12_DESCRIPTOR_HEAP_TYPE_RTV;
            rtvHeapDesc.Flags = D3D12_DESCRIPTOR_HEAP_FLAG_NONE;
            ThrowIfFailed(m_device->CreateDescriptorHeap(&rtvHeapDesc, IID_PPV_ARGS(&m_rtvHeap)));

            m_rtvDescriptorSize = m_device->GetDescriptorHandleIncrementSize(D3D12_DESCRIPTOR_HEAP_TYPE_RTV);
        }

        // Create frame resources.
        {
            CD3DX12_CPU_DESCRIPTOR_HANDLE rtvHandle(m_rtvHeap->GetCPUDescriptorHandleForHeapStart());

            // Create a RTV for each frame.
            for (UINT n = 0; n < FrameCount; n++)
            {
                ThrowIfFailed(m_swapChain->GetBuffer(n, IID_PPV_ARGS(&m_renderTargets[n])));
                m_device->CreateRenderTargetView(m_renderTargets[n].Get(), nullptr, rtvHandle);
                rtvHandle.Offset(1, m_rtvDescriptorSize);
            }
        }

        ThrowIfFailed(
            m_device->CreateCommandAllocator(D3D12_COMMAND_LIST_TYPE_DIRECT, IID_PPV_ARGS(&m_commandAllocator)));
    }

    void LoadAssets() // Loads all the assets currently necessary.
    {
        // Create an empty root signature
        {
            CD3DX12_ROOT_SIGNATURE_DESC rootSignatureDesc;
        }
    }

    void PopulateCommandList(); // Fills the command list variable with everything we need to run this frame.

    void WaitForPreviousFrame() // Ensures that the previous frame successfully rendered, and waits for it to do so.
    {
        const UINT64 fence = m_fenceValue;
        ThrowIfFailed(m_commandQueue->Signal(m_fence.Get(), fence));
        m_fenceValue++;

        // Wait until the previous frame is finished
        if (m_fence->GetCompletedValue() < fence)
        {
            ThrowIfFailed(m_fence->SetEventOnCompletion(fence, m_fenceEvent));
            WaitForSingleObject(m_fenceEvent, INFINITE);
        }

        m_frameIndex = m_swapChain->GetCurrentBackBufferIndex();
    }

public:
    extern "C" {
    DLL_EXPORT Renderer(UINT width, UINT height, std::wstring name);

    DLL_EXPORT void Initialize(Scene* scene)
    {
        m_scene = scene;
        LoadPipeline();
        LoadAssets();
    }

    DLL_EXPORT void Render()
    {
        // Record all the commands we need to render the scene into the command list
        PopulateCommandList();

        // Execute command list
        ID3D12CommandList* ppCommandLists[] = {m_commandList->Get()};
        m_commandQueue->ExecuteCommandLists(_countof(ppCommandLists), ppCommandLists);

        // Present the frame
        ThrowIfFailed(m_swapChain->Present(1, 0));

        WaitForPreviousFrame();
    }

    DLL_EXPORT void Shutdown()
    {
        // Wait for the previous frame to be rendered
        WaitForPreviousFrame();

        CloseHandle(m_fenceEvent);
    }
    }
};
