using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LetterButton : MonoBehaviour
{
    public TMP_Text LetterText;
    public TMP_Text BonusText;

    private Button button;

    public char Letter { get; private set; }

    public BonusType Bonus { get; private set; }

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    public void SetLetter(char letter)
    {
        Letter = letter;
        Bonus = BonusType.None;

        LetterText.text = letter.ToString();
        BonusText.text = "";

        button.interactable = true;
    }

    public void SetBonus(BonusType bonus)
    {
        Bonus = bonus;

        if (bonus == BonusType.None)
        {
            BonusText.text = "";
        }
        else
        {
            BonusText.text = GetBonusDisplay(bonus);
        }
    }

    private string GetBonusDisplay(BonusType bonus)
    {
        switch (bonus)
        {
            case BonusType.Percent30:
                return "+30%";

            case BonusType.Points50:
                return "+50";

            case BonusType.ExtraGauge:
                return "+G";

            default:
                return "";
        }
    }

    public void HideLetter()
    {
        LetterText.text = "";
        BonusText.text = "";
        button.interactable = false;
    }

    public void ShowLetter()
    {
        LetterText.text = Letter.ToString();
        BonusText.text = GetBonusDisplay(Bonus);
        button.interactable = true;
    }

    public void ReplaceLetter(char letter)
    {
        Letter = letter;
        Bonus = BonusType.None;

        LetterText.text = letter.ToString();
        BonusText.text = "";

        button.interactable = true;
    }

    public void OnClicked()
    {
        WordGameManager.Instance.SelectLetter(this);
    }
}