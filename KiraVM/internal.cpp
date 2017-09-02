#include <cstdio>

#include "error.h"
#include "stack.h"
#include "internal.h"

extern String ** strings;

void internal_execute(int32_t internal)
{
    switch(internal)
    {
        case INTERNAL_PRINT:
        {
            auto value = stack_pop();

            switch (value.type)
            {
                case LiteralType::Integer:
                    printf("%d", value.i);
                    break;
                case LiteralType::Float:
                    printf("%f", value.f);
                    break;
                case LiteralType::String:
                    printf("%s", strings[value.s]->data);
                    break;
            }
        }
            break;
        case INTERNAL_PRINTLN:
        {
            auto value = stack_pop();

            switch (value.type)
            {
                case LiteralType::Integer:
                    printf("%d\n", value.i);
                    break;
                case LiteralType::Float:
                    printf("%f\n", value.f);
                    break;
                case LiteralType::String:
                    printf("%s\n", strings[value.s]->data);
                    break;
            }
        }
        default:
            throwError("Unknown internal value called.");
    }
}
