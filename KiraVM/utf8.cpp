#include <cstdlib>
#include <cstring>

#include "utf8.h"

#define ONEMASK ((size_t)(-1) / 0xFF)

String * utf8_create(const unsigned char * str, int32_t blen)
{
    auto ret = static_cast<String *>(malloc(sizeof(String)));

    ret->data = static_cast<unsigned char *>(malloc(sizeof(unsigned char) * blen + 1));
    memcpy(ret->data, str, static_cast<size_t>(blen));
    ret->data[blen] = '\0';

    ret->length = utf8_length(ret->data);

    return ret;
}

// http://www.daemonology.net/blog/2008-06-05-faster-utf8-strlen.html
int32_t utf8_length(const unsigned char * str)
{
    unsigned char b;
    size_t count = 0;
    const unsigned char * s;

    for (s = str; (uintptr_t) (s) & (sizeof(size_t) - 1); s++)
    {
        b = *s;

        if (b == '\0')
            goto done;

        count += (b >> 7) & ((~b) >> 6);
    }

    size_t u;

    for (;; s += sizeof(size_t))
    {
        __builtin_prefetch(&s[256], 0, 0);

        u = *(size_t *) (s);

        if ((u - ONEMASK) & (~u) & (ONEMASK * 0x80))
            break;

        u = ((u & (ONEMASK * 0x80)) >> 7) & ((~u) >> 6);
        count += (u * ONEMASK) >> ((sizeof(size_t) - 1) * 8);
    }

    for (;; s++)
    {
        b = *s;

        if (b == '\0')
            break;

        count += (b >> 7) & ((~b) >> 6);
    }

    done:
    return static_cast<int32_t>((s - str) - count);
}
