#include "scanner.h"

#include <cstdio>
#include <cstring>

void Scanner::init(const char *source) {
    m_start = source;
    m_current = source;
    m_line = 1;
}

Token Scanner::scan_token() {
    m_start = m_current;
    if (is_at_end()) return make_token(TOKEN_EOF);

    char c = advance();

    switch (c) {
        case '(':
            return make_token(TOKEN_LEFT_PAREN);
        case ')':
            return make_token(TOKEN_RIGHT_PAREN);
        case ',':
            return make_token(TOKEN_COMMA);
        case '-':
            return make_token(TOKEN_MINUS);
        case '+':
            return make_token(TOKEN_PLUS);
        case '*':
            return make_token(TOKEN_STAR);
        case '/':
            return make_token(TOKEN_SLASH);
        case '~':
            return make_token(TOKEN_TILDE);
        case ':':
            return make_token(TOKEN_COLON);
        case '!':
            if (match('='))
                return make_token(TOKEN_BANG_EQUAL);
            else
                return error_token("Expected '=' after '!'.");
        case '=':
            return make_token(match('=') ? TOKEN_EQUAL_EQUAL : TOKEN_EQUAL);
        case '<':
            return make_token(match('=') ? TOKEN_LESS_EQUAL : TOKEN_LESS);
        case '>':
            return make_token(match('=') ? TOKEN_GREATER_EQUAL : TOKEN_GREATER);


    }

    return error_token("Unexpected character.");
}

bool Scanner::is_at_end() {
    return *m_current == '\0';
}

Token Scanner::make_token(TokenType type) {
    Token token = {
            .type = type,
            .start = m_start,
            .length = (size_t)(m_current - m_start),
            .line = m_line
    };

    return token;
}

Token Scanner::error_token(const char *message) {
    Token token = {
            .type = TOKEN_ERROR,
            .start = message,
            .length = strlen(message),
            .line = m_line
    };

    return token;
}

char Scanner::advance() {
    m_current++;
    return m_current[-1];
}

bool Scanner::match(char expected) {
    if (is_at_end())
        return false;
    if (*m_current != expected)
        return false;
    m_current++;
    return true;
}
