using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Linq;

public class DictionaryManager : MonoBehaviour
{
    public static DictionaryManager Instance;

    private HashSet<string> dictionary = new HashSet<string>();
    private List<string> sortedWords = new List<string>();

    private Dictionary<char, List<string>> wordsByLetter =
        new Dictionary<char, List<string>>();

    private void Awake()
    {
        Instance = this;
        LoadDictionary();
    }

    private void LoadDictionary()
    {
        TextAsset json =
            Resources.Load<TextAsset>("words_dictionary");

        if (json == null)
        {
            Debug.LogError("Could not find words_dictionary.json!");
            return;
        }

        Dictionary<string, int> words =
            JsonConvert.DeserializeObject<Dictionary<string, int>>(json.text);

        foreach (string word in words.Keys)
        {
            dictionary.Add(word.ToLower());
        }

        Debug.Log("Loaded words: " + dictionary.Count);

        sortedWords = dictionary
            .Where(word => word.Length >= 2 && word.Length <= 6)
            .OrderByDescending(word => word.Length)
            .ToList();

        BuildWordsByLetter();
    }

    private void BuildWordsByLetter()
    {
        wordsByLetter.Clear();

        foreach (char letter in "abcdefghijklmnopqrstuvwxyz")
        {
            wordsByLetter[letter] = new List<string>();
        }

        foreach (string word in sortedWords)
        {
            HashSet<char> uniqueLetters = new HashSet<char>(word);

            foreach (char letter in uniqueLetters)
            {
                wordsByLetter[letter].Add(word);
            }
        }
    }

    public bool IsValidWord(string word)
    {
        return dictionary.Contains(word.ToLower());
    }

    public HashSet<string> GetDictionary()
    {
        return dictionary;
    }

    public List<string> GetWords()
    {
        return new List<string>(dictionary);
    }

    public List<string> GetSortedWords()
    {
        return sortedWords;
    }

    public List<string> GetWordsContainingLetter(char letter)
    {
        letter = char.ToLower(letter);

        if (wordsByLetter.TryGetValue(letter, out List<string> words))
            return words;

        return new List<string>();
    }
}
