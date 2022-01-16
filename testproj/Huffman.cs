using LogicBase.HuffmanLib;
using NUnit.Framework;
using static LogicBase.HuffmanLib.Huffman;

namespace testproj {
    [TestFixture]
    public class HuffmanTest {
        [SetUp]
        public void Setup() {
        }

        [Test]
        public void Test1() {
            string data = "AABBCCDD";
            Huffman huff = new Huffman();
            var t = huff.getCharacterFrequency(data);
            Assert.AreEqual(t.Count, 4);
            var actual = t[new KeyObj(2, "A")];
            var expected = new TreeObj { frequency = 2, Branch = 'A' };
            Assert.AreEqual(actual.Branch, expected.Branch);
            Assert.AreEqual(actual.frequency, expected.frequency);
        }
        [TestCase]
        public void Test2() {
            string data = "BBDDDDCCCCA";
            Huffman huff = new Huffman();
            var t = huff.getCharacterFrequency(data);
            Assert.AreEqual(t.Count, 4);
            //Assert.AreEqual(t[new KeyObj(2, 'A'.GetHashCode())], 1);
            //Assert.AreEqual(t[t.Keys[0]], 1);
        }
    }
}