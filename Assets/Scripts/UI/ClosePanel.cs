using DG.Tweening;
using UnityEngine;

public class ClosePanel : MonoBehaviour
{
    [SerializeField] private RectTransform _panelToClose;
    [SerializeField] private Vector2 _closePanelPosition;
    [SerializeField] private RectTransform _panelToReopen;
    [SerializeField] private Vector2 _reopenPanelPosition;

    public void Close()
    {
        _panelToClose.DOAnchorPos(_closePanelPosition, .5f).SetEase(Ease.OutBack)
            .OnComplete(() => _panelToClose.gameObject.SetActive(false));

        if (_panelToReopen != null)
        {
            _panelToReopen.gameObject.SetActive(true);
            _panelToReopen.DOAnchorPos(_reopenPanelPosition, .5f).SetEase(Ease.OutBack);
        }
    }
}
