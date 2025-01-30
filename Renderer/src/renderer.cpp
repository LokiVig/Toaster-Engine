#include <renderer.h>

void Renderer::Initialize()
{
	SetThreadDpiAwarenessContext(DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2);

	// Get our HINSTANCE
	HINSTANCE hInstance = GetModuleHandleW(nullptr);

	// Window class name, used for registering / creating the window
	const wchar_t* winClassName = L"ToasterEngineWindowClass";

	ParseCommandLineArguments();
	EnableDebugLayer();

	m_tearingSupported = CheckTearingSupport();

	RegisterWindowClass(hInstance, winClassName);
	m_windowHandle = CreateWindow(winClassName, hInstance, L"Toaster Engine Rendered Window", m_clientWidth, m_clientHeight);

	// Initialize the global window rect variable
	GetWindowRect(m_windowHandle, &m_windowRect);

	ComPtr<IDXGIAdapter4> dxgiAdapter4 = GetAdapter(m_useWarp);

	m_pDevice = CreateDevice(dxgiAdapter4);

	m_pCommandQueue = CreateCommandQueue(m_pDevice, D3D12_COMMAND_LIST_TYPE_DIRECT);

	m_pSwapChain = CreateSwapChain(m_windowHandle, m_pCommandQueue, m_clientWidth, m_clientHeight, m_numFrames);

	m_currentBackBufferIndex = m_pSwapChain->GetCurrentBackBufferIndex();

	m_pRTVDescriptorHeap = CreateDescriptorHeap(m_pDevice, D3D12_DESCRIPTOR_HEAP_TYPE_RTV, m_numFrames);
	m_rtvDescriptorSize = m_pDevice->GetDescriptorHandleIncrementSize(D3D12_DESCRIPTOR_HEAP_TYPE_RTV);

	UpdateRenderTargetViews(m_pDevice, m_pSwapChain, m_pRTVDescriptorHeap);

	for (int i = 0; i < m_numFrames; i++)
	{
		m_pCommandAllocators[i] = CreateCommandAllocator(m_pDevice, D3D12_COMMAND_LIST_TYPE_DIRECT);
	}

	m_pCommandList = CreateCommandList(m_pDevice, m_pCommandAllocators[m_currentBackBufferIndex], D3D12_COMMAND_LIST_TYPE_DIRECT);

	m_pFence = CreateFence(m_pDevice);
	m_fenceEvent = CreateEventHandle();

	m_isInitialized = true;

	ShowWindow(m_windowHandle, SW_SHOW);
}

void Renderer::Update()
{
	static uint64_t frameCounter = 0;
	static double elapsedSeconds = 0.0;
	static std::chrono::high_resolution_clock clock;
	static auto t0 = clock.now();

	frameCounter++;
	auto t1 = clock.now();
	auto deltaTime = t1 - t0;
	t0 = t1;

	elapsedSeconds += deltaTime.count() * 1e-9;

	if (elapsedSeconds > 1.0)
	{
		char buffer[500];
		auto fps = frameCounter / elapsedSeconds;
		sprintf_s(buffer, 500, "FPS: %f\n", fps);
		OutputDebugString((wchar_t*)buffer);

		frameCounter = 0;
		elapsedSeconds = 0.0;
	}

	MSG msg = {};

	if (PeekMessage(&msg, NULL, 0, 0, PM_REMOVE))
	{
		TranslateMessage(&msg);
		DispatchMessage(&msg);
	}

	Render();
}

