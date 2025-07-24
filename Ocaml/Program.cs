using System;

class Program
{
    static void Main()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== Menú compilador OCaml ===");
            Console.WriteLine("1) Tokenizar código (Lexer)");
            Console.WriteLine("2) Parsear código (Parser)");
            Console.WriteLine("3) Salir");
            Console.Write("Elige una opción: ");

            var option = Console.ReadLine();

            if (option == "3")
                break;

            Console.WriteLine("\nIntroduce el código OCaml a analizar (ENTER dos veces para terminar):");

            string line;
            bool lastLineEmpty = false;
            var codeBuilder = new System.Text.StringBuilder();

            while (true)
            {
                line = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(line))
                {
                    if (lastLineEmpty) break; // Doble ENTER detectado, termina entrada
                    lastLineEmpty = true;
                }
                else
                {
                    lastLineEmpty = false;
                    codeBuilder.AppendLine(line);
                }
            }

            string code = codeBuilder.ToString();

            switch (option)
            {
                case "1":
                    RunLexer(code);
                    break;

                case "2":
                    RunParser(code);
                    break;

                default:
                    Console.WriteLine("Opción inválida.");
                    break;
            }

            Console.WriteLine("\nPresiona una tecla para volver al menú...");
            Console.ReadKey();
        }
    }

    static void RunLexer(string code)
    {
        var lexer = new OcamlLexer(code);
        Console.WriteLine("\nTokens encontrados:\n");

        foreach (var token in lexer.Tokenize())
        {
            var color = Console.ForegroundColor;
            if (token.Type == TokenType.Error) Console.ForegroundColor = ConsoleColor.Red;

            Console.WriteLine($"{token.Line}:{token.Column} {token.Type} → '{token.Lexeme}'");

            Console.ForegroundColor = color;
        }
    }

    static void RunParser(string code)
    {
        var lexer = new OcamlLexer(code);
        var tokens = lexer.Tokenize();

        var parser = new Parser(tokens);
        parser.Parse();
    }
}
