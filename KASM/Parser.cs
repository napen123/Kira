using System.Linq;
using System.Collections.Generic;

namespace KASM
{
    public static class Parser
    {
        public static Statement Parse(List<Token> tokens)
        {
            var first = tokens[0];
            var args = tokens.Skip(1).ToArray();

            switch (first.Type)
            {
                case TokenType.Special:
                    return ParseSpecial(first, args);
                case TokenType.Instruction:
                    return ParseInstruction(first, args);
                default:
                    Error.ThrowError("Expected an instruction");

                    return null;
            }
        }

        private static Statement ParseSpecial(Token first, IReadOnlyList<Token> args)
        {
            var ret = new Statement {IsSpecial = true};

            switch (first.Special)
            {
                case Ast.SpecialType.Local:
                {
                    if (args.Count != 1)
                    {
                        Error.ThrowError("Invalid number of arguments given to .local; expected 1 but got " +
                                         args.Count + ".");
                    }

                    var local = args[0];

                    if (local.Type != TokenType.Identifier)
                    {
                        Error.ThrowError("Invalid argument given to .local; expected an identifier but got " +
                                         local.Type + ".");
                    }

                    ret.Special = Ast.SpecialType.Local;
                    ret.Arguments = new[] {local};
                }
                    break;
            }

            return ret;
        }

        private static Statement ParseInstruction(Token first, IReadOnlyList<Token> args)
        {
            var ret = new Statement();

            switch (first.Instruction)
            {
                case Ast.InstructionType.Internal:
                {
                    if (args.Count != 2)
                    {
                        Error.ThrowError("Invalid number of arguments given to internal; expected 2 but got " +
                                         args.Count + ".");
                    }

                    var @internal = args[0];

                    if (@internal.Type != TokenType.Identifier)
                    {
                        Error.ThrowError("Invalid argument given to internal; expected an identifier but got " +
                                         @internal.Type + ".");
                    }

                    var code = args[1];
                    
                    if (code.Type != TokenType.Integer)
                    {
                        Error.ThrowError("Invalid argument given to internal; expected an integer but got " +
                                         @internal.Type + ".");
                    }

                    ret.Instruction = Ast.InstructionType.Internal;
                    ret.Arguments = new[] {@internal, code};
                }
                    break;
                case Ast.InstructionType.External:
                {
                    if (args.Count != 2)
                    {
                        Error.ThrowError("Invalid number of arguments given to external; expected 2 but got " +
                                         args.Count + ".");
                    }

                    var external = args[0];

                    if (external.Type != TokenType.Identifier)
                    {
                        Error.ThrowError("Invalid argument given to external; expected an identifier but got " +
                                         external.Type + ".");
                    }

                    var library = args[1];

                    if (library.Type != TokenType.String)
                    {
                        Error.ThrowError("Invalid argument given to external; expected a string but got " +
                                         library.Type + ".");
                    }

                    ret.Instruction = Ast.InstructionType.External;
                    ret.Arguments = new[] {external, library};
                }
                    break;
                case Ast.InstructionType.Push:
                {
                    if (args.Count != 1)
                    {
                        Error.ThrowError("Invalid number of arguments given to push; expected 1 but got " +
                                         args.Count + ".");
                    }

                    var item = args[0];

                    if (!item.Literal)
                    {
                        Error.ThrowError("Invalid argument given to external; expected a literal but got " +
                                         item.Type + ".");
                    }

                    ret.Instruction = Ast.InstructionType.Push;
                    ret.Arguments = new[] {item};
                }
                    break;
                case Ast.InstructionType.Pop:
                {
                    if (args.Count != 0)
                    {
                        Error.ThrowError("Invalid number of arguments given to push; expected 0 but got " +
                                         args.Count + ".");
                    }

                    ret.Instruction = Ast.InstructionType.Pop;
                    ret.Arguments = null;
                }
                    break;
                case Ast.InstructionType.Set:
                {
                    if (args.Count != 2)
                    {
                        Error.ThrowError("Invalid number of arguments given to call; expected 2 but got " +
                                         args.Count + ".");
                    }

                    var item = args[0];

                    if (item.Type != TokenType.Identifier)
                    {
                        Error.ThrowError("Invalid argument given to set; expected an identifier but got " +
                                         item.Type + ".");
                    }

                    var value = args[1];

                    if (!value.Literal)
                    {
                        Error.ThrowError("Invalid argument given to set; expected either an identifier or literal " +
                                         "but got " + item.Type + ".");
                    }

                    ret.Instruction = Ast.InstructionType.Set;
                    ret.Arguments = new[] {item, value};
                }
                    break;
                case Ast.InstructionType.Call:
                {
                    if (args.Count != 1)
                    {
                        Error.ThrowError("Invalid number of arguments given to call; expected 1 but got " +
                                         args.Count + ".");
                    }

                    var func = args[0];

                    if (func.Type != TokenType.Identifier)
                    {
                        Error.ThrowError("Invalid argument given to call; expected an identifier but got " +
                                         func.Type + ".");
                    }

                    ret.Instruction = Ast.InstructionType.Call;
                    ret.Arguments = new[] {func};
                }
                    break;
            }

            return ret;
        }
    }
}
