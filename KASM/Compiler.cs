using System;
using System.IO;
using System.Linq;
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
            using (var memory = new MemoryStream())
            {
                using (_programWriter = new BinaryWriter(memory))
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
                            
                            foreach(var token in tokens)
                                Console.WriteLine(token.ToString());

                            var statement = Parser.Parse(tokens);

                            switch (statement)
                            {
                                case SpecialStatement spec:
                                    ExecuteSpecial(spec);
                                    break;
                                case StandardStatement stan:
                                    ExecuteStandard(stan);
                                    break;
                            }
                        }
                    }

                    using (var bytecodeWriter = new BytecodeWriter(File.Open(output, FileMode.Create)))
                        bytecodeWriter.WriteFile(Locals.Count, Strings.ToArray(), memory);
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

                return value is IdentifierToken ident ? GetLocalValue(ident.Value) : value;
            }
                
            Error.ThrowError("No local with that name has been declared.");

            return null;
        }
        
        private static Token ValidateLiteral(Token token)
        {
            return token is IdentifierToken ident ? GetLocalValue(ident.Value) : token;
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
            switch (token)
            {
                case IntegerToken i:
                    _programWriter.Write(VM.Integer);
                    _programWriter.Write(i.Value);
                    break;
                case FloatToken f:
                    _programWriter.Write(VM.Float);
                    _programWriter.Write(f.Value);
                    break;
                case StringToken s:
                    var index = Strings.IndexOf(s.Value);
                    
                    _programWriter.Write(VM.String);

                    if (index == -1)
                    {
                        Strings.Add(s.Value);
                        _programWriter.Write(Strings.Count - 1);
                    }
                    else
                        _programWriter.Write(index);
                    
                    break;
            }
        }

        private static void ExecuteSpecial(SpecialStatement statement)
        {
            switch (statement.Instruction)
            {
                case Ast.SpecialType.Local:
                {
                    if (!(statement.Arguments[0] is IdentifierToken name))
                        return;

                    CheckNameAvailable(name.Value);
                    
                    Locals.Add(new Local(name.Value, new IntegerToken(0)));
                }
                    break;
            }
        }

        private static void ExecuteStandard(StandardStatement statement)
        {
            switch (statement.Instruction)
            {
                case Ast.InstructionType.Internal:
                {
                    if (!(statement.Arguments[0] is IdentifierToken name))
                        return;

                    CheckNameAvailable(name.Value);
                    
                    if (!(statement.Arguments[1] is IntegerToken code))
                        return;
                    
                    if (!VM.Internals.Contains(code.Value))
                    {
                        Error.ThrowError("No such internal in VM.");

                        break;
                    }

                    Functions.Add(new Function
                    {
                        Name = name.Value,
                        Type = FunctionType.Internal,
                        InternalCode = code.Value
                    });
                }
                    break;
                case Ast.InstructionType.External:
                {
                    if (!(statement.Arguments[0] is IdentifierToken name))
                        return;

                    CheckNameAvailable(name.Value);
                    
                    if (!(statement.Arguments[1] is StringToken library))
                        return;
                    
                    Functions.Add(new Function
                    {
                        Name = name.Value,
                        Type = FunctionType.External,
                        Library = library.Value
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
                    if (!(statement.Arguments[0] is IdentifierToken name))
                        return;
                    
                    var index = Locals.FindIndex(loc => loc.Name == name.Value);
                    
                    if(index == -1)
                        Error.ThrowError("No literal has been declared with that name.");
                    
                    var value = ValidateLiteral(statement.Arguments[1]);

                    Locals[index] = new Local(name.Value, value);
                    
                    _programWriter.Write(VM.Set);
                    _programWriter.Write(index);
                    WriteLiteral(value);
                }
                    break;
                case Ast.InstructionType.Call:
                {
                    if (!(statement.Arguments[0] is IdentifierToken name))
                        return;
                    
                    var found = false;

                    foreach (var func in Functions)
                    {
                        if(func.Name != name.Value)
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
