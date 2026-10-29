using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StartHereButton : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] GraphAligner aligner;
    [SerializeField] Button button;

    bool lastAligned;

    void OnEnable()
    {
        lastAligned = aligner != null && aligner.IsAligned;
        if (button != null)
            button.interactable = !lastAligned;
    }

    void Update()
    {
        bool aligned = aligner != null && aligner.IsAligned;
        if (aligned == lastAligned || button == null)
            return;

        lastAligned = aligned;
        button.interactable = !aligned;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (aligner != null)
            aligner.AlignInFrontOfCamera();
    }
}
