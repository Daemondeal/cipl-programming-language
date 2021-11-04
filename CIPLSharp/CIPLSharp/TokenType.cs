namespace CIPLSharp
{
    public enum TokenType
    {
        // Single-character tokens
        LEFT_PAREN, RIGHT_PAREN, COMMA, DOT, DOT_DOT, THREE_DOTS,
        MINUS, PLUS, SLASH, STAR, TILDE, COLON,
        
        // Indents
        INDENT, DEDENT,
        
        LINE_END,
        
        // One or two character tokens
        BANG_EQUAL, EQUAL, EQUAL_EQUAL, 
        GREATER, GREATER_EQUAL,
        LESS, LESS_EQUAL,
        
        // Literals
        IDENTIFIER, STRING, NUMBER,
        
        // Keywords
        TRUE, FALSE, NULL,
        NOT, AND, OR, 
        LET, 
        IF, ELSE, WHILE, REPEAT, FOR, BREAK,
        IN,
        PROC, RETURN, PASS,
        CLASS, SUPER, THIS,
        
        EOF
    }
}