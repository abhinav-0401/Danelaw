global using Mercia.Lexing;
global using Mercia.Parsing;

namespace Mercia;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length > 1)
        {
            Console.WriteLine("Usage: mercia [script]");
            Environment.Exit(64);
        }
        else if (args.Length == 1){ RunFile(args[0]); }
        else { Repl(); }
    }

    static void PrintArgs(string[] args)
    {
        foreach (var arg in args) { Console.WriteLine("{0}", arg); }
    }

    static void RunFile(string path)
    {
        try
        {
            string source = File.ReadAllText(path);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            Environment.Exit(64);
        }
    }

    static void Repl()
    {
        for (;;)
        {
            Console.Write("> ");
            string? line = Console.ReadLine();
            if (line is null) { Environment.Exit(0); }

            if (line == "exit") { return; }
            
            Lexer lexer = new(line);
            var tokens = lexer.Lex();
            //Lexer.PrintTokens(tokens);
            Parser parser = new(tokens);
            parser.Parse();
        }
    }
}