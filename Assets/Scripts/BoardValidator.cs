using System.Collections.Generic;
using UnityEngine;

public static class BoardValidator
{
    public static bool HasPossibleWord(
        LetterButton[] board,
        HashSet<string> dictionary)
    {
        Dictionary<char, int> letters =
            new Dictionary<char, int>();

        foreach (var button in board)
        {
            char c = char.ToLower(button.Letter);

            if (!letters.ContainsKey(c))
                letters[c] = 0;

            letters[c]++;
        }

        foreach (var word in dictionary)
        {
            if (CanMakeWord(word, letters))
                return true;
        }

        return false;
    }

    static bool CanMakeWord(
        string word,
        Dictionary<char, int> letters)
    {
        Dictionary<char, int> copy =
            new Dictionary<char, int>(letters);

        foreach (char c in word)
        {
            if (!copy.ContainsKey(c))
                return false;

            copy[c]--;

            if (copy[c] < 0)
                return false;
        }

        return true;
    }
}