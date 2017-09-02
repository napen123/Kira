#include <cstdio>
#include <cstdlib>

#include "error.h"

void throwError(const char * msg)
{
    fprintf(stderr, "Error: %s", msg);
    exit(1);
}
