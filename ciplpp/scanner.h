#pragma once

#include "utils/common.h"

enum TokenType {
    // Single-character tokens
    TOKEN_LEFT_PAREN, TOKEN_RIGHT_PAREN, TOKEN_COMMA, TOKEN_DOT,
    TOKEN_DOT_DOT, TOKEN_THREE_DOTS, TOKEN_MINUS, TOKEN_PLUS,
    TOKEN_SLASH, TOKEN_STAR, TOKEN_TILDE, TOKEN_COLON,

    // Indents
    TOKEN_INDENT, TOKEN_DEDENT,

    TOKEN_LINE_END,

    // One or two character tokens
    TOKEN_BANG_EQUAL, TOKEN_EQUAL, TOKEN_EQUAL_EQUAL,
    TOKEN_GREATER, TOKEN_GREATER_EQUAL,
    TOKEN_LESS, TOKEN_LESS_EQUAL,

    // Literals
    TOKEN_IDENTIFIER, TOKEN_STRING, TOKEN_NUMBER,

    // Keywords
    TOKEN_TRUE, TOKEN_FALSE, TOKEN_NULL,
    TOKEN_NOT, TOKEN_AND, TOKEN_OR,
    TOKEN_LET,
    TOKEN_IF, TOKEN_ELSE, TOKEN_WHILE, TOKEN_REPEAT, TOKEN_FOR, TOKEN_BREAK,
    TOKEN_IN,
    TOKEN_PROC, TOKEN_RETURN, TOKEN_PASS,
    TOKEN_CLASS, TOKEN_SUPER, TOKEN_THIS, TOKEN_EXTENDS,

    TOKEN_ERROR, TOKEN_EOF
};

struct Token {
    TokenType type;
    const char *start;
    size_t length;
    int line;
};

class Scanner {
private:
    const char *m_start;
    const char *m_current;
    int m_line;

    bool is_at_end();
    Token make_token(TokenType type);
    Token error_token(const char *message);

    char advance();
    bool match(char expected);

public:
    Scanner() = default;
    ~Scanner() = default;

    void init(const char *source);
    Token scan_token();
};