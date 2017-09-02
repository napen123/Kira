using System.Runtime.InteropServices;

namespace KASM
{
    public enum FunctionType
    {
        Standard,
        Internal,
        External
    }
    
    [StructLayout(LayoutKind.Auto)]
    public struct Function
    {
        public FunctionType Type;
        public int InternalCode;       
     
        public string Name;
        public string Library;
        public Statement[] Statements;
    }
}
