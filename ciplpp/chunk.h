#pragma once

#include "utils/common.h"
#include "utils/vector.h"
#include "value.h"

enum OpCode {
    OP_CONSTANT,
    OP_CONSTANT_LONG,
    OP_ADD,
    OP_SUBTRACT,
    OP_MULTIPLY,
    OP_DIVIDE,
    OP_NEGATE,
    OP_RETURN
};

class Chunk {
private:
    Vector<int> m_lines;
public:
    Vector<uint8_t> code;
    Vector<Value> constants;

    Chunk() = default;
    ~Chunk() = default;

    void add_line(int line);
    int get_line(size_t offset);

    void write(uint8_t byte, int line);
    void write_constant(Value value, int line);

    uint32_t read_24bit_operand(size_t offset);
};