using DG.Tweening;
using UnityEngine;

public class UILevelButtonAnimation : MonoBehaviour
{
    [SerializeField] private float duration = 0.3f;
    [SerializeField] private float delay = 0f;

    private Vector3 originalScale;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    private void Start()
    {
        transform.localScale = Vector3.zero;

        transform
            .DOScale(originalScale, duration)
            .SetDelay(delay)
            .SetEase(Ease.OutBack);
    }
}