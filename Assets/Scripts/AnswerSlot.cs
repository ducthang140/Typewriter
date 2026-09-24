using DG.Tweening;
using TMPro;
using UnityEngine;

public class AnswerSlot : MonoBehaviour
{
    public TMP_Text LetterText;
    public TMP_Text BonusText;

    [Header("Letter Animation")]
    [SerializeField] private float slideDistance = 60f;
    [SerializeField] private float slideDuration = 0.18f;

    private RectTransform letterRect;
    private Vector2 originalPosition;

    private void Awake()
    {
        letterRect = LetterText.GetComponent<RectTransform>();
        originalPosition = letterRect.anchoredPosition;
    }

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

        PlayLetterAnimation();
    }

    private void PlayLetterAnimation()
    {
        letterRect.DOKill();

        letterRect.anchoredPosition =
            originalPosition + Vector2.down * slideDistance;

        letterRect
            .DOAnchorPos(originalPosition, slideDuration)
            .SetEase(Ease.OutBack);
    }

    public void ClearSlot()
    {
        letterRect.DOKill();
        letterRect.anchoredPosition = originalPosition;

        LetterText.text = "";
        BonusText.text = "";
    }
}