using DG.Tweening;
using UnityEngine;

public class UIButtonAnimation : MonoBehaviour
{
    [SerializeField] private float pressedScale = 0.92f;
    [SerializeField] private float duration = 0.08f;

    private Vector3 originalScale;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    public void PlayPressAnimation()
    {
        transform.DOKill();

        transform
            .DOScale(originalScale * pressedScale, duration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                transform
                    .DOScale(originalScale, duration)
                    .SetEase(Ease.OutBack);
            });
    }
}