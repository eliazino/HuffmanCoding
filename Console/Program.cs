using LogicBase.Cryptography;
using LogicBase.HuffmanLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;

namespace ConsoleApp {
    class Program {
        static void Main(string[] args) {
            Performance p = new Performance();
            int startingValue = 2;
            int endValue = 62;
            int increment = 2;
            var result = Enumerable
              .Range(0, (endValue - startingValue) / increment + 1)
              .Select(i => startingValue + increment * i);
            string csv = "N,n,elapsed,bits,ecbdig,cbcdig,ecbelapsed,cbcelapsed";
            List<int> Ns = new List<int>() { 50, 100, 150, 200};
            string key = "Sample Key";
            foreach(int N in Ns) {
                foreach (int i in result) {
                    string h = p.generateChars(N, i);
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    Huffman huff = new Huffman();
                    var k = huff.encode(h);
                    sw.Stop();
                    double elt = sw.Elapsed.TotalMilliseconds;
                    int bitlength = getOutStrbit(k.encodedObj, h);
                    string raw = getOutStrbitVal(k.encodedObj, h);
                    byte[] arr = binToByteArr(raw);
                    sw = new Stopwatch();
                    sw.Start();
                    byte[] outCodedECB = new AES(CipherMode.ECB).Encrypt(arr, key);
                    sw.Stop();
                    double ecbelt = sw.Elapsed.TotalMilliseconds;
                    sw = new Stopwatch();
                    sw.Start();
                    byte[] outCodedCBC = new AES(CipherMode.CBC).Encrypt(arr, key);
                    sw.Stop();
                    double cbcelt = sw.Elapsed.TotalMilliseconds;
                    csv += string.Concat("\n", i, ",", N, ",", elt, ",", bitlength, ",", (outCodedECB.Length*8), ",", (outCodedCBC.Length*8), ",", ecbelt, ",", cbcelt);
                }
            }            
            Console.WriteLine(csv);
            Console.ReadLine();
        }

        public static int getOutStrbit(Dictionary<char, string> dic, string raw) {
            string v = "";
            foreach(char c in raw) {
                v += dic[c];
            }
            return v.Length;
        }
        public static string getOutStrbitVal(Dictionary<char, string> dic, string raw) {
            string v = "";
            foreach (char c in raw) {
                v += dic[c];
            }
            return v;
        }
        public static byte[] binToByteArr(string input) {
            int numOfBytes = input.Length / 8;
            byte[] bytes = new byte[numOfBytes];
            for (int i = 0; i < numOfBytes; ++i) {
                bytes[i] = Convert.ToByte(input.Substring(8 * i, 8), 2);
            }
            return bytes;
        }
    }

   public class Performance {
        private string charDic = "1234567890abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private static Random rand = new Random();
        public string generateChars(int N, int n) {
            string usable = charDic.Substring(0, n);
            shuffleDic();
            string s = "";
            int j = 0;
            for (int i = 0; i < N; i++) {
                if (j == n)
                    j = 0;
                s += charDic[j];
                j++;
            }
            return s;
        }
        public void shuffleDic() {
            var list = new SortedList<int, char>();
            foreach (var c in charDic)
                list.Add(rand.Next(), c);
            this.charDic = new string(list.Values.ToArray());
        }
        public byte[] binToByteArr(string input) {
            int numOfBytes = input.Length / 8;
            byte[] bytes = new byte[numOfBytes];
            for (int i = 0; i < numOfBytes; ++i) {
                bytes[i] = Convert.ToByte(input.Substring(8 * i, 8), 2);
            }
            return bytes;
        }
    }
}
