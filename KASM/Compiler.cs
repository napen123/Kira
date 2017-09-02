using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace KASM
{
    public static class Compiler
    {
        private static BinaryWriter _programWriter;

        private static readonly List<Local> Locals = new List<Local>();
        private static readonly List<string> Strings = new List<string>();
        private static readonly List<Function> Functions = new List<Function>();
        
        public static void Compile(string input, string output)
        {
            using (var realWriter = new BinaryWriter(File.Open(output, FileMode.Create)))
            {
                using (var stream = new MemoryStream())
                {
                    using (_programWriter = new BinaryWriter(stream))
                    {
                        using (var reader = new StreamReader(input))
                        {
                            string line;

                            while ((line = reader.ReadLine()) != null)
                            {
                                var trimmed = line.Trim();

                                if (trimmed.Length == 0 || trimmed.StartsWith("#", StringComparison.Ordinal))
                                    continue;

                                var tokens = Lexer.Tokenize(trimmed);

                                if (tokens.Count == 0)
                                    continue;

                                var statement = Parser.Parse(tokens);

#if DEBUG
                                foreach (var token in tokens)
                                    Console.WriteLine($"{token.Type} :: [{token.Value}]");

                                Console.WriteLine(statement);
#endif

                                if (statement.IsSpecial)
                                    ExecuteSpecial(statement);
                                else
                                    ExecuteInstruction(statement);
                            }
                        }
                        
                        realWriter.Write(Encoding.ASCII.GetBytes("KIRA"));
                        realWriter.Write(Locals.Count);
                        realWriter.Write(Strings.Count);

                        foreach (var str in Strings)
                        {
                            var bytes = Encoding.UTF8.GetBytes(str);
                        
                            realWriter.Write(bytes.Length);
                            realWriter.Write(bytes);
                        }
                    
                        realWriter.Write(stream.Length);

                        stream.Position = 0;
                        stream.CopyTo(realWriter.BaseStream);
                    }
                }
            }
        }
        
        private static Token GetLocalValue(string name)
        {
            foreach (var loc in Locals)
            {
                if (loc.Name != name)
                    continue;

                var value = loc.Value;

                return value.Type == TokenType.Identifier ? GetLocalValue(value.String) : value;
            }
                
            Error.ThrowError("No local with that name has been declared.");

            return new Token();
        }
        
        private static Token ValidateLiteral(Token token)
        {
            return token.Type != TokenType.Identifier ? token : GetLocalValue(token.String);
        }

        private static Token[] ValidateLiterals(IReadOnlyList<Token> tokens)
        {
            var ret = new Token[tokens.Count];

            for (var i = 0; i < tokens.Count; i++)
                ret[i] = ValidateLiteral(tokens[i]);

            return ret;
        }

        private static void CheckNameAvailable(string name)
        {
            if (Locals.TrueForAll(local => local.Name != name) && Functions.TrueForAll(func => func.Name != name))
                return;
            
            Error.ThrowError("Name already used in declaration.");
        }

        private static void WriteLiteral(Token token)
        {
            switch (token.Type)
            {
                case TokenType.Integer:
                    _programWriter.Write(VM.Integer);
                    _programWriter.Write(token.Integer);
                    break;
                case TokenType.Float:
                    _programWriter.Write(VM.Float);
                    _programWriter.Write(token.Float);
                    break;
                case TokenType.String:
                    var index = Strings.IndexOf(token.String);
                    
                    _programWriter.Write(VM.String);

                    if (index == -1)
                    {
                        Strings.Add(token.String);
                        _programWriter.Write(Strings.Count - 1);
                    }
                    else
                        _programWriter.Write(index);
                    break;
            }
        }

        private static void ExecuteSpecial(Statement statement)
        {
            switch (statement.Special)
            {
                case Ast.SpecialType.Local:
                {
                    var name = statement.Arguments[0].String;

                    CheckNameAvailable(name);
                    
                    Locals.Add(new Local(name, new Token {Type = TokenType.Integer, Integer = 0}));
                }
                    break;
            }
        }

        private static void ExecuteInstruction(Statement statement)
        {
            switch (statement.Instruction)
            {
                case Ast.InstructionType.Internal:
                {
                    var name = statement.Arguments[0].String;

                    CheckNameAvailable(name);
                    
                    var code = statement.Arguments[1].Integer;
                    
                    if(!VM.Internals.Contains(code))
                        Error.ThrowError("No such internal in VM.");
                    
                    Functions.Add(new Function
                    {
                        Name = name,
                        Type = FunctionType.Internal,
                        InternalCode = code
                    });
                }
                    break;
                case Ast.InstructionType.External:
                {
                    var name = statement.Arguments[0].String;
                    
                    CheckNameAvailable(name);
                    
                    var library = statement.Arguments[1].String;
                    
                    Functions.Add(new Function
                    {
                        Name = name,
                        Type = FunctionType.External,
                        Library = library
                    });
                }
                    break;
                case Ast.InstructionType.Push:
                {
                    var args = ValidateLiterals(statement.Arguments);
                    var value = args[0];

                    _programWriter.Write(VM.Push);
                    WriteLiteral(value);
                }
                    break;
                case Ast.InstructionType.Pop:
                    _programWriter.Write(VM.Pop);
                    break;
                case Ast.InstructionType.Set:
                {
                    var name = statement.Arguments[0].String;
                    var index = Locals.FindIndex(loc => loc.Name == name);
                    
                    if(index == -1)
                        Error.ThrowError("No literal has been declared with that name.");
                    
                    var value = ValidateLiteral(statement.Arguments[1]);

                    Locals[index] = new Local(name, value);
                    
                    _programWriter.Write(VM.Set);
                    _programWriter.Write(index);
                    WriteLiteral(value);
                }
                    break;
                case Ast.InstructionType.Call:
                {
                    var name = statement.Arguments[0].String;

                    var found = false;

                    foreach (var func in Functions)
                    {
                        if(func.Name != name)
                            continue;

                        found = true;

                        switch (func.Type)
                        {
                            case FunctionType.Standard:
                                _programWriter.Write(VM.StandardCall);
                                // TODO: Finish
                                break;
                            case FunctionType.Internal:
                                _programWriter.Write(VM.InternalCall);
                                _programWriter.Write(func.InternalCode);
                                break;
                            case FunctionType.External:
                                _programWriter.Write(VM.ExternalCall);
                                // TODO: Finish
                                break;
                        }

                        break;
                    }
                    
                    if(!found)
                        Error.ThrowError("No function has been declared with that name.");
                }
                    break;
            }
        }
    }
}
