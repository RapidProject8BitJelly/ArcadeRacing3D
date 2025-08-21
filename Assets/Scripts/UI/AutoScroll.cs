using UnityEngine;
using UnityEngine.UI;

public class AutoScroll : MonoBehaviour
{
    public ScrollRect scrollRect;
    public float speed = 0.1f;

    void Update()
    {
        scrollRect.verticalNormalizedPosition -= speed * Time.deltaTime;

        // Opcjonalne zapętlenie
        if (scrollRect.verticalNormalizedPosition <= 0f)
            scrollRect.verticalNormalizedPosition = 1f;
    }
}
