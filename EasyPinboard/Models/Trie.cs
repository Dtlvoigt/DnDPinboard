namespace DnDPinboard.Models
{
    public class TrieNode
    {
        public Dictionary<char, TrieNode> Children { get; }
        public bool IsWord { get; set; }

        public TrieNode()
        {
            Children = new Dictionary<char, TrieNode>();
            IsWord = false;
        }
    }

    public class Trie
    {
        private TrieNode root;

        public Trie()
        {
            root = new TrieNode();
        }

        public void Insert(string word)
        {
            TrieNode node = root;

            foreach (char c in word)
            {
                if (!node.Children.ContainsKey(c))
                {
                    node.Children[c] = new TrieNode();
                }

                node = node.Children[c];
            }

            node.IsWord = true;
        }

        public bool Search(string word)
        {
            TrieNode node = root;

            foreach (char c in word)
            {
                if (!node.Children.ContainsKey(c))
                {
                    return false;
                }

                node = node.Children[c];
            }

            return node.IsWord;
        }

        public List<string> GetAllWordsWithPrefix(string prefix)
        {
            TrieNode node = root;

            // Traverse to the last node of the prefix
            foreach (char c in prefix)
            {
                if (!node.Children.ContainsKey(c))
                {
                    return new List<string>();
                }

                node = node.Children[c];
            }

            // Collect all words with the given prefix using depth-first search
            List<string> words = new List<string>();
            CollectWords(node, prefix, words);

            return words;
        }

        private void CollectWords(TrieNode node, string prefix, List<string> words)
        {
            if (node.IsWord)
            {
                words.Add(prefix);
            }

            foreach (var childNode in node.Children)
            {
                char c = childNode.Key;
                TrieNode child = childNode.Value;
                CollectWords(child, prefix + c, words);
            }
        }
    }

}
