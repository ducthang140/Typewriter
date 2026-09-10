using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StageData
{
    public int levelNumber;

    public List<LetterCondition> conditions = new List<LetterCondition>();
}