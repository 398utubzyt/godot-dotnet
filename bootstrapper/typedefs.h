#ifndef _TYPEDEFS_H_
#define _TYPEDEFS_H_

#ifndef __cplusplus
#define true 1
#define false 0
#define nullptr 0
typedef char bool;
#endif

#ifdef _MSC_VER
#define LIBEXPORT __declspec(dllexport)
#else
#define LIBEXPORT
#endif

#ifndef _WIN64
typedef signed int isize;
typedef unsigned int usize;
#else
typedef signed long long isize;
typedef unsigned long long usize;
#endif

#endif