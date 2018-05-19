using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    partial class Parser
    {
        private class Sentences
        {
            private List<Word> line = new List<Word>();
            private int PosOfLabelOrName = -1;
            private int PosOfMnem = -1;
            private int PosOfFirstOperand = -1;
            private int PosOfSecondOperand = -1;
            public int CurrentLine { get; set; }
            private string errorMessage = null;
            private string offsetOfThisLine;

            public Sentences(List<Word> tokens, int line)
            {

                if (tokens.Count >= 1)
                {

                    this.line = tokens;
                    analyseSentence();
                }
                this.CurrentLine = line;
            }

            public void SetError(string error)
            {
                if (errorMessage != null)
                {
                    this.errorMessage += " " + error;

                }
                else
                {
                    errorMessage = error;
                }
                return;
            }


            public Sentences(List<Word> tokens, int line, string error)
            {

                this.line = tokens;
                this.CurrentLine = line;
                this.errorMessage = error;
            }
            private string offsetToString(int offset)
            {
                string result = Convert.ToString(offset, 16);
                if (result.Length == 1)
                {
                    return "000" + result;
                }
                if (result.Length == 2)
                {
                    return "00" + result;
                }
                if (result.Length == 3)
                {
                    return "0" + result;
                }

                return result;

            }
            public override string ToString()
            {
                if (line.Count < 1)
                {
                    return null;
                }
                string lineLecs = null;
                lineLecs += offsetOfThisLine + "\t\t";
                foreach (Word wr in line)
                {
                    lineLecs += wr.Token + " ";
                }
                if (errorMessage != null)
                {
                    lineLecs += "\n" + errorMessage;
                }

                return lineLecs;
            }
            //check for structure of sentence and some errors
            private void checkForMnemOp(int index)
            {
                int k = this.line.Count;
                if (this.line[index].GetTypeWord() == typeArr[2])
                {
                    this.PosOfMnem = index;
                    if (k >= index + 1)
                    {
                        if (k == index + 1)
                        {
                            if (this.line[index].Token.ToLower() == "stc")
                            {
                                return;
                            }
                            else
                            {
                                this.errorMessage = "error comand";
                                return;
                            }

                        }
                        if (k > index + 1)
                        {
                            if (this.line[index + 1].GetTypeWord() == typeArr[0] ||
                            this.line[index + 1].GetTypeWord() == typeArr[1] ||
                            this.line[index + 1].GetTypeWord() == typeArr[4] ||
                            this.line[index + 1].GetTypeWord() == typeArr[5] ||
                            this.line[index + 1].GetTypeWord() == typeArr[6] ||
                            this.line[index + 1].GetTypeWord() == typeArr[7] ||
                            this.line[index + 1].GetTypeWord() == typeArr[8])
                            {
                                this.PosOfFirstOperand = this.line.IndexOf(this.line[index + 1]);

                                int indexOfComa = -1;
                                int countOfComas = 0;
                                var coma = this.line.Where(t => String.Equals(t.Token, ",")).Select(t => t);
                                foreach (var c in coma)
                                {
                                    indexOfComa = this.line.IndexOf(c);
                                    countOfComas++;
                                }

                                if (countOfComas == 1)
                                {
                                    this.PosOfSecondOperand = indexOfComa + 1;
                                    return;
                                }

                                if (countOfComas > 1)
                                {
                                    this.errorMessage = "more then one operand";
                                    return;
                                }
                            }
                        }
                    }
                }
            }

            private void someMoreChecks()
            {
                if ((this.PosOfLabelOrName == -1 ||
                    this.PosOfLabelOrName == 0) &&
                    this.PosOfMnem == -1 &&
                    this.PosOfFirstOperand == -1)
                {
                    this.errorMessage = "error type of sentence";
                }

            }

            private void analyseSentence()
            {
                int k = this.line.Count;
                if (this.line[0].GetTypeWord() == typeArr[3])
                {
                    this.PosOfMnem = 0;
                    return;
                }

                checkForMnemOp(0);
                if (this.errorMessage != null)
                {
                    return;
                }
                if (this.line[0].GetTypeWord() == typeArr[8])
                {
                    this.PosOfLabelOrName = 0;
                    if (k > 1)
                    {
                        if (this.line[1].Token == ":")
                        {
                            if (k == 2)
                            {
                                this.PosOfLabelOrName = 1;
                            }
                            if (k > 2)
                            {
                                this.PosOfLabelOrName = 1;
                                checkForMnemOp(2);
                                if (this.errorMessage != null)
                                {
                                    return;
                                }
                            }
                        }
                        if (k == 2 && this.line[1].GetTypeWord() == typeArr[3])
                        {
                            this.PosOfMnem = 1;
                            return;
                        }
                        if (k > 2)
                        {
                            if (this.line[1].GetTypeWord() == typeArr[4])
                            {
                                this.PosOfMnem = 1;
                                this.PosOfFirstOperand = 1;
                                return;
                            }
                        }

                    }
                }
                if (this.errorMessage == null)
                {
                    someMoreChecks();
                }

            }

            //determination of offset
            private bool addMark(List<Vars> marks, List<Vars> marksUnchecked, Vars var)
            {
                for (int i = 0; i < marksUnchecked.Count; i++)
                {
                    if (marksUnchecked[i].Name == var.Name)
                    {
                        marksUnchecked.RemoveAt(i);
                    }
                }
                for (int i = 0; i < marks.Count; i++)
                {
                    if (marks[i].Name == var.Name)
                    {
                        errorMessage = "multiple definition//checkMark";
                        return false;
                    }
                }


                marks.Add(var);
                return true;
            }
            private int checkMark(List<Vars> marks, List<Vars> marksUnchecked, Vars var)
            {
                for (int i = 0; i < marks.Count; i++)
                {
                    if (marks[i].Name == var.Name)
                    {
                        if (var.Value - marks[i].Value >= 128)
                        {
                            return 6;
                        }
                        else
                        {
                            return 2;
                        }

                    }
                }

                marksUnchecked.Add(var);
                return 6;
            }

            public int OffsetFinding(List<Vars> vars, List<Vars> varsUnchecked, int currentOffset)
            {
                this.offsetOfThisLine = offsetToString(currentOffset);
                if (line.Count < 1)
                {
                    return 0;
                }
                if (PosOfLabelOrName == 1)
                {
                    Vars mark = new Vars(line[PosOfLabelOrName - 1].Token.ToLower(), currentOffset, CurrentLine);
                    addMark(vars, varsUnchecked, mark);
                    if (errorMessage != null)
                    {
                        return 0;
                    }

                }
                if (PosOfMnem == -1)
                {
                    return 0;
                }
                if (PosOfMnem == PosOfFirstOperand && PosOfFirstOperand != -1)
                {
                    if (PosOfSecondOperand != -1)
                    {
                        errorMessage = "too many operand var//OffsetFinding";
                        return 0;
                    }
                    if (PosOfFirstOperand + 1 > line.Count - 1)
                    {
                        errorMessage = "no const var//OffsetFinding";
                        return 0;
                    }
                    if (line[PosOfFirstOperand].Token.ToLower() == "db")
                    {
                        double result = CheckForOvervalued(PosOfFirstOperand + 1, line.Count - 1, 128);
                        if (errorMessage == null)
                        {
                            return 1;
                        }
                    }
                    if (line[PosOfFirstOperand].Token.ToLower() == "dw")
                    {
                        double result = CheckForOvervalued(PosOfFirstOperand + 1, line.Count - 1, 65536);
                        if (errorMessage == null)
                        {
                            return 2;
                        }
                    }
                    if (line[PosOfFirstOperand].Token.ToLower() == "dd")
                    {
                        double result = CheckForOvervalued(PosOfFirstOperand + 1, line.Count - 1, 4294967296);
                        if (errorMessage == null)
                        {
                            return 4;
                        }
                    }

                    return 0;
                }

                switch (line[PosOfMnem].Token.ToLower())
                {
                    case "dec":
                        if (PosOfFirstOperand == -1 || PosOfSecondOperand != -1)
                        {
                            errorMessage = "no Operands dec //OffsetFinding";
                            return 0;
                        }
                        if (line[PosOfFirstOperand].Token.ToLower() == "byte" &&
                            line[PosOfFirstOperand + 1].GetTypeWord() == typeArr[7])
                        {
                            int result = checkOperand(PosOfFirstOperand + 2, line.Count - 1);
                            if (result == -1)
                            {
                                return 0;
                            }
                            else
                            {
                                return result;
                            }
                        }
                        if (line[PosOfFirstOperand].Token.ToLower() == "word" &&
                            line[PosOfFirstOperand + 1].GetTypeWord() == typeArr[7])
                        {
                            int result = checkOperand(PosOfFirstOperand + 2, line.Count - 1);
                            if (result == -1)
                            {
                                return 0;
                            }
                            else
                            {
                                return result + 1;
                            }
                        }
                        if (line[PosOfFirstOperand].Token.ToLower() == "dword" &&
                            line[PosOfFirstOperand + 1].GetTypeWord() == typeArr[7])
                        {
                            int result = checkOperand(PosOfFirstOperand + 2, line.Count - 1);
                            if (result == -1)
                            {
                                return 0;
                            }
                            else
                            {
                                return result;
                            }
                        }
                        errorMessage = "incorect operand type sbb//OffsetFinding";
                        return 0;

                    case "stc":
                        if (line.Count == 1)
                        {
                            return 1;
                        }
                        else
                        {
                            errorMessage = "too many lecs//OffsetFinding";
                            return 0;
                        }
                    case "inc":
                        if (PosOfFirstOperand == -1 || PosOfSecondOperand != -1)
                        {
                            errorMessage = "no Operands inc//OffsetFind";
                            return 0;
                        }

                        if (line[PosOfFirstOperand].GetTypeWord() == typeArr[1])
                        {
                            return 2;
                        }
                        if (line[PosOfFirstOperand].GetTypeWord() == typeArr[0])
                        {
                            return 1;
                        }
                        errorMessage = "incorect operand type inc//OffsetFinding";
                        return 0;

                    case "sbb":
                        if (PosOfFirstOperand == -1 || PosOfSecondOperand == -1)
                        {
                            errorMessage = "no Operands sbb//OffsetFind";
                            return 0;
                        }
                        if (line[PosOfFirstOperand].GetTypeWord() == typeArr[1] &&
                            line[PosOfSecondOperand].GetTypeWord() == typeArr[1])
                        {
                            return 2;
                        }
                        if (line[PosOfFirstOperand].GetTypeWord() == typeArr[0] &&
                            line[PosOfSecondOperand].GetTypeWord() == typeArr[0])
                        {
                            return 2;
                        }
                        errorMessage = "incorect operand type sbb//OffsetFinding";
                        return 0;
                    case "and":
                        if (PosOfFirstOperand == -1 || PosOfSecondOperand == -1)
                        {
                            errorMessage = "no Operands and//OffsetFind";
                            return 0;
                        }
                        if (line[PosOfFirstOperand].GetTypeWord() == typeArr[0] ||
                            line[PosOfFirstOperand].GetTypeWord() == typeArr[1])
                        {
                            if (PosOfSecondOperand - PosOfFirstOperand != 2)
                            {
                                errorMessage = "too long reg8/32 operand and//OffsetFind";
                                return 0;
                            }
                            else
                            {
                                int result = checkOperand(PosOfSecondOperand, line.Count - 1);
                                if (result == -1)
                                {
                                    return 0;
                                }
                                else
                                {
                                    return result;
                                }

                            }
                        }
                        if (line[PosOfSecondOperand].GetTypeWord() == typeArr[0] ||
                           line[PosOfSecondOperand].GetTypeWord() == typeArr[1])
                        {
                            if (PosOfSecondOperand != line.Count - 1)
                            {
                                errorMessage = "too long reg8/32 operand";
                                return 0;
                            }
                            else
                            {
                                int result = checkOperand(PosOfFirstOperand, PosOfSecondOperand - 2);

                                if (result == -1)
                                {
                                    return 0;
                                }
                                else
                                {
                                    return result;
                                }

                            }
                        }
                        errorMessage = "incorect operand type and//OffsetFinding";
                        return 0;
                    case "mov":
                        if (PosOfFirstOperand == -1 || PosOfSecondOperand == -1)
                        {
                            errorMessage = "no Operands mov//OffsetFind";
                            return 0;
                        }
                        if (PosOfSecondOperand - PosOfFirstOperand != 2)
                        {
                            errorMessage = "too long reg8/32 operand mov//OffsetFind";
                            return 0;
                        }
                        else
                        {
                            if (line[PosOfFirstOperand].GetTypeWord() == typeArr[0])
                            {
                                double imm = calculating(PosOfSecondOperand, line.Count - 1);
                                if (errorMessage != null)
                                {
                                    return 0;
                                }
                                else
                                {
                                    return 5;
                                }
                            }
                            if (line[PosOfFirstOperand].GetTypeWord() == typeArr[1])
                            {
                                double imm = calculating(PosOfSecondOperand, line.Count - 1);
                                if (errorMessage != null)
                                {
                                    return 0;
                                }
                                else
                                {
                                    return 2;
                                }
                            }
                            errorMessage = "incorect operand type mov//OffsetFinding";
                            return 0;
                        }
                    case "or":
                        if (PosOfFirstOperand == -1 || PosOfSecondOperand == -1)
                        {
                            errorMessage = "no Operands or //OffsetFinding";
                            return 0;
                        }
                        if (line[PosOfFirstOperand].Token.ToLower() == "byte" &&
                            line[PosOfFirstOperand + 1].GetTypeWord() == typeArr[7])
                        {
                            int result = checkOperand(PosOfFirstOperand + 2, PosOfSecondOperand - 2);
                            if (result == -1)
                            {
                                return 0;
                            }
                            else
                            {
                                int offsetop = constOffset(PosOfSecondOperand, line.Count - 1, 128);
                                if (errorMessage == null)
                                {
                                    return result + offsetop;
                                }
                            }
                        }
                        if (line[PosOfFirstOperand].Token.ToLower() == "word" &&
                            line[PosOfFirstOperand + 1].GetTypeWord() == typeArr[7])
                        {
                            int result = checkOperand(PosOfFirstOperand + 2, PosOfSecondOperand - 2);
                            if (result == -1)
                            {
                                return 0;
                            }
                            else
                            {
                                int offsetop = constOffset(PosOfSecondOperand, line.Count - 1, 65536);
                                if (errorMessage == null)
                                {
                                    return result + offsetop + 1;
                                }
                            }
                        }
                        if (line[PosOfFirstOperand].Token.ToLower() == "dword" &&
                            line[PosOfFirstOperand + 1].GetTypeWord() == typeArr[7])
                        {
                            int result = checkOperand(PosOfFirstOperand + 2, PosOfSecondOperand - 2);
                            if (result == -1)
                            {
                                return 0;
                            }
                            else
                            {
                                int offsetop = constOffset(PosOfSecondOperand, line.Count - 1, 4294967296);
                                if (errorMessage == null)
                                {
                                    return result + offsetop;
                                }
                            }
                        }
                        errorMessage = "incorect operand type or//OffsetFinding";
                        return 0;
                    case "jnle":
                        if (PosOfFirstOperand == -1 || PosOfSecondOperand != -1)
                        {
                            errorMessage = "no Operands jnle //OffsetFinding";
                            return 0;
                        }
                        if (PosOfFirstOperand != line.Count - 1)
                        {
                            errorMessage = "too many operands jnle //OffsetFinding";
                            return 0;
                        }
                        Vars mark = new Vars(line[PosOfFirstOperand].Token.ToLower(), currentOffset, CurrentLine);
                        int res = checkMark(vars, varsUnchecked, mark);
                        if (res == -1)
                        {
                            return 0;
                        }

                        return res;
                    case "ends":
                        return -currentOffset;



                }

                return 0;
            }
            private int constOffset(int posFirst, int posLast, long size)
            {

                double op = CheckForOvervalued(PosOfSecondOperand, line.Count - 1, size);
                if (errorMessage != null)
                {
                    return 0;
                }
                if (size == 128)
                {
                    return 1;
                }
                if (size == 65536)
                {
                    if ((op > 127 && op < 65408) || op < -128)
                    {
                        return 2;
                    }
                    else
                    {
                        return 1;
                    }

                }
                if (size == 4294967296)
                {
                    if (op > 127 || op < -128)
                    {
                        return 4;
                    }
                    else
                    {
                        return 1;
                    }
                }
                errorMessage = "error //constOffset";
                return 0;
            }

            private int checkOperand(int posFirst, int posLast)
            {
                if (posLast > line.Count - 1 || posFirst > posLast)
                {
                    errorMessage = "error //checkOperand";
                    return 0;
                }
                int sizeList = posLast - posFirst;
                string defaultRegister = "ds";

                if (this.line[posFirst].GetTypeWord() == typeArr[5])
                {
                    if (String.Equals(defaultRegister, this.line[posFirst].Token.ToLower()))
                    {
                        if (sizeList >= posFirst + 1 && this.line[posFirst + 1].Token == ":")
                        {
                            int result = checkForOffsetInMemory(posFirst + 2, posLast);
                            if (errorMessage == "ebp" ||
                            errorMessage == "esp")
                            {
                                errorMessage = null;
                                return result;
                            }

                            return result;
                        }
                    }
                    else
                    {
                        if (sizeList >= posFirst + 1 && this.line[posFirst + 1].Token == ":")
                        {
                            int result = checkForOffsetInMemory(posFirst + 2, posLast);

                            if (errorMessage == "ebp" ||
                                errorMessage == "esp")
                            {
                                errorMessage = null;
                                return result;
                            }
                            if (result == -1)
                            {
                                return result;
                            }
                            return result + 1;
                        }

                    }
                    errorMessage = "erorr operand//checkOperand";
                    return -1;
                }
                int finalOffset = checkForOffsetInMemory(posFirst, posLast);

                if (errorMessage == "ebp" ||
                    errorMessage == "esp")
                {
                    errorMessage = null;
                }

                return finalOffset;
            }

            private int checkForOffsetInMemory(int posFirst, int posLast)
            {
                if (this.line[posFirst].Token != "[" || this.line[posLast].Token != "]")
                {
                    this.errorMessage = "error operand '[' or ']' not found//checkForOffsetInMem";
                    return -1;
                }
                int size = posLast - posFirst;
                if (size < 2)
                {
                    errorMessage = "error operand to small//checkForOffsetInMem";
                    return -1;
                }
                if (size == 2)
                {
                    if (line[posFirst + 1].GetTypeWord() == typeArr[0])
                    {
                        if (line[posFirst + 1].Token.ToLower() == "ebp" ||
                            line[posFirst + 1].Token.ToLower() == "esp")
                        {
                            return 4;
                        }
                        else
                        {
                            return 2;
                        }
                    }
                    else
                    {
                        errorMessage = "error operand//checkForOffsetInMem";
                        return -1;
                    }
                }


                if (size >= 4)
                {
                    string dataReg = null;

                    if (line[posFirst + 1].GetTypeWord() == typeArr[0])
                    {
                        if (line[posFirst + 1].Token.ToLower() == "ebp")
                        {
                            dataReg = "ebp";
                        }
                        if (line[posFirst + 1].Token.ToLower() == "esp")
                        {
                            dataReg = "esp";
                        }
                        if (line[posFirst + 2].Token == "+")
                        {
                            double offset = 0;
                            offset = calculating(posFirst + 3, posLast - 1);
                            return offsetFinding(dataReg, offset);

                        }
                        if (line[posFirst + 2].Token == "-")
                        {
                            double offset = 0;
                            offset = -calculating(posFirst + 3, posLast - 1);
                            return offsetFinding(dataReg, offset);

                        }
                        return -1;
                    }
                    else
                    {
                        errorMessage = "error operand//checkForOffsetInMem";
                        return -1;
                    }
                }


                errorMessage = "error operand//checkForOffsetInMem";
                return -1;
            }

            private double CheckForOvervalued(int posFirst, int posLast, long size)
            {
                double res = calculating(posFirst, posLast);

                if (res > size)
                {
                    errorMessage = "overvalued operand //CheckForOvervalued";
                    return -1;
                }

                return res;
            }
            private int offsetFinding(string dataReg, double offset)
            {
                if (errorMessage != null)
                {
                    return -1;
                }

                if (offset < 128 && offset >= -128)
                {
                    if (dataReg == null)
                    {
                        return 3;
                    }
                    if (dataReg == "ebp")
                    {
                        errorMessage = "ebp";
                        return 4;
                    }
                    if (dataReg == "esp")
                    {
                        errorMessage = "esp";
                        return 5;
                    }
                }
                else
                {
                    if (dataReg == null)
                    {
                        return 6;
                    }
                    if (dataReg == "ebp")
                    {
                        errorMessage = "ebp";
                        return 7;
                    }
                    if (dataReg == "esp")
                    {
                        errorMessage = "esp";
                        return 8;
                    }
                }
                return -1;

            }


            //calculating of terms

            private double calculating(int posFirst, int posLast)
            {
                int size = posLast - posFirst;
                string[] terms = new string[size];
                Stack<double> operands = new Stack<double>();
                Stack<Operators> operators = new Stack<Operators>();
                Operators.CountOfBracket = 0;
                bool fg = true;
                int k = 0;

                if (line[posFirst].Token == "-")
                {
                    k = 1;
                }

                for (int i = k; i <= size; i++)
                {
                    if ((line[posFirst + i].GetTypeWord() == typeArr[9] ||
                       line[posFirst + i].GetTypeWord() == typeArr[10] ||
                       line[posFirst + i].GetTypeWord() == typeArr[11]) && fg)
                    {
                        operands.Push(changeConstType(line[posFirst + i]));
                        fg = false;
                        continue;
                    }

                    if (line[posFirst + i].Token == "(" && fg)
                    {
                        operators.Push(new Operators(line[posFirst + i].Token));
                        continue;
                    }

                    if (line[posFirst + i].GetTypeWord() == typeArr[6] && !fg)
                    {
                        Operators op = new Operators(line[posFirst + i].Token);

                        if (op.Priority == -1)
                        {
                            errorMessage = "error operand operation";
                            return 0;
                        }

                        if (operators.Count < 1)
                        {
                            operators.Push(op);
                            fg = true;
                        }

                        else
                        {
                            if (checkPriority(operators.Peek(), op, operators, operands))
                            {
                                if (op.Value == ")")
                                {
                                    fg = false;

                                }
                                else
                                {
                                    fg = true;
                                }
                            }
                            else
                            {
                                return 0;
                            }
                        }

                        continue;
                    }


                    errorMessage = $"error operand cycle{i}//calculating";
                    return 0;
                }
                if (operators.Count > 0)
                {
                    while (operators.Count > 0)
                    {
                        doCalc(operators, operands);
                    }
                }
                if (operands.Count == 1)
                {
                    if (k == 1)
                    {
                        return -operands.Pop();
                    }
                    return operands.Pop();
                }

                errorMessage = "too many el in operands//calculating";
                return 0;
            }

            private int changeConstType(Word lecs)
            {
                string result;

                if (lecs.GetTypeWord() == typeArr[9])
                {
                    result = lecs.Token.Remove(lecs.Token.Length - 1);
                    return Convert.ToInt32(result, 2);
                }

                if (lecs.GetTypeWord() == typeArr[10])
                {
                    result = lecs.Token;
                    return Int32.Parse(result);

                }

                if (lecs.GetTypeWord() == typeArr[11])
                {
                    result = lecs.Token.Remove(lecs.Token.Length - 1);
                    return Convert.ToInt32(result, 16);
                }

                return 0;
            }

            private bool checkPriority(Operators op1, Operators op2, Stack<Operators> ops, Stack<double> opnds)
            {

                if (op2.Value == ")")
                {
                    while (ops.Peek().Value != "(")
                    {

                        if (doCalc(ops, opnds) == false)
                        {
                            return false;
                        }
                    }
                    ops.Pop();
                    return true;

                }

                if (op1.Priority > op2.Priority)
                {
                    ops.Push(op2);
                }

                else
                {
                    if (doCalc(ops, opnds))
                    {
                        if (ops.Count < 1)
                        {
                            ops.Push(op2);
                            return true;
                        }
                        checkPriority(ops.Peek(), op2, ops, opnds);
                    }
                    else
                    {
                        return false;
                    }
                }

                return true;
            }


            private bool doCalc(Stack<Operators> ops, Stack<double> opnds)
            {
                if (opnds.Count < 2 || ops.Count < 1)
                {
                    errorMessage = "error operand //doCalc";
                    return false;
                }

                double operand1 = opnds.Pop(), operand2 = opnds.Pop();
                Operators op = ops.Pop();

                if (String.Equals(op.Value, "*"))
                {
                    double result = operand2 * operand1;
                    opnds.Push(result);
                }
                if (String.Equals(op.Value, "/"))
                {
                    double result = operand2 / operand1;
                    opnds.Push(result);
                }
                if (String.Equals(op.Value, "-"))
                {
                    double result = operand2 - operand1;
                    opnds.Push(result);
                }
                if (String.Equals(op.Value, "+"))
                {
                    double result = operand2 + operand1;
                    opnds.Push(result);
                }
                return true;

            }


            //printing
            public void printLine()
            {


                if (this.line.Count == 0)
                {
                    Console.WriteLine();
                    return;
                }

                Console.Write($"{CurrentLine} ");
                var tokens = this.line.Select(t => t.Token);
                foreach (var to in tokens)
                {
                    Console.Write($"{to} ");
                }
                if (this.errorMessage != null)
                {
                    Console.WriteLine();
                    Console.WriteLine($"{this.errorMessage}");
                    Console.WriteLine("\n\n");
                    return;
                }
                Console.WriteLine();
                Console.WriteLine($"Position of label or name {this.PosOfLabelOrName}");
                Console.WriteLine($"Position of mnem {this.PosOfMnem}");
                Console.WriteLine($"Position of first operand {this.PosOfFirstOperand}");
                Console.WriteLine($"Position of second operand {this.PosOfSecondOperand}");
                Console.WriteLine("\n\n");

            }

        }
    }
}