void Renderer::SetFullscreen(bool fullscreen)
{
	if (m_fullscreen != fullscreen)
	{
		m_fullscreen = fullscreen;

		if (m_fullscreen) // Switching to fullscreen
		{
			// Store the current window dimensions so they can be restored
			// when switching out of fullscreen
			GetWindowRect(m_windowHandle, &m_windowRect);

			// Set the window style to a borderless window so the client area fills
			// the entire screen
			UINT windowStyle = WS_OVERLAPPEDWINDOW & ~(WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX);

			SetWindowLongW(m_windowHandle, GWL_STYLE, windowStyle);

			// Query the name of the nearest display device for the window
			// This is required to set the fullscreen dimensions of the window
			// when using a multi-monitor setup
			HMONITOR hMonitor = MonitorFromWindow(m_windowHandle, MONITOR_DEFAULTTONEAREST);
			MONITORINFOEX monitorInfo = {};
			monitorInfo.cbSize = sizeof(MONITORINFOEX);
			GetMonitorInfo(hMonitor, &monitorInfo);

			SetWindowPos(m_windowHandle, HWND_TOP, monitorInfo.rcMonitor.left, monitorInfo.rcMonitor.top, monitorInfo.rcMonitor.right - monitorInfo.rcMonitor.left, monitorInfo.rcMonitor.bottom - monitorInfo.rcMonitor.top, SWP_FRAMECHANGED | SWP_NOACTIVATE);

			ShowWindow(m_windowHandle, SW_MAXIMIZE);
		}
		else
		{
			// Restore all the window decorators
			SetWindowLong(m_windowHandle, GWL_STYLE, WS_OVERLAPPEDWINDOW);

			SetWindowPos(m_windowHandle, HWND_NOTOPMOST, m_windowRect.left, m_windowRect.top, m_windowRect.right - m_windowRect.left, m_windowRect.bottom - m_windowRect.top, SWP_FRAMECHANGED | SWP_NOACTIVATE);

			ShowWindow(m_windowHandle, SW_NORMAL);
		}
	}
}

void Renderer::Shutdown()
{
	// Make sure the command queue has finished all commands before closing
	Flush(m_pCommandQueue, m_pFence, m_fenceValue, m_fenceEvent);

	CloseHandle(m_fenceEvent);
}

void Renderer::Render()
{
	auto commandAllocator = m_pCommandAllocators[m_currentBackBufferIndex];
	auto backBuffer = m_pBackBuffers[m_currentBackBufferIndex];

	commandAllocator->Reset();
	m_pCommandList->Reset(commandAllocator.Get(), nullptr);

	// Clear the render target
	{
		CD3DX12_RESOURCE_BARRIER barrier = CD3DX12_RESOURCE_BARRIER::Transition(backBuffer.Get(), D3D12_RESOURCE_STATE_PRESENT, D3D12_RESOURCE_STATE_RENDER_TARGET);

		m_pCommandList->ResourceBarrier(1, &barrier);

		FLOAT clearColor[] = { 0.4f, 0.6f, 0.9f, 1.0f };
		CD3DX12_CPU_DESCRIPTOR_HANDLE rtv(m_pRTVDescriptorHeap->GetCPUDescriptorHandleForHeapStart(), m_currentBackBufferIndex, m_rtvDescriptorSize);

		m_pCommandList->ClearRenderTargetView(rtv, clearColor, 0, nullptr);
	}

	// Present
	{
		CD3DX12_RESOURCE_BARRIER barrier = CD3DX12_RESOURCE_BARRIER::Transition(backBuffer.Get(), D3D12_RESOURCE_STATE_RENDER_TARGET, D3D12_RESOURCE_STATE_PRESENT);
		m_pCommandList->ResourceBarrier(1, &barrier);

		ThrowIfFailed(m_pCommandList->Close());

		ID3D12CommandList* const pCommandLists[] =
		{
			m_pCommandList.Get()
		};

		m_pCommandQueue->ExecuteCommandLists(_countof(pCommandLists), pCommandLists);

		UINT syncInterval = m_vSync ? 1 : 0;
		UINT presentFlags = m_tearingSupported && !m_vSync ? DXGI_PRESENT_ALLOW_TEARING : 0;
		ThrowIfFailed(m_pSwapChain->Present(syncInterval, presentFlags));

		m_frameFenceValues[m_currentBackBufferIndex] = Signal(m_pCommandQueue, m_pFence, m_fenceValue);

		m_currentBackBufferIndex = m_pSwapChain->GetCurrentBackBufferIndex();

		WaitForFenceValue(m_pFence, m_frameFenceValues[m_currentBackBufferIndex], m_fenceEvent, std::chrono::milliseconds::max());
	}
}

