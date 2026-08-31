using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WordGameManager : MonoBehaviour
{
    public static WordGameManager Instance;

    [Header("Grid")]
    public Transform LetterGrid;
    public GameObject LetterButtonPrefab;

    [Header("Answer")]
    public Transform AnswerGrid;
    public GameObject AnswerSlotPrefab;

    public int BoardSize = 18;
    public int MaxWordLength = 10;

    public int Score { get; private set; }

    private LetterButton[] letterButtons;
    private AnswerSlot[] answerSlots;

    private List<LetterData> selectedLetters = new List<LetterData>();

    private int consecutiveValidWords = 0;

    public TMP_Text ScoreText;

    [Header("Bonus Phase")]
    public Slider BonusGauge;

    private float bonusGauge = 94f;
    private bool bonusPhaseActive = false;
    private float bonusPhaseTimer = 0f;

    private const float BonusPhaseDuration = 10f;
    private const float BonusGaugeRequired = 100f;

    private bool bonusWordProcessing = false;

    private Dictionary<char, string> longestWordsByLetter =
    new Dictionary<char, string>();

    private Dictionary<char, int> boardLetterCounts =
    new Dictionary<char, int>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Score = 0;
        ScoreText.text = Score.ToString();

        CreateBoard();
        CreateAnswerSlots();

        GenerateBoard();

        BuildBoardLetterCounts();
        BuildBonusWordCache();

        GrantStartingBonuses();
    }

    private void CreateBoard()
    {
        letterButtons = new LetterButton[BoardSize];

        for (int i = 0; i < BoardSize; i++)
        {
            GameObject obj = Instantiate(
                LetterButtonPrefab,
                LetterGrid
            );

            LetterButton button =
                obj.GetComponent<LetterButton>();

            letterButtons[i] = button;
        }
    }

    private void CreateAnswerSlots()
    {
        answerSlots = new AnswerSlot[MaxWordLength];

        for (int i = 0; i < MaxWordLength; i++)
        {
            GameObject obj = Instantiate(
                AnswerSlotPrefab,
                AnswerGrid
            );

            answerSlots[i] =
                obj.GetComponent<AnswerSlot>();
        }
    }

    public void SelectLetter(LetterButton button)
    {
        if (bonusPhaseActive)
        {
            if (bonusWordProcessing)
                return;

            bonusWordProcessing = true;
            StartBonusWord(button);
            return;
        }

        if (selectedLetters.Count >= MaxWordLength)
            return;

        LetterData data = new LetterData();

        data.letter = button.Letter;
        data.button = button;
        data.bonus = button.Bonus;

        selectedLetters.Add(data);

        answerSlots[selectedLetters.Count - 1]
            .SetLetter(button.Letter, button.Bonus);

        button.HideLetter();
    }

    public void ClearSelection()
    {
        foreach (LetterData letter in selectedLetters)
        {
            letter.button.ShowLetter();
        }

        selectedLetters.Clear();

        foreach (AnswerSlot slot in answerSlots)
        {
            slot.ClearSlot();
        }
    }

    private char GetRandomLetter()
    {
        string letters =
            "EEEEEE" +
            "TTTTT" +
            "AAAAA" +
            "OOOOO" +
            "IIII" +
            "NNNN" +
            "SSS" +
            "HHH" +
            "RRR" +
            "DD" +
            "LL" +
            "CC" +
            "UU" +
            "MM" +
            "WW" +
            "FF" +
            "GG" +
            "YY" +
            "PP" +
            "B" +
            "V" +
            "K" +
            "J" +
            "X" +
            "Q" +
            "Z";

        return letters[Random.Range(0, letters.Length)];
    }

    private void GenerateBoard()
    {
        for (int i = 0; i < letterButtons.Length; i++)
        {
            letterButtons[i].ReplaceLetter(GetRandomLetter());
        }

        if (!HasPossibleWord())
        {
            GenerateBoard();
        }
    }

    private bool HasPossibleWord()
    {
        return BoardValidator.HasPossibleWord(
            letterButtons,
            DictionaryManager.Instance.GetDictionary()
        );
    }

    public void CheckWord()
    {
        string word = "";

        foreach (LetterData letter in selectedLetters)
        {
            word += letter.letter;
        }

        bool valid =
            DictionaryManager.Instance.IsValidWord(word);

        if (!valid)
        {
            consecutiveValidWords = 0;
            RestoreLetters();
            return;
        }

        // Valid word
        consecutiveValidWords++;

        int points = GetWordScore(word.Length);

        int flatBonus = 0;
        int percentageBonus = 0;
        int gaugeBonusCount = 0;

        foreach (LetterData letter in selectedLetters)
        {
            if (letter.bonus == BonusType.Points50)
            {
                flatBonus += 50;
            }
            else if (letter.bonus == BonusType.Percent30)
            {
                percentageBonus += 30;
            }
            else if (letter.bonus == BonusType.ExtraGauge)
            {
                gaugeBonusCount++;
            }
        }

        // Add flat bonuses first
        points += flatBonus;

        // Then apply percentage bonuses
        points = Mathf.RoundToInt(
            points * (1f + percentageBonus / 100f)
        );

        // Add to total score
        Score += points;
        ScoreText.text = Score.ToString();
        if (!bonusPhaseActive)
        {
            float gaugeGain = GetGaugeGain(word.Length);

            gaugeGain *= 1f + (gaugeBonusCount * 0.5f);

            AddBonusGauge(gaugeGain);
        }

        Debug.Log("Word: " + word);
        Debug.Log("Points gained: " + points);
        Debug.Log("Total Score: " + Score);

        // Bonus Letter conditions
        if (word.Length >= 7)
        {
            GrantBonusLetter();
        }

        if (points >= 1000)
        {
            GrantBonusLetter();
        }

        if (consecutiveValidWords >= 3)
        {
            GrantBonusLetter();
            consecutiveValidWords = 0;
        }

        // Replace used letters with new ones
        ConsumeLetters();

        // Check if the new board has a possible word
        CheckBoard();

        BuildBoardLetterCounts();
        BuildBonusWordCache();
    }

    private void ConsumeLetters()
    {
        foreach (LetterData letter in selectedLetters)
        {
            letter.button.ReplaceLetter(GetRandomLetter());
        }

        selectedLetters.Clear();

        foreach (AnswerSlot slot in answerSlots)
        {
            slot.ClearSlot();
        }
    }

    void RestoreLetters()
    {
        foreach (var letter in selectedLetters)
        {
            letter.button.ShowLetter();
        }

        selectedLetters.Clear();

        foreach (var slot in answerSlots)
            slot.ClearSlot();
    }

    public void ShuffleLetters()
    {
        ClearSelection();

        List<BonusType> bonuses =
            new List<BonusType>();

        foreach (LetterButton button in letterButtons)
        {
            if (button.Bonus != BonusType.None)
            {
                bonuses.Add(button.Bonus);
            }
        }

        foreach (LetterButton button in letterButtons)
        {
            button.ReplaceLetter(GetRandomLetter());
        }

        foreach (BonusType bonus in bonuses)
        {
            List<LetterButton> available =
                new List<LetterButton>();

            foreach (LetterButton button in letterButtons)
            {
                if (button.Bonus == BonusType.None)
                {
                    available.Add(button);
                }
            }

            LetterButton target =
                available[Random.Range(0, available.Count)];

            target.SetBonus(bonus);
        }

        CheckBoard();

        BuildBoardLetterCounts();
        BuildBonusWordCache();
    }

    void CheckBoard()
    {
        bool possible =
            BoardValidator.HasPossibleWord(
                letterButtons,
                DictionaryManager.Instance.GetDictionary());

        if (!possible)
        {
            Debug.Log("No valid words. Auto shuffle.");

            ShuffleLetters();
        }
    }

    private int GetWordScore(int wordLength)
    {
        switch (wordLength)
        {
            case 2: return 50;
            case 3: return 100;
            case 4: return 150;
            case 5: return 250;
            case 6: return 400;
            case 7: return 650;
            case 8: return 1000;
            case 9: return 1500;
            case 10: return 2500;

            default: return 0;
        }
    }

    private void GrantBonusLetter()
    {
        List<LetterButton> availableLetters =
            new List<LetterButton>();

        foreach (LetterButton button in letterButtons)
        {
            if (button.LetterText.text != "" &&
                button.Bonus == BonusType.None)
            {
                availableLetters.Add(button);
            }
        }

        if (availableLetters.Count == 0)
            return;

        LetterButton target =
            availableLetters[
                Random.Range(0, availableLetters.Count)
            ];

        BonusType bonus =
            (BonusType)Random.Range(1, 4);

        target.SetBonus(bonus);
    }

    private void GrantStartingBonuses()
    {
        for (int i = 0; i < 3; i++)
        {
            GrantBonusLetter();
        }
    }

    private void AddBonusGauge(float amount)
    {
        bonusGauge += amount;

        if (bonusGauge >= BonusGaugeRequired)
        {
            bonusGauge = BonusGaugeRequired;
            StartBonusPhase();
        }

        BonusGauge.value = bonusGauge;
    }

    private void StartBonusPhase()
    {
        bonusPhaseActive = true;
        bonusPhaseTimer = BonusPhaseDuration;

        bonusGauge = BonusGaugeRequired;
        BonusGauge.value = bonusGauge;
    }

    private void Update()
    {
        if (!bonusPhaseActive)
            return;

        bonusPhaseTimer -= Time.deltaTime;

        if (bonusPhaseTimer <= 0f)
        {
            EndBonusPhase();
        }
    }

    private void EndBonusPhase()
    {
        bonusPhaseActive = false;

        bonusPhaseTimer = 0f;
        bonusGauge = 0f;

        BonusGauge.value = 0f;
    }

    private void StartBonusWord(LetterButton selectedButton)
    {
        char letter = char.ToLower(selectedButton.Letter);

        if (!longestWordsByLetter.TryGetValue(letter, out string word))
            return;

        FillBonusWord(word);
    }

    private string FindLongestWord(char requiredLetter)
    {
        foreach (string word in DictionaryManager.Instance.GetSortedWords())
        {
            if (!word.Contains(
                requiredLetter.ToString().ToLower()))
                continue;

            if (CanMakeWordFromBoard(word))
            {
                return word;
            }
        }

        return "";
    }

    private bool CanMakeWordFromBoard(string word)
    {
        Dictionary<char, int> required =
            new Dictionary<char, int>();

        foreach (char letter in word)
        {
            if (!required.ContainsKey(letter))
                required[letter] = 0;

            required[letter]++;
        }

        foreach (var pair in required)
        {
            if (!boardLetterCounts.ContainsKey(pair.Key))
                return false;

            if (boardLetterCounts[pair.Key] < pair.Value)
                return false;
        }

        return true;
    }

    private void FillBonusWord(string word)
    {
        selectedLetters.Clear();

        foreach (AnswerSlot slot in answerSlots)
        {
            slot.ClearSlot();
        }

        foreach (char character in word)
        {
            LetterButton matchingButton =
                FindMatchingLetter(character);

            if (matchingButton == null)
                return;

            LetterData data = new LetterData();

            data.letter = matchingButton.Letter;
            data.button = matchingButton;
            data.bonus = matchingButton.Bonus;

            selectedLetters.Add(data);

            answerSlots[selectedLetters.Count - 1]
                .SetLetter(
                    matchingButton.Letter,
                    matchingButton.Bonus
                );

            matchingButton.HideLetter();
        }

        Invoke(nameof(CheckBonusWord), 0.2f);
    }

    private LetterButton FindMatchingLetter(char character)
    {
        foreach (LetterButton button in letterButtons)
        {
            if (button.LetterText.text == "")
                continue;

            if (char.ToLower(button.Letter) ==
                char.ToLower(character))
            {
                return button;
            }
        }

        return null;
    }

    private void CheckBonusWord()
    {
        if (!bonusPhaseActive)
            return;

        bonusWordProcessing = false;
        CheckWord();
    }

    private float GetGaugeGain(int wordLength)
    {
        switch (wordLength)
        {
            case 2: return 8f;
            case 3: return 9f;
            case 4: return 10f;
            case 5: return 11f;
            case 6: return 12f;
            case 7: return 13f;
            case 8: return 14f;
            case 9: return 15f;
            case 10: return 16f;

            default: return 0f;
        }
    }

    private void BuildBonusWordCache()
    {
        longestWordsByLetter.Clear();

        HashSet<char> availableLetters = new HashSet<char>();

        // Get letters currently available on the board
        foreach (LetterButton button in letterButtons)
        {
            if (button.LetterText.text == "")
                continue;

            availableLetters.Add(
                char.ToLower(button.Letter)
            );
        }

        // Search for each available letter
        foreach (char letter in availableLetters)
        {
            string longestWord = FindLongestWord(letter);

            if (!string.IsNullOrEmpty(longestWord))
            {
                longestWordsByLetter[letter] = longestWord;
            }
        }
    }

    private void BuildBoardLetterCounts()
    {
        boardLetterCounts.Clear();

        foreach (LetterButton button in letterButtons)
        {
            if (button.LetterText.text == "")
                continue;

            char letter = char.ToLower(button.Letter);

            if (!boardLetterCounts.ContainsKey(letter))
                boardLetterCounts[letter] = 0;

            boardLetterCounts[letter]++;
        }
    }
}