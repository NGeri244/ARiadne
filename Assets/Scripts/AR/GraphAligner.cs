using System;
using UnityEngine;

public class GraphAligner : MonoBehaviour
{
    const float PlaceAheadMeters = 1.2f;
    const float EyeHeightMeters = 1.4f;

    [SerializeField] Transform graphSpace;
    [SerializeField] Transform viewCamera;

    public bool IsAligned { get; private set; }

    public event Action Aligned;

    public event Action AlignmentReset;

    Transform placement;

    public void AlignInFrontOfCamera()
    {
        if (IsAligned || graphSpace == null)
            return;

        if (viewCamera == null && Camera.main != null)
            viewCamera = Camera.main.transform;

        if (viewCamera == null)
            return;

        Vector3 forward = viewCamera.forward;
        forward.y = 0f;
        if (forward.sqrMagnitude < 0.0001f)
            forward = Vector3.forward;
        else
            forward.Normalize();

        Vector3 position = viewCamera.position + forward * PlaceAheadMeters;
        position.y -= EyeHeightMeters;

        var holder = new GameObject("GraphPlacement");
        holder.transform.SetPositionAndRotation(position, Quaternion.LookRotation(forward, Vector3.up));
        placement = holder.transform;

        graphSpace.SetParent(placement, false);
        graphSpace.localPosition = Vector3.zero;
        graphSpace.localRotation = Quaternion.identity;
        graphSpace.localScale = Vector3.one;
        IsAligned = true;
        Aligned?.Invoke();
    }

    public void ResetAlignment()
    {
        IsAligned = false;

        if (graphSpace != null)
        {
            graphSpace.SetParent(null, false);
            graphSpace.localPosition = Vector3.zero;
            graphSpace.localRotation = Quaternion.identity;
            graphSpace.localScale = Vector3.one;
        }

        if (placement != null)
        {
            Destroy(placement.gameObject);
            placement = null;
        }

        AlignmentReset?.Invoke();
    }
}
