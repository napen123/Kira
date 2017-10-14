namespace KASM
{
    public enum FunctionType
    {
        Standard,
        Internal,
        External
    }
    
    public abstract class Function
    {
        public abstract string Name { get; }
    }

    public class StandardFunction : Function
    {
        public override string Name { get; }

        public StandardFunction(string name)
        {
            Name = name;
        }
    }

    public class InternalFunction : Function
    {
        public readonly int Code;
        
        public override string Name { get; }
        
        public InternalFunction(string name, int code)
        {
            Name = name;
            Code = code;
        }
    }

    public class ExternalFunction : Function
    {
        public readonly string Library;
    
        public override string Name { get; }
    
        public ExternalFunction(string name, string library)
        {
            Name = name;
            Library = library;
        }
    }
}
