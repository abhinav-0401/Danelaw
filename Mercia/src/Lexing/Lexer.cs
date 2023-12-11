namespace Mercia.Lexing;

public class Lexer
{
    private int _start = 0;
    private int _current = 0;
    private readonly string _source;

    private readonly Dictionary<string, TokenType> _keywords = new()
    {
        { "and", TokenType.And },
        { "class", TokenType.Class },
        { "else", TokenType.Else },
        { "false", TokenType.False },
        { "fun", TokenType.Fun },
        { "for", TokenType.For },
        { "if", TokenType.If },
        { "nil", TokenType.Nil },
        { "or", TokenType.Or },
        { "print", TokenType.Print },
        { "return", TokenType.Return },
        { "super", TokenType.Super },
        { "this", TokenType.This },
        { "true", TokenType.True },
        { "var", TokenType.Var },
        { "while", TokenType.While },
    };


    public List<Token> Tokens { get; init; }

    public Lexer(string source)
    {
        _source = source;
        Tokens = new List<Token>();
    }

    public List<Token> Lex()
    {
        Token token;
        for (token = LexToken();
             token.Type != TokenType.Eof &&
             token.Type != TokenType.Illegal;
             token = LexToken()) { }

        if (token.Type == TokenType.Illegal)
        {
            Console.WriteLine("{0}", Tokens[Tokens.Count - 1].Literal);
            throw new Exception("Unexpected character");
        }

        return Tokens;
    }

    private Token LexToken()
    {
        Token token = new("", TokenType.Illegal);
        SkipWhitespace();
        _start = _current;
        char c = Advance();

        switch (c)
        {
            case '(':
                token = AddToken(c, TokenType.LeftParen);
                break;
            case ')':
                token = AddToken(c, TokenType.RightParen);
                break;
            case '{':
                token = AddToken(c, TokenType.LeftBrace);
                break;
            case '}':
                token = AddToken(c, TokenType.RightBrace);
                break;
            case ',':
                token = AddToken(c, TokenType.Comma);
                break;
            case '.':
                token = AddToken(c, TokenType.Dot);
                break;
            case '+':
                token = AddToken(c, TokenType.Plus);
                break;
            case '-':
                token = AddToken(c, TokenType.Minus);
                break;
            case ';':
                token = AddToken(c, TokenType.Semicolon);
                break;
            case '*':
                token = AddToken(c, TokenType.Star);
                break;
            case '/':
                if (Peek() == '/')
                {
                    while (Peek() != '\n' && !IsAtEnd()) { Advance(); }
                }
                else { token = AddToken(c, TokenType.Slash); }

                break;
            case '=':
                token = AddDualCharToken(TokenType.EqualEqual, TokenType.Equal, '=');
                break;
            case '>':
                token = AddDualCharToken(TokenType.GreaterEqual, TokenType.Greater, '=');
                break;
            case '<':
                token = AddDualCharToken(TokenType.LessEqual, TokenType.Less, '=');
                break;
            case '!':
                token = AddDualCharToken(TokenType.BangEqual, TokenType.Bang, '=');
                break;
            case '\0':
                token = AddToken("", TokenType.Eof);
                break;
            default:
                if (Char.IsDigit(c)) { token = LexNumber(); }
                else if (IsAlpha(c)) { token = LexIdentifier(); }

                break;
        }

        return token;
    }

    private Token LexNumber()
    {
        while (!IsAtEnd() && Char.IsDigit(_source[_current])) { Advance(); }

        if (!IsAtEnd() && _source[_current] == '.' && Char.IsDigit(PeekNext()))
        {
            Advance();
            while (!IsAtEnd() && Char.IsDigit(_source[_current])) { Advance(); }
        }

        var literal = _source.Substring(_start, _current - _start);
        return AddToken(literal, TokenType.Number);
    }

    private Token LexIdentifier()
    {
        while (!IsAtEnd() && IsAlphaNumeric(_source[_current])) { Advance(); }

        var literal = _source.Substring(_start, _current - _start);

        if (_keywords.TryGetValue(literal, out var type)) { return AddToken(literal, type); }

        return AddToken(literal, TokenType.Identifier);
    }

    private static bool IsAlpha(char c)
    {
        return (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z');
    }

    private static bool IsAlphaNumeric(char c)
    {
        return IsAlpha(c) || (c >= '0' && c <= '9') || c == '_';
    }

    private Token AddDualCharToken(TokenType dualType, TokenType singleType, char c)
    {
        if (Peek() == c)
        {
            string literal = _source.Substring(_start, 2);
            Advance();
            return AddToken(literal, dualType);
        }
        else { return AddToken(_source[_start], singleType); }
    }

    private Token AddToken(char c, TokenType type)
    {
        var newToken = new Token(c.ToString(), type);
        Tokens.Add(newToken);
        return newToken;
    }

    private Token AddToken(string literal, TokenType type)
    {
        var newToken = new Token(literal, type);
        Tokens.Add(newToken);
        return newToken;
    }

    private bool IsAtEnd()
    {
        return _current >= _source.Length;
    }

    private char Advance()
    {
        if (!IsAtEnd()) { return _source[_current++]; }

        return '\0';
    }

    private char Peek()
    {
        if (!IsAtEnd()) { return _source[_current]; }

        return '\0';
    }

    private char PeekNext()
    {
        if (!IsAtEnd()) { return _source[_current + 1]; }

        return '\0';
    }

    private void SkipWhitespace()
    {
        for (char c = Peek(); c == ' ' || c == '\n' || c == '\t' || c == '\r'; c = Peek())
        {
            Advance();
        }
    }

    private static void PrintToken(Token token)
    {
        Console.WriteLine("{0}, {1}", token.Literal, token.Type.ToString());
    }

    public static void PrintTokens(List<Token> tokens)
    {
        foreach (var token in tokens) { PrintToken(token); }
    }
}