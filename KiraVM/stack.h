#ifndef KIRAVM_STACK_H
#define KIRAVM_STACK_H

#include "utf8.h"

enum class LiteralType
{
    Integer = 0x0,
    Float = 0x1,
    String = 0x2
};

struct Literal
{
    LiteralType type;

    union
    {
        int32_t i;
        float f;
        int32_t s;
    };
};

void stack_initialize();
void stack_deinitialize();

void stack_push(Literal);
Literal stack_pop();

int32_t stack_size();

#endif //KIRAVM_STACK_H
