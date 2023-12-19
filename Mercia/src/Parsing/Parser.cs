using System.Text.Json;

namespace Mercia.Parsing;

enum Precedence
{
    None,
    Assignment,
    Conditional,
    Sum,
    Product,
    Exponent,
    Prefix,
    Call,
}

public class Parser
{
    private int _current = 0;
    private List<Token> _tokens;

    private delegate Expr PrefixParser(Token token, Precedence precedence);
    private delegate Expr InfixParser(Token token, Expr left, Precedence precedence);

    private Dictionary<TokenType, PrefixParser> _prefixParsers;
    private Dictionary<TokenType, InfixParser> _infixParsers;
    private Dictionary<TokenType, Precedence> _precedence = new()
    {
        { TokenType.Number, Precedence.None },
        { TokenType.Equal, Precedence.Assignment },
        { TokenType.Plus, Precedence.Sum },
        { TokenType.Minus, Precedence.Sum },
        { TokenType.Star, Precedence.Product },
        { TokenType.Slash, Precedence.Product },
    };

    public Parser(List<Token> tokens)
    {
        _tokens = tokens;
        _prefixParsers = new()
        {
            { TokenType.Minus, ParseUnary },
            { TokenType.Number, ParseNumber },
            { TokenType.LeftParen, ParseGrouping },
        };
        _infixParsers = new()
        {
            { TokenType.Plus, ParseBinary },
            { TokenType.Minus, ParseBinary },
            { TokenType.Star, ParseBinary },
            { TokenType.Slash, ParseBinary },
        };
    }

    public void Parse()
    {
        Expr? expr = null;
        while (Peek().Type != TokenType.Eof)
        {
            expr = ParseExpr(Precedence.Assignment);
        }
        JsonSerializerOptions jsonOptions = new() { WriteIndented = true }; 
        string exprJson = JsonSerializer.Serialize(expr, jsonOptions);
        Console.WriteLine("{0}", exprJson);
    }

    private Expr ParseExpr(Precedence precedence)
    {
        Token op = Advance();
        if (!_prefixParsers.TryGetValue(op.Type, out var prefixParser)) 
        { throw new Exception("parsing not happening"); }

        Expr left = prefixParser(op, precedence);
        if (left is null) { throw new Exception("Expected an expression"); }

        while (precedence < GetPrecedence())
        {
            op = Advance();
            if (!_infixParsers.TryGetValue(op.Type, out var infixParser))
            { throw new Exception("parsing not happening"); }

            left = infixParser(op, left, precedence + 1);
            if (left is null) { throw new Exception("Expected an expression"); }
        }

        return left;
    }

    private BinaryExpr ParseBinary(Token token, Expr left, Precedence precedence)
    {
        Token op = token;
        Expr right = ParseExpr(precedence);
        return new BinaryExpr(op, left, right);
    }

    private UnaryExpr ParseUnary(Token token, Precedence precedence)
    {
        Token op = token;
        Expr right = ParseExpr(precedence);
        return new UnaryExpr(op, right);
    }

    private Expr ParseNumber(Token token, Precedence precedence)
    {
        return new NumericLiteral(token);
    }

    private Expr ParseGrouping(Token token, Precedence precedence)
    {
        Expr expr = ParseExpr(precedence);
        Match(TokenType.RightParen, "Expect closing parenthesis"); // )
        return expr;
    }

    private Precedence GetPrecedence()
    {
        _precedence.TryGetValue(Peek().Type, out var precedence);
        return precedence;
    }

    private Token Advance()
    {
        return _tokens[_current++];
    }

    private Token Peek()
    {
        return _tokens[_current];
    }

    private Token Match(TokenType type, string error)
    {
        if (Peek().Type == type) 
        { 
            var token = Advance();
            return token;
        }

        throw new Exception(error);
    }
}