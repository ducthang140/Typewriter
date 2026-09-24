using DG.Tweening;
using UnityEngine;

public class UIEntranceAnimation : MonoBehaviour
{
    public enum AnimationType
    {
        Pop,
        SlideUp,
        SlideDown
    }

    [SerializeField] private AnimationType animationType = AnimationType.Pop;
    [SerializeField] private float duration = 0.4f;
    [SerializeField] private float delay = 0f;
    [SerializeField] private float slideDistance = 150f;

    private RectTransform rectTransform;
    private Vector2 originalPosition;
    private Vector3 originalScale;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        originalPosition = rectTransform.anchoredPosition;
        originalScale = rectTransform.localScale;
    }

    private void Start()
    {
        Play();
    }

    public void Play()
    {
        rectTransform.DOKill();

        switch (animationType)
        {
            case AnimationType.Pop:
                PlayPop();
                break;

            case AnimationType.SlideUp:
                PlaySlideUp();
                break;

            case AnimationType.SlideDown:
                PlaySlideDown();
                break;
        }
    }

    private void PlayPop()
    {
        rectTransform.localScale = Vector3.zero;

        rectTransform
            .DOScale(originalScale, duration)
            .SetDelay(delay)
            .SetEase(Ease.OutBack);
    }

    private void PlaySlideUp()
    {
        rectTransform.anchoredPosition =
            originalPosition + Vector2.down * slideDistance;

        rectTransform
            .DOAnchorPos(originalPosition, duration)
            .SetDelay(delay)
            .SetEase(Ease.OutBack);
    }

    private void PlaySlideDown()
    {
        rectTransform.anchoredPosition =
            originalPosition + Vector2.up * slideDistance;

        rectTransform
            .DOAnchorPos(originalPosition, duration)
            .SetDelay(delay)
            .SetEase(Ease.OutBack);
    }
}