#include "common.h"
#include "chunk.h"
#include "debug.h"

#include <stdio.h>

int main(int argc, const char *argv[]) {
    Chunk chunk;


    initChunk(&chunk);
    pushChunk(&chunk, OP_RETURN, 1);
    
    usize constant = addConstant(&chunk, 1.2);
    pushChunk(&chunk, OP_CONSTANT, 2);
    pushChunk(&chunk, constant, 2);


    usize constant2 = addConstant(&chunk, 24.7);
    pushChunk(&chunk, OP_CONSTANT, 3);
    pushChunk(&chunk, constant2, 3);

    pushChunk(&chunk, OP_RETURN, 4);
    pushChunk(&chunk, OP_RETURN, 4);
    pushChunk(&chunk, OP_RETURN, 4);
    pushChunk(&chunk, OP_RETURN, 4);
    pushChunk(&chunk, OP_RETURN, 4);
    pushChunk(&chunk, OP_RETURN, 4);
    pushChunk(&chunk, OP_RETURN, 4);

    pushChunk(&chunk, OP_RETURN, 5);
    pushChunk(&chunk, OP_RETURN, 6);
    

    disassembleChunk(&chunk, "test chunk");
    freeChunk(&chunk);
    return 0;
}