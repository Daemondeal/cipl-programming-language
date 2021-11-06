using System;
using System.IO;
using System.Text.RegularExpressions;
using CIPLSharp.Printers;

namespace CIPLSharp
{
    public static class Cipl
    {
        private static readonly Interpreter Interpreter = new Interpreter(); 
        private static bool hadError;
        private static bool hadRuntimeError;

        private static bool shouldReportToStdout;
        
        public static void Main(string[] args)
        {
            var filePath = "";
            
            var outputTokens = false;
            var outputAst = false;
            var showUsage = false;

            foreach (var arg in args)
            {
                if (arg.StartsWith("--"))
                {
                    switch (arg)
                    {
                        case "--tokens":
                            outputTokens = true;
                            break;
                        case "--ast":
                            outputAst = true;
                            break;
                        case "--help":
                            showUsage = true;
                            break;
                        case "--stdout":
                            shouldReportToStdout = true;
                            break;
                    }
                }
                else
                {
                    filePath = arg;
                }
            }

            if (showUsage)
            {
                Console.WriteLine("Usage: ciplsharp [script] [--tokens]");
            }
            else if (outputTokens)
            {
                if (filePath == "")
                    Console.WriteLine("Must provide a file to output tokens.");
                else
                    OutputTokens(filePath);
            } else if (outputAst)
            {
                if (filePath == "")
                    Console.WriteLine("Must provide a file to output ast.");
                else
                    OutputAst(filePath, new ParensPrinter(), "ast");
            }
            else
            {
                if (filePath == "")
                    RunPrompt();
                else
                    RunFile(filePath);
            }
        }

        private static void RunFile(string filePath)
        {
            var code = File.ReadAllText(filePath);
            Run(code);
            if (hadError) System.Environment.Exit(65);
            if (hadRuntimeError) System.Environment.Exit(70);
        }

        private static void RunPrompt()
        {
            while (true)
            {
                Console.Write("> ");
                var line = Console.ReadLine();
                if (line is null) break;
                
                Run(line);
                hadError = false;
            }
        }

        private static void OutputTokens(string filePath)
        {
            var source = File.ReadAllText(filePath);
            
            var scanner = new Scanner(source);
            var tokens = scanner.ScanTokens();
            if (hadError) System.Environment.Exit(65);


            if (shouldReportToStdout)
            {
                Console.WriteLine($"{"Token".PadRight(15)}{"Lexeme".PadRight(15)}{"Line".PadRight(5)}");
                foreach (var token in tokens)
                    Console.WriteLine(token.ToString());
            }
            else
            {
                using var outFile = new StreamWriter(filePath + ".tokens");
                outFile.WriteLine($"{"Token".PadRight(15)}{"Lexeme".PadRight(15)}{"Line".PadRight(5)}");
                foreach (var token in tokens)
                    outFile.WriteLine(token.ToString());
            }
        }
        
        private static void OutputAst(string filePath, AstPrinter printer, string extension)
        {
            var source = File.ReadAllText(filePath);
            
            var scanner = new Scanner(source);
            var tokens = scanner.ScanTokens();

            var parser = new Parser(tokens);
            var expr = parser.Parse();
            
            if (hadError) System.Environment.Exit(65);

            if (shouldReportToStdout)
                Console.WriteLine(printer.PrintStatements(expr));
            else
            {
                using var outFile = new StreamWriter(filePath + "." + extension);
                outFile.WriteLine(printer.PrintStatements(expr));
            }
        }

        private static void Run(string source)
        {
            var scanner = new Scanner(source);
            var tokens = scanner.ScanTokens();
            
            var parser = new Parser(tokens);
            var statements = parser.Parse();

            if (hadError) return;

            var resolver = new Resolver(Interpreter);
            resolver.Resolve(statements);

            if (hadError) return;

            Interpreter.Interpret(statements);
        }

        public static void Error(int line, string message)
        {
            Report(line, "", message);
        }

        public static void RuntimeError(RuntimeError error)
        {
            Console.Error.WriteLine(error.Message + " at line " + error.Token.Line);
            hadRuntimeError = true;
        }
        
        public static void Error(Token token, string message)
        {
            Report(token.Line, $" at `{Regex.Escape(token.Lexeme)}`", message);
        }

        private static void Report(int line, string where, string message)
        {
            hadError = true;
            Console.Error.WriteLine($"Error on line {line}{where}: {message}");
        }
    }
}