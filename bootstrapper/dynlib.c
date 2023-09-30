#include "dynlib.h"

#ifdef WIN32

#define WIN32_LEAN_AND_MEAN 1
#include <Windows.h>

bool libopen(const char* path, void** lib)
{
	return (*lib = LoadLibraryA(path)) != nullptr;
}
void libclose(void* lib)
{
	FreeLibrary(lib);
}
bool libget(void* lib, const char* symbol, void** out)
{
	return (*out = GetProcAddress(lib, symbol)) != nullptr;
}

bool libpath(char* out, usize len)
{
	HANDLE heap = GetProcessHeap();
	if (heap == nullptr)
		return false;

	// Temporary path buffer
	char* buffer = HeapAlloc(heap, HEAP_ZERO_MEMORY, len);
	if (buffer == nullptr)
		return false;

	usize c = GetEnvironmentVariableA("PATH", buffer, len);
	if (c == -1)
	{
		HeapFree(heap, 0, buffer);
		return false;
	}

	usize i = 0;
	while (i < c)
	{
		if (buffer[i] == ';')
			buffer[i] = 0;
		++i;
	}

	char* b2 = buffer;
	i = 0;
	bool result = false;
	while (i < c)
	{
		if (b2[i] != 0)
		{
			++i;
			continue;
		}
		
		strcpy_s(out, len, b2);
		usize i2 = i - 1;
		if (out[i2++] != '\\')
			out[i2++] = '\\';
		strcpy_s(out + i2, len - i2, "host\\fxr\\");
		i2 += 9;
		out[i2] = 0;

		DWORD attr = GetFileAttributes(out);
		if ((attr != INVALID_FILE_ATTRIBUTES && (attr & FILE_ATTRIBUTE_DIRECTORY)))
		{
			// Possible dotnet path?
			// Enumerate version directories to get the latest version

			out[i2++] = '*';
			out[i2] = 0;

			// Size of WIN32_FIND_DATA.cFileName is 260
			char latest[260] = { 0 };
			WIN32_FIND_DATAA data;
			HANDLE f = FindFirstFileA(out, &data);

			if (f == INVALID_HANDLE_VALUE)
				break;

			do
			{
				if (!(data.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY) || data.cFileName[0] == '.')
					continue;

				// Set final character to null
				data.cFileName[sizeof(data.cFileName)-1] = 0;

				if (strcmp(data.cFileName, latest) > 0)
					strcpy_s(latest, sizeof(latest), data.cFileName);
			} while (FindNextFileA(f, &data));
			strcpy_s(out + (--i2), len - i2, latest);
			i2 += strlen(latest);
			out[i2++] = '\\';
			out[i2] = 0;

			FindClose(f);

			result = true;
			break;
		}

		b2 += ++i;
		i = 0;
	}

	HeapFree(heap, 0, buffer);
	return result;
}

#else

#error This doesn't support anything other than Win32 at the moment.

#include <dlfcn.h>

bool libopen(const char* path, void** lib)
{
	return (*lib = dlopen(path, RTLD_NOW)) != nullptr;
}
void libclose(void* lib)
{
	dlclose(lib);
}
bool libget(void* lib, const char* symbol, void** out)
{
	return (*out = dlsym(lib, symbol)) != nullptr;
}

bool libpath(char* out, usize len)
{
	return false;
}

#endif