using System.Collections.Generic;
using UnityEngine;

public class StageConditionManager : MonoBehaviour
{
    public static StageConditionManager Instance;

    private List<LetterCondition> conditions = new List<LetterCondition>();

    private void Awake()
    {
        Instance = this;
        LoadCurrentStage();
    }

    private void LoadCurrentStage()
    {
        if (StageManager.Instance == null)
        {
            Debug.LogError("StageManager not found!");
            return;
        }

        StageData stage = StageManager.Instance.GetCurrentStage();

        if (stage == null)
        {
            Debug.LogError("Current stage data not found!");
            return;
        }

        conditions.Clear();

        foreach (LetterCondition condition in stage.conditions)
        {
            LetterCondition newCondition = new LetterCondition();

            newCondition.letter = condition.letter;
            newCondition.requiredAmount = condition.requiredAmount;

            conditions.Add(newCondition);
        }
    }

    public void RegisterWord(string word)
    {
        if (string.IsNullOrEmpty(word))
            return;

        foreach (char character in word)
        {
            foreach (LetterCondition condition in conditions)
            {
                if (char.ToUpper(character) == char.ToUpper(condition.letter))
                {
                    condition.currentAmount++;
                }
            }
        }

        CheckStageComplete();
    }

    private void CheckStageComplete()
    {
        foreach (LetterCondition condition in conditions)
        {
            if (condition.currentAmount < condition.requiredAmount)
            {
                return;
            }
        }

        WordGameManager.Instance.StartStageCompletionSequence();
    }

    public List<char> GetRequiredLetters()
    {
        List<char> letters = new List<char>();

        foreach (LetterCondition condition in conditions)
        {
            char letter = char.ToUpper(condition.letter);

            if (!letters.Contains(letter))
            {
                letters.Add(letter);
            }
        }

        return letters;
    }
}