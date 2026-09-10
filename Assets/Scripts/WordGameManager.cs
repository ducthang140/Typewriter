using System.Collections;
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

    private float bonusGauge = 0f;
    private bool bonusPhaseActive = false;
    private float bonusPhaseTimer = 0f;

    private const float BonusPhaseDuration = 10f;
    private const float BonusGaugeRequired = 100f;

    private bool bonusWordProcessing = false;

    [Header("Stage")]

    private Dictionary<char, string> longestWordsByLetter =
        new Dictionary<char, string>();

    private int[] boardLetterCounts = new int[26];

    private Coroutine bonusCacheCoroutine;
    private int bonusCacheVersion = 0;

    private bool bonusCacheReady = false;

    [Header("Word Limit")]
    [SerializeField] private int maxWords = 10;
    private int remainingWords;

    [SerializeField] private TMP_Text wordCountText;

    [SerializeField] private GameObject stageFailPopup;

    private bool stageCompleted = false;

    [Header("Stage Completion Sequence")]
    private bool stageCompletionSequenceActive = false;
    private bool canGenerateBonusLetters = true;

    private bool processingCompletionWord = false;
    private bool processingCompletionShuffle = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        remainingWords = maxWords;
        UpdateWordCountUI();

        Score = 0;
        ScoreText.text = Score.ToString();

        if (stageCompletePopup != null)
        {
            stageCompletePopup.SetActive(false);
        }

        if (stageFailPopup != null)
        {
            stageFailPopup.SetActive(false);
        }

        CreateBoard();
        CreateAnswerSlots();

        GenerateBoard();

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
        if (stageCompletionSequenceActive)
            return;

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
        // Block player interaction during stage completion.
        if (stageCompletionSequenceActive)
            return;

        ClearSelectionInternal();
    }

    private void ClearSelectionInternal()
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

        EnsureStageConditionLetters();

        if (!HasPossibleWord())
        {
            GenerateBoard();
        }
    }

    private void EnsureStageConditionLetters()
    {
        if (StageConditionManager.Instance == null)
            return;

        List<char> requiredLetters =
            StageConditionManager.Instance.GetRequiredLetters();

        if (requiredLetters.Count == 0)
            return;

        if (requiredLetters.Count > letterButtons.Length)
        {
            Debug.LogError(
                "There are more required letters than board slots!"
            );
            return;
        }

        // Randomly choose different board positions
        List<int> positions = new List<int>();

        for (int i = 0; i < letterButtons.Length; i++)
        {
            positions.Add(i);
        }

        // Shuffle positions
        for (int i = 0; i < positions.Count; i++)
        {
            int randomIndex = Random.Range(i, positions.Count);

            int temp = positions[i];
            positions[i] = positions[randomIndex];
            positions[randomIndex] = temp;
        }

        // Force one copy of every required letter onto the board
        for (int i = 0; i < requiredLetters.Count; i++)
        {
            int position = positions[i];

            letterButtons[position].ReplaceLetter(
                requiredLetters[i]
            );
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
        if (stageCompletionSequenceActive && !processingCompletionWord)
            return;

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

        if (!bonusPhaseActive && !stageCompletionSequenceActive)
        {
            remainingWords--;
            UpdateWordCountUI();
        }

        // Register letters for the current stage conditions
        if (StageConditionManager.Instance != null)
        {
            StageConditionManager.Instance.RegisterWord(word);
        }

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
        if (word.Length >= 5)
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

        if (!bonusPhaseActive)
        {
            CheckWordLimit();
        }

        // Check if the new board has a possible word
        if (!stageCompletionSequenceActive)
        {
            CheckBoard();
        }

        if (bonusPhaseActive)
        {
            StartBonusCacheRebuild();
        }
    }

    private void ConsumeLetters()
    {
        foreach (LetterData letter in selectedLetters)
        {
            letter.button.ReplaceLetter(GetRandomLetter());
        }

        EnsureStageConditionLetters();

        BuildBoardLetterCounts();

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
        if (stageCompletionSequenceActive &&
        !processingCompletionShuffle)
            return;

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

        EnsureStageConditionLetters();

        BuildBoardLetterCounts();

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

        if (bonusPhaseActive)
        {
            StartBonusCacheRebuild();
        }
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
        if (!canGenerateBonusLetters)
            return;

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
        bonusCacheReady = false;

        bonusGauge = BonusGaugeRequired;
        BonusGauge.value = bonusGauge;

        StartBonusCacheRebuild();
    }

    private void Update()
    {
        if (!bonusPhaseActive)
            return;

        if (!bonusCacheReady)
            return;

        bonusPhaseTimer -= Time.unscaledDeltaTime;

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

        if (bonusCacheCoroutine != null)
        {
            StopCoroutine(bonusCacheCoroutine);
            bonusCacheCoroutine = null;
        }

        longestWordsByLetter.Clear();
        bonusWordProcessing = false;
        bonusCacheReady = false;

        if (remainingWords <= 0 && !stageCompletionSequenceActive)
        {
            StageFail();
        }
    }

    public void StartStageCompletionSequence()
    {
        if (stageCompletionSequenceActive)
            return;

        stageCompletionSequenceActive = true;
        canGenerateBonusLetters = false;

        // End current Bonus Phase
        if (bonusPhaseActive)
        {
            EndBonusPhase();
        }

        StartCoroutine(StageCompletionBonusSequence());
    }

    private IEnumerator StageCompletionBonusSequence()
    {
        // Convert remaining words into completion bonus letters
        int bonusCount = remainingWords;

        remainingWords = 0;
        UpdateWordCountUI();

        // Generate completion bonus letters
        for (int i = 0; i < bonusCount; i++)
        {
            GrantCompletionBonusLetter();
        }

        while (HasBonusLettersOnBoard())
        {
            BuildBoardLetterCounts();

            // Find all bonus letters that can actually make a word
            List<LetterButton> usableBonusLetters =
                GetUsableBonusLetters();

            // None of the bonus letters can make a word
            if (usableBonusLetters.Count == 0)
            {
                processingCompletionShuffle = true;

                ShuffleLetters();

                processingCompletionShuffle = false;

                // Let the new board/cache settle
                yield return null;

                continue;
            }

            // Pick one random usable bonus letter
            LetterButton selectedButton =
                usableBonusLetters[
                    Random.Range(0, usableBonusLetters.Count)
                ];

            string word = FindLongestWord(selectedButton.Letter);

            if (string.IsNullOrEmpty(word))
                continue;

            // Auto-fill
            FillBonusWord(word);

            yield return new WaitForSecondsRealtime(0.2f);

            // Check the completion word
            processingCompletionWord = true;

            CheckWord();

            processingCompletionWord = false;

            yield return new WaitForSecondsRealtime(0.2f);
        }

        StageComplete();
    }

    private void StartBonusWord(LetterButton selectedButton)
    {
        char letter = char.ToLower(selectedButton.Letter);

        if (!longestWordsByLetter.TryGetValue(letter, out string word))
        {
            bonusWordProcessing = false;
            return;
        }

        FillBonusWord(word);

        StartCoroutine(CheckBonusWordAfterDelay());
    }

    private string FindLongestWord(char requiredLetter)
    {
        char lowerLetter = char.ToLower(requiredLetter);

        List<string> candidates =
            DictionaryManager.Instance.GetWordsContainingLetter(lowerLetter);

        for (int i = 0; i < candidates.Count; i++)
        {
            string word = candidates[i];

            if (word.Length > MaxWordLength)
                continue;

            if (CanMakeWordFromBoard(word))
                return word;
        }

        return "";
    }

    private bool CanMakeWordFromBoard(string word)
    {
        int[] required = new int[26];

        for (int i = 0; i < word.Length; i++)
        {
            int index = word[i] - 'a';

            if (index < 0 || index >= 26)
                return false;

            required[index]++;

            if (required[index] > boardLetterCounts[index])
                return false;
        }

        return true;
    }

    private void StartBonusCacheRebuild()
    {
        if (!bonusPhaseActive)
            return;

        if (bonusCacheCoroutine != null)
        {
            StopCoroutine(bonusCacheCoroutine);
        }

        bonusCacheVersion++;
        int version = bonusCacheVersion;

        bonusCacheCoroutine =
            StartCoroutine(BuildBonusWordCacheCoroutine(version));
    }

    private System.Collections.IEnumerator BuildBonusWordCacheCoroutine(int version)
    {
        longestWordsByLetter.Clear();

        BuildBoardLetterCounts();

        HashSet<char> availableLetters = new HashSet<char>();

        foreach (LetterButton button in letterButtons)
        {
            if (button.LetterText.text == "")
                continue;

            availableLetters.Add(char.ToLower(button.Letter));
        }

        foreach (char letter in availableLetters)
        {
            if (version != bonusCacheVersion || !bonusPhaseActive)
                yield break;

            string longestWord = FindLongestWord(letter);

            if (!string.IsNullOrEmpty(longestWord))
            {
                longestWordsByLetter[letter] = longestWord;
            }

            // Give Unity a frame between each letter's search.
            yield return null;
        }

        bonusCacheCoroutine = null;

        // The 10-second timer starts only when the initial cache is ready.
        bonusCacheCoroutine = null;

        if (!bonusPhaseActive)
            yield break;

        if (!bonusCacheReady)
        {
            bonusCacheReady = true;
            bonusPhaseTimer = BonusPhaseDuration;
        }
    }

    private void BuildBoardLetterCounts()
    {
        System.Array.Clear(boardLetterCounts, 0, boardLetterCounts.Length);

        foreach (LetterButton button in letterButtons)
        {
            if (button.LetterText.text == "")
                continue;

            int index = char.ToLower(button.Letter) - 'a';

            if (index >= 0 && index < 26)
            {
                boardLetterCounts[index]++;
            }
        }
    }

    private float GetGaugeGain(int wordLength)
    {
        switch (wordLength)
        {
            case 2: return 18f;
            case 3: return 19f;
            case 4: return 20f;
            case 5: return 21f;
            case 6: return 22f;
            case 7: return 23f;
            case 8: return 24f;
            case 9: return 25f;
            case 10: return 26f;

            default: return 0f;
        }
    }

    private void FillBonusWord(string word)
    {
        ClearSelectionInternal();

        List<LetterButton> availableButtons =
            new List<LetterButton>();

        foreach (LetterButton button in letterButtons)
        {
            if (button.LetterText.text != "")
            {
                availableButtons.Add(button);
            }
        }

        foreach (char character in word)
        {
            LetterButton matchingButton = null;

            foreach (LetterButton button in availableButtons)
            {
                if (char.ToLower(button.Letter) == character)
                {
                    matchingButton = button;
                    break;
                }
            }

            if (matchingButton == null)
            {
                Debug.LogWarning(
                    "Could not find letter " + character +
                    " while filling bonus word."
                );

                ClearSelection();
                return;
            }

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

            availableButtons.Remove(matchingButton);
        }
    }

    private System.Collections.IEnumerator CheckBonusWordAfterDelay()
    {
        yield return new WaitForSecondsRealtime(0.2f);

        if (!bonusPhaseActive)
        {
            bonusWordProcessing = false;
            yield break;
        }

        CheckWord();

        bonusWordProcessing = false;
    }

    [SerializeField] private GameObject stageCompletePopup;

    public void StageComplete()
    {
        if (stageCompleted)
            return;

        stageCompleted = true;

        if (stageCompletePopup != null)
        {
            stageCompletePopup.SetActive(true);
        }
    }

    private void UpdateWordCountUI()
    {
        if (wordCountText != null)
        {
            wordCountText.text = "" + remainingWords;
        }
    }

    private void CheckWordLimit()
    {
        if (remainingWords > 0)
            return;

        if (bonusPhaseActive)
            return;

        if (stageCompletionSequenceActive)
            return;

        StageFail();
    }

    public void StageFail()
    {
        if (stageCompleted)
            return;

        if (stageFailPopup != null)
        {
            stageFailPopup.SetActive(true);
        }
    }

    private void GrantCompletionBonusLetter()
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

    private bool HasBonusLettersOnBoard()
    {
        foreach (LetterButton button in letterButtons)
        {
            if (button.LetterText.text != "" &&
                button.Bonus != BonusType.None)
            {
                return true;
            }
        }

        return false;
    }

    private LetterButton GetRandomBonusLetter()
    {
        List<LetterButton> bonusLetters =
            new List<LetterButton>();

        foreach (LetterButton button in letterButtons)
        {
            if (button.LetterText.text != "" &&
                button.Bonus != BonusType.None)
            {
                bonusLetters.Add(button);
            }
        }

        if (bonusLetters.Count == 0)
            return null;

        return bonusLetters[
            Random.Range(0, bonusLetters.Count)
        ];
    }

    private List<LetterButton> GetUsableBonusLetters()
    {
        List<LetterButton> usable =
            new List<LetterButton>();

        foreach (LetterButton button in letterButtons)
        {
            if (button.LetterText.text == "")
                continue;

            if (button.Bonus == BonusType.None)
                continue;

            string word = FindLongestWord(button.Letter);

            if (!string.IsNullOrEmpty(word))
            {
                usable.Add(button);
            }
        }

        return usable;
    }
}