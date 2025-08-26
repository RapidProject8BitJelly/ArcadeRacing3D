using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class CreditsAutoScroller : MonoBehaviour
{
    [SerializeField, Min(0.1f)] private float _scrollDuration = 15f;
    [SerializeField] private bool _autoPlayOnOpen = true;

    private ScrollRect _sr;
    private Tween _tween;
    private Coroutine _cr;

    private void Awake()
    {
        _sr = GetComponent<ScrollRect>();
        DOTween.Init(false, true); // safe mode helps catch UI weirdness
    }

    private void OnEnable()
    {
        ResetToTop();
        if (_autoPlayOnOpen) Play();
    }

    private void OnDisable()
    {
        _tween?.Kill();
        if (_cr != null) { StopCoroutine(_cr); _cr = null; }
    }

    [ContextMenu("Play")]
    public void Play()
    {
        _tween?.Kill();
        if (_cr != null) StopCoroutine(_cr);
        _cr = StartCoroutine(CoPlay());
    }

    public void ResetToTop()
    {
        _tween?.Kill();
        if (_cr != null) { StopCoroutine(_cr); _cr = null; }
        _sr.StopMovement();
        _sr.velocity = Vector2.zero;
        _sr.verticalNormalizedPosition = 1f;

        // keep fallback path sane
        var ap = _sr.content.anchoredPosition;
        ap.y = 0f;
        _sr.content.anchoredPosition = ap;
    }

    private IEnumerator CoPlay()
    {
        // let layout settle
        yield return null;
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(_sr.content);
        yield return null;

        var viewport = _sr.viewport != null ? _sr.viewport : (RectTransform)_sr.transform;
        float contentH = _sr.content.rect.height;
        float viewportH = viewport.rect.height;

        _sr.StopMovement();
        _sr.velocity = Vector2.zero;
        _sr.verticalNormalizedPosition = 1f;

        if (contentH <= viewportH + 0.5f) yield break; // nothing to scroll

        _tween = DOTween.To(
                    () => _sr.verticalNormalizedPosition,
                    v  => _sr.verticalNormalizedPosition = v,
                    0f,
                    _scrollDuration
                 )
                 .SetEase(Ease.Linear)
                 .SetUpdate(true)
                 .SetLink(gameObject, LinkBehaviour.KillOnDisable)
                 .OnComplete(() => _cr = null);
    }
}