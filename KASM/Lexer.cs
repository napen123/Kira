using System.Collections.Generic;

namespace KASM
{
    public static class Lexer
    {
        public static List<Token> Tokenize(string line)
        {
            var ret = new List<Token>();
            
            for (var i = 0; i < line.Length; i++)
            {
                var c = line[i];

                if (c == '#')
                    break;
                if(char.IsWhiteSpace(c))
                    continue;

                if (c == '\"')
                {
                    var start = i;
                    var end = 1;

                    var terminated = false;

                    while (start + end < line.Length)
                    {
                        c = line[start + end];
                        
                        if (c == '\"')
                        {
                            end++;
                            terminated = true;

                            break;
                        }

                        end++;
                    }
                    
                    if(!terminated)
                        Error.ThrowError("Unterminated string.");

                    i = start + end - 1;
                    var str = line.Substring(start + 1, end - 2);

                    ret.Add(new StringToken(str));
                    
                    continue;
                }
                
                if (char.IsDigit(c))
                {
                    var start = i;
                    var end = 1;

                    var isInteger = true;
                    
                    while (start + end < line.Length)
                    {
                        c = line[start + end];

                        if (!char.IsDigit(c))
                        {
                            if (c == '.')
                            {
                                end++;
                                isInteger = false;

                                while (start + end < line.Length)
                                {
                                    c = line[start + end];

                                    if (!char.IsDigit(c))
                                        break;

                                    end++;
                                }
                            }
                            
                            break;
                        }

                        end++;
                    }

                    i = start + end - 1;
                    var value = line.Substring(start, end);

                    if (isInteger)
                        ret.Add(new IntegerToken(int.Parse(value)));
                    else
                        ret.Add(new FloatToken(float.Parse(value)));

                    continue;
                }

                if (c == '.')
                {
                    var start = i;
                    var end = 1;

                    while (start + end < line.Length)
                    {
                        c = line[start + end];

                        if (!char.IsLetter(c) && c != '_')
                            break;

                        end++;
                    }

                    i = start + end - 1;
                    var special = line.Substring(start + 1, end - 1);
                    
                    if(!Ast.Specials.TryGetValue(special, out var spec))
                        Error.ThrowError("Unknown special instruction.");
                    
                    ret.Add(new SpecialToken(spec));
                    
                    continue;
                }
                
                if (char.IsLetter(c) || c == '_')
                {
                    var start = i;
                    var end = 1;
                    
                    while (start + end < line.Length)
                    {
                        c = line[start + end];
                        
                        if (!char.IsLetterOrDigit(c) && c != '_')
                            break;

                        end++;
                    }

                    i = start + end - 1;
                    var ident = line.Substring(start, end);

                    if (Ast.Instructions.TryGetValue(ident, out var inst))
                        ret.Add(new InstructionToken(inst));
                    else
                        ret.Add(new IdentifierToken(ident));

                    continue;
                }
                
                Error.ThrowError("Unexpected token: " + c);
            }
            
            return ret;
        }
    }
}