void Renderer::Resize(uint32_t width, uint32_t height)
{
	if (m_clientWidth != width || m_clientHeight != height)
	{
		// Don't allow 0 size swap chain back buffers
		m_clientWidth = std::max(1u, width);
		m_clientHeight = std::max(1u, height);

		// Flush the GPU queue to make sure the swap chain's back buffers
		// are not being referenced by an in-flight command list
		Flush(m_pCommandQueue, m_pFence, m_fenceValue, m_fenceEvent);

		for (int i = 0; i < m_numFrames; i++)
		{
			// Any references to the back buffers must be released
			// before the swap chain can be resized
			m_pBackBuffers[i].Reset();
			m_frameFenceValues[i] = m_frameFenceValues[m_currentBackBufferIndex];

			DXGI_SWAP_CHAIN_DESC swapChainDesc = {};
			ThrowIfFailed(m_pSwapChain->GetDesc(&swapChainDesc));
			ThrowIfFailed(m_pSwapChain->ResizeBuffers(m_numFrames, m_clientWidth, m_clientHeight, swapChainDesc.BufferDesc.Format, swapChainDesc.Flags));

			m_currentBackBufferIndex = m_pSwapChain->GetCurrentBackBufferIndex();

			UpdateRenderTargetViews(m_pDevice, m_pSwapChain, m_pRTVDescriptorHeap);
		}
	}
}

HWND Renderer::CreateWindow(const wchar_t* winClassName, HINSTANCE hInstance, const wchar_t* winTitle, uint32_t width, uint32_t height)
{
	int scrWidth = GetSystemMetrics(SM_CXSCREEN); // User's screen's width
	int scrHeight = GetSystemMetrics(SM_CYSCREEN); // User's screen's height

	// Make a rect from our input width and height
	RECT winRect = { 0, 0, static_cast<LONG>(width), static_cast<LONG>(height) };
	AdjustWindowRect(&winRect, WS_OVERLAPPEDWINDOW, FALSE);

	// Get the actual window's width and height from our rect
	int winWidth = winRect.right - winRect.left;
	int winHeight = winRect.bottom - winRect.top;

	// Center the window within the screen, clamp to 0, 0 for the top-left corner
	int winX = std::max<int>(0, (scrWidth - winWidth) / 2);
	int winY = std::max<int>(0, (scrHeight - winHeight) / 2);

	// Create the window instance
	HWND hWnd = CreateWindowEx
	(
		NULL,
		winClassName,
		winTitle,
		WS_OVERLAPPEDWINDOW,
		winX,
		winY,
		winWidth,
		winHeight,
		NULL,
		NULL,
		hInstance,
		NULL
	);

	// Make sure we actually made a valid handle...
	assert(hWnd && "Failed to create window");

	// Return the newly made window!
	return hWnd;
}

void Renderer::RegisterWindowClass(HINSTANCE hInstance, const wchar_t* winClassName)
{
	// Register a window class for creating our render window with
	WNDCLASSEXW winClass = {};

	winClass.cbSize = sizeof(WNDCLASSEXW);
	winClass.style = CS_HREDRAW | CS_VREDRAW;
	winClass.lpfnWndProc = &WndProc;
	winClass.cbClsExtra = NULL;
	winClass.cbWndExtra = NULL;
	winClass.hInstance = hInstance;
	winClass.hIcon = LoadIcon(hInstance, NULL);
	winClass.hIconSm = LoadIcon(hInstance, NULL);
	winClass.hCursor = LoadCursor(NULL, IDC_ARROW);
	winClass.hbrBackground = (HBRUSH)(COLOR_WINDOW + 1);
	winClass.lpszMenuName = NULL;
	winClass.lpszClassName = winClassName;

	static ATOM atom = RegisterClassEx(&winClass);
	assert(atom > 0);
}

