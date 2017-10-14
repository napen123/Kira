using System;
using System.IO;
using System.Text;

namespace KASM
{
    public class BytecodeWriter : IDisposable
    {
        private readonly BinaryWriter _writer;

        public BytecodeWriter(Stream stream)
        {
            _writer = new BinaryWriter(stream);
            _writer.Write(Encoding.ASCII.GetBytes("KIRA"));
        }
        
        public void WriteFile(int localCount, string[] strings, Stream stream)
        {
            _writer.Write(localCount);
            _writer.Write(strings.Length);

            foreach (var str in strings)
            {
                var bytes = Encoding.UTF8.GetBytes(str);
                        
                _writer.Write(bytes.Length);
                _writer.Write(bytes);
            }
            
            _writer.Write(stream.Length);

            stream.Position = 0;
            stream.CopyTo(_writer.BaseStream);
        }
        
        public void Dispose()
        {
            _writer.Dispose();
        }
    }
}