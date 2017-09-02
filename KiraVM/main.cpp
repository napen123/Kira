#include <cstdio>
#include <cstdlib>

#include "vm.h"

#define BUFFER_SIZE 255

int main(int argc, char ** argv)
{
    if(argc != 2)
    {
        printf("Usage : KiraVM [input file]");

        return 0;
    }

    FILE * input = fopen(argv[1], "rb");

    if(input == nullptr)
    {
        fprintf(stderr, "Error: Could not open input file.");

        return 1;
    }

    fseek(input, 0, SEEK_END);
    auto size = static_cast<size_t>(ftell(input));
    fseek(input, 0, SEEK_SET);

    if(size < 10)
    {
        fprintf(stderr, "Error: Invalid bytecode file provided.");

        return 1;
    }

    auto * buffer = static_cast<unsigned char *>(malloc(BUFFER_SIZE));

    if(fread(buffer, sizeof * buffer, size, input) != size)
    {
        fprintf(stderr, "Error: Could not read input file.");

        return 1;
    }

    if(buffer[0] != 'K' || buffer[1] != 'I' || buffer[2] != 'R' || buffer[3] != 'A')
    {
        fprintf(stderr, "Error: Invalid bytecode header in input file.");

        return 1;
    }

    vm_initialize(buffer);
    vm_run(buffer);
    vm_deinitialize();

    free(buffer);
    fclose(input);

    return 0;
}