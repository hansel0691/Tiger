using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Antlr.Runtime;
using Tiger.AST_Nodes;
using Tiger.AST_Nodes.AST_Utils;
using Tiger.Tiger_Error;

namespace Tiger
{
    class Program
    {
        static void CheckFile(string path)
        {
            var lexer = new WrapperLexer(new ANTLRFileStream(path));
            var tokens = new CommonTokenStream(lexer);
            var parser = new WrapperParser(tokens) {TreeAdaptor = new Adaptor()};
            var adaptor = new Adaptor();
            var errors = new List<SemanticError>();
            var syntacticErrors = new List<TigerError>();
            parser.TreeAdaptor = adaptor;

            try
            {
                //Syntactic Analisis
                var expression = parser.program();

                if (parser.NumberOfSyntaxErrors > 0 || lexer.NumberOfSyntaxErrors > 0 || parser.Errors.Count > 0 || lexer.Errors.Count > 0)
                {
                    syntacticErrors.AddRange(lexer.Errors.Concat(parser.Errors).Cast<TigerError>());
                    syntacticErrors.ForEach(PrintError);
                    Environment.ExitCode = 1;
                }
                else
                {
                    var scope = new Scope();
                    scope.SetTypes();
                    scope.SetFunctions();
                    var ast = (LanguageNode) expression.Tree;
                    //Semantic Analisis
                    ast.CheckSemantics(scope, errors);
                    if (errors.Count > 0 )
                        errors.ForEach(PrintError);
                    else
                    {
                        //Generate Code.
                        Console.WriteLine("No semantic error found.");
                        GenerateCode(ast, path);
                        Environment.ExitCode = 0;
                        Console.WriteLine("Successfull Code Generation");
                    }
                }
            }
            catch (Exception)
            {
                errors.ForEach(PrintError);
            }
        }

        static void PrintError(TigerError error)
        {
            var header = error is SyntacticError ? "Syntactic Error: " : "Semantic Error: ";
            Console.WriteLine("{0}({1},{2}): {3}", header, error.Line, error.Column, error.Message);
            Environment.ExitCode = 1;
        }

        public static void GenerateCode(LanguageNode root, string path)
        {
            var s = new Symbols(path);
            var generator = s.MainMethod.GetILGenerator();

            root.Generate(generator, s);
            generator.Emit(OpCodes.Ret);
            s.ProgramType.CreateType();
            s.Assembly.SetEntryPoint(s.MainMethod);
            s.Assembly.Save(s.AssemblyName.Name + ".exe");
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Tiger Compiler version 1.0");
            Console.WriteLine("Copyright (C) 2013-2014 Hansel García");

            //input checkups
            if (args.Length != 1)
            {
                Console.WriteLine("(0,0): Invalid number of arguments");
                return;
            }
            if (!File.Exists(args[0]))
            {
                Console.WriteLine(string.Format("(0,0):File 'C:\\{0} cannot be found.'", args[0]));
                return;
            }
            if (Path.GetExtension(args[0]) != ".tig")
            {
                Console.WriteLine(string.Format("(0,0):Extension 'Invalid extension.'"));
                return;
            }
            CheckFile(args[0]);

            //CheckFile("fail.tig");
        }
    }

}
