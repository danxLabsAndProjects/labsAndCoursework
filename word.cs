using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    partial class Parser
    {
        private class Word
        {

            public string Token { get; set; }
            public int Column { get; set; }
            public int Row { get; set; }
            private string type;

            


            public Word(string token, int column, int row)
            {
                this.Token = token;
                this.Column = column;
                this.Row = row;
                this.type = determinateType(token);
            }

            public Word(Word word)
            {
                this.Token = word.Token;
                this.Column = word.Column;
                this.Row = word.Row;
                this.type = word.GetType();
            }
            public override string ToString()
            {
                return "\t" + Token + " " + Column + " " + Row + " " + type;
            }

            public string GetType()
            {
                return type;
            }


            private string determinateType(string lexem)
            {
                lexem = lexem.ToLower();

                string[] registers32 = { "eax", "ebx", "ecx", "edx" };
                string[] registers8 = { "ah", "al", "bh", "bl", "ch", "cl", "dh", "dl" };
                string[] commands = { "stc", "inc", "dec", "sbb", "and", "mov", "or", "jnle" };
                string[] derectives = { "segment", "ends", "end" };
                string[] typesOfVars = { "db", "dw", "dd", "byte", "word", "dword" };
                string[] segmentRegistres = { "cs", "ds", "ss", "es" };
                string[] singleSymbols = { "+", "-", "/", "*", "[", "]", ":", ";", "," };

                bool determinated = false;

                foreach (string str in registers32)
                {
                    if (String.Compare(str, lexem) == 0)
                    {
                        determinated = true;
                    }
                }
                if (determinated)
                {
                    return typeArr[0];
                }

                foreach (string str in registers8)
                {
                    if (String.Compare(str, lexem) == 0)
                    {
                        determinated = true;
                    }
                }
                if (determinated)
                {
                    return typeArr[1];
                }

                foreach (string str in commands)
                {
                    if (String.Compare(str, lexem) == 0)
                    {
                        determinated = true;
                    }
                }
                if (determinated)
                {
                    return typeArr[2];
                }

                foreach (string str in derectives)
                {
                    if (String.Compare(str, lexem) == 0)
                    {
                        determinated = true;
                    }
                }
                if (determinated)
                {
                    return typeArr[3];
                }

                foreach (string str in typesOfVars)
                {
                    if (String.Compare(str, lexem) == 0)
                    {
                        determinated = true;
                    }
                }
                if (determinated)
                {
                    return typeArr[4];
                }

                foreach (string str in segmentRegistres)
                {
                    if (String.Compare(str, lexem) == 0)
                    {
                        determinated = true;
                    }
                }
                if (determinated)
                {
                    return typeArr[5];
                }

                foreach (string str in singleSymbols)
                {
                    if (String.Compare(str, lexem) == 0)
                    {
                        determinated = true;
                    }
                }
                if (determinated)
                {
                    return typeArr[6];
                }

                if (String.Compare("ptr", lexem) == 0)
                {
                    return typeArr[7];
                }

                char ch = lexem[0];
                if (Char.IsLetter(ch) && lexem.Length <= 4)
                {
                    return typeArr[8];
                }

                if (Char.IsDigit(ch))
                {
                    return determinateTypeConstants(lexem);
                }


                return typeArr[typeArr.Length - 1];


            }

            private string determinateTypeConstants(string lexem)
            {
                char[] ch = lexem.ToCharArray();

                string hexLetters = "abcdef";

                if (Char.IsDigit(ch[0]))
                {
                    int j = 0;

                    for (int i = 1; i < ch.Length; i++)
                    {
                        if (Char.IsDigit(ch[i]))
                        {
                            j++;
                        }
                    }

                    if (j == ch.Length - 1)
                    {
                        return typeArr[10];
                    }
                    if (j == ch.Length - 2 && ch[j + 1] == 'd')
                    {
                        return typeArr[10];
                    }
                }

                if (ch[0] == '0' || ch[0] == '1')
                {
                    int j = 0;

                    for (int i = 1; i < ch.Length; i++)
                    {
                        if (ch[i] == '0' || ch[i] == '1')
                        {
                            j++;
                        }
                    }


                    if (j == ch.Length - 2 && ch[j + 1] == 'b')
                    {
                        return typeArr[9];
                    }
                }

                if (ch[0] == '0')
                {
                    int j = 0;

                    for (int i = 1; i < ch.Length; i++)
                    {
                        if (Char.IsDigit(ch[i]) || hexLetters.Contains(ch[i].ToString()))
                        {
                            j++;
                        }
                    }


                    if (j == ch.Length - 2 && ch[j + 1] == 'h')
                    {
                        return typeArr[11];
                    }
                }

                return typeArr[typeArr.Length - 1];
            }
        }
    }
}
