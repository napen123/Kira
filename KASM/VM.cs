namespace KASM
{
    public static class VM
    {
        public const byte Integer = 0x0;
        public const byte Float = 0x1;
        public const byte String = 0x2;
        
        public const int Push = 0x01;
        public const int Pop = 0x02;
        
        public const int Set = 0x03;

        public const int StandardCall = 0x10;
        public const int InternalCall = 0x11;
        public const int ExternalCall = 0x12;
        
        public static readonly int[] Internals =
        {
            0xF0,
            0xF1
        };
    }
}
