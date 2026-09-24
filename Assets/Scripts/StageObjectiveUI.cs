using TMPro;
using UnityEngine;

public class StageObjectiveUI : MonoBehaviour
{
    [SerializeField] private TMP_Text objectiveText;

    private void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (StageConditionManager.Instance == null)
            return;

        objectiveText.text = "";

        var conditions = StageConditionManager.Instance.GetConditions();

        for (int i = 0; i < conditions.Count; i++)
        {
            LetterCondition condition = conditions[i];

            int remaining =
                Mathf.Max(
                    0,
                    condition.requiredAmount - condition.currentAmount
                );

            if (i > 0)
                objectiveText.text += " ";

            string color =
    remaining == 0 ? "#4CFF6A" : "#FFFFFF";

            objectiveText.text += "<color=" + color + ">" + remaining + "x" + char.ToUpper(condition.letter);
        }
    }
}