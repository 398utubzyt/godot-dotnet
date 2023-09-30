cmake_minimum_required(VERSION 3.26)
project("GodotSharp Bootstrapper")

add_library(GDBootstrap SHARED "loader.c" "loader.h" "dynlib.h" "dynlib.c" "typedefs.h")

string(CONCAT THIRDPARTY_INCLUDE_DIR ${CMAKE_CURRENT_SOURCE_DIR} "/../thirdparty/")
string(CONCAT GDEXTENSION_INCLUDE_DIR ${CMAKE_CURRENT_SOURCE_DIR} "/../gdextension/")
target_include_directories(GDBootstrap PUBLIC ${THIRDPARTY_INCLUDE_DIR} ${GDEXTENSION_INCLUDE_DIR})
set_target_properties(GDBootstrap PROPERTIES OUTPUT_NAME "gdext_dotnet_bs")

add_executable(GDBootstrap_TEST "loader.c" "loader.h" "dynlib.h" "dynlib.c" "typedefs.h" "test.c")
target_include_directories(GDBootstrap_TEST PUBLIC ${THIRDPARTY_INCLUDE_DIR} ${GDEXTENSION_INCLUDE_DIR})
set_target_properties(GDBootstrap_TEST PROPERTIES OUTPUT_NAME "gdbs_test")
target_compile_definitions(GDBootstrap_TEST PUBLIC TESTS_ENABLED=1)

if (GD_TOOLS)
	target_compile_definitions(GDBootstrap PUBLIC IS_EDITOR=1)
	target_compile_definitions(GDBootstrap_TEST PUBLIC IS_EDITOR=1)
endif()

# add_custom_command(TARGET GDBootstrap POST_BUILD COMMAND "C:\\Users\\Admin\\Documents\\godot-dotnet\\bin\\dotnet\\copy.cmd")

add_custom_command(TARGET GDBootstrap POST_BUILD COMMAND "C:\\Users\\Admin\\Downloads\\godot_4.2b6\\godot_console.exe" VERBATIM)