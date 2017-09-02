#ifndef KIRAVM_INTERNAL_H
#define KIRAVM_INTERNAL_H

#include <cstdint>

#define INTERNAL_PRINT 0xF0
#define INTERNAL_PRINTLN 0xF1

void internal_execute(int32_t);

#endif //KIRAVM_INTERNAL_H
