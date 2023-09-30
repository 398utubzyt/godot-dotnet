#if TESTS_ENABLED

#include "loader.h"
#include "dynlib.h"

#include <stdio.h>

void* test_get_proc_address(const char* name) { return 0; }

int main()
{
	GDExtensionInitialization init;
	printf(godot_dotnet_initialize(&test_get_proc_address, -1, &init) ? "Test succeeded" : "Test failed.");
	return 0;
}

#endif