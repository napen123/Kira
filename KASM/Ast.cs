using System.Collections.Generic;

namespace KASM
{
    public static class Ast
    {
        public enum SpecialType
        {
            Local
        }
        
        public enum InstructionType
        {
            Internal,
            External,
            
            Push,
            Pop,
            
            Set,
            
            Call
        }

        public static readonly Dictionary<string, SpecialType> Specials = new Dictionary<string, SpecialType>
        {
            ["local"] = SpecialType.Local
        };

        public static readonly Dictionary<string, InstructionType> Instructions =
            new Dictionary<string, InstructionType>
            {
                ["internal"] = InstructionType.Internal,
                ["external"] = InstructionType.External,

                ["push"] = InstructionType.Push,
                ["pop"] = InstructionType.Pop,
                
                ["set"] = InstructionType.Set,

                ["call"] = InstructionType.Call
            };
    }
}
