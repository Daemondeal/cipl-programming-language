#include "chunk.h"
#include "memory.h"

#include <stdio.h>
#include <stdlib.h>


void initLineArray(LineArray *lines) {
    lines->capacity = 0;
    lines->line_data = NULL;
}

void writeLineArray(LineArray *lines, u32 line) {
    if (lines->capacity <= line) {
        usize old_capacity = lines->capacity;

        // Grow the capacity until it can fit the new line
        while (lines->capacity < line) {
            lines->capacity = GROW_CAPACITY(lines->capacity);
        }

        if (lines->capacity != old_capacity) {
            // Grow the array
            lines->line_data = GROW_ARRAY(u32, lines->line_data, old_capacity, lines->capacity);

            // Initialize every value to zero
            for (usize i = old_capacity; i < lines->capacity; i++) {
                lines->line_data[i] = 0;
            }
        }
    }

    lines->line_data[line]++;
}

void freeLineArray(LineArray *lines) {
    FREE_ARRAY(u32, lines->line_data, lines->capacity);
    initLineArray(lines);
}

u32 getLineArray(LineArray *lines, usize chunk_index) {
    // RLA Encoding
    // This does not need to be efficent
    if (lines->capacity == 0) {
        fprintf(stderr, "Line Array out of bounds.\n");
        exit(-1);
    }

    usize count = 0;
    usize i = 0;

    while (count + lines->line_data[i] <= chunk_index) {
        count += lines->line_data[i];
        i++;

        if (lines->line_data[i] > lines->capacity && (count + lines->line_data[i] > chunk_index)) {
            fprintf(stderr, "Line Array out of bounds.\n");
            exit(-1);
        }
    }

    return i;
}

void initChunk(Chunk *chunk) {
    chunk->count = 0;
    chunk->capacity = 0;
    chunk->code = NULL;

    initLineArray(&chunk->lines);
    initValueArray(&chunk->constants);
}

void pushChunk(Chunk *chunk, u8 byte, u32 line) {
    if (chunk->capacity < chunk->count + 1){
        usize old_capacity = chunk->capacity;
        chunk->capacity = GROW_CAPACITY(old_capacity);
        chunk->code = GROW_ARRAY(u8, chunk->code, old_capacity, chunk->capacity);
    }
    chunk->code[chunk->count] = byte;
    chunk->count++;
    writeLineArray(&chunk->lines, line);
}

void freeChunk(Chunk *chunk) {
    FREE_ARRAY(u8, chunk->code, chunk->capacity);
    freeLineArray(&chunk->lines);
    freeValueArray(&chunk->constants);

    initChunk(chunk);
}

usize addConstant(Chunk *chunk, Value value) {
    pushValueArray(&chunk->constants, value);
    return chunk->constants.count - 1;
}