using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace os_lab_2
{
    public static class PassDict
    {
        public static void GenerateDictionary()
        {
            if (!File.Exists(Program.DictPath))
            {
                Console.WriteLine("Password dictionary not found, generating...");
                var writer = new StreamWriter(Program.DictPath);
                var passwords = GetPermutationsWithRept(Program.Alphabet, Program.PassLen);
                foreach (var pass in passwords)
                {
                    writer.WriteLine(String.Concat(pass));
                }
                writer.Close();
            }
            else
                Console.WriteLine("Starting bruteforce...");
        }
        
        private static IEnumerable<IEnumerable<T>> 
            GetPermutationsWithRept<T>(IEnumerable<T> list, int length)
        {
            if (length == 1) return list.Select(t => new T[] { t });
            return GetPermutationsWithRept(list, length - 1)
                .SelectMany(t => list, 
                    (t1, t2) => t1.Concat(new T[] { t2 }));
        }
    }
}