using UnityEngine;

public class SafeAreaFitter : MonoBehaviour
{
    RectTransform rect;
    Rect last;
    bool hasLast;

    void Awake()
    {
        rect = (RectTransform)transform;
        Apply();
    }

    void Update()
    {
        Rect area = Screen.safeArea;
        if (hasLast && area == last)
            return;

        Apply();
    }

    void Apply()
    {
        if (rect == null)
            return;

        int width = Screen.width;
        int height = Screen.height;
        if (width < 1 || height < 1)
            return;

        Rect area = Screen.safeArea;
        last = area;
        hasLast = true;

        Vector2 min = area.position;
        Vector2 max = area.position + area.size;
        min.x /= width;
        min.y /= height;
        max.x /= width;
        max.y /= height;

        rect.anchorMin = min;
        rect.anchorMax = max;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
}
