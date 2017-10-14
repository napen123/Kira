
namespace KASM
{
    public abstract class Statement
    {
        public Token[] Arguments { get; }

        protected Statement(Token[] arguments)
        {
            Arguments = arguments;
        }
    }

    public class SpecialStatement : Statement
    {
        public readonly Ast.SpecialType Instruction;

        public SpecialStatement(Ast.SpecialType instruction, Token[] arguments)
            : base(arguments)
        {
            Instruction = instruction;
        }
    }

    public class StandardStatement : Statement
    {
        public readonly Ast.InstructionType Instruction;

        public StandardStatement(Ast.InstructionType instruction, Token[] arguments)
            : base(arguments)
        {
            Instruction = instruction;
        }
    }
}
