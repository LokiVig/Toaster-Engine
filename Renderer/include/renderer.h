#ifndef _RENDERER_H
#define _RENDERER_H

#define WIN32_LEAN_AND_MEAN
#include <windows.h>
#include <shellapi.h>

// The min / max macros conflict with like-named member functions
// Only use std::min and std::max defined in <algorithm>
#if defined(min)
#undef min
#endif // defined(min)

#if defined(max)
#undef max
#endif // defined(max)

// In order to define a function called CreateWindow, the windows macro needs
// to be undefined
#if defined(CreateWindow)
#undef CreateWindow
#endif // defined(CreateWindow)

// Windows Runtime Library, needed for Microsoft::WRL::ComPtr<> template class
#include <wrl.h>
using namespace Microsoft::WRL;

// DirectX 12 specific headers
#include <d3d12.h>
#include <dxgi1_6.h>
#include <d3dcompiler.h>
#include <directxmath.h>

// D3D12 extension library
#include <d3dx12.h>

// STL headers
#include <algorithm>
#include <cassert>
#include <chrono>

// Helper functions
#include <helpers.h>

class Renderer
{
public:

private:
	HWND CreateWindow(const wchar_t*, HINSTANCE, const wchar_t*, uint32_t, uint32_t);
	void RegisterWindowClass(HINSTANCE, const wchar_t*);

private:
	ComPtr<IDXGIAdapter4> GetAdapter(bool);
	ComPtr<ID3D12Device2> CreateDevice(ComPtr<IDXGIAdapter4>);
	ComPtr<ID3D12CommandQueue> CreateCommandQueue(ComPtr<ID3D12Device2>, D3D12_COMMAND_LIST_TYPE);
	ComPtr<IDXGISwapChain4> CreateSwapChain(HWND, ComPtr<ID3D12CommandQueue>, uint32_t, uint32_t, uint32_t);

private:
	bool CheckTearingSupport();

private:
	void ParseCommandLineArguments();
	void EnableDebugLayer();

private:
	// The number of swap chain back buffers
	static const uint8_t m_numFrames = 3;

	// Use WARP adapter
	bool m_useWarp = false;

	uint32_t m_clientWidth = 1280;
	uint32_t m_clientHeight = 720;

	// Set to true once the DX12 objects have been initialized
	bool m_isInitialized = false;

	// Window handle
	HWND m_windowHandle;

	// Window rectangle (used to toggle fullscreen state)
	RECT m_windowRect;

	// DirectX 12 objects
	ComPtr<ID3D12Device2> m_pDevice;
	ComPtr<ID3D12CommandQueue> m_pCommandQueue;
	ComPtr<IDXGISwapChain4> m_pSwapChain;
	ComPtr<ID3D12Resource> m_pBackBuffers[m_numFrames];
	ComPtr<ID3D12GraphicsCommandList> m_pCommandList;
	ComPtr<ID3D12CommandAllocator> m_pCommandAllocators[m_numFrames];
	ComPtr<ID3D12DescriptorHeap> m_pRTVDescriptorHeap;
	UINT m_rtvDescriptorSize;
	UINT m_currentBackBufferIndex;

	// Synchronization objects
	ComPtr<ID3D12Fence> m_pFence;
	uint64_t m_fenceValue = 0;
	uint64_t m_frameFenceValues[m_numFrames] = {};
	HANDLE m_fenceEvent;

	// By default, enable V-Sync
	// Can be toggled with the console command "r_vsync"
	bool m_vSync = true;
	bool m_tearingSupported = false;

	// By default, use windowed mode
	// Can be changed with the console command "r_windowstate"
	bool m_fullscreen = false;
};

#endif // _RENDERER_H