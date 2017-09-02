#ifndef KIRAVM_VM_H
#define KIRAVM_VM_H

#include "utf8.h"

enum class Instruction
{
    Push = 0x01,
    Pop = 0x02,

    Set = 0x03,

    CallS = 0x10,
    CallI = 0x11,
    CallE = 0x12
};

void vm_initialize(const unsigned char *);
void vm_run(const unsigned char *);
void vm_deinitialize();

#endif //KIRAVM_VM_H
