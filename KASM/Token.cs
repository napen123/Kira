
namespace KASM
{
    public abstract class Token
    {
        public abstract bool Literal { get; }
        
        public override string ToString()
        {
            return "<unknown>";
        }
    }

    public class IntegerToken : Token
    {
        public readonly int Value;

        public override bool Literal { get; } = true;

        public IntegerToken(int value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return $"<integer: {Value:D}>";
        }
    }

    public class FloatToken : Token
    {
        public readonly float Value;

        public override bool Literal { get; } = true;

        public FloatToken(float value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return $"<float: {Value:F}>";
        }
    }

    public class StringToken : Token
    {
        public readonly string Value;

        public override bool Literal { get; } = true;
        
        public StringToken(string value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return $"<string: \"{Value}\">";
        }
    }

    public class InstructionToken : Token
    {
        public readonly Ast.InstructionType Value;

        public override bool Literal { get; } = false;
        
        public InstructionToken(Ast.InstructionType value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return $"<instruction: {Value.ToString()}>";
        }
    }
    
    public class SpecialToken : Token
    {
        public readonly Ast.SpecialType Value;

        public override bool Literal { get; } = false;
        
        public SpecialToken(Ast.SpecialType value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return $"<special: {Value.ToString()}>";
        }
    }
    
    public class IdentifierToken : Token
    {
        public readonly string Value;

        public override bool Literal { get; } = false;
        
        public IdentifierToken(string value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return $"<identifier: {Value}>";
        }
    }
}
