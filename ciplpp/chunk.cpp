#include "chunk.h"

#include <cstdio>
#include <cstdlib>

const size_t UINT24_MAX = 0x00FFFFFF;

void Chunk::add_line(int line) {
    if (m_lines.count() > 0 && m_lines.last() == line) {
        m_lines[m_lines.count() - 2]++;
    } else {
        m_lines.push(1);
        m_lines.push(line);
    }
}

int Chunk::get_line(size_t offset) {
    size_t i = 0;

    while (offset >= m_lines[i]) {
        offset -= m_lines[i];
        i += 2;

        if (i >= m_lines.count()) {
            fprintf(stderr, "Line array out of bounds (index %lu).\n", i);
            exit(1);
        }
    }

    return m_lines[i + 1];
}

void Chunk::write(uint8_t byte, int line) {
    code.push(byte);
    add_line(line);
}

void Chunk::write_constant(Value value, int line) {
    constants.push(value);
    size_t constant = constants.count() - 1;

    if (constant <= UINT8_MAX) {
        write(OP_CONSTANT, line);
        write(constant, line);
    } else if (constant <= UINT24_MAX) {
        write(OP_CONSTANT_LONG, line);

        const size_t mask = 0xFF;
        for (int i = 0; i < 24; i += 8) {
            write((constant & (mask << i)) >> i, line);
        }
    } else {
        fprintf(stderr, "Error: You can't use more than %lu constants in a chunk.\n", UINT24_MAX);
        exit(1);
    }
}

uint32_t Chunk::read_24bit_operand(size_t offset) {
    uint32_t result = 0;
    for (int i = 0; i < 3; i++)
        result += code[offset + i] << (i * 8);


    return result;
}
