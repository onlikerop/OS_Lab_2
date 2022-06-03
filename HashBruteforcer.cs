using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace os_lab_2
{
    public static class HashBruteforcer
    {
        private static bool CheckHash(string password, string hash)
        {
            var crypto = SHA256.Create();
            var testHash = crypto.ComputeHash(Encoding.ASCII.GetBytes(password));
            return HashToString(testHash) == hash;
        }

        private static string HashToString(byte[] hash)
        {
            var builder = new StringBuilder();  
            foreach (var hashByte in hash)
                builder.Append(hashByte.ToString("x2"));  
            return builder.ToString();  
        }

        public static string BruteForceSingle(string hash)
        {
            var reader = new StreamReader(Program.DictPath);
            while (!reader.EndOfStream)
            {
                var pass = reader.ReadLine();
                if (CheckHash(pass, hash))
                {
                    reader.Close();
                    return pass;
                }
            }
            reader.Close();
            return null;
        }

        public static async Task<string> BruteForceMulti(string hash, int threadAmount)
        {
            var dict = File.ReadLines(Program.DictPath).ToList();
            
            var passPositions = new List<Tuple<int, int>>();
            var curStart = 0;
            var defaultInterval = dict.Count / threadAmount;
            for (var i = 0; i < threadAmount - 1; i++)
            {
                passPositions.Add(new Tuple<int, int>(curStart, defaultInterval));
                curStart += defaultInterval;
            }
            passPositions.Add(new Tuple<int, int>(curStart, dict.Count - curStart));
            
            var tasks = new List<Task<string>>();
            for (var i = 0; i < threadAmount; i++)
            {
                var seg = dict.GetRange(passPositions[i].Item1,passPositions[i].Item2);
                var task = Task.Run(() => BruteforceTask(seg, hash));
                tasks.Add(task);
            }
            
            while (tasks.Any())
            {
                Task<string> finishedTask = await Task.WhenAny(tasks);
                if (finishedTask.Result != null)
                    return finishedTask.Result;
                tasks.Remove(finishedTask);
            }
            return null;
        }

        private static string BruteforceTask(List<string> segment, string hash)
        {
            string result = null;
            foreach (var pass in segment)
            {   
                if (CheckHash(pass, hash))
                {
                    result = pass;
                    break;
                }
            }
            return result;
        }
    }
}