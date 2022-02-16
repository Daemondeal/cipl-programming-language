#include <cstdio>

#include "utils/common.h"
#include "utils/vector.h"

#include "chunk.h"
#include "debug.h"
#include "vm.h"

int main() {
    Chunk chunk;
    VM vm;

    chunk.write_constant(1.2, 0);
    chunk.write_constant(3.4, 0);
    chunk.write(OP_ADD, 0);
    chunk.write_constant(5.6, 0);
    chunk.write(OP_DIVIDE, 0);
    chunk.write(OP_NEGATE, 0);

    chunk.write(OP_RETURN, 1);
//    disassembleChunk(chunk, "test chunk");

    vm.interpret(chunk);

    return 0;
}
