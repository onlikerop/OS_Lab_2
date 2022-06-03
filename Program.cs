using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace os_lab_2
{
    class Program
    {
        public const string Alphabet = "abcdefghijklmnopqrstuvwxyz";
        public const int PassLen = 5;
        private const string HashesFile = "../../../input.txt";
        public const string DictPath = "../../../dict.txt";
        private static int _threads = Environment.ProcessorCount;
        
        static void Main(string[] args)
        {
            Console.WriteLine("Choose hash input method:");
            Console.WriteLine("1. Read from file");
            Console.WriteLine("2. Enter via console");
            
            var choice = Convert.ToInt32(Console.ReadLine());
           
            List<string> hashes = new List<string>();
            switch (choice)
            {
                case 1:
                    hashes = ReadHashesFile(); 
                    break;
                case 2:
                    hashes = ReadHashesConsole();
                    break;
                default:
                    Console.WriteLine("Unknown option");
                    break;
            }
            
            Console.Write("Choose single- or multithreading mode (s/m): ");
            
            var mode = Console.ReadLine();
            
            if (mode == "m")
            {
                Console.Write($"Enter number of threads or press 'Return' to use default value({_threads}): ");
                var input = Console.ReadLine();
                if (!String.IsNullOrEmpty(input))
                    _threads = Convert.ToInt32(input);
            }
            
            PassDict.GenerateDictionary();

            foreach (var hash in hashes)
            {
                var watch = Stopwatch.StartNew();
                string pass = mode switch
                {
                    "s" => HashBruteforcer.BruteForceSingle(hash),
                    "m" => HashBruteforcer.BruteForceMulti(hash, _threads).Result,
                    _ => null,
                };
                watch.Stop();

                if (pass != null)
                    Console.WriteLine($"{pass}: {watch.Elapsed.Seconds} seconds spent cracking");
                else
                    Console.WriteLine("Password not found");
            }
            
        }

        private static List<string> ReadHashesFile()
        {
            var reader = new StreamReader(HashesFile);
            var hashes = new List<string>();
            while (!reader.EndOfStream)
                hashes.Add(reader.ReadLine());
            reader.Close();
            return hashes;
        }

        private static List<string> ReadHashesConsole()
        {
            Console.WriteLine("Enter your hashes line-by-line. Enter 0 to stop");
            var hashes = new List<string>();
            while (true)
            {
                var hash = Console.ReadLine();
                if (hash != "0")
                    hashes.Add(hash);
                else
                    break;
            }
            return hashes;
        }
    }
}