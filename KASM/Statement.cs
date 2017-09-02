using System.Runtime.InteropServices;

namespace KASM
{
    [StructLayout(LayoutKind.Explicit)]
    public class Statement
    {
        [FieldOffset(0)]
        public bool IsSpecial;
        
        [FieldOffset(1)]
        public Ast.SpecialType Special;
        
        [FieldOffset(1)]
        public Ast.InstructionType Instruction;

        [FieldOffset(8)] // TODO: Correctly aligned?
        public Token[] Arguments;
        
#if DEBUG

        public override string ToString()
        {
            return "<" + (IsSpecial ? Special.ToString() : Instruction.ToString()) + ">";
        }

#endif
    }
}
