using Microsoft.Owin.Hosting;
using NativeCommand;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiteRemote
{
    class Program
    {
        static void Main(string[] args)
        {
            loadKeyMapping();

            using (WebApp.Start("http://*:54321/"))
            {
                Console.WriteLine("Server is running on port 54321");
                Console.ReadLine();
            }
        }

        static void loadKeyMapping()
        {
            using(var reader = new StreamReader("keymap.txt"))
            {
                string line = reader.ReadLine();
                while (line != null)
                {
                    line = line.Trim();
                    if(line != "")
                    {
                        var tokens = line.Split(',');
                        KeyMapping.KeyCode.Add(tokens[0], Convert.ToInt32(tokens[1]));
                    }
                    line = reader.ReadLine();
                }
            }
        }
    }
}
