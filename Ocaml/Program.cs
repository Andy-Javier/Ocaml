using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public enum TokenType
{
    Keyword, Identifier,
    Int, Float,
    String, Char,
    Operator, Delimiter,
    Error, EOF
}

public class Token
{
    public TokenType Type { get; }
    public string Lexeme { get; }
    public int Line { get; }
    public int Column { get; }

    public Token(TokenType type, string lexeme, int line, int column)
    {
        Type = type;
        Lexeme = lexeme;
        Line = line;
        Column = column;
    }
}

public class Lexer
{
    private static readonly HashSet<string> Keywords = new() {
        "let","in","match","with","fun","type","if","then","else","rec","module","open"
    };

    private static readonly string Patterns = @"
        (?<Whitespace>\s+)
      | (?<Comment>\(\*.*?\*\))
      | (?<String>""([^""\\]|\\.)*"")
      | (?<Char>'([^'\\]|\\.)')
      | (?<Float>\d+\.\d+)
      | (?<Int>\d+)
      | (?<Ident>[a-zA-Z_][a-zA-Z0-9_']*)
      | (?<Op>==|->|:=|::|\+|\-|\*|\/|=|<>|<|>|&&|\|\|)
      | (?<Delim>[()\[\]{};|,])
      | (?<Error>.)
    ";

    private readonly Regex regex = new(Patterns, RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);
    private readonly string source;
    private int index, line = 1, col = 1;

    public Lexer(string src) { source = src; }

    public IEnumerable<Token> Tokenize()
    {
        foreach (Match m in regex.Matches(source))
        {
            if (m.Groups["Whitespace"].Success || m.Groups["Comment"].Success)
            {
                Advance(m.Value); continue;
            }

            string lex = m.Value;
            TokenType type = TokenType.Identifier;

            if (m.Groups["Int"].Success) type = TokenType.Int;
            else if (m.Groups["Float"].Success) type = TokenType.Float;
            else if (m.Groups["String"].Success) type = TokenType.String;
            else if (m.Groups["Char"].Success) type = TokenType.Char;
            else if (m.Groups["Ident"].Success)
                type = Keywords.Contains(lex) ? TokenType.Keyword : TokenType.Identifier;
            else if (m.Groups["Op"].Success) type = TokenType.Operator;
            else if (m.Groups["Delim"].Success) type = TokenType.Delimiter;
            else if (m.Groups["Error"].Success) type = TokenType.Error;

            yield return new Token(type, lex, line, col);
            Advance(lex);
        }

        yield return new Token(TokenType.EOF, "", line, col);
    }

    private void Advance(string txt)
    {
        foreach (char c in txt)
        {
            if (c == '\n') { line++; col = 1; }
            else col++;
            index++;
        }
    }
}

class Program
{
    static void Main()
    {
        Console.WriteLine("Introduce el código OCaml a analizar (ENTER dos veces para terminar):");

        string line;
        string code = "";
        while (true)
        {
            line = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(line)) break;
            code += line + "\n";
        }

        Console.WriteLine("\nTokens encontrados:\n");

        var lexer = new Lexer(code);
        foreach (var token in lexer.Tokenize())
        {
            ConsoleColor color = Console.ForegroundColor;
            if (token.Type == TokenType.Error) Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{token.Line}:{token.Column} {token.Type} → '{token.Lexeme}'");
            Console.ForegroundColor = color;
        }

        Console.WriteLine("\nPresiona una tecla para salir...");
        Console.ReadKey();
    }
}
