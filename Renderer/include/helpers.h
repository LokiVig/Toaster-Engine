#ifndef _HELPERS_H
#define _HELPERS_H

#define WIN32_LEAN_AND_MEAN
#include <windows.h> // For HRESULT

// From DXSampleHelper.h
// Source: https://github.com/Microsoft/DirectX-Graphics-Samples
inline void ThrowIfFailed(HRESULT hr)
{
	if (FAILED(hr))
	{
		throw std::exception();
	}
}

#endif // _HELPERS_H