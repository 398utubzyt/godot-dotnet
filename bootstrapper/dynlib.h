#ifndef _DYNLIB_H_
#define _DYNLIB_H_

#include "typedefs.h"

bool libopen(const char* path, void** lib);
void libclose(void* lib);
bool libget(void* lib, const char* symbol, void** out);

bool libpath(char* out, usize len);

#endif