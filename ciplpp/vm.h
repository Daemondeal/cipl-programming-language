#pragma once

#include "./utils/common.h"
#include "./utils/vector.h"

#include "value.h"
#include "chunk.h"

typedef enum {
    INTERPRET_OK,
    INTERPRET_COMPILE_ERROR,
    INTERPRET_RUNTIME_ERROR
} InterpretResult;

class VM {
private:
    Vector<Value> m_stack;
    Chunk *m_chunk;
    uint8_t *m_ip;

    InterpretResult run();
public:
    VM() = default;
    ~VM() = default;

    InterpretResult interpret(Chunk &chunk);
};