namespace KASM
{
    public class Local
    {
        public string Name;
        public Token Value;

        public Local(string name, Token value)
        {
            Name = name;
            Value = value;
        }
    }
}
