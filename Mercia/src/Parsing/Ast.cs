using System.Text.Json.Serialization;

namespace Mercia.Parsing;

enum NodeType
{
    Stmt,
    Expr,
    UnaryExpr,
    BinaryExpr,
    NumericLiteral,
}

abstract class Stmt
{
    public NodeType StmtType { get; set; }
}

[JsonDerivedType(typeof(UnaryExpr))]
[JsonDerivedType(typeof(BinaryExpr))]
[JsonDerivedType(typeof(NumericLiteral))]
abstract class Expr : Stmt
{
    public NodeType ExprType { get; set; }
}

class UnaryExpr : Expr
{
    public Token Operator { get; init; }
    public Expr Right { get; init; }

    public UnaryExpr(Token op, Expr right)
    {
        Operator = op;
        Right = right;
        StmtType = ExprType = NodeType.UnaryExpr;
    }

    public override string ToString()
    {
        return string.Format("NodeType: {0}\nOperator: {1}\nRight: \t{2}", ExprType, Operator.Literal, Right.ToString());
    }
}

class BinaryExpr : Expr
{
    public Token Operator { get; init; }
    public Expr Left { get; init; }
    public Expr Right { get; init; }

    public BinaryExpr(Token op, Expr left, Expr right)
    {
        Operator = op;
        Left = left;
        Right = right;
        StmtType = ExprType = NodeType.BinaryExpr;
    }
}

class NumericLiteral : Expr
{
    public Token Literal { get; init; }

    public NumericLiteral(Token literal) 
    { 
        Literal = literal;
        StmtType = ExprType = NodeType.NumericLiteral;
    }

    public override string ToString()
    {
        return String.Format("NodeType: {0}\nLiteral: {1}", ExprType, Literal.Literal);
    }

}