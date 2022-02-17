#include "vm.h"

#include "debug.h"

InterpretResult VM::run() {
#define READ_BYTE() (*m_ip++)
#define READ_CONSTANT() (m_chunk->constants[READ_BYTE()])
#define BINARY_OP(op) \
    do { \
        double b = m_stack.pop(); \
        double a = m_stack.pop(); \
        m_stack.push(a op b); \
    } while (false)

    for (;;) {
#ifdef DEBUG_TRACE_EXECUTION
        // Stack Trace
        printf("          ");
        for (auto const& element : m_stack) {
            printf("[ ");
            printValue(element);
            printf(" ]");
        }
        printf("\n");

        // Instruction
        disassembleInstruction(*m_chunk, (size_t)(m_ip - &m_chunk->code[0]));
#endif

        uint8_t instruction;

        switch (instruction = READ_BYTE()) {
            case OP_CONSTANT: {
                Value constant = READ_CONSTANT();
                m_stack.push(constant);
                break;
            }
            case OP_CONSTANT_LONG: {
                uint8_t low = READ_BYTE();
                uint8_t mid = READ_BYTE();
                uint8_t high = READ_BYTE();


                Value constant = m_chunk->constants[low + (mid << 8) + (high << 16)];
                m_stack.push(constant);
                break;
            }
            case OP_NEGATE:
                m_stack.push(-m_stack.pop());
                break;
            case OP_ADD:
                BINARY_OP(+);
                break;
            case OP_SUBTRACT:
                BINARY_OP(-);
                break;
            case OP_MULTIPLY:
                BINARY_OP(*);
                break;
            case OP_DIVIDE:
                BINARY_OP(/);
                break;
            case OP_RETURN: {
                printValue(m_stack.pop());
                printf("\n");
                return INTERPRET_OK;
            }
        }
    }
#undef READ_BYTE
#undef READ_CONSTANT
#undef BINARY_OP
}

// TODO
InterpretResult VM::interpret(const char *source) {
    compile(source);
    return INTERPRET_OK;
}

void VM::compile(const char *source) {
    m_scanner.init(source);
    int line = -1;
    for (;;) {
        Token token = m_scanner.scan_token();
        if (token.line != line) {
            printf("%4d ", token.line);
            line = token.line;
        } else {
            printf("   | ");
        }
        printf("%2d '%.*s'\n", token.type, token.length, token.start);

        if (token.type == TOKEN_EOF) break;
    }
}
