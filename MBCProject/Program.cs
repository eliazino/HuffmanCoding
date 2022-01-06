using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBCProject {
    class Program {
        static void Main(string[] args) {
            var data = new Huffman().encode("go go gophers");
            foreach(KeyValuePair<char, string> kvp in data.encodedObj) {
                Console.WriteLine("{0} => {1}", kvp.Key, kvp.Value);
            }
            var verification = new Huffman().verifyEncoding(data.encodedObj, data.treeObj);
            Console.WriteLine(verification);
            Console.ReadLine();
        }
    }
}