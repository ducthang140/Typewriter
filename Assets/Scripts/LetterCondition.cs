using System;

[Serializable]
public class LetterCondition
{
    public char letter;
    public int requiredAmount;

    [NonSerialized]
    public int currentAmount;
}