// Window callback function
LRESULT CALLBACK Renderer::WndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
	Renderer* pRenderer = reinterpret_cast<Renderer*>(GetWindowLongPtr(hWnd, GWLP_USERDATA));

	if (pRenderer)
	{
		if (pRenderer->m_isInitialized)
		{
			switch (message)
			{
			case WM_PAINT:
				pRenderer->Update();
				pRenderer->Render();
				break;

			case WM_SYSKEYDOWN:
			case WM_KEYDOWN:
			{
				bool alt = (GetAsyncKeyState(VK_MENU) & 0x8000) != 0;

				switch (wParam)
				{
				case 'V':
					pRenderer->m_vSync = !pRenderer->m_vSync;
					break;

				case VK_ESCAPE:
					PostQuitMessage(0);
					break;

				case VK_RETURN:
					if (alt)
					{
				case VK_F11:
					pRenderer->SetFullscreen(!pRenderer->m_fullscreen);
					}
					break;
				}
			} break;

			// The default window procedure will play a system notification
			// when pressing the Alt+Enter keyboard combination if this message
			// is not handled
			case WM_SYSCHAR:
				break;

			case WM_SIZE:
			{
				RECT clientRect = {};
				GetClientRect(pRenderer->m_windowHandle, &clientRect);

				int width = clientRect.right - clientRect.left;
				int height = clientRect.bottom - clientRect.top;

				pRenderer->Resize(width, height);
			} break;

			case WM_DESTROY:
				PostQuitMessage(0);
				break;

			default:
				return DefWindowProcW(hWnd, message, wParam, lParam);
			}
		}
		else
		{
			return DefWindowProcW(hWnd, message, wParam, lParam);
		}
	}

	return 0;
}

void Renderer::UpdateRenderTargetViews(ComPtr<ID3D12Device2> device, ComPtr<IDXGISwapChain4> swapChain, ComPtr<ID3D12DescriptorHeap> descriptorHeap)
{
	auto rtvDescriptorSize = device->GetDescriptorHandleIncrementSize(D3D12_DESCRIPTOR_HEAP_TYPE_RTV);

	CD3DX12_CPU_DESCRIPTOR_HANDLE rtvHandle(descriptorHeap->GetCPUDescriptorHandleForHeapStart());

	for (int i = 0; i < m_numFrames; i++)
	{
		ComPtr<ID3D12Resource> backBuffer;
		ThrowIfFailed(swapChain->GetBuffer(i, IID_PPV_ARGS(&backBuffer)));

		device->CreateRenderTargetView(backBuffer.Get(), nullptr, rtvHandle);

		m_pBackBuffers[i] = backBuffer;

		rtvHandle.Offset(rtvDescriptorSize);
	}
}

ComPtr<IDXGIAdapter4> Renderer::GetAdapter(bool useWarp)
{
	ComPtr<IDXGIFactory4> dxgiFactory;
	UINT createFactoryFlags = 0;
#if defined(_DEBUG)
	createFactoryFlags = DXGI_CREATE_FACTORY_DEBUG;
#endif // _DEBUG

	ThrowIfFailed(CreateDXGIFactory2(createFactoryFlags, IID_PPV_ARGS(&dxgiFactory)));

	ComPtr<IDXGIAdapter1> dxgiAdapter1;
	ComPtr<IDXGIAdapter4> dxgiAdapter4;

	if (useWarp)
	{
		ThrowIfFailed(dxgiFactory->EnumWarpAdapter(IID_PPV_ARGS(&dxgiAdapter1)));
		ThrowIfFailed(dxgiAdapter1.As(&dxgiAdapter4));
	}
	else
	{
		SIZE_T maxDedicatedVideoMemory = 0;

		for (UINT i = 0; dxgiFactory->EnumAdapters1(i, &dxgiAdapter1) != DXGI_ERROR_NOT_FOUND; i++)
		{
			DXGI_ADAPTER_DESC1 dxgiAdapterDesc1;
			dxgiAdapter1->GetDesc1(&dxgiAdapterDesc1);

			// Check to see if the adapter can create a D3D12 device without actually
			// creating it, the adapter with the largest dedicated video memory
			// is favored
			if ((dxgiAdapterDesc1.Flags & DXGI_ADAPTER_FLAG_SOFTWARE) == 0 && SUCCEEDED(D3D12CreateDevice(dxgiAdapter1.Get(), D3D_FEATURE_LEVEL_11_0, __uuidof(ID3D12Device), nullptr)) && dxgiAdapterDesc1.DedicatedVideoMemory > maxDedicatedVideoMemory)
			{
				maxDedicatedVideoMemory = dxgiAdapterDesc1.DedicatedVideoMemory;
				ThrowIfFailed(dxgiAdapter1.As(&dxgiAdapter4));
			}
		}
	}

	return dxgiAdapter4;
}

