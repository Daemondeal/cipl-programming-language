#include <stdio.h>

#include "debug.h"


void disassembleChunk(Chunk *chunk, const char *name) {
    printf("== %s ==\n", name);

    for (usize offset = 0; offset < chunk->count;) {
        offset = disassembleInstruction(chunk, offset);
    }
}

usize simpleInstruction(const char *name, usize offset) {
    printf("%s\n", name);
    return offset + 1;
}

usize constantInstruction(const char *name, Chunk *chunk, usize offset) {
    u8 constant = chunk->code[offset + 1];
    printf("%-16s %4d '", name, constant);
    printValue(chunk->constants.values[constant]);
    printf("'\n");

    return offset + 2;
}

usize disassembleInstruction(Chunk *chunk, usize offset) {
    printf("%04lu ", offset);

    u32 line = getLineArray(&chunk->lines, offset);

    if (offset > 0 && line == getLineArray(&chunk->lines, offset - 1)) {
        printf("   | ");
    } else {
        printf("%4u ", line);
    }

    u8 instruction = chunk->code[offset];

    switch(instruction) {
        case OP_RETURN:
            return simpleInstruction("OP_RETURN", offset);
        case OP_CONSTANT:
            return constantInstruction("OP_CONSTANT", chunk, offset);
        default:
            printf("Unknown opcode %d\n", instruction);
            return offset + 1;
    }
}
