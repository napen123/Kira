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

            switch (first)
            {
                case SpecialToken spec:
                    return ParseSpecial(spec, args);
                case InstructionToken inst:
                    return ParseInstruction(inst, args);
                default:
                    Error.ThrowError("Expected an instruction.");

                    return null;
            }
        }

        private static Statement ParseSpecial(SpecialToken first, IReadOnlyList<Token> args)
        {
            switch (first.Value)
            {
                case Ast.SpecialType.Local:
                {
                    if (args.Count != 1)
                    {
                        Error.ThrowError("Invalid number of arguments given to .local; expected 1 but got " +
                                         args.Count + ".");

                        break;
                    }

                    var local = args[0];

                    if (!(local is IdentifierToken))
                    {
                        Error.ThrowError("Invalid argument given to .local; expected an <identifier> but got " +
                                         local + ".");

                        break;
                    }

                    return new SpecialStatement(Ast.SpecialType.Local, new[] {local});
                }
            }

            return null;
        }

        private static Statement ParseInstruction(InstructionToken first, IReadOnlyList<Token> args)
        {
            switch (first.Value)
            {
                case Ast.InstructionType.Internal:
                {
                    if (args.Count != 2)
                    {
                        Error.ThrowError("Invalid number of arguments given to internal; expected 2 but got " +
                                         args.Count + ".");

                        break;
                    }

                    var @internal = args[0];

                    if (!(@internal is IdentifierToken))
                    {
                        Error.ThrowError("Invalid argument given to internal; expected an <identifier> but got " +
                                         @internal + ".");

                        break;
                    }

                    var code = args[1];

                    if (!(code is IntegerToken))
                    {
                        Error.ThrowError("Invalid argument given to internal; expected an <integer> but got " +
                                         code + ".");

                        break;
                    }

                    return new StandardStatement(Ast.InstructionType.Internal, new[] {@internal, code});
                }
                case Ast.InstructionType.External:
                {
                    if (args.Count != 2)
                    {
                        Error.ThrowError("Invalid number of arguments given to external; expected 2 but got " +
                                         args.Count + ".");

                        break;
                    }

                    var external = args[0];

                    if (!(external is IdentifierToken))
                    {
                        Error.ThrowError("Invalid argument given to external; expected an <identifier> but got " +
                                         external + ".");

                        break;
                    }

                    var library = args[1];

                    if (!(library is StringToken))
                    {
                        Error.ThrowError("Invalid argument given to external; expected a <string> but got " +
                                         library + ".");

                        break;
                    }

                    return new StandardStatement(Ast.InstructionType.External, new[] {external, library});
                }
                case Ast.InstructionType.Push:
                {
                    if (args.Count != 1)
                    {
                        Error.ThrowError("Invalid number of arguments given to push; expected 1 but got " +
                                         args.Count + ".");

                        break;
                    }

                    var item = args[0];

                    if (!item.Literal && !(item is IdentifierToken))
                    {
                        Error.ThrowError("Invalid argument given to external; expected a local name or literal but got " +
                                         item + ".");

                        break;
                    }

                    return new StandardStatement(Ast.InstructionType.Push, new[] {item});
                }
                case Ast.InstructionType.Pop:
                {
                    if (args.Count != 0)
                    {
                        Error.ThrowError("Invalid number of arguments given to push; expected 0 but got " +
                                         args.Count + ".");

                        break;
                    }

                    return new StandardStatement(Ast.InstructionType.Pop, null);
                }
                case Ast.InstructionType.Set:
                {
                    if (args.Count != 2)
                    {
                        Error.ThrowError("Invalid number of arguments given to call; expected 2 but got " +
                                         args.Count + ".");

                        break;
                    }

                    var item = args[0];

                    if (!(item is IdentifierToken))
                    {
                        Error.ThrowError("Invalid argument given to set; expected an <identifier> but got " +
                                         item + ".");

                        break;
                    }

                    var value = args[1];

                    if (!value.Literal && !(value is IdentifierToken))
                    {
                        Error.ThrowError("Invalid argument given to set; expected either a local name or literal " +
                                         "but got " + item + ".");

                        break;
                    }

                    return new StandardStatement(Ast.InstructionType.Set, new[] {item, value});
                }
                case Ast.InstructionType.Call:
                {
                    if (args.Count != 1)
                    {
                        Error.ThrowError("Invalid number of arguments given to call; expected 1 but got " +
                                         args.Count + ".");

                        break;
                    }

                    var func = args[0];

                    if (!(func is IdentifierToken))
                    {
                        Error.ThrowError("Invalid argument given to call; expected an <identifier> but got " +
                                         func + ".");

                        break;
                    }

                    return new StandardStatement(Ast.InstructionType.Call, new[] {func});
                }
            }

            return null;
        }
    }
}
