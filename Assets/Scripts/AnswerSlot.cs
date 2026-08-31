using TMPro;
using UnityEngine;

public class AnswerSlot : MonoBehaviour
{
    public TMP_Text LetterText;
    public TMP_Text BonusText;

    public void SetLetter(char letter, BonusType bonus)
    {
        LetterText.text = letter.ToString();

        switch (bonus)
        {
            case BonusType.Percent30:
                BonusText.text = "+30%";
                break;

            case BonusType.Points50:
                BonusText.text = "+50";
                break;

            case BonusType.ExtraGauge:
                BonusText.text = "+G";
                break;

            default:
                BonusText.text = "";
                break;
        }
    }

    public void ClearSlot()
    {
        LetterText.text = "";
        BonusText.text = "";
    }
}