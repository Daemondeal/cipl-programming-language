#ifndef cipl_chunk_h
#define cipl_chunk_h

#include "common.h"
#include "value.h"
#include <stdio.h>

typedef enum {
    OP_RETURN,
    OP_CONSTANT,

    // Used if there are more than 255 constants in the chunk
    // Takes three bytes as params
    OP_CONSTANT_LONG
} OpCode;

typedef struct {
    u32 *line_data;
    usize capacity;
} LineArray;

// A dynamic array to store the bytecode
typedef struct {
    u8 *code;
    usize count;
    usize capacity;


    // Line data, used to show errors
    // It's stored with run-length encoding
    LineArray lines;

    // Array to store constants
    ValueArray constants;
} Chunk;

void writeLineArray(LineArray *lines, u32 line);
u32 getLineArray(LineArray *lines, usize chunk_index);
void printLineArray(LineArray *lines);

void initChunk(Chunk *chunk);
void pushChunk(Chunk *chunk, u8 byte, u32 line);
void freeChunk(Chunk *chunk);


void pushConstant(Chunk *chunk, Value value, u32 line);
usize addConstant(Chunk *chunk, Value value);

#endif