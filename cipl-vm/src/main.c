#include "common.h"
#include "chunk.h"
#include "debug.h"

#include <stdio.h>

int main(int argc, const char *argv[]) {
    Chunk chunk;

    initChunk(&chunk);
    pushChunk(&chunk, OP_RETURN, 1);
    
    pushConstant(&chunk, 123.456, 2);

    disassembleChunk(&chunk, "test chunk");
    freeChunk(&chunk);
    return 0;
}