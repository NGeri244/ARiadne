using UnityEngine;
using TMPro;

public class DistanceLabel : MonoBehaviour
{
    const int CacheSize = 48;

    [SerializeField] NavigationController navigation;
    [SerializeField] TMP_Text label;

    readonly string hidden = "\u2014";
    string[] meterLabels;
    int shown = int.MinValue;

    void Awake()
    {
        meterLabels = new string[CacheSize];
        for (int i = 0; i < CacheSize; i++)
            meterLabels[i] = i + " m";

        if (label != null)
            label.text = hidden;
    }

    void OnEnable()
    {
        if (navigation != null)
            navigation.RouteChanged += Refresh;
        Refresh();
    }

    void OnDisable()
    {
        if (navigation != null)
            navigation.RouteChanged -= Refresh;
    }

    void Refresh()
    {
        if (label == null || meterLabels == null)
            return;

        float meters = navigation != null ? navigation.RouteLengthMeters : -1f;
        int whole = meters < 0f ? -1 : Mathf.FloorToInt(meters);
        if (whole == shown)
            return;

        shown = whole;
        if (whole < 0)
            label.text = hidden;
        else if (whole >= CacheSize)
            label.text = meterLabels[CacheSize - 1];
        else
            label.text = meterLabels[whole];
    }
}
