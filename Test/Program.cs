using NativeCommand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ReadLine();
            NativeIOCommand.Instance.SendLeftClick(444, 2427);
            Console.ReadLine();
        }
    }
}
