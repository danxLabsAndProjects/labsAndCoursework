using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ConsoleApp1
{
   partial class Parser
    {

        private List<Word> tokenList = new List<Word>();

        public static string[] typeArr =
            {
            "register32",
            "register8",
            "command",
            "derective",
            "constant type",
            "segment register",
            "single symbol",
            "type identeficator",
            "identeficator",
            "binary constant",
            "decimal constant",
            "hexadecimal constant",
            "undefined type"
            };


        public void Pars(string FileName)
        {
            using (FileStream file = new FileStream(FileName, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader reader = new StreamReader(file))
                {



                    char ch = '\0';
                    int column = 0, row = 0;

                    string dividers = "+-:/*[],";

                    if (!reader.EndOfStream)
                    {
                        ch = (char)reader.Read();
                        column++;
                    }
                    while (!reader.EndOfStream)
                    {

                        if (ch == ' ')
                        {
                            row++;
                            ch = (char)reader.Read();
                        }

                        if (Char.IsDigit(ch))//const
                        {
                            string lexem = null;
                            row++;

                            while (Char.IsLetterOrDigit(ch))
                            {
                                lexem += ch;
                                ch = (char)reader.Read();
                            }
                            tokenList.Add(new Word(lexem, column, row));
                            row += lexem.Length; //TODO lexem.Length - 1;
                        }

                        if (Char.IsLetter(ch))//names or comands
                        {
                            string lexem = null;
                            row++;

                            while (Char.IsLetterOrDigit(ch))
                            {
                                lexem += ch;
                                ch = (char)reader.Read();

                            }
                            tokenList.Add(new Word(lexem, column, row));
                            row += lexem.Length;
                        }


                        if (dividers.Contains(ch.ToString()))
                        {
                            row++;
                            tokenList.Add(new Word(ch.ToString(), column, row));
                        }

                        if (ch == '\n')
                        {
                            column++;
                            row = 0;
                        }

                        if (ch == ';')
                        {
                            while (ch != '\n')
                            {
                                ch = (char)reader.Read();
                            }
                            column++;
                            row = 0;
                        }

                        if ((int)ch >= 0 || (int)ch <= 31)
                        {
                            ch = (char)reader.Read();
                        }

                    }

                }
            }
        }

        public void ShowTokenList()
        {
            foreach (Word word in tokenList)
            {
                Console.WriteLine(word);
            }
        }

        public void lineAnalyzer(string fileName)
        {
            int i = 0;
            int curentColumn = 1;

            using (FileStream file = new FileStream(fileName, FileMode.Truncate, FileAccess.Write))
            {
                using (StreamWriter writer = new StreamWriter(file))
                {



                    while (tokenList.Count - 1 >= i)
                    {
                        List<Word> lineList = new List<Word>();
                        int indexOfComa = -1;
                        for (int j = 0; i < tokenList.Count && tokenList[i].Column == curentColumn; i++)
                        {
                            lineList.Add(new Word(tokenList[i]));
                            if (tokenList[i].Token == ",")
                            {
                                indexOfComa = j;
                            }
                            j++;
                        }

                        int k = lineList.Count;
                        if (k == 0)
                        {
                            writer.WriteLine("line is empty");
                            curentColumn++;
                            continue;
                        }

                        writer.Write($"{curentColumn}. ");

                        foreach (Word s in lineList)
                        {
                            writer.Write(s.Token + " ");
                        }

                        writer.WriteLine();

                        foreach (Word s in lineList)
                        {
                            writer.WriteLine(s);
                        }

                        if (k >= 1)
                        {

                            if (lineList[0].GetType() == typeArr[2])
                            {
                                if (lineList[0].Token.ToLower() == "stc" && k == 1)
                                {
                                    writer.WriteLine("<mnemcom> sz = 1,pos = 1");
                                    curentColumn++;
                                    continue;
                                }
                                if (k > 1 && (lineList[1].GetType() == typeArr[0] ||
                                    lineList[1].GetType() == typeArr[1] ||
                                    lineList[1].GetType() == typeArr[4] ||
                                    lineList[1].GetType() == typeArr[5] ||
                                    lineList[1].GetType() == typeArr[6] ||
                                    lineList[1].GetType() == typeArr[7] ||
                                    lineList[1].GetType() == typeArr[8]))
                                {
                                    if (indexOfComa >= 0)
                                    {
                                        writer.WriteLine($"<mnemcom> sz = 1,pos = 1. <op1> sz = {indexOfComa - 1} ,pos = 2." +
                                            $"<op2> sz = {k - 1 - indexOfComa} ,pos = {indexOfComa + 2}");
                                        curentColumn++;
                                        indexOfComa = -1;
                                        continue;
                                    }
                                    writer.WriteLine($"<mnemcom> sz = 1,pos = 1. <op> sz = {k - 1} ,pos = 2");
                                    curentColumn++;
                                    continue;
                                }
                            }

                            if (lineList[0].GetType() == typeArr[8] && k > 1)
                            {
                                if (k >= 3)
                                {
                                    if (lineList[1].GetType() == typeArr[2] &&
                                    (lineList[2].GetType() == typeArr[0] ||
                                    lineList[2].GetType() == typeArr[1] ||
                                    lineList[2].GetType() == typeArr[8]))
                                    {
                                        writer.WriteLine($"<name> sz = 1,pos = 1.<mnem>sz = 1,pos = 2.<op>sz=" +
                                            $"{k - 2} ,pos = 3");
                                        curentColumn++;
                                        continue;
                                    }
                                    if (lineList[1].GetType() == typeArr[4])
                                    {
                                        writer.WriteLine($"<name>: sz = 1,pos = 1.<mnem>sz = 1,pos = 2.<op>sz=" +
                                            $"{k - 2} ,pos = 3");
                                        curentColumn++;
                                        continue;
                                    }
                                }

                                if (k == 2 && lineList[1].GetType() == typeArr[3])
                                {
                                    writer.WriteLine("<name>: sz = 1,pos = 1.<mnem>sz = 1,pos = 2");
                                    curentColumn++;
                                    continue;
                                }


                                if (lineList[1].Token == ":")
                                {

                                    if (k == 2)
                                    {
                                        writer.WriteLine("<label>: sz = 2,pos = 1.");
                                        curentColumn++;
                                        continue;
                                    }

                                    if (k == 3 && lineList[2].Token.ToLower() == "stc")
                                    {
                                        writer.WriteLine("<label>: sz = 2,pos = 1. <mnem>sz = 1,pos = 3");
                                        curentColumn++;
                                        continue;
                                    }

                                    if (k >= 3 && lineList[2].GetType() == typeArr[2] &&
                                    (lineList[3].GetType() == typeArr[0] ||
                                    lineList[3].GetType() == typeArr[1] ||
                                    lineList[3].GetType() == typeArr[4] ||
                                    lineList[3].GetType() == typeArr[5] ||
                                    lineList[3].GetType() == typeArr[6] ||
                                    lineList[3].GetType() == typeArr[7] ||
                                    lineList[3].GetType() == typeArr[8]))
                                    {
                                        if (indexOfComa >= 0)
                                        {
                                            writer.WriteLine($"<label>: sz = 2,pos = 1. <op1> sz = {indexOfComa - 2} ,pos = 3." +
                                                $"<op2> sz = {k - 1 - indexOfComa} ,pos = {indexOfComa + 2}");
                                            curentColumn++;
                                            indexOfComa = -1;
                                            continue;
                                        }
                                        writer.WriteLine($"<label>: sz = 2,pos = 1.<mnem>sz = 1,pos = 3.<op>sz=" +
                                            $"{k - 3} ,pos = 4");
                                        curentColumn++;
                                        continue;
                                    }
                                }


                            }
                            if (lineList[0].GetType() == typeArr[3])
                            {
                                writer.WriteLine("<mnemcom> sz = 1,pos = 1");
                                curentColumn++;
                                continue;
                            }
                        }
                        writer.WriteLine("error line");
                        curentColumn++;
                    }
                }
            }
        }
        
    }
}

