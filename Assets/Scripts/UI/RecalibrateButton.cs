using UnityEngine;
using UnityEngine.EventSystems;

public class RecalibrateButton : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] GraphAligner aligner;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (aligner != null)
            aligner.ResetAlignment();
    }
}
