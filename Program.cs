using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {

        static void doWork()
        {
            Parser parser = new Parser();
            parser.Pars("E:\\parser\\parser\\ConsoleApp1\\test.asm");
            parser.ShowTokenList();
            //parser.lineAnalyzer("E:\\parser\\parser\\ConsoleApp1\\lineAnalyzerResult.txt");
            parser.testc("E:\\parser\\parser\\ConsoleApp1\\listing.txt");
  
         


        }

        static void Main()
        {
            //try
            //{
                doWork();
            //}
            //catch (Exception ex)
            //{
              // Console.WriteLine("Exception: {0}", ex.Message);
            //}
            //finally
            //{
                Console.ReadKey();
            //}
        }
    }
}
