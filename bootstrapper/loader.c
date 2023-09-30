#include "dynlib.h"
#include "loader.h"

#if TESTS_ENABLED
#include <stdio.h>
#endif

static GDExtensionInterfaceGetProcAddress gd_get_proc;
static GDExtensionClassLibraryPtr gd_lib;

static struct hostfxr_api_fns
{
	hostfxr_initialize_for_dotnet_command_line_fn initialize_for_dotnet_command_line;
	hostfxr_initialize_for_runtime_config_fn initialize_for_runtime_config;
	hostfxr_get_runtime_delegate_fn get_runtime_delegate;
	hostfxr_close_fn close;
} hostfxr_api;

static void wstrcpy(char_t* to, const char_t* from)
{
	while (*from)
		*to++ = *from++;
	*to = 0;
}

static char_t* wstrrchr(const char_t* to, char_t character)
{
	char_t* last = 0;
	while (*to)
	{
		if (*to++ == character)
			last = to;
	}
	return last;
}

static void strapd(char* to, const char* append)
{
	while (*(++to));
	while (*append)
		*to++ = *append++;
	*to = 0;
}

static bool find_hostfxr(char* path, usize len)
{
	if (!libpath(path, len))
		return false;
	strapd(path, "hostfxr.dll");
	return true;
}

static char search[32767];

static bool load_hostfxr()
{
#if TESTS_ENABLED
	printf("Find hostfxr.dll\n");
#endif

	if (!find_hostfxr(search, sizeof(search)))
		return false;

#if TESTS_ENABLED
	printf("Open lib: '%s'\n", search);
#endif

	void* lib;
	if (!libopen(search, &lib))
		return false;

#if TESTS_ENABLED
	printf("Get hostfxr API\n");
#endif

#if TESTS_ENABLED
#define STR(name) #name
#define MKSTR(name) STR(name)
#define GETSYM(name) if (!libget(lib, "hostfxr_" MKSTR(name), &hostfxr_api.name)) { printf("Could not get function: " MKSTR(name) "\n"); return false; }
	GETSYM(initialize_for_dotnet_command_line);
	GETSYM(initialize_for_runtime_config);
	GETSYM(get_runtime_delegate);
	GETSYM(close);
#undef STR
#undef MKSTR
#undef GETSYM

	return true;
#else
	bool result =
		libget(lib, "hostfxr_initialize_for_dotnet_command_line", &hostfxr_api.initialize_for_dotnet_command_line) &&
		libget(lib, "hostfxr_initialize_for_runtime_config", &hostfxr_api.initialize_for_runtime_config) &&
		libget(lib, "hostfxr_get_runtime_delegate", &hostfxr_api.get_runtime_delegate) &&
		libget(lib, "hostfxr_close", &hostfxr_api.close);

	libclose(lib);

	return result;
#endif
}

static bool find_godotsharp(GDExtensionInterfaceGetProcAddress p_get_proc_address, GDExtensionClassLibraryPtr p_library, char_t* path, usize len)
{
	GDExtensionInterfaceGetLibraryPath get_library_path;
	get_library_path = p_get_proc_address("get_library_path");
	GDExtensionInterfaceObjectDestroy object_destroy;
	object_destroy = p_get_proc_address("object_destroy");

#if defined(_WIN32)
	GDExtensionInterfaceStringToUtf16Chars string_to_fxr_chars;
	string_to_fxr_chars = p_get_proc_address("string_to_utf16_chars");
#else
	GDExtensionInterfaceStringToUtf8Chars string_to_fxr_chars;
	string_to_fxr_chars = p_get_proc_address("string_to_utf8_chars");
#endif

	if (!get_library_path || !string_to_fxr_chars || !object_destroy)
		return false;

	void* gdpath = 0;
	get_library_path(p_library, &gdpath);
	path[string_to_fxr_chars(&gdpath, path, len)] = 0;
}

static bool initialize_hostfxr(char_t* path, load_assembly_and_get_function_pointer_fn* fn)
{
	hostfxr_handle handle;
	int32_t result = hostfxr_api.initialize_for_runtime_config(path, nullptr, &handle);
	if (result)
	{
		hostfxr_api.close(handle);
		return false;
	}

	result = hostfxr_api.get_runtime_delegate(handle, hdt_load_assembly_and_get_function_pointer, fn);
	hostfxr_api.close(handle);
	return result == 0 && *fn;
}

GDExtensionBool godot_dotnet_initialize(GDExtensionInterfaceGetProcAddress p_get_proc_address, GDExtensionClassLibraryPtr p_library, GDExtensionInitialization* r_initialization)
{
	if (p_get_proc_address == NULL || p_library == NULL || r_initialization == NULL)
		return false;

#if IS_EDITOR
	if (!load_hostfxr())
		return false;

	char_t path[512];
	if (!find_godotsharp(p_get_proc_address, p_library, path, sizeof(path)))
		return false;

	char_t* end = wstrrchr(path, FXRPATH);
	if (end == 0)
		return false;
	wstrcpy(end, FXRSTR("GodotSharp.runtimeconfig.json"));

	load_assembly_and_get_function_pointer_fn loadfn;
	if (!initialize_hostfxr(path, &loadfn))
		return false;
	
	wstrcpy(end, FXRSTR("GodotSharp.dll"));
	GDExtensionInitializationFunction init = 0;
	if (loadfn(path, FXRSTR("Godot.GdExtension.Main, GodotSharp"), FXRSTR("Load"), UNMANAGEDCALLERSONLY_METHOD, nullptr, &init))
		return false;
	
	return init(p_get_proc_address, p_library, r_initialization);
#else
#error Cannot make export template build yet!!
#endif
}