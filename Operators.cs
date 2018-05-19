using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    partial class Parser
    {
        class Operators
        {
            public string Value { get; set; }
            public int Priority { get; set; }
            public static int CountOfBracket = 0;
            public Operators(string op)
            {

                if (String.Equals(op, ")"))
                {
                    this.Value = op;
                    if(CountOfBracket < 1)
                    {
                        this.Priority = -1;
                    }
                    this.Priority = 4;
                    CountOfBracket--;
                    return;
                }
                if (String.Equals(op, "("))
                {
                    this.Value = op;
                    this.Priority = 3;
                    CountOfBracket++;
                    return;
                }
                if (String.Equals(op, "*"))
                {
                    this.Value = op;
                    this.Priority = 1;
                    return;
                }
                if (String.Equals(op, "/"))
                {
                    this.Value = op;
                    this.Priority = 1;
                    return;
                }
                if (String.Equals(op, "-"))
                {
                    this.Value = op;
                    this.Priority = 2;
                    return;
                }
                if (String.Equals(op, "+"))
                {
                    this.Value = op;
                    this.Priority = 2;
                    return;
                }
                this.Value = op;
                this.Priority = -1;

            }
        }
    }
}
