using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Linq;

public class DictionaryManager : MonoBehaviour
{
    public static DictionaryManager Instance;

    private HashSet<string> dictionary = new HashSet<string>();

    private List<string> sortedWords = new List<string>();

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
            .Where(word => word.Length >= 2 && word.Length <= 10)
            .OrderByDescending(word => word.Length)
            .ToList();
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
}