#pragma once

#include "./utils/common.h"
#include "./utils/vector.h"

#include "value.h"
#include "chunk.h"
#include "parser.h"
#include "scanner.h"

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

    Parser m_parser;
    Scanner m_scanner;

    InterpretResult run();
public:
    VM() = default;
    ~VM() = default;

    void compile(const char *source);
    InterpretResult interpret(const char *source);
};