ComPtr<ID3D12Device2> Renderer::CreateDevice(ComPtr<IDXGIAdapter4> adapter)
{
	ComPtr<ID3D12Device2> d3d12Device2;
	ThrowIfFailed(D3D12CreateDevice(adapter.Get(), D3D_FEATURE_LEVEL_11_0, IID_PPV_ARGS(&d3d12Device2)));

	// Enable debug messages when debugging
#if defined(_DEBUG)
	ComPtr<ID3D12InfoQueue> pInfoQueue;

	if (SUCCEEDED(d3d12Device2.As(&pInfoQueue)))
	{
		pInfoQueue->SetBreakOnSeverity(D3D12_MESSAGE_SEVERITY_CORRUPTION, TRUE);
		pInfoQueue->SetBreakOnSeverity(D3D12_MESSAGE_SEVERITY_ERROR, TRUE);
		pInfoQueue->SetBreakOnSeverity(D3D12_MESSAGE_SEVERITY_WARNING, TRUE);

		// Suppress whole categories of messages
		//D3D12_MESSAGE_CATEGORY categories[] = {};

		// Suppress messages based on their severity level
		D3D12_MESSAGE_SEVERITY severities[] =
		{
			D3D12_MESSAGE_SEVERITY_INFO
		};

		// Suppress individual messages by their ID
		D3D12_MESSAGE_ID denyIDs[] =
		{
			D3D12_MESSAGE_ID_CLEARRENDERTARGETVIEW_MISMATCHINGCLEARVALUE,   // I'm really not sure how to avoid this message.
			D3D12_MESSAGE_ID_MAP_INVALID_NULLRANGE,                         // This warning occurs when using capture frame while graphics debugging.
			D3D12_MESSAGE_ID_UNMAP_INVALID_NULLRANGE,                       // This warning occurs when using capture frame while graphics debugging.
		};

		D3D12_INFO_QUEUE_FILTER filter = {};
		// filter.DenyList.NumCategories = _countof(categories);
		// filter.DenyList.pCategoryList = categories;
		filter.DenyList.NumSeverities = _countof(severities);
		filter.DenyList.pSeverityList = severities;
		filter.DenyList.NumIDs = _countof(denyIDs);
		filter.DenyList.pIDList = denyIDs;

		ThrowIfFailed(pInfoQueue->PushStorageFilter(&filter));
	}
#endif // _DEBUG

	return d3d12Device2;
}

ComPtr<ID3D12CommandQueue> Renderer::CreateCommandQueue(ComPtr<ID3D12Device2> device, D3D12_COMMAND_LIST_TYPE type)
{
	ComPtr<ID3D12CommandQueue> d3d12CommandQueue;

	D3D12_COMMAND_QUEUE_DESC desc = {};
	desc.Type = type;
	desc.Priority = D3D12_COMMAND_QUEUE_PRIORITY_NORMAL;
	desc.Flags = D3D12_COMMAND_QUEUE_FLAG_NONE;
	desc.NodeMask = 0;

	ThrowIfFailed(device->CreateCommandQueue(&desc, IID_PPV_ARGS(&d3d12CommandQueue)));

	return d3d12CommandQueue;
}

