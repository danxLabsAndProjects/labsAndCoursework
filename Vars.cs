using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    partial class Parser
    {
        class Vars : IEquatable<Vars>
        {
            public string Name { get; set; }
            public int Value { get; set; }
            public int Column { get; set; }

            public Vars(string name, int value,int column)
            {
                this.Name = name;
                this.Value = value;
                this.Column = column;
            }

            bool IEquatable<Vars>.Equals(Vars other)
            {
                if(other == null)
                {
                    return false;
                }

                if(this.Name == other.Name)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
