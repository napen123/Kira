#ifndef KIRAVM_UTF8_H
#define KIRAVM_UTF8_H

#include <cstdint>

struct String
{
    int32_t length;
    unsigned char * data;
};

String * utf8_create(const unsigned char *, int32_t blen);

int32_t utf8_length(const unsigned char *);

#endif //KIRAVM_UTF8_H