ComPtr<IDXGISwapChain4> Renderer::CreateSwapChain(HWND hWnd, ComPtr<ID3D12CommandQueue> commandQueue, uint32_t width, uint32_t height, uint32_t bufferCount)
{
	ComPtr<IDXGISwapChain4> dxgiSwapChain4;
	ComPtr<IDXGIFactory4> dxgiFactory4;
	UINT createFactoryFlags = 0;
#if defined(_DEBUG)
	createFactoryFlags = DXGI_CREATE_FACTORY_DEBUG;
#endif

	ThrowIfFailed(CreateDXGIFactory2(createFactoryFlags, IID_PPV_ARGS(&dxgiFactory4)));

	DXGI_SWAP_CHAIN_DESC1 swapChainDesc = {};
	swapChainDesc.Width = width;
	swapChainDesc.Height = height;
	swapChainDesc.Format = DXGI_FORMAT_R8G8B8A8_UNORM;
	swapChainDesc.Stereo = FALSE;
	swapChainDesc.SampleDesc = { 1, 0 };
	swapChainDesc.BufferUsage = DXGI_USAGE_RENDER_TARGET_OUTPUT;
	swapChainDesc.BufferCount = bufferCount;
	swapChainDesc.Scaling = DXGI_SCALING_STRETCH;
	swapChainDesc.SwapEffect = DXGI_SWAP_EFFECT_FLIP_DISCARD;
	swapChainDesc.AlphaMode = DXGI_ALPHA_MODE_UNSPECIFIED;
	// It is recommended to always allow tearing if tearing support is available
	swapChainDesc.Flags = CheckTearingSupport() ? DXGI_SWAP_CHAIN_FLAG_ALLOW_TEARING : 0;

	ComPtr<IDXGISwapChain1> swapChain1;
	ThrowIfFailed(dxgiFactory4->CreateSwapChainForHwnd(commandQueue.Get(), hWnd, &swapChainDesc, nullptr, nullptr, &swapChain1));

	// Disable the Alt+Enter fullscreen toggle feature, switching to fullscreen
	// will be handled manually
	ThrowIfFailed(dxgiFactory4->MakeWindowAssociation(hWnd, DXGI_MWA_NO_ALT_ENTER));

	ThrowIfFailed(swapChain1.As(&dxgiSwapChain4));

	return dxgiSwapChain4;
}

ComPtr<ID3D12DescriptorHeap> Renderer::CreateDescriptorHeap(ComPtr<ID3D12Device2> device, D3D12_DESCRIPTOR_HEAP_TYPE type, uint32_t numDescriptors)
{
	ComPtr<ID3D12DescriptorHeap> descriptorHeap;

	D3D12_DESCRIPTOR_HEAP_DESC desc = {};
	desc.NumDescriptors = numDescriptors;
	desc.Type = type;

	ThrowIfFailed(device->CreateDescriptorHeap(&desc, IID_PPV_ARGS(&descriptorHeap)));

	return descriptorHeap;
}

ComPtr<ID3D12CommandAllocator> Renderer::CreateCommandAllocator(ComPtr<ID3D12Device2> device, D3D12_COMMAND_LIST_TYPE type)
{
	ComPtr<ID3D12CommandAllocator> commandAllocator;
	ThrowIfFailed(device->CreateCommandAllocator(type, IID_PPV_ARGS(&commandAllocator)));

	return commandAllocator;
}

ComPtr<ID3D12GraphicsCommandList> Renderer::CreateCommandList(ComPtr<ID3D12Device2> device, ComPtr<ID3D12CommandAllocator> commandAllocator, D3D12_COMMAND_LIST_TYPE type)
{
	ComPtr<ID3D12GraphicsCommandList> commandList;
	ThrowIfFailed(device->CreateCommandList(0, type, commandAllocator.Get(), nullptr, IID_PPV_ARGS(&commandList)));

	ThrowIfFailed(commandList->Close());

	return commandList;
}

