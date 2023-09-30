#ifndef _LOADER_H_
#define _LOADER_H_

#include "typedefs.h"

#include "hostfxr/coreclr_delegates.h"
#include "hostfxr/hostfxr.h"

#include "gdextension_interface.h"

// I hate Microsoft!!!
#if defined(_WIN32)
#define FXRSTR(str) L ## str
#define FXRPATH L'\\'
#else
#define FXRSTR(str) str
#define FXRPATH '/'
#endif

GDExtensionBool LIBEXPORT godot_dotnet_initialize(GDExtensionInterfaceGetProcAddress p_get_proc_address, GDExtensionClassLibraryPtr p_library, GDExtensionInitialization* r_initialization);

#endif