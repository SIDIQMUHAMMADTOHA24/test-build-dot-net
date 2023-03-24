using Api.Share.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.ConsolePk
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Input private key");
            var pk = Console.ReadLine();
            DpApi dp_api = new DpApi();
            var enkrip_pk = dp_api.Encrypt(pk);
            StreamWriter sw = null;
            sw = new StreamWriter("c:\\Temp\\Pk.txt", false);
            sw.WriteLine(enkrip_pk);
            sw.Flush();
            sw.Close();
            Console.Write($"{Environment.NewLine} {enkrip_pk}");
            Console.Write($"{Environment.NewLine}Success, Press any key to exit...");
            Console.ReadKey(true);
        }
    }
}