ComPtr<ID3D12Fence> Renderer::CreateFence(ComPtr<ID3D12Device2> device)
{
	ComPtr<ID3D12Fence> fence;

	ThrowIfFailed(device->CreateFence(0, D3D12_FENCE_FLAG_NONE, IID_PPV_ARGS(&fence)));

	return fence;
}

HANDLE Renderer::CreateEventHandle()
{
	HANDLE fenceEvent;

	fenceEvent = CreateEvent(NULL, FALSE, FALSE, NULL);
	assert(fenceEvent && "Failed to create fence event.");

	return fenceEvent;
}

uint64_t Renderer::Signal(ComPtr<ID3D12CommandQueue> commandQueue, ComPtr<ID3D12Fence> fence, uint64_t& fenceValue)
{
	uint64_t fenceValueForSignal = ++fenceValue;
	ThrowIfFailed(commandQueue->Signal(fence.Get(), fenceValueForSignal));

	return fenceValueForSignal;
}

void Renderer::WaitForFenceValue(ComPtr<ID3D12Fence> fence, uint64_t fenceValue, HANDLE fenceEvent, std::chrono::milliseconds duration = std::chrono::milliseconds::max())
{
	if (fence->GetCompletedValue() < fenceValue)
	{
		ThrowIfFailed(fence->SetEventOnCompletion(fenceValue, fenceEvent));
		WaitForSingleObject(fenceEvent, static_cast<DWORD>(duration.count()));
	}
}

bool Renderer::CheckTearingSupport()
{
	BOOL allowTearing = FALSE;

	// Rather than create the DXGI 1.5 factory interface directly, we create the DXGI 1.4
	// interface and query for the 1.5 interface, this is to enable the graphics debugging
	// tools which will not support the 1.5 factory interface until a future update
	ComPtr<IDXGIFactory4> factory4;

	if (SUCCEEDED(CreateDXGIFactory1(IID_PPV_ARGS(&factory4))))
	{
		ComPtr<IDXGIFactory5> factory5;

		if (SUCCEEDED(factory4.As(&factory5)))
		{
			if (FAILED(factory5->CheckFeatureSupport(DXGI_FEATURE_PRESENT_ALLOW_TEARING, &allowTearing, sizeof(allowTearing))))
			{
				allowTearing = FALSE;
			}
		}
	}

	return allowTearing == TRUE;
}

void Renderer::Flush(ComPtr<ID3D12CommandQueue> commandQueue, ComPtr<ID3D12Fence> fence, uint64_t& fenceValue, HANDLE fenceEvent)
{
	uint64_t fenceValueForSignal = Signal(commandQueue, fence, fenceValue);
	WaitForFenceValue(fence, fenceValueForSignal, fenceEvent);
}

void Renderer::ParseCommandLineArguments()
{
	int argc;
	wchar_t** argv = CommandLineToArgvW(GetCommandLineW(), &argc);

	for (size_t i = 0; i < argc; i++)
	{
		if (wcscmp(argv[i], L"-w") == 0 || wcscmp(argv[i], L"--width") == 0)
		{
			m_clientWidth = wcstol(argv[++i], nullptr, 10);
		}

		if (wcscmp(argv[i], L"-h") == 0 || wcscmp(argv[i], L"--height") == 0)
		{
			m_clientHeight = wcstol(argv[++i], nullptr, 10);
		}

		if (wcscmp(argv[i], L"-warp") == 0 || wcscmp(argv[i], L"--warp") == 0)
		{
			m_useWarp = true;
		}
	}

	// Free memory allocated by CommandLineToArgW
	LocalFree(argv);
}

void Renderer::EnableDebugLayer()
{
#if defined (_DEBUG)
	// Always enable the debug layer before doing anything DX12 related
	// so all possible errors generated while creating DX12 objects are
	// caught by the debug layer
	ComPtr<ID3D12Debug> debugInterface;
	ThrowIfFailed(D3D12GetDebugInterface(IID_PPV_ARGS(&debugInterface)));
	debugInterface->EnableDebugLayer();
#endif // _DEBUG
}