#include <cstdio>
#include <cstdint>
#include <cstdlib>

#include "error.h"

#include "stack.h"
#include "internal.h"

#include "vm.h"

static Literal * locals;
static int32_t localCount;

String ** strings;
static int32_t stringCount;

static int64_t programLength;
static int32_t programOffset;

static int32_t readOffset = 4;

static int32_t getInt32(const unsigned char * buffer)
{
    auto ret = *((int32_t *) (buffer + readOffset));
    readOffset += 4;
    return ret;
}

static int64_t getInt64(const unsigned char * buffer)
{
    auto ret = *((int64_t *) (buffer + readOffset));
    readOffset += 8;
    return ret;
}

static float getFloat(const unsigned char * buffer)
{
    auto ret = *((float *) (buffer + readOffset));
    readOffset += 4;
    return ret;
}

static Literal getLiteral(const unsigned char * buffer)
{
    Literal ret{};
    auto type = static_cast<LiteralType>(*((int8_t *) (buffer + readOffset)));
    readOffset += 1;

    ret.type = type;

    switch(type)
    {
        case LiteralType::Integer:
            ret.i = getInt32(buffer);
            break;
        case LiteralType::Float:
            ret.f = getFloat(buffer);
            break;
        case LiteralType::String:
            ret.s = getInt32(buffer);
            break;
    }

    return ret;
}

static void setLiteral(int32_t index, Literal value)
{
    if(index < 0 || index > localCount)
        throwError("Invalid local index.");

    locals[index] = value;
}

void vm_initialize(const unsigned char * buffer)
{
    localCount = getInt32(buffer);
    locals = static_cast<Literal *>(calloc(static_cast<size_t>(localCount), sizeof(Literal)));

    stringCount = getInt32(buffer);
    strings = static_cast<String **>(malloc(sizeof(String) * stringCount));

    for(int i = 0; i < stringCount; i++)
    {
        auto byteLength = getInt32(buffer);
        auto str = utf8_create(buffer + readOffset, byteLength);
        strings[i] = str;
        readOffset += byteLength;
    }

    programLength = getInt64(buffer);
    programOffset = readOffset;

    stack_initialize();
}

void vm_run(const unsigned char * buffer)
{
    while(readOffset - programOffset < programLength)
    {
        auto instruction = static_cast<Instruction>(getInt32(buffer));

        switch(instruction)
        {
            case Instruction::Push:
            {
                auto value = getLiteral(buffer);

                stack_push(value);
            }
                break;
            case Instruction::Pop:
                stack_pop();
                break;
            case Instruction::Set:
            {
                auto index = getInt32(buffer);
                auto value = getLiteral(buffer);

                setLiteral(index, value);
            }
                break;
            case Instruction::CallS:
                break; // TODO: Implement
            case Instruction::CallI:
            {
                auto internal = getInt32(buffer);

                internal_execute(internal);
            }
                break;
            case Instruction::CallE:
                break; // TODO: Implement
        }
    }
}

void vm_deinitialize()
{
    stack_deinitialize();

    free(strings);
    free(locals);
}
