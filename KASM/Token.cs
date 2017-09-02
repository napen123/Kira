using System;
using System.Runtime.InteropServices;

namespace KASM
{
    public enum TokenType
    {
        Integer,
        Float,
        String,
        
        Instruction,
        Special,
        Identifier
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct Token
    {
        [FieldOffset(0)]
        public TokenType Type;

        [FieldOffset(4)]
        public int Integer;

        [FieldOffset(4)]
        public float Float;

        [FieldOffset(4)]
        public Ast.SpecialType Special;
        
        [FieldOffset(4)]
        public Ast.InstructionType Instruction;

        [FieldOffset(8)]
        public string String;

        public bool Literal =>
            Type == TokenType.Integer ||
            Type == TokenType.Float ||
            Type == TokenType.String ||
            Type == TokenType.Identifier;
        
#if DEBUG

        public object Value
        {
            get
            {
                switch (Type)
                {
                    case TokenType.Integer:
                        return Integer;
                    case TokenType.Float:
                        return Float;
                    case TokenType.Special:
                        return Special;
                    case TokenType.Instruction:
                        return Instruction;
                    default:
                        return String;
                }
            }
        }
        
#endif
    }
}
