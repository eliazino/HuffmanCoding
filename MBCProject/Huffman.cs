using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBCProject {
    public class Huffman {
        public (Dictionary<char, string> encodedObj, SortedDictionary<KeyObj, TreeObj> treeObj) encode(string text) {
            var dictionary = getCharacterFrequency(text);
            var encodingDictionary = new Dictionary<char, string>();
            if(dictionary.Count > 0) {
                while (true) {
                    if(dictionary.Count > 1) {
                        //Dump the new Keys
                        KeyObj[] keys = new KeyObj[dictionary.Count];
                        dictionary.Keys.CopyTo(keys, 0);
                        // Find the least two frequencies
                        var least = dictionary[keys[0]];
                        var secondLeast = dictionary[keys[1]];
                        // Compute the new Frequency
                        int newFrequency = keys[0].frequency + keys[1].frequency;
                        // Expand the char Lib
                        string newCharLib = string.Concat(keys[0].charLib, keys[1].charLib);
                        // Generate the new Key Obj
                        var newKey = new KeyObj(newFrequency, newCharLib);

                        // Find the prefix
                        if (least.BranchIsLeaf()) {
                            encodingDictionary = logCharMap(encodingDictionary, (char)least.Branch, "0");
                        } else {
                            encodingDictionary = logCharMap(encodingDictionary, keys[0].charLib, "0");
                        }

                        if (secondLeast.BranchIsLeaf()) {
                            encodingDictionary = logCharMap(encodingDictionary, (char)secondLeast.Branch, "1");
                        } else {
                            encodingDictionary = logCharMap(encodingDictionary, keys[1].charLib, "1");
                        }

                        //Create the new Branch Obj
                        var branched = new SortedDictionary<KeyObj, TreeObj>();
                        branched.Add(keys[0], least);
                        branched.Add(keys[1], secondLeast);
                        var newTreeObj = new TreeObj { frequency = newFrequency, Branch = branched  };

                        // Remove the least two object colapsed
                        dictionary.Remove(keys[0]);
                        dictionary.Remove(keys[1]);

                        // Add new Obj
                        dictionary.Add(newKey, newTreeObj);

                        //Order Dictionary in prep for next Iter
                        dictionary.OrderBy(x => x.Key.frequency);
                    } else {
                        break;
                    }
                }
            }
            return (encodingDictionary, dictionary);
        }


        public string verifyEncoding(Dictionary<char, string> encodedObj, SortedDictionary<KeyObj, TreeObj> treeObj) {
            string decoded = "";
            foreach(KeyValuePair<char, string> kvp in encodedObj) {
                var leftOrRight = kvp.Value.ToList();
                var activeObj = treeObj.FirstOrDefault().Value.Branch;
                var branchAsDic = (SortedDictionary<KeyObj, TreeObj>)activeObj;
                foreach (char s in leftOrRight) {
                    TreeObj nextDepth = new TreeObj();
                    KeyObj[] keys = new KeyObj[branchAsDic.Count];
                    branchAsDic.Keys.CopyTo(keys, 0);
                    if (goRight(s.ToString())) {
                        nextDepth = branchAsDic[keys[1]];
                    } else {
                        nextDepth = branchAsDic[keys[0]];
                    }
                    if (nextDepth.BranchIsLeaf()) {
                        decoded += nextDepth.Branch;
                        break;
                    }
                    activeObj = nextDepth.Branch;
                    branchAsDic = (SortedDictionary<KeyObj, TreeObj>)activeObj;
                }
            }
            return decoded;
        }
        private Dictionary<char, string> logCharMap(Dictionary<char, string> map, char character, string position) {
            if (map.ContainsKey(character)) {
                map[character] = position + "" + map[character];
            } else {
                map.Add(character, position);
            }
            return map;
        }
        private Dictionary<char, string> logCharMap(Dictionary<char, string> map, string characters, string position) {
            var updatable = map.Where(kvp => characters.Contains(kvp.Key));
            var newMap = map;
            foreach (KeyValuePair<char, string> keyValue in updatable.ToList()) {
                newMap[keyValue.Key] = string.Concat(position, newMap[keyValue.Key]);
            }
            return newMap;
        }
        private bool goRight(string charBool) {
            return charBool.Equals("1");
        }
        public SortedDictionary<KeyObj, TreeObj> getCharacterFrequency(string text) {
            SortedDictionary<KeyObj, TreeObj> maps = new SortedDictionary<KeyObj, TreeObj>();
            string newstr = String.Join("", text.Distinct());
            foreach (char c in newstr) {
                int count = text.Count(o=> o == c);
                var key = new KeyObj(count, c.ToString());
                var treeObj = new TreeObj() { Branch = c, frequency = count };
                maps.Add(key, treeObj);
            }
            maps.OrderBy(x => x.Key.frequency);
            return maps;
        }
    }
    public class TreeObj {
        public int frequency { get; set; }
        public object Branch { get; set; }
        public bool BranchIsLeaf() {
            if (Branch.GetType().Equals(typeof(char))) {
                return true;
            }
            return false;
        }
    }
    public class KeyObj : Tuple<int, string> {
        public KeyObj(int frequency, string charLib) : base(frequency, charLib) { }
        public int frequency => Item1;
        public string charLib => Item2;
    }
}
