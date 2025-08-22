using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AutoScroll : MonoBehaviour
{
    [SerializeField] private Button _creditsButton;
    [SerializeField] private Button _backButton;
    [SerializeField] private GameObject _creditsContent;
    [SerializeField] private float _scrollDuration;
    [SerializeField] private float _endScrollPositionY;

    private Vector2 _startPosition;
    private Tween _creditsTween;

    private void Awake()
    {
        _startPosition = _creditsContent.transform.localPosition;
    }

    private void OnEnable()
    {
        _creditsButton.onClick.AddListener(StartScrollCredits);
        _backButton.onClick.AddListener(ResetCreditsPosition);
    }

    private void StartScrollCredits()
    {
        _creditsTween = _creditsContent.transform.DOLocalMoveY(_endScrollPositionY, _scrollDuration).SetEase(Ease.Linear);
    }

    private void ResetCreditsPosition()
    {
        if (_creditsTween != null)
        {
            _creditsTween.Kill();
            _creditsTween = null;
        }

        Vector3 pos = _creditsContent.transform.localPosition;
        pos.y = _startPosition.y;
        _creditsContent.transform.localPosition = pos;
    }

}
