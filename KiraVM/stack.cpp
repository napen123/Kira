#include <cstdlib>

#include "stack.h"

#define STACK_SIZE 16
#define STACK_GROW 8

struct
{
    int top;
    int capacity;
    Literal * data;
} stack;

void stack_initialize()
{
    stack.top = -1;
    stack.capacity = STACK_SIZE;
    stack.data = static_cast<Literal *>(malloc(sizeof(Literal) * STACK_SIZE));
}

void stack_deinitialize()
{
    free(stack.data);
}

void stack_push(Literal value)
{
    if(stack_size() == STACK_SIZE)
    {
        stack.capacity += STACK_GROW;
        stack.data = static_cast<Literal *>(realloc(stack.data, static_cast<size_t>(stack.capacity)));
    }

    stack.top++;
    stack.data[stack.top] = value;
}

Literal stack_pop()
{
    if(stack.top < 0)
        return {};

    return stack.data[stack.top--];
}

int32_t stack_size()
{
    return stack.top;
}
