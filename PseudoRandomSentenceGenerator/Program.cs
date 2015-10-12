using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PseudoRandomSentenceGenerator {

    public class RandomGen {
        public RandomGen() {
            words = new List<string>();
        }
        public string GenerateSentence() {
            string sentence = "";
            var n = _rand.Next(1, 10);
            for(int i = 0; i < n; i++) {
                sentence += words[_rand.Next(words.Count)] + " ";
            }
            return sentence;
        }
        private List<string> words;
        public void AddWord(string word) {
            var wordWithoutSpace = RemoveSpaces(word);
            if(wordWithoutSpace != string.Empty)
                this.words.Add(wordWithoutSpace);
        }
        public void AddWordList(List<string> wordslist) {
            foreach(var word in wordslist) {
                var wordWithoutSpace = RemoveSpaces(word);
                if(wordWithoutSpace != string.Empty)
                    this.words.Add(wordWithoutSpace);
            }
        }

        private string RemoveSpaces(string word) {
            string endWord = string.Empty;
            for(int i = 0; i < word.Length; ++i)
                if(char.IsLetter(word[i]))
                    endWord += word[i];
                else
                    continue;
            return endWord;
        }
        public void Load(string path) {
            var sr = new StreamReader(path);
            while(true) {
                var line = sr.ReadLine();
                if(line == null) {
                    break;
                }
                AddWordList(line.Split(' ').ToList());
            }
            sr.Close();
            sr.Dispose(); // sprawdzić
        }
        public List<string> GetWordList() {
            return words;
        }
        static Random _rand = new Random();
    }

    class RandomGenStruct {
        static Random rand = new Random();
        #region MacarovChain Structure
        struct MacarovChain {
            public string first;
            public string second;
            public List<string> third;
            public MacarovChain(string first, string second, string third) {
                this.first = first;
                this.second = second;
                this.third = new List<string>();
                this.third.Add(third);
            }
            public MacarovChain(string first, string second) {
                this.first = first;
                this.second = second;
                this.third = new List<string>();
            }
            public void AddThird(string third) {
                this.third.Add(third);
            }
            public bool KeyConsentient(string first, string second) {
                if(this.first.Equals(first) && this.second.Equals(second))
                    return true;
                else
                    return false;
            }
            public void Write() {
                Console.Write("{0} {1}: ", first, second);
                foreach(var i in third) {
                    Console.Write("{0} ", i);
                }
                Console.WriteLine();
            }
        }
        #endregion

        List<MacarovChain> WordsList;
        public RandomGenStruct() {
            WordsList = new List<MacarovChain>();
            MacarovChain mc;
            mc.first = "2";
            mc.second = "D";
            mc.third = new List<string>();
        }
        public void Add(string key1, string key2, string value) {
            if(key1 != "" && key2 != "") {
                if(WordsList.Count == 0)
                    WordsList.Add(new MacarovChain(key1, key2, value));
                else {
                    bool keyExist = false;
                    for(int i = 0; i < WordsList.Count; i++) {
                        if(WordsList[i].KeyConsentient(key1, key2)) {
                            WordsList[i].AddThird(value);
                            keyExist = true;
                            break;
                        }
                    }
                    if(!keyExist)
                        WordsList.Add(new MacarovChain(key1, key2, value));
                }
            }
        }
        public void Add(string key1, string key2) {
            if(key1 != "" && key2 != "") {
                if(WordsList.Count == 0)
                    WordsList.Add(new MacarovChain(key1, key2));
                else {
                    bool keyExist = false;
                    for(int i = 0; i < WordsList.Count; i++) {
                        if(WordsList[i].KeyConsentient(key1, key2)) {
                            keyExist = true;
                            break;
                        }
                    }
                    if(!keyExist)
                        WordsList.Add(new MacarovChain(key1, key2));
                }
            }
        }
        private void AddFromList(List<string> words) {
            if(words.Count > 1) {
                if(words.Count > 2)
                    for(int i = 0; i < words.Count - 2; i++) {
                        Add(words[i], words[i + 1], words[i + 2]);
                    }
                Add(words[words.Count - 2], words[words.Count - 1]);
            }
        }
        public void Write() {
            foreach(var i in WordsList) {
                i.Write();
            }
        }
        public void Load(string path) {
            StreamReader sr = new StreamReader(path);
            string line = "";
            while(true) {
                line = sr.ReadLine();//readtoend
                if(line == null)
                    break;
                AddFromList(line.Split(' ').ToList());
            }
            sr.Close();
            sr.Dispose();
        }
        public void LoadAll(string path) {
            StreamReader sr = new StreamReader(path);
            string line = "";
            line = sr.ReadToEnd();
            var sentences = line.Split(".;?".ToCharArray()).ToList();
            foreach(var tmp in sentences) {
                var words = tmp.Split(' ').ToList();
                for(int i = 0; i < words.Count; i++) {
                    words[i] = RemoveSymbol(words[i]);
                }
                AddFromList(words);
            }
            sr.Close();
            sr.Dispose();
        }
        private string RemoveSymbol(string word) {
            string endWord = "";
            for(int i = 0; i < word.Length; i++) {
                if(char.IsLetter(word[i]))
                    endWord += word[i];
                else continue;
            }
            return endWord;
        }
        public string GenerateSentence() {
            string sentence;
            var firstWord = RandomCapital();
            sentence = WordsList[firstWord].first + " " + WordsList[firstWord].second + " " + WordsList[firstWord].third[rand.Next(WordsList[firstWord].third.Count())];
            while(true) {
                var sCount = sentence.Split(' ').ToList().Count();
                var key1 = sentence.Split(' ').ToList()[sCount - 2];
                var key2 = sentence.Split(' ').ToList()[sCount - 1];
                for(int i = 0; i < WordsList.Count; i++) {
                    if(WordsList[i].KeyConsentient(key1, key2)) {
                        if(WordsList[i].third.Count > 0) {
                            sentence += " " + WordsList[i].third[rand.Next(WordsList[i].third.Count())];
                            break;
                        }
                        else {
                            return sentence;
                        }
                    }
                    else if(i < WordsList.Count-1)
                        continue;
                    throw new KeyNotFoundException("BLAD");
                }
                //return string.Empty;
            }
        }
        private int RandomCapital() {
            while(true) {
                var i = rand.Next(WordsList.Count);
                if(char.IsUpper(WordsList[i].first.First()))
                    if(WordsList[i].third.Count() > 0)
                        return i;
            }
        }
    }
    class Program {
        static Random rand = new Random();
        static void Main(string[] args) {
            RandomGenStruct rgs = new RandomGenStruct();
            rgs.LoadAll("Book.txt");
 //            rgs.Write();
            while(true) {
                try {
                    Console.WriteLine(rgs.GenerateSentence());
                    Console.ReadKey();
                }
                catch(KeyNotFoundException knfe) {
                    Console.WriteLine(knfe.Data);
                    
                }

            }
        }
    }
}
