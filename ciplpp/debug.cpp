#include "debug.h"

#include <cstdio>

#include "value.h"

static size_t simpleInstruction(const char *name, size_t offset) {
    printf("%s\n", name);
    return offset + 1;
}

static size_t constantInstruction(const char *name, Chunk &chunk, size_t offset) {
    uint8_t constant = chunk.code[offset + 1];

    printf("%-16s %4d '", name, constant);
    printValue(chunk.constants[constant]);
    printf("'\n");
    return offset + 2;
}

static size_t longConstantInstruction(const char *name, Chunk &chunk, size_t offset) {
    uint32_t constant = chunk.read_24bit_operand(offset + 1);

    printf("%-16s %4d '", name, constant);
    printValue(chunk.constants[constant]);
    printf("'\n");
    return offset + 4;
}


size_t disassembleInstruction(Chunk &chunk, size_t offset) {
    printf("%04lu ", offset);
    int line = chunk.get_line(offset);

    if (offset > 0 && line == chunk.get_line(offset - 1))
        printf("   | ");
    else
        printf("%4d ", line);

    uint8_t instruction = chunk.code[offset];
    switch (instruction) {
        case OP_CONSTANT:
            return constantInstruction("OP_CONSTANT", chunk, offset);
        case OP_CONSTANT_LONG:
            return longConstantInstruction("OP_CONSTANT_LONG", chunk, offset);
        case OP_NEGATE:
            return simpleInstruction("OP_NEGATE", offset);
        case OP_RETURN:
            return simpleInstruction("OP_RETURN", offset);
        case OP_ADD:
            return simpleInstruction("OP_ADD", offset);
        case OP_SUBTRACT:
            return simpleInstruction("OP_SUBTRACT", offset);
        case OP_MULTIPLY:
            return simpleInstruction("OP_MULTIPLY", offset);
        case OP_DIVIDE:
            return simpleInstruction("OP_DIVIDE", offset);
        default:
            printf("Unknown opcode %d\n", instruction);
            return offset + 1;
    }
}

void disassembleChunk(Chunk &chunk, const char *name) {
    printf("=== %s ===\n", name);
    for (size_t offset = 0; offset < chunk.code.count(); ) {
        offset = disassembleInstruction(chunk, offset);
    }